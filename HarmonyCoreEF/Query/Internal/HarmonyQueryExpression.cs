// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore;
using Harmony.Core.EF.Extensions.Internal;
using Harmony.Core.FileIO.Queryable;
using Harmony.Core.Enumerations;
using System.Diagnostics;

namespace Harmony.Core.EF.Query.Internal
{
    public partial class HarmonyQueryExpression : Expression, IPrintableExpression
    {
        private readonly List<Expression> _valueBufferSlots = new List<Expression>();

        private readonly IDictionary<EntityProjectionExpression, IDictionary<IProperty, int>> _entityProjectionCache
            = new Dictionary<EntityProjectionExpression, IDictionary<IProperty, int>>();

        private readonly ParameterExpression _valueBufferParameter;

        private IDictionary<ProjectionMember, Expression> _projectionMapping = new Dictionary<ProjectionMember, Expression>();
        private ParameterExpression _groupingParameter;

        public virtual IReadOnlyList<Expression> Projection => _valueBufferSlots;
        public virtual Expression ServerQueryExpression { get; set; }
        public Dictionary<Expression, HarmonyTableExpression> RootExpressions { get; set; }

        private static QueryBuffer.TypeBuffer GetTypeBuffer(HarmonyTableExpression table, List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>> flatList)
        {
            var queryTables = table.RootExpression.RootExpressions.Values.OfType<HarmonyTableExpression>().Where(qt => qt != table);
            var made = new QueryBuffer.TypeBuffer
            {
                DataObjectType = table.ItemType,
                IsCollection = table.IsCollection,
                IsInnerJoin = table.IsInnerJoin,
                ParentFieldName = table.Name,
                Metadata = DataObjectMetadataBase.LookupType(table.ItemType)
            };
            
            if (table.Top != null)
            {
                var bakedFunction = Expression.Lambda<Func<QueryContext, long>>(Expression.Convert(table.Top, typeof(long)), QueryCompilationContext.QueryContextParameter).Compile();
                made.Top = (obj) => bakedFunction(obj as QueryContext);
            }

            if (table.Skip != null)
            {
                var bakedFunction = Expression.Lambda<Func<QueryContext, long>>(Expression.Convert(table.Skip, typeof(long)), QueryCompilationContext.QueryContextParameter).Compile();
                made.Skip = (obj) => bakedFunction(obj as QueryContext);
            }

            if (made.Top != null || made.Skip != null)
            {
                made.SelectResult = CollectionFilterMethod.MakeGenericMethod(made.DataObjectType).CreateDelegate(typeof(Func<QueryBuffer, QueryBuffer.TypeBuffer, object, object>), null) as Func<QueryBuffer, QueryBuffer.TypeBuffer, object, object>;
            }

            flatList.Add(Tuple.Create(table, made));
            var joinedBuffers = queryTables.OfType<HarmonyTableExpression>().Where(qt => !qt.Name.Contains(".")).Select(qt => GetTypeBuffer(qt, flatList)).ToList();
            var namedLookup = joinedBuffers.ToDictionary(tb => tb.ParentFieldName);
            foreach (var nestedTable in queryTables.OfType<HarmonyTableExpression>().Where(qt => qt.Name.Contains(".")).OrderBy(qt => qt.Name.Length))
            {
                var nameParts = nestedTable.Name.Split(".");
                if (namedLookup.TryGetValue(string.Join(".", nameParts.Take(nameParts.Length - 1)), out var foundBuffer))
                {
                    var fullName = nestedTable.Name;
                    nestedTable.Name = nameParts.Last();
                    var madeBuffer = GetTypeBuffer(nestedTable, flatList);
                    foundBuffer.JoinedBuffers.Add(madeBuffer);
                    namedLookup.Add(fullName, madeBuffer);
                }
                else
                    throw new Exception(string.Format("failed to find parent table while processing query {0}", nestedTable.Name));
            }
            made.JoinedBuffers = joinedBuffers;
            return made;
        }

        private static MethodInfo CollectionFilterMethod = typeof(HarmonyQueryExpression).GetMethod("CollectionFilter");

        public static object CollectionFilter<T>(QueryBuffer query, QueryBuffer.TypeBuffer typeBuf, object result)
        {
            var typedResult = result as IEnumerable<T>;
            if (typedResult != null)
            {
                if (typeBuf.Skip != null)
                {
                    typedResult = typedResult.Skip((int)typeBuf.Skip(query.Context));
                }

                if (typeBuf.Top != null)
                {
                    typedResult = typedResult.Take((int)typeBuf.Top(query.Context));
                }
                return typedResult.ToList();
            }
            else
                return result;
        }

        public HarmonyTableExpression FindServerExpression()
        {
            var subQueryTable = ServerQueryExpression as HarmonyTableExpression;
            if (subQueryTable == null)
            {
                var finder = new HarmonyTableExpressionFindingExpressionVisitor();
                finder.Visit(ServerQueryExpression);
                if (finder.TableExpressions.Count == 1)
                {
                    subQueryTable = finder.TableExpressions.First();
                }
                else
                {
                    throw new NotImplementedException("oddly shaped query");
                }
            }
            return subQueryTable;
        }

        private sealed class HarmonyTableExpressionFindingExpressionVisitor : ExpressionVisitor
        {
            public List<HarmonyTableExpression> TableExpressions = new List<HarmonyTableExpression>();

            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                foreach (var arg in methodCallExpression.Arguments)
                {
                    Visit(arg);
                }
                return methodCallExpression;
            }

            protected override Expression VisitExtension(Expression node)
            {
                if (node is HarmonyTableExpression expr && !TableExpressions.Contains(expr))
                {
                    TableExpressions.Add(expr);
                }
                return node;
            }
        }

        public Expression PrepareQuery(MethodInfo tableMethodInfo, ParameterExpression queryContextParameter, HarmonyQueryCompilationContext compilationContext)
        {
            var rootExpr = RootExpressions[_valueBufferParameter];
            var processedOns = new List<object>();
            List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>> flatList;
            WhereExpressionBuilder whereBuilder;
            List<object> processedWheres;
            List<Tuple<FieldReference, bool>> orderBys;
            var fieldReferences = new Dictionary<int, List<FieldDataDefinition>>();
            MakeFlatList(rootExpr, processedOns, out flatList, out whereBuilder, out processedWheres, out orderBys);

            //check for where expressions that have Table1Expression OR InnerJoinedTable2Expression
            //its not possible to match a driving OR innerJoined expression in a select
            //split it into multiple calls and just union the result
            var targetExpressionMap = new Dictionary<int, Dictionary<object, object>>();
            if (processedWheres.Count == 1 && processedWheres.First() is ConnectorPart conPart)
            {
                var splitState = SplitExpression(conPart);
                List<Expression> unionParts = new List<Expression>();
                foreach (var split in splitState.LeafOns)
                {
                    var unionOns = new List<object>();
                    List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>> unionFlatList;
                    List<object> unionWheres;
                    WhereExpressionBuilder unionWhereBuilder;
                    List<Tuple<FieldReference, bool>> unionOrderBys;
                    MakeFlatList(rootExpr, unionOns, out unionFlatList, out unionWhereBuilder, out unionWheres, out unionOrderBys);
                    unionWheres = splitState.BaseWhere != null ? new List<object> { splitState.BaseWhere } : new List<object>();

                    var queryBuffer = MakeQueryBuffer(rootExpr, unionOns, unionFlatList, unionWhereBuilder, unionOrderBys, fieldReferences);

                    var existingJoin = queryBuffer.TypeBuffers[split.Key].JoinOn;

                    if (split.Value != null)
                        existingJoin = new ConnectorPart { Left = existingJoin, Op = WhereClauseConnector.AndOperator, Right = split.Value };

                    queryBuffer.TypeBuffers[split.Key].JoinOn = existingJoin;
                    MarkJoinBuffer(splitState.LeafOns, queryBuffer);

                    var queryPlan = new PreparedQueryPlan(true, unionWheres, fieldReferences, unionOns,
                        unionOrderBys, queryBuffer, "");

                    unionParts.Add(Expression.Call(
                        tableMethodInfo,
                        QueryCompilationContext.QueryContextParameter,
                        Expression.Constant(rootExpr.EntityType),
                        Expression.Constant(queryPlan),
                        Expression.Constant(compilationContext.IsTracking)));
                }

                if (splitState.DrivingWhere != null)
                {
                    var queryBuffer = MakeQueryBuffer(rootExpr, processedOns, flatList, whereBuilder, orderBys, fieldReferences);
                    MarkJoinBuffer(splitState.LeafOns, queryBuffer);
                    var queryPlan = new PreparedQueryPlan(true, splitState.DrivingWhere != null ? new List<object> { splitState.DrivingWhere } : new List<object>(), fieldReferences, processedOns,
                        orderBys, queryBuffer, "");
                    unionParts.Add(Expression.Call(
                        tableMethodInfo,
                        QueryCompilationContext.QueryContextParameter,
                        Expression.Constant(rootExpr.EntityType),
                        Expression.Constant(queryPlan),
                        Expression.Constant(compilationContext.IsTracking)));
                }

                //this is already distinct union so we shouldnt return any duplicates from this operation
                var unionMethodInfo = typeof(HarmonyQueryExpression).GetMethod("Union");
                Expression result = null;
                foreach (var exprPart in unionParts)
                {
                    if (result == null)
                        result = exprPart;
                    else
                    {
                        result = Expression.Call(
                            unionMethodInfo,
                            result,
                            exprPart);
                    }
                }

                return result;
            }
            else
            { 
                var queryBuffer = MakeQueryBuffer(rootExpr, processedOns, flatList, whereBuilder, orderBys, fieldReferences);
                var queryPlan = new PreparedQueryPlan(true, processedWheres, fieldReferences, processedOns,
                    orderBys, queryBuffer, "");
                return Expression.Call(
                    tableMethodInfo,
                    QueryCompilationContext.QueryContextParameter,
                    Expression.Constant(rootExpr.EntityType),
                    Expression.Constant(queryPlan),
                    Expression.Constant(compilationContext.IsTracking));
            }

            static void MarkJoinBuffer(Dictionary<int, object> splits, QueryBuffer queryBuffer)
            {
                foreach(var split in splits)
                    queryBuffer.TypeBuffers[split.Key].IsInnerJoin = true;
            }
        }

        private static void MakeFlatList(HarmonyTableExpression rootExpr, List<object> processedOns, out List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>> flatList, out WhereExpressionBuilder whereBuilder, out List<object> processedWheres, out List<Tuple<FieldReference, bool>> orderBys)
        {
            flatList = new List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>>();
            var typeBuffers = new QueryBuffer.TypeBuffer[] { GetTypeBuffer(rootExpr, flatList) };
            var expressionTableMapping = flatList.ToDictionary(kvp => kvp.Item1.RootExpression.ConvertedParameter as Expression, kvp => kvp.Item1 as IHarmonyQueryTable, new ExpressionValueComparer());
            var tableList = flatList.Select(tpl => tpl.Item1 as IHarmonyQueryTable).ToList();
            //extract all of expressions that might represent a given table and add them to the mapping dictionary
            foreach (var table in tableList)
            {
                foreach (var alias in ((HarmonyTableExpression)table).Aliases)
                {
                    if (!expressionTableMapping.ContainsKey(alias))
                    {
                        expressionTableMapping.Add(alias, table);
                    }
                }
            }

            whereBuilder = new WhereExpressionBuilder(rootExpr.IsCaseSensitive, tableList, expressionTableMapping);
            processedWheres = new List<Object>();
            orderBys = new List<Tuple<FileIO.Queryable.FieldReference, bool>>();
            foreach (var expr in rootExpr.WhereExpressions)
            {
                whereBuilder.VisitForWhere(expr, processedWheres, processedOns);
            }
        }

        private static QueryBuffer MakeQueryBuffer(HarmonyTableExpression rootExpr, List<object> processedOns, List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>> flatList, WhereExpressionBuilder whereBuilder, List<Tuple<FieldReference, bool>> orderBys, Dictionary<int, List<FieldDataDefinition>> fieldReferences)
        {
            foreach (var tpl in flatList)
            {
                foreach (var expr in tpl.Item1.OnExpressions)
                {
                    var madeOn = whereBuilder.VisitForOn(expr);
                    if (madeOn != null)
                    {
                        processedOns.Add(madeOn);
                        if (tpl.Item2.JoinOn == null)
                        {
                            tpl.Item2.JoinOn = madeOn;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }

                if (tpl.Item1 != rootExpr)
                {
                    foreach (var expr in tpl.Item1.WhereExpressions)
                    {
                        var madeOn = whereBuilder.VisitForOn(expr);
                        if (madeOn != null)
                        {
                            processedOns.Add(madeOn);
                            if (tpl.Item2.JoinOn != null)
                                madeOn = new ConnectorPart() { Op = WhereClauseConnector.AndOperator, Left = tpl.Item2.JoinOn, Right = madeOn };

                            tpl.Item2.JoinOn = madeOn;
                        }
                    }
                }

                foreach (var expr in tpl.Item1.OrderByExpressions)
                {
                    var fieldRef = whereBuilder.VisitForOrderBy(expr.Item1);
                    if (fieldRef != null)
                    {
                        orderBys.Add(Tuple.Create(fieldRef, expr.Item2));
                    }
                }
            }

            var queryBuffer = new QueryBuffer(flatList.Select(tpl => tpl.Item2).ToList());
            
            foreach (var queryExpr in flatList)
            {
                if (queryExpr.Item1.ReferencedFields.Count > 0)
                {
                    var bufferIndex = queryBuffer.TypeBuffers.IndexOf(queryExpr.Item2);
                    foreach(var refKvp in queryExpr.Item1.ReferencedFields)
                    {
                        var innerBufferIndex = bufferIndex;
                        if (refKvp.Item1 != "")
                        {
                            innerBufferIndex = queryBuffer.TypeBuffers.FindIndex(tbuf => tbuf.ParentFieldName == refKvp.Item1);
                            if (innerBufferIndex < 0)
                                throw new ApplicationException(string.Format("failed to find referenced field parent name {0}", refKvp.Item1));
                        }

                        if (!fieldReferences.TryGetValue(innerBufferIndex, out var fieldDefs))
                        {
                            fieldDefs = new List<FieldDataDefinition>();
                            fieldReferences.Add(innerBufferIndex, fieldDefs);
                        }

#if DEBUG
                        //ensure we dont accidentally point at a different type
                        Debug.Assert(refKvp.Item2 == queryBuffer.TypeBuffers[innerBufferIndex].Metadata.GetFieldByName(refKvp.Item2.LanguageName));
                        
#endif

                        fieldDefs.Add(refKvp.Item2);
                    }
                }
            }
            return queryBuffer;
        }

        public static IEnumerable<DataObjectBase> Union(IEnumerable<DataObjectBase> first, IEnumerable<DataObjectBase> second)
        {
            return first.Union(second);
        }

        public static IEnumerable<DataObjectBase> Distinct(IEnumerable<DataObjectBase> collection)
        {
            return collection.Distinct();
        }


        //need base where
        //driving where
        //leaf on's anded togeather
        class ExpressionSplitState
        {
            public object BaseWhere;
            public object DrivingWhere;
            public Dictionary<int, Object> LeafOns;
        }

        private ExpressionSplitState SplitExpression(object expr)
        {
            if (expr is ConnectorPart conPart)
            {
                var result = new ExpressionSplitState { LeafOns = new Dictionary<int, object>() };
                if (NeedsSplit(conPart))
                {
                    var leftState = SplitExpression(conPart.Left);
                    var rightState = SplitExpression(conPart.Right);

                    ConcatLeaves(conPart, leftState, rightState, result);

                    //we're at the split point now so dont produce a BaseWhere, everything goes either driving table or LeafOns
                    var rightQueryKeys = QuerySourceKeyForExpr(rightState.DrivingWhere);
                    var leftQueryKeys = QuerySourceKeyForExpr(leftState.DrivingWhere);

                    if (rightQueryKeys.Count > 1 || !rightQueryKeys.Contains(0))
                    {
                        if (rightQueryKeys.Count == 1)
                        {
                            var targetKey = rightQueryKeys.First();
                            if (!result.LeafOns.TryAdd(targetKey, rightState.DrivingWhere))
                            {
                                result.LeafOns[targetKey] = new ConnectorPart { Left = result.LeafOns[targetKey], Op = conPart.Op, Right = rightState.DrivingWhere };
                            }
                        }
                        //ignore this leg of the drivingWhere its not for us
                        rightState.DrivingWhere = null;
                    }

                    if (leftQueryKeys.Count > 1 || !leftQueryKeys.Contains(0))
                    {
                        if (leftQueryKeys.Count == 1)
                        {
                            var targetKey = leftQueryKeys.First();
                            if (!result.LeafOns.TryAdd(targetKey, leftState.DrivingWhere))
                            {
                                result.LeafOns[targetKey] = new ConnectorPart { Left = result.LeafOns[targetKey], Op = conPart.Op, Right = leftState.DrivingWhere };
                            }
                        }
                        //ignore this leg of the drivingWhere its not for us
                        leftState.DrivingWhere = null;
                    }

                    MergeDrivingTables(conPart, result, leftState, rightState);
                }
                else
                {
                    var leftState = SplitExpression(conPart.Left);
                    var rightState = SplitExpression(conPart.Right);
                    ConcatLeaves(conPart, leftState, rightState, result);

                    MergeBaseTables(conPart, result, leftState, rightState);
                    MergeDrivingTables(conPart, result, leftState, rightState);
                }
                return result;
            }
            return new ExpressionSplitState { BaseWhere = expr, DrivingWhere = expr, LeafOns = new Dictionary<int, object>() };

            static void ConcatLeaves(ConnectorPart conPart, ExpressionSplitState leftState, ExpressionSplitState rightState, ExpressionSplitState result)
            {
                foreach (var item in leftState.LeafOns.Concat(rightState.LeafOns))
                {
                    if (!result.LeafOns.TryAdd(item.Key, item.Value))
                    {
                        result.LeafOns[item.Key] = new ConnectorPart { Left = result.LeafOns[item.Key], Op = conPart.Op, Right = item.Value };
                    }
                }
            }

            static void MergeDrivingTables(ConnectorPart conPart, ExpressionSplitState result, ExpressionSplitState leftState, ExpressionSplitState rightState)
            {
                if (rightState.DrivingWhere != null && leftState.DrivingWhere != null)
                {
                    result.DrivingWhere = new ConnectorPart { Left = leftState.DrivingWhere, Op = conPart.Op, Right = rightState.DrivingWhere };
                }
                else
                {
                    result.DrivingWhere = leftState.DrivingWhere ?? rightState.DrivingWhere;
                }
            }

            static void MergeBaseTables(ConnectorPart conPart, ExpressionSplitState result, ExpressionSplitState leftState, ExpressionSplitState rightState)
            {
                if (rightState.BaseWhere != null && leftState.BaseWhere != null)
                {
                    result.BaseWhere = new ConnectorPart { Left = leftState.BaseWhere, Op = conPart.Op, Right = rightState.BaseWhere };
                }
                else
                {
                    result.BaseWhere = leftState.BaseWhere ?? rightState.BaseWhere;
                }
            }
        }

        private bool NeedsSplit(ConnectorPart part)
        {
            if(part.Op == WhereClauseConnector.OrOperator)
            {
                //we're in an OR but the left and right have different source id's
                var leftSources = QuerySourceKeyForExpr(part.Left);
                var rightSources = QuerySourceKeyForExpr(part.Right);
                return leftSources != rightSources;
            }
            else
            {
                return false;
            }
        }

        private HashSet<int> QuerySourceKeyForExpr(object part)
        {
            var queryKeys = new HashSet<int>();
            if (part is ExprPart expr)
            {
                queryKeys.Add(QuerySourceKeyForExpr(expr));
            }
            else if(part is ConnectorPart conPart)
            {
                QuerySourceKeyForExpr(conPart, queryKeys);
            }
            return queryKeys;
        }

        private void QuerySourceKeyForExpr(ConnectorPart part, HashSet<int> queryKeys)
        {
            var result = new HashSet<int>();
            if (part.Left is ExprPart leftRef)
            {
                var leftKey = QuerySourceKeyForExpr(leftRef);
                if(!queryKeys.Contains(leftKey))
                {
                    queryKeys.Add(leftKey);
                }
            }
            else if (part.Right is ExprPart rightRef)
            {
                var rightKey = QuerySourceKeyForExpr(rightRef);
                if (!queryKeys.Contains(rightKey))
                {
                    queryKeys.Add(rightKey);
                }
            }
            else if(part.Left is ConnectorPart leftCon)
            {
                QuerySourceKeyForExpr(leftCon, queryKeys);
            }
            else if (part.Right is ConnectorPart rightCon)
            {
                QuerySourceKeyForExpr(rightCon, queryKeys);
            }
        }

        private int QuerySourceKeyForExpr(ExprPart part)
        {
            if (part.Left is FieldReference leftRef)
            {
                return leftRef.QuerySourceKey;
            }
            else if (part.Right is FieldReference rightRef)
            {
                return rightRef.QuerySourceKey;
            }
            else
                return -1;
        }

        public virtual ParameterExpression CurrentParameter => _groupingParameter ?? _valueBufferParameter;
        public Expression ConvertedParameter { get; }
        public override Type Type { get; }
        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public HarmonyQueryExpression(IEntityType entityType)
        {
            Type = typeof(IQueryable<>).MakeGenericType(new Type[] { entityType.ClrType });
            _valueBufferParameter = Parameter(typeof(DataObjectBase), "valueBuffer");
            var rootTable = new HarmonyTableExpression(entityType, "", this);
            RootExpressions = new Dictionary<Expression, HarmonyTableExpression> { { CurrentParameter, rootTable } };
            ServerQueryExpression = rootTable;
            //var readExpressionMap = new Dictionary<IProperty, Expression>();
            //foreach (var property in entityType.GetAllBaseTypesInclusive().SelectMany(et => et.GetDeclaredProperties()))
            //{
            //    readExpressionMap[property] = CreateReadValueExpression(property.ClrType, property.GetIndex(), property);
            //}

            //foreach (var property in entityType.GetDerivedTypes().SelectMany(et => et.GetDeclaredProperties()))
            //{
            //    readExpressionMap[property] = CreateReadValueExpression(property.ClrType, property.GetIndex(), property);
            //}

            //var entityProjection = new EntityProjectionExpression(entityType, readExpressionMap);
            _projectionMapping[new ProjectionMember()] = ConvertedParameter = Expression.Convert(CurrentParameter, entityType.ClrType);
        }

        public virtual Expression GetSingleScalarProjection()
        {
            ConvertToEnumerable();
            var expression = CreateReadValueExpression(ServerQueryExpression.Type, 0, null);

            return expression;
        }

        public virtual void ConvertToEnumerable()
        {
            if (ServerQueryExpression.Type.TryGetSequenceType() == null)
            {
                if (ServerQueryExpression.Type != typeof(DataObjectBase))
                {
                    if (ServerQueryExpression.Type.IsValueType)
                    {
                        ServerQueryExpression = Convert(ServerQueryExpression, typeof(object));
                    }

                    var resultEnumerableType = typeof(ResultEnumerable<>).MakeGenericType(ServerQueryExpression.Type);
                    var funcType = typeof(Func<>).MakeGenericType(ServerQueryExpression.Type);
                    
                    ServerQueryExpression = New(
                        resultEnumerableType.GetConstructors().Single(),
                        Lambda(funcType, ServerQueryExpression));
                }
                else
                {
                    ServerQueryExpression = New(
                        typeof(ResultEnumerable<DataObjectBase>).GetConstructors().Single(),
                        Lambda<Func<DataObjectBase>>(ServerQueryExpression));
                }
            }
        }

        public virtual void ReplaceProjectionMapping(IDictionary<ProjectionMember, Expression> projectionMappings)
        {
            _projectionMapping.Clear();
            foreach (var kvp in projectionMappings)
            {
                _projectionMapping[kvp.Key] = kvp.Value;
            }
        }

        public virtual IDictionary<IProperty, int> AddToProjection(EntityProjectionExpression entityProjectionExpression)
        {
            if (!_entityProjectionCache.TryGetValue(entityProjectionExpression, out var indexMap))
            {
                indexMap = new Dictionary<IProperty, int>();
                foreach (var property in GetAllPropertiesInHierarchy(entityProjectionExpression.EntityType))
                {
                    indexMap[property] = AddToProjection(entityProjectionExpression.BindProperty(property));
                }

                _entityProjectionCache[entityProjectionExpression] = indexMap;
            }

            return indexMap;
        }

        public virtual int AddToProjection(Expression expression)
        {
            _valueBufferSlots.Add(expression);

            return _valueBufferSlots.Count - 1;
        }

        public virtual int AddSubqueryProjection(ShapedQueryExpression shapedQueryExpression, out Expression innerShaper)
        {
            var subquery = (HarmonyQueryExpression)shapedQueryExpression.QueryExpression;
            subquery.ApplyProjection();
            var serverQueryExpression = subquery.ServerQueryExpression;

            if (serverQueryExpression is MethodCallExpression selectMethodCall
                && selectMethodCall.Arguments[0].Type == typeof(ResultEnumerable<DataObjectBase>))
            {
                var terminatingMethodCall =
                    (MethodCallExpression)((LambdaExpression)((NewExpression)selectMethodCall.Arguments[0]).Arguments[0]).Body;
                selectMethodCall = selectMethodCall.Update(
                    null, new[] { terminatingMethodCall.Arguments[0], selectMethodCall.Arguments[1] });
                serverQueryExpression = terminatingMethodCall.Update(null, new[] { selectMethodCall });
            }

            innerShaper = new ShaperRemappingExpressionVisitor(subquery._projectionMapping)
                .Visit(shapedQueryExpression.ShaperExpression);

            innerShaper = Lambda(innerShaper, subquery.CurrentParameter);

            return AddToProjection(serverQueryExpression);
        }

        private sealed class ShaperRemappingExpressionVisitor : ExpressionVisitor
        {
            private readonly IDictionary<ProjectionMember, Expression> _projectionMapping;

            public ShaperRemappingExpressionVisitor(IDictionary<ProjectionMember, Expression> projectionMapping)
            {
                _projectionMapping = projectionMapping;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression is ProjectionBindingExpression projectionBindingExpression
                    && projectionBindingExpression.ProjectionMember != null)
                {
                    var mappingValue = ((ConstantExpression)_projectionMapping[projectionBindingExpression.ProjectionMember]).Value;
                    if (mappingValue is IDictionary<IProperty, int> indexMap)
                    {
                        return new ProjectionBindingExpression(projectionBindingExpression.QueryExpression, indexMap);
                    }

                    if (mappingValue is int index)
                    {
                        return new ProjectionBindingExpression(
                            projectionBindingExpression.QueryExpression, index, projectionBindingExpression.Type);
                    }

                    throw new InvalidOperationException("Invalid ProjectionMapping.");
                }

                return base.Visit(expression);
            }
        }

        private IEnumerable<IProperty> GetAllPropertiesInHierarchy(IEntityType entityType)
            => entityType.GetTypesInHierarchy().SelectMany(Microsoft.EntityFrameworkCore.EntityTypeExtensions.GetDeclaredProperties);

        public virtual Expression GetMappedProjection(ProjectionMember member)
            => _projectionMapping[member];

        public virtual void PushdownIntoSubquery()
        {
            var clientProjection = _valueBufferSlots.Count != 0;
            if (!clientProjection)
            {
                var result = new Dictionary<ProjectionMember, Expression>();
                foreach (var keyValuePair in _projectionMapping)
                {
                    if (keyValuePair.Value is EntityProjectionExpression entityProjection)
                    {
                        var map = new Dictionary<IProperty, Expression>();
                        foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                        {
                            var expressionToAdd = entityProjection.BindProperty(property);
                            var index = AddToProjection(expressionToAdd);
                            map[property] = CreateReadValueExpression(expressionToAdd.Type, index, property);
                        }

                        result[keyValuePair.Key] = new EntityProjectionExpression(entityProjection.EntityType, map);
                    }
                    else
                    {
                        var index = AddToProjection(keyValuePair.Value);
                        result[keyValuePair.Key] = CreateReadValueExpression(
                            keyValuePair.Value.Type, index, InferPropertyFromInner(keyValuePair.Value));
                    }
                }

                _projectionMapping = result;
            }

            _groupingParameter = null;

            if (clientProjection)
            {
                var newValueBufferSlots = _valueBufferSlots
                    .Select((e, i) => CreateReadValueExpression(e.Type, i, InferPropertyFromInner(e)))
                    .ToList();

                _valueBufferSlots.Clear();
                _valueBufferSlots.AddRange(newValueBufferSlots);
            }
            else
            {
                _valueBufferSlots.Clear();
            }
        }

        public virtual void ApplyDefaultIfEmpty()
        {
            if (_valueBufferSlots.Count != 0)
            {
                throw new InvalidOperationException("Cannot apply DefaultIfEmpty after a client-evaluated projection.");
            }

            var result = new Dictionary<ProjectionMember, Expression>();
            foreach (var keyValuePair in _projectionMapping)
            {
                if (keyValuePair.Value is EntityProjectionExpression entityProjection)
                {
                    var map = new Dictionary<IProperty, Expression>();
                    foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                    {
                        var expressionToAdd = entityProjection.BindProperty(property);
                        var index = AddToProjection(expressionToAdd);
                        map[property] = CreateReadValueExpression(expressionToAdd.Type.MakeNullable(), index, property);
                    }

                    result[keyValuePair.Key] = new EntityProjectionExpression(entityProjection.EntityType, map);
                }
                else
                {
                    var index = AddToProjection(keyValuePair.Value);
                    result[keyValuePair.Key] = CreateReadValueExpression(
                        keyValuePair.Value.Type.MakeNullable(), index, InferPropertyFromInner(keyValuePair.Value));
                }
            }

            _projectionMapping = result;
            _groupingParameter = null;

            _valueBufferSlots.Clear();
        }

        private static IPropertyBase InferPropertyFromInner(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression
                && methodCallExpression.Method.IsGenericMethod
                && methodCallExpression.Method.GetGenericMethodDefinition() == EntityMaterializerSource.TryReadValueMethod)
            {
                return (IPropertyBase)((ConstantExpression)methodCallExpression.Arguments[2]).Value;
            }

            return null;
        }

        public virtual void ApplyProjection()
        {
            if (_valueBufferSlots.Count == 0)
            {
                var result = new Dictionary<ProjectionMember, Expression>();
                foreach (var keyValuePair in _projectionMapping)
                {
                    if (keyValuePair.Value is EntityProjectionExpression entityProjection)
                    {
                        var map = new Dictionary<IProperty, int>();
                        foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                        {
                            map[property] = AddToProjection(entityProjection.BindProperty(property));
                        }

                        result[keyValuePair.Key] = Constant(map);
                    }
                    else
                    {
                        result[keyValuePair.Key] = Constant(AddToProjection(keyValuePair.Value));
                    }
                }

                _projectionMapping = result;
            }
        }

        public virtual HarmonyGroupByShaperExpression ApplyGrouping(Expression groupingKey, Expression shaperExpression)
        {
            PushdownIntoSubquery();

            var selectMethod = (MethodCallExpression)ServerQueryExpression;
            var groupBySource = selectMethod.Arguments[0];
            var elementSelector = selectMethod.Arguments[1];
            _groupingParameter = Parameter(typeof(IGrouping<DataObjectBase, DataObjectBase>), "grouping");
            var groupingKeyAccessExpression = PropertyOrField(_groupingParameter, nameof(IGrouping<int, int>.Key));
            var groupingKeyExpressions = new List<Expression>();
            groupingKey = GetGroupingKey(groupingKey, groupingKeyExpressions, groupingKeyAccessExpression);
            //var keySelector = Lambda(
            //    New(
            //        _valueBufferConstructor,
            //        NewArrayInit(
            //            typeof(object),
            //            groupingKeyExpressions.Select(e => e.Type.IsValueType ? Convert(e, typeof(object)) : e))),
            //    _valueBufferParameter);

            //ServerQueryExpression = Call(
            //    EnumerableMethods.GroupByWithKeyElementSelector.MakeGenericMethod(
            //        typeof(DataObjectBase), typeof(DataObjectBase), typeof(DataObjectBase)),
            //    selectMethod.Arguments[0],
            //    keySelector,
            //    selectMethod.Arguments[1]);

            throw new NotImplementedException();

            return new HarmonyGroupByShaperExpression(
                groupingKey,
                shaperExpression,
                _groupingParameter,
                _valueBufferParameter);
        }

        private Expression GetGroupingKey(Expression key, List<Expression> groupingExpressions, Expression groupingKeyAccessExpression)
        {
            switch (key)
            {
                case NewExpression newExpression:
                    var arguments = new Expression[newExpression.Arguments.Count];
                    for (var i = 0; i < arguments.Length; i++)
                    {
                        arguments[i] = GetGroupingKey(newExpression.Arguments[i], groupingExpressions, groupingKeyAccessExpression);
                    }

                    return newExpression.Update(arguments);

                case MemberInitExpression memberInitExpression:
                    if (memberInitExpression.Bindings.Any(mb => !(mb is MemberAssignment)))
                    {
                        goto default;
                    }

                    var updatedNewExpression = (NewExpression)GetGroupingKey(
                        memberInitExpression.NewExpression, groupingExpressions, groupingKeyAccessExpression);
                    var memberBindings = new MemberAssignment[memberInitExpression.Bindings.Count];
                    for (var i = 0; i < memberBindings.Length; i++)
                    {
                        var memberAssignment = (MemberAssignment)memberInitExpression.Bindings[i];
                        memberBindings[i] = memberAssignment.Update(
                            GetGroupingKey(
                                memberAssignment.Expression,
                                groupingExpressions,
                                groupingKeyAccessExpression));
                    }

                    return memberInitExpression.Update(updatedNewExpression, memberBindings);

                default:
                    var index = groupingExpressions.Count;
                    groupingExpressions.Add(key);
                    return CreateReadValueExpression(
                        groupingKeyAccessExpression,
                        key.Type,
                        index,
                        InferPropertyFromInner(key));
            }
        }

        private static Expression CreateReadValueExpression(
            Expression valueBufferParameter, Type type, int index, IPropertyBase property)

        {
            if (property != null)
                return Expression.PropertyOrField(Expression.Convert(valueBufferParameter, property.DeclaringType.ClrType), property.Name);
            else
                return valueBufferParameter;
        }
            /*Call(
                EntityMaterializerSource.TryReadValueMethod.MakeGenericMethod(type),
                valueBufferParameter,
                Constant(index),
                Constant(property, typeof(IPropertyBase)));*/

        private Expression CreateReadValueExpression(Type type, int index, IPropertyBase property)
            => CreateReadValueExpression(_valueBufferParameter, type, index, property);

        public virtual void AddInnerJoin(
            HarmonyQueryExpression innerQueryExpression,
            LambdaExpression outerKeySelector,
            LambdaExpression innerKeySelector,
            Type transparentIdentifierType)
        {
            var outerParameter = Parameter(typeof(DataObjectBase), "outer");
            var innerParameter = Parameter(typeof(DataObjectBase), "inner");
            var resultValueBufferExpressions = new List<Expression>();
            var projectionMapping = new Dictionary<ProjectionMember, Expression>();
            var replacingVisitor = new ReplacingExpressionVisitor(
                new Dictionary<Expression, Expression>
                {
                    { CurrentParameter, outerParameter }, { innerQueryExpression.CurrentParameter, innerParameter }
                });

            var index = 0;
            var outerMemberInfo = transparentIdentifierType.GetTypeInfo().GetDeclaredField("Outer");
            foreach (var projection in _projectionMapping)
            {
                if (projection.Value is EntityProjectionExpression entityProjection)
                {
                    var readExpressionMap = new Dictionary<IProperty, Expression>();
                    foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                    {
                        var replacedExpression = replacingVisitor.Visit(entityProjection.BindProperty(property));
                        resultValueBufferExpressions.Add(replacedExpression);
                        readExpressionMap[property] = CreateReadValueExpression(replacedExpression.Type, index++, property);
                    }

                    projectionMapping[projection.Key.Prepend(outerMemberInfo)]
                        = new EntityProjectionExpression(entityProjection.EntityType, readExpressionMap);
                }
                else
                {
                    resultValueBufferExpressions.Add(replacingVisitor.Visit(projection.Value));
                    projectionMapping[projection.Key.Prepend(outerMemberInfo)]
                        = CreateReadValueExpression(projection.Value.Type, index++, InferPropertyFromInner(projection.Value));
                }
            }

            var innerMemberInfo = transparentIdentifierType.GetTypeInfo().GetDeclaredField("Inner");
            foreach (var projection in innerQueryExpression._projectionMapping)
            {
                if (projection.Value is EntityProjectionExpression entityProjection)
                {
                    var readExpressionMap = new Dictionary<IProperty, Expression>();
                    foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                    {
                        var replacedExpression = replacingVisitor.Visit(entityProjection.BindProperty(property));
                        resultValueBufferExpressions.Add(replacedExpression);
                        readExpressionMap[property] = CreateReadValueExpression(replacedExpression.Type, index++, property);
                    }

                    projectionMapping[projection.Key.Prepend(innerMemberInfo)]
                        = new EntityProjectionExpression(entityProjection.EntityType, readExpressionMap);
                }
                else
                {
                    resultValueBufferExpressions.Add(replacingVisitor.Visit(projection.Value));
                    projectionMapping[projection.Key.Prepend(innerMemberInfo)]
                        = CreateReadValueExpression(projection.Value.Type, index++, InferPropertyFromInner(projection.Value));
                }
            }

            //var resultSelector = Lambda(
            //    New(
            //        _valueBufferConstructor,
            //        NewArrayInit(
            //            typeof(object),
            //            resultValueBufferExpressions
            //                .Select(e => e.Type.IsValueType ? Convert(e, typeof(object)) : e)
            //                .ToArray())),
            //    outerParameter,
            //    innerParameter);

            //ServerQueryExpression = Call(
            //    EnumerableMethods.Join.MakeGenericMethod(
            //        typeof(DataObjectBase), typeof(DataObjectBase), outerKeySelector.ReturnType, typeof(DataObjectBase)),
            //    ServerQueryExpression,
            //    innerQueryExpression.ServerQueryExpression,
            //    outerKeySelector,
            //    innerKeySelector,
            //    resultSelector);

            throw new NotImplementedException();

            _projectionMapping = projectionMapping;
        }

        public virtual void AddLeftJoin(
            HarmonyQueryExpression innerQueryExpression,
            Expression outerKeySelector,
            Expression innerKeySelector)
        {
            var serverQueryExpression = innerQueryExpression.FindServerExpression();
            if (!RootExpressions.ContainsKey(innerQueryExpression.CurrentParameter))
            {
                RootExpressions.Add(innerQueryExpression.CurrentParameter, serverQueryExpression);
            }

            serverQueryExpression.OnExpressions.Add(Expression.Equal(innerKeySelector, outerKeySelector));
        }

        public virtual void AddSelectMany(HarmonyQueryExpression innerQueryExpression, Type transparentIdentifierType, bool innerNullable)
        {
            var outerParameter = Parameter(typeof(DataObjectBase), "outer");
            var innerParameter = Parameter(typeof(DataObjectBase), "inner");
            var resultValueBufferExpressions = new List<Expression>();
            var projectionMapping = new Dictionary<ProjectionMember, Expression>();
            var replacingVisitor = new ReplacingExpressionVisitor(
                new Dictionary<Expression, Expression>
                {
                    { CurrentParameter, outerParameter }, { innerQueryExpression.CurrentParameter, innerParameter }
                });

            var index = 0;
            var outerMemberInfo = transparentIdentifierType.GetTypeInfo().GetDeclaredField("Outer");
            foreach (var projection in _projectionMapping)
            {
                if (projection.Value is EntityProjectionExpression entityProjection)
                {
                    var readExpressionMap = new Dictionary<IProperty, Expression>();
                    foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                    {
                        var replacedExpression = replacingVisitor.Visit(entityProjection.BindProperty(property));
                        resultValueBufferExpressions.Add(replacedExpression);
                        readExpressionMap[property] = CreateReadValueExpression(replacedExpression.Type, index++, property);
                    }

                    projectionMapping[projection.Key.Prepend(outerMemberInfo)]
                        = new EntityProjectionExpression(entityProjection.EntityType, readExpressionMap);
                }
                else
                {
                    resultValueBufferExpressions.Add(replacingVisitor.Visit(projection.Value));
                    projectionMapping[projection.Key.Prepend(outerMemberInfo)]
                        = CreateReadValueExpression(projection.Value.Type, index++, InferPropertyFromInner(projection.Value));
                }
            }

            var innerMemberInfo = transparentIdentifierType.GetTypeInfo().GetDeclaredField("Inner");
            var nullableReadValueExpressionVisitor = new NullableReadValueExpressionVisitor();
            foreach (var projection in innerQueryExpression._projectionMapping)
            {
                if (projection.Value is EntityProjectionExpression entityProjection)
                {
                    var readExpressionMap = new Dictionary<IProperty, Expression>();
                    foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                    {
                        var replacedExpression = replacingVisitor.Visit(entityProjection.BindProperty(property));
                        if (innerNullable)
                        {
                            replacedExpression = nullableReadValueExpressionVisitor.Visit(replacedExpression);
                        }

                        resultValueBufferExpressions.Add(replacedExpression);
                        readExpressionMap[property] = CreateReadValueExpression(replacedExpression.Type, index++, property);
                    }

                    projectionMapping[projection.Key.Prepend(innerMemberInfo)]
                        = new EntityProjectionExpression(entityProjection.EntityType, readExpressionMap);
                }
                else
                {
                    var replacedExpression = replacingVisitor.Visit(projection.Value);
                    if (innerNullable)
                    {
                        replacedExpression = nullableReadValueExpressionVisitor.Visit(replacedExpression);
                    }

                    resultValueBufferExpressions.Add(replacedExpression);
                    projectionMapping[projection.Key.Prepend(innerMemberInfo)]
                        = CreateReadValueExpression(replacedExpression.Type, index++, InferPropertyFromInner(projection.Value));
                }
            }

            //var resultSelector = Lambda(
            //    New(
            //        _valueBufferConstructor,
            //        NewArrayInit(
            //            typeof(object),
            //            resultValueBufferExpressions
            //                .Select(e => e.Type.IsValueType ? Convert(e, typeof(object)) : e)
            //                .ToArray())),
            //    outerParameter,
            //    innerParameter);

            //ServerQueryExpression = Call(
            //    EnumerableMethods.SelectManyWithCollectionSelector.MakeGenericMethod(
            //        typeof(DataObjectBase), typeof(DataObjectBase), typeof(DataObjectBase)),
            //    ServerQueryExpression,
            //    Lambda(innerQueryExpression.ServerQueryExpression, CurrentParameter),
            //    resultSelector);


            throw new NotImplementedException();

            _projectionMapping = projectionMapping;
        }

        public virtual EntityShaperExpression AddNavigationToWeakEntityType(
            EntityProjectionExpression entityProjectionExpression,
            INavigation navigation,
            HarmonyQueryExpression innerQueryExpression,
            LambdaExpression outerKeySelector,
            LambdaExpression innerKeySelector)
        {
            // GroupJoin phase
            var groupTransparentIdentifierType = TransparentIdentifierFactory.Create(
                typeof(DataObjectBase), typeof(IEnumerable<DataObjectBase>));
            var outerParameter = Parameter(typeof(DataObjectBase), "outer");
            var innerParameter = Parameter(typeof(IEnumerable<DataObjectBase>), "inner");
            var outerMemberInfo = groupTransparentIdentifierType.GetTypeInfo().GetDeclaredField("Outer");
            var innerMemberInfo = groupTransparentIdentifierType.GetTypeInfo().GetDeclaredField("Inner");
            var resultSelector = Lambda(
                New(
                    groupTransparentIdentifierType.GetTypeInfo().DeclaredConstructors.Single(),
                    new[] { outerParameter, innerParameter }, outerMemberInfo, innerMemberInfo),
                outerParameter,
                innerParameter);

            var groupJoinExpression = Call(
                EnumerableMethods.GroupJoin.MakeGenericMethod(
                    typeof(DataObjectBase), typeof(DataObjectBase), outerKeySelector.ReturnType, groupTransparentIdentifierType),
                ServerQueryExpression,
                innerQueryExpression.ServerQueryExpression,
                outerKeySelector,
                innerKeySelector,
                resultSelector);

            // SelectMany phase
            var collectionParameter = Parameter(groupTransparentIdentifierType, "collection");
            var collection = MakeMemberAccess(collectionParameter, innerMemberInfo);
            outerParameter = Parameter(groupTransparentIdentifierType, "outer");
            innerParameter = Parameter(typeof(DataObjectBase), "inner");

            var resultValueBufferExpressions = new List<Expression>();
            var projectionMapping = new Dictionary<ProjectionMember, Expression>();
            var replacingVisitor = new ReplacingExpressionVisitor(
                new Dictionary<Expression, Expression>
                {
                    { CurrentParameter, MakeMemberAccess(outerParameter, outerMemberInfo) },
                    { innerQueryExpression.CurrentParameter, innerParameter }
                });
            var index = 0;

            EntityProjectionExpression copyEntityProjectionToOuter(EntityProjectionExpression entityProjection)
            {
                var readExpressionMap = new Dictionary<IProperty, Expression>();
                foreach (var property in GetAllPropertiesInHierarchy(entityProjection.EntityType))
                {
                    var replacedExpression = replacingVisitor.Visit(entityProjection.BindProperty(property));
                    resultValueBufferExpressions.Add(replacedExpression);
                    readExpressionMap[property] = CreateReadValueExpression(replacedExpression.Type, index++, property);
                }

                var newEntityProjection = new EntityProjectionExpression(entityProjection.EntityType, readExpressionMap);
                if (ReferenceEquals(entityProjectionExpression, entityProjection))
                {
                    entityProjectionExpression = newEntityProjection;
                }

                // Also lift nested entity projections
                foreach (var navigation in entityProjection.EntityType.GetTypesInHierarchy()
                    .SelectMany(Microsoft.EntityFrameworkCore.EntityTypeExtensions.GetDeclaredNavigations))
                {
                    var boundEntityShaperExpression = entityProjection.BindNavigation(navigation);
                    if (boundEntityShaperExpression != null)
                    {
                        var innerEntityProjection = (EntityProjectionExpression)boundEntityShaperExpression.ValueBufferExpression;
                        var newInnerEntityProjection = copyEntityProjectionToOuter(innerEntityProjection);
                        boundEntityShaperExpression = boundEntityShaperExpression.Update(newInnerEntityProjection);
                        newEntityProjection.AddNavigationBinding(navigation, boundEntityShaperExpression);
                    }
                }

                return newEntityProjection;
            }

            foreach (var projection in _projectionMapping)
            {
                if (projection.Value is EntityProjectionExpression entityProjection)
                {
                    projectionMapping[projection.Key] = copyEntityProjectionToOuter(entityProjection);
                }
                else
                {
                    var replacedExpression = replacingVisitor.Visit(projection.Value);
                    resultValueBufferExpressions.Add(replacedExpression);
                    projectionMapping[projection.Key]
                        = CreateReadValueExpression(replacedExpression.Type, index++, InferPropertyFromInner(projection.Value));
                }
            }

            _projectionMapping = projectionMapping;

            var outerIndex = index;
            var nullableReadValueExpressionVisitor = new NullableReadValueExpressionVisitor();
            var innerEntityProjection = (EntityProjectionExpression)innerQueryExpression.GetMappedProjection(new ProjectionMember());

            var innerReadExpressionMap = new Dictionary<IProperty, Expression>();
            foreach (var property in GetAllPropertiesInHierarchy(innerEntityProjection.EntityType))
            {
                var replacedExpression = replacingVisitor.Visit(innerEntityProjection.BindProperty(property));
                replacedExpression = nullableReadValueExpressionVisitor.Visit(replacedExpression);
                resultValueBufferExpressions.Add(replacedExpression);
                innerReadExpressionMap[property] = CreateReadValueExpression(replacedExpression.Type, index++, property);
            }

            innerEntityProjection = new EntityProjectionExpression(innerEntityProjection.EntityType, innerReadExpressionMap);

            //var collectionSelector = Lambda(
            //    Call(
            //        EnumerableMethods.DefaultIfEmptyWithArgument.MakeGenericMethod(typeof(DataObjectBase)),
            //        collection,
            //        New(
            //            _valueBufferConstructor,
            //            NewArrayInit(
            //                typeof(object),
            //                Enumerable.Range(0, index - outerIndex).Select(i => Constant(null))))),
            //    collectionParameter);

            //resultSelector = Lambda(
            //    New(
            //        _valueBufferConstructor,
            //        NewArrayInit(
            //            typeof(object),
            //            resultValueBufferExpressions
            //                .Select(e => e.Type.IsValueType ? Convert(e, typeof(object)) : e)
            //                .ToArray())),
            //    outerParameter,
            //    innerParameter);

            //ServerQueryExpression = Call(
            //    EnumerableMethods.SelectManyWithCollectionSelector.MakeGenericMethod(
            //        groupTransparentIdentifierType, typeof(DataObjectBase), typeof(DataObjectBase)),
            //    groupJoinExpression,
            //    collectionSelector,
            //    resultSelector);

            throw new NotImplementedException();

            var entityShaper = new EntityShaperExpression(innerEntityProjection.EntityType, innerEntityProjection, nullable: true);
            entityProjectionExpression.AddNavigationBinding(navigation, entityShaper);

            return entityShaper;
        }

        public virtual void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.AppendLine(nameof(HarmonyQueryExpression) + ": ");
            using (expressionPrinter.Indent())
            {
                expressionPrinter.AppendLine(nameof(ServerQueryExpression) + ": ");
                using (expressionPrinter.Indent())
                {
                    expressionPrinter.Visit(ServerQueryExpression);
                }

                expressionPrinter.AppendLine("ProjectionMapping:");
                using (expressionPrinter.Indent())
                {
                    foreach (var projectionMapping in _projectionMapping)
                    {
                        expressionPrinter.Append("Member: " + projectionMapping.Key + " Projection: ");
                        expressionPrinter.Visit(projectionMapping.Value);
                    }
                }

                expressionPrinter.AppendLine();
            }
        }

        private sealed class NullableReadValueExpressionVisitor : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.IsGenericMethod
                    && methodCallExpression.Method.GetGenericMethodDefinition() == EntityMaterializerSource.TryReadValueMethod
                    && !methodCallExpression.Type.IsNullableType())
                {
                    return Call(
                        EntityMaterializerSource.TryReadValueMethod.MakeGenericMethod(methodCallExpression.Type.MakeNullable()),
                        methodCallExpression.Arguments);
                }

                return base.VisitMethodCall(methodCallExpression);
            }

            protected override Expression VisitConditional(ConditionalExpression conditionalExpression)
            {
                var test = Visit(conditionalExpression.Test);
                var ifTrue = Visit(conditionalExpression.IfTrue);
                var ifFalse = Visit(conditionalExpression.IfFalse);

                if (ifTrue.Type.IsNullableType()
                    && conditionalExpression.IfTrue.Type == ifTrue.Type.UnwrapNullableType()
                    && ifFalse is DefaultExpression)
                {
                    ifFalse = Default(ifTrue.Type);
                }

                return Condition(test, ifTrue, ifFalse);
            }
        }
    }
}
