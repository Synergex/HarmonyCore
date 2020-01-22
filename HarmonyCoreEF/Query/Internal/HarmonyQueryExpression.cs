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
                ParentFieldName = table.Name,
                Metadata = DataObjectMetadataBase.LookupType(table.ItemType)
            };
            flatList.Add(Tuple.Create(table, made));
            made.JoinedBuffers = queryTables.OfType<HarmonyTableExpression>().Select(qt => GetTypeBuffer(qt, flatList)).ToList();
            return made;
        }

        public PreparedQueryPlan PrepareQuery(HarmonyQueryCompilationContext compilationContext)
        {
            var rootExpr = RootExpressions[_valueBufferParameter];
            var processedOns = new List<object>();
            var flatList = new List<Tuple<HarmonyTableExpression, QueryBuffer.TypeBuffer>>();
            var typeBuffers = new QueryBuffer.TypeBuffer[] { GetTypeBuffer(rootExpr, flatList) };
            var expressionTableMapping = flatList.ToDictionary(kvp => kvp.Item1.RootExpression.ConvertedParameter as Expression, kvp => kvp.Item1 as IHarmonyQueryTable, new ExpressionValueComparer());
            var tableList = flatList.Select(tpl => tpl.Item1 as IHarmonyQueryTable).ToList();
            //extract all of expressions that might represent a given table and add them to the mapping dictionary
            foreach (var table in tableList)
            {
                foreach(var alias in ((HarmonyTableExpression)table).Aliases)
                {
                    if (!expressionTableMapping.ContainsKey(alias))
                    {
                        expressionTableMapping.Add(alias, table);
                    }
                }
            }

            var whereBuilder = new WhereExpressionBuilder(rootExpr.IsCaseSensitive, tableList, expressionTableMapping);

            var processedWheres = new List<Object>();
            var orderBys = new List<Tuple<FileIO.Queryable.FieldReference, bool>>();
            foreach (var expr in rootExpr.WhereExpressions)
            {
                whereBuilder.VisitForWhere(expr, processedWheres, processedOns);
            }

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

                foreach (var expr in tpl.Item1.OrderByExpressions)
                {
                    var fieldRef = whereBuilder.VisitForOrderBy(expr.Item1);
                    if (fieldRef != null)
                    {
                        orderBys.Add(Tuple.Create(fieldRef, expr.Item2));
                    }
                }
            }

            var queryPlan = new PreparedQueryPlan(true, processedWheres, new Dictionary<int, List<FieldDataDefinition>>(), processedOns,
                orderBys, new QueryBuffer(flatList.Select(tpl => tpl.Item2).ToList()), "");

            return queryPlan;
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
            var expression = CreateReadValueExpression(ServerQueryExpression.Type, 0, null);
            _projectionMapping.Clear();
            _projectionMapping[new ProjectionMember()] = expression;

            ConvertToEnumerable();

            return new ProjectionBindingExpression(this, new ProjectionMember(), expression.Type);
        }

        public virtual void ConvertToEnumerable()
        {
            if (ServerQueryExpression.Type.TryGetSequenceType() == null)
            {
                if (ServerQueryExpression.Type != typeof(DataObjectBase))
                {
                    //if (ServerQueryExpression.Type.IsValueType)
                    //{
                    //    ServerQueryExpression = Convert(ServerQueryExpression, typeof(object));
                    //}

                    //ServerQueryExpression = New(
                    //    typeof(ResultEnumerable).GetConstructors().Single(),
                    //    Lambda<Func<DataObjectBase>>(
                    //        New(
                    //            _valueBufferConstructor,
                    //            NewArrayInit(typeof(object), ServerQueryExpression))));
                    throw new NotImplementedException();
                }
                else
                {
                    ServerQueryExpression = New(
                        typeof(ResultEnumerable).GetConstructors().Single(),
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
                && selectMethodCall.Arguments[0].Type == typeof(ResultEnumerable))
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
            if (!RootExpressions.ContainsKey(innerQueryExpression.CurrentParameter))
            {
                RootExpressions.Add(innerQueryExpression.CurrentParameter, innerQueryExpression.ServerQueryExpression as HarmonyTableExpression);
            }

            (innerQueryExpression.ServerQueryExpression as HarmonyTableExpression).OnExpressions.Add(Expression.Equal(innerKeySelector, outerKeySelector));
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
