// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Harmony.Core.EF.Extensions.Internal;
using Harmony.Core.FileIO.Queryable;
using Harmony.Core.FileIO.Queryable.Expressions;
using Harmony.Core.Utility;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace Harmony.Core.EF.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyQueryModelVisitor : EntityQueryModelVisitor
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyQueryModelVisitor(
            EntityQueryModelVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
            _projectionExpressionVisitorFactory = dependencies.ProjectionExpressionVisitorFactory;
        }
        private readonly IProjectionExpressionVisitorFactory _projectionExpressionVisitorFactory;

        private Expression ProcessWeirdJoin(Expression inner, Expression outer, QueryModel queryModel)
        {

            //var joiningType = QueryCompilationContext.Model.FindEntityType(outerQuery.ItemType);
            //var mainEntityType = QueryCompilationContext.Model.FindEntityType(innerQuery.ItemType);
            //var targetProperty = joiningType.FindProperty(propName);
            //var maybeForeignKeys = joiningType.FindForeignKeys(targetProperty);
            //var targetKey = maybeForeignKeys.FirstOrDefault((key) => key.PrincipalEntityType == mainEntityType);

            //if (targetKey == null)
            //{
            //    var primaryKey = joiningType.FindKey(targetProperty);
            //    if (primaryKey == null)
            //        throw new NotImplementedException();
            //    targetKey = primaryKey.GetReferencingForeignKeys().FirstOrDefault((key) => key.DeclaringEntityType == mainEntityType);
            //    if (targetKey == null)
            //        throw new NotImplementedException();
            //}

            //return Expression.Equal(expr, Expression.Call(typeof(Microsoft.EntityFrameworkCore.EF), "Property", new Type[] { expr.Type }, new QuerySourceReferenceExpression(outerQuery), Expression.Constant(targetKey.PrincipalKey.Properties.First().Name)));
            throw new NotImplementedException();
        }

        private static bool IsPartOfLeftJoinPattern(AdditionalFromClause additionalFromClause, QueryModel queryModel)
        {
            int num = queryModel.BodyClauses.IndexOf(additionalFromClause);
            SubQueryExpression obj = ((additionalFromClause != null) ? additionalFromClause.FromExpression : null) as SubQueryExpression;
            QueryModel queryModel2 = (obj != null) ? obj.QueryModel : null;
            IQuerySource querySource = (queryModel2 != null) ? queryModel2.MainFromClause.FromExpression.TryGetReferencedQuerySource() : null;
            GroupJoinClause groupJoinClause;
            if ((groupJoinClause = (queryModel.BodyClauses.ElementAtOrDefault(num - 1) as GroupJoinClause)) != null && groupJoinClause == querySource && queryModel.CountQuerySourceReferences(groupJoinClause) == 1 && queryModel2.BodyClauses.Count == 0 && queryModel2.ResultOperators.Count == 1 && ((Collection<ResultOperatorBase>)queryModel2.ResultOperators)[0] is DefaultIfEmptyResultOperator)
            {
                return true;
            }
            return false;
        }

        private void TryOptimizeCorrelatedCollections(QueryModel queryModel)
        {
            if (!queryModel.BodyClauses.OfType<AdditionalFromClause>().Any((AdditionalFromClause c) => !IsPartOfLeftJoinPattern(c, queryModel)))
            {
                CorrelatedCollectionOptimizingVisitor correlatedCollectionOptimizingVisitor = new CorrelatedCollectionOptimizingVisitor(this, queryModel);
                Expression expression = correlatedCollectionOptimizingVisitor.Visit(queryModel.SelectClause.Selector);
                if (expression != queryModel.SelectClause.Selector)
                {
                    queryModel.SelectClause.Selector = expression;
                    if (correlatedCollectionOptimizingVisitor.ParentOrderings.Count > 0)
                    {
                        this.RemoveOrderings(queryModel);
                        OrderByClause orderByClause = new OrderByClause();
                        foreach (Ordering parentOrdering in correlatedCollectionOptimizingVisitor.ParentOrderings)
                        {
                            orderByClause.Orderings.Add(parentOrdering);
                        }
						AddClauseIfUnique(queryModel.BodyClauses, orderByClause);
                        this.VisitOrderByClause(orderByClause, queryModel, queryModel.BodyClauses.IndexOf(orderByClause));
                    }
                }
            }
        }

		private static void AddClauseIfUnique(IList<IBodyClause> clauses, IBodyClause adding)
		{
			if (!clauses.Any(body => MatchingClause(body, adding)))
				clauses.Add(adding);
		}

		private static bool MatchingClause(IBodyClause clause1, IBodyClause clause2)
		{
			if (clause1 is GroupJoinClause || clause2 is GroupJoinClause || clause1 is JoinClause || clause2 is JoinClause)
			{
				var join1 = (clause1 as GroupJoinClause)?.JoinClause ?? clause1 as JoinClause;
				var join2 = (clause2 as GroupJoinClause)?.JoinClause ?? clause2 as JoinClause;
				if (join1 != null && join2 != null)
					return join1.ItemName == join2.ItemName && join1.ItemType == join2.ItemType;
			}
			
			return false;
		}

        private static MethodInfo StringToLower = typeof(string).GetMethod("ToLower", new Type[0]);
        private static MethodInfo StringToUpper = typeof(string).GetMethod("ToUpper", new Type[0]);


        public static InExpression MakeInExpression(List<string> values, Expression predicate)
        {
            if (predicate is MethodCallExpression)
            {
                var methodNode = predicate as MethodCallExpression;
                if (methodNode.Method == StringToLower || methodNode.Method == StringToLower)
                {
                    return new InExpression { CaseInsensitive = true, Collection = values, Predicate = methodNode.Object };
                }
            }
            return new InExpression { CaseInsensitive = false, Collection = values, Predicate = predicate };
        }

        private static Expression StripNullConditional(Expression expr)
        {
            if (expr is NullConditionalExpression)
            {
                var nullExpr = expr as NullConditionalExpression;
                return nullExpr.AccessOperation;
            }
            else if (expr is BinaryExpression)
            {
                var binaryExpr = expr as BinaryExpression;
                return binaryExpr.Update(StripNullConditional(binaryExpr.Left), binaryExpr.Conversion, StripNullConditional(binaryExpr.Right));
            }
            else
                return expr;
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            DebugLogSession.Logging.LogDebug("HarmonyCoreEF: VisitQueryModel -> {0}", queryModel);

            MainQueryType = queryModel.MainFromClause.ItemType;
            this.TryOptimizeCorrelatedCollections(queryModel);
            this.CurrentParameter = Expression.Parameter(MainQueryType, queryModel.MainFromClause.ItemName);

            //some of the query sources are pointless SQLism around their inability to return collections outside of grouping
            //we need to replace those sources with the materialized properties to allow Selectors to operate on a consistant model
            var aliasMapping = new Dictionary<IQuerySource, IQuerySource>();
            foreach (var bodyClause in queryModel.BodyClauses)
            {
                var groupJoin = bodyClause as GroupJoinClause;
                var whereClause = bodyClause as WhereClause;
                var additionalFrom = bodyClause as AdditionalFromClause;
                if (groupJoin != null && groupJoin.ItemName != groupJoin.JoinClause.ItemName)
                {
                    if (!aliasMapping.ContainsKey(groupJoin))
                        aliasMapping.Add(groupJoin, groupJoin.JoinClause);
                }

                if (additionalFrom != null)
                {
                    var flatGroupJoin = additionalFrom.TryGetFlattenedGroupJoinClause();
                    if (flatGroupJoin != null && aliasMapping.ContainsKey(flatGroupJoin))
                    {
                        aliasMapping.Add(additionalFrom, flatGroupJoin.JoinClause);
                    }
                }
                
                LiftInOperator(whereClause);

                if (whereClause?.Predicate is BinaryExpression)
                {
                    whereClause.Predicate = StripNullConditional(whereClause.Predicate);
                }

                if (bodyClause is OrderByClause)
                {
                    LiftOrderByOperator(bodyClause as OrderByClause);
                }
            }

            Type resultTypeParameter = queryModel.ResultTypeOverride.ForceSequenceType();

            //check if we're returning some kind of aggregate rather than a sequence
            var valueFromSequenceOperators = queryModel.ResultOperators.OfType<Remotion.Linq.Clauses.ResultOperators.ValueFromSequenceResultOperatorBase>().ToArray();
            if (valueFromSequenceOperators.Length > 0)
            {
                resultTypeParameter = this.CurrentParameter.Type;
            }
            


            if (!(from ro in queryModel.ResultOperators
                  select ro.GetType()).Any(delegate (Type t)
                  {
                      if (!(t == typeof(GroupResultOperator)))
                      {
                          return t == typeof(AllResultOperator);
                      }
                      return true;
                  }))
            {
                
                var subQueryVisitor = new SubQueryRewriter { QueryModel = queryModel, TopQueryModel = queryModel, Parent = this, QuerySourceMapping = QueryCompilationContext.QuerySourceMapping, QuerySourceAliases = aliasMapping };
                //Expression expression = selectorRewriter.Visit(queryModel.SelectClause.Selector);
                var selector = subQueryVisitor.Visit(queryModel.SelectClause.Selector);

                QueryPlan = QueryModelVisitor.PrepareQuery(queryModel, ProcessWeirdJoin, aliasMapping, out var querySourceBuffer);
                Expression = Expression.Call(
                        HarmonyQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(MainQueryType),
                        EntityQueryModelVisitor.QueryContextParameter,
                        Expression.Constant(QueryPlan),
                        Expression.Constant(QueryCompilationContext.IsTrackingQuery));

                var memberAccessVisitor = new HarmonyMemberAccessBindingExpressionVisitor(QueryCompilationContext.QuerySourceMapping, this, false);
                var buffers = QueryPlan.GetQueryBuffer().TypeBuffers;
                foreach (var kvp in querySourceBuffer)
                {
                    if (!QueryCompilationContext.QuerySourceMapping.ContainsMapping(kvp.Key))
                    {
                        if (!string.IsNullOrWhiteSpace(buffers[kvp.Value].ParentFieldName))
                        {
                            //this might lead to some trouble later on if we get two sources with the same ParentFieldName at different levels and a different actual parent
                            var foundProperty = MainQueryType.GetProperty(buffers[kvp.Value].ParentFieldName);
                            if (foundProperty != null)
                            {
                                var propertyNode = Expression.Property(this.CurrentParameter, foundProperty);
                                QueryCompilationContext.QuerySourceMapping.AddMapping(kvp.Key, propertyNode);
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            QueryCompilationContext.QuerySourceMapping.AddMapping(kvp.Key, this.CurrentParameter);
                        }
                    }
                }

                foreach (var queryAlias in aliasMapping)
                {
                    if (QueryCompilationContext.QuerySourceMapping.ContainsMapping(queryAlias.Value))
                    {
                        if (!QueryCompilationContext.QuerySourceMapping.ContainsMapping(queryAlias.Key))
                        {
                            QueryCompilationContext.QuerySourceMapping.AddMapping(queryAlias.Key, QueryCompilationContext.QuerySourceMapping.GetExpression(queryAlias.Value));
                        }
                        else
                        {
                            QueryCompilationContext.QuerySourceMapping.ReplaceMapping(queryAlias.Key, QueryCompilationContext.QuerySourceMapping.GetExpression(queryAlias.Value));
                        }
                    }
                }

                var expression = memberAccessVisitor.Visit(selector);
                Expression expression2 = expression;
                TaskLiftingExpressionVisitor taskLiftingExpressionVisitor = new TaskLiftingExpressionVisitor();
                if (Expression.Type.TryGetElementType(typeof(IAsyncEnumerable<>)) != (Type)null)
                {
                    expression2 = taskLiftingExpressionVisitor.LiftTasks(expression);
                }

                Expression = ((expression2 == expression) ?
                    Expression.Call(this.LinqOperatorProvider.Select.MakeGenericMethod(this.CurrentParameter.Type, resultTypeParameter), Expression, Expression.Lambda(expression, this.CurrentParameter)) :
                    Expression.Call(AsyncLinqOperatorProvider.SelectAsyncMethod.MakeGenericMethod(this.CurrentParameter.Type, resultTypeParameter), Expression, Expression.Lambda(expression2, this.CurrentParameter, taskLiftingExpressionVisitor.CancellationTokenParameter)));
                
            }

            VisitResultOperators(queryModel.ResultOperators, queryModel);

            DebugLogSession.Logging.LogDebug("HarmonyCoreEF: VisitedQueryModel -> {0}", Expression);
        }

        internal static Expression LiftInOperator(Expression whereClause)
        {
            if (whereClause is BinaryExpression)
            {
                var binaryOp = whereClause as BinaryExpression;
                var liftedLeft = LiftInOperator(binaryOp.Left);
                var liftedRight = LiftInOperator(binaryOp.Right);

                return binaryOp.Update(liftedLeft, binaryOp.Conversion, liftedRight);
            }

            var subQuery = whereClause as SubQueryExpression;
            //hopefully this is supposed to be an "IN" operation is just needs to be transformated a little bit to make things work better later on in the process
            if (subQuery != null && subQuery.QueryModel.MainFromClause.FromExpression is ConstantExpression)
            {
                var constant = subQuery.QueryModel.MainFromClause.FromExpression as ConstantExpression;
                if (constant.Value is System.Collections.IEnumerable)
                {
                    var resultOperator = subQuery.QueryModel.ResultOperators.OfType<ContainsResultOperator>().FirstOrDefault();
                    if (resultOperator != null)
                    {
                        var collection = (constant.Value as System.Collections.IEnumerable).OfType<object>().Select(obj => obj.ToString()).ToList();
                        return MakeInExpression(collection, resultOperator.Item);
                    }
                    else
                        throw new NotImplementedException();
                    //
                }
                else
                    throw new NotImplementedException();
            }
            else
                return whereClause;
        }

        internal static void LiftInOperator(WhereClause whereClause)
        {
            if(whereClause?.Predicate != null)
                whereClause.Predicate = LiftInOperator(whereClause.Predicate);
        }

        internal static void LiftOrderByOperator(OrderByClause orderClause)
        {
            //this needs to be de-null'safed in order to be handled properly by the underlying infrastructure. This expression isnt being run so the null safe operator is unneeded anyway.
            if (orderClause != null)
            {
                foreach (var order in orderClause.Orderings)
                {
                    if (order.Expression is NullConditionalExpression)
                    {
                        order.Expression = ((NullConditionalExpression)order.Expression).AccessOperation;
                    }
                }
            }
        }

        protected override Func<QueryContext, TResults> CreateExecutorLambda<TResults>()
        {
            return base.CreateExecutorLambda<TResults>();
        }

        

        public class SubQueryRewriter : System.Linq.Expressions.ExpressionVisitor
        {
            public Dictionary<IQuerySource, IQuerySource> QuerySourceAliases;
            public QuerySourceMapping QuerySourceMapping;
            public HarmonyQueryModelVisitor Parent;
            public QueryModel QueryModel;
            public QueryModel TopQueryModel;
            private Stack<QuerySourceReferenceExpression> SourceStack= new Stack<QuerySourceReferenceExpression>();
            private Stack<Expression> ParameterStack = new Stack<Expression>();
            public Dictionary<Expression, Expression> SelectorRewriterLookup = new Dictionary<Expression, Expression>();
            private Stack<string> SubQueryTargetNames = new Stack<string>();
            private ReadOnlyCollection<ParameterExpression> CurrentParameters;

            private string SubQueryTargetName
            {
                get
                {
                    return SubQueryTargetNames.Count > 0 ? SubQueryTargetNames.Peek() : null;
                }
            }

            private QuerySourceReferenceExpression Source
            {
                get
                {
                    return SourceStack.Count > 0  ? SourceStack.Peek() : new QuerySourceReferenceExpression(QueryModel.MainFromClause);
                }
            }

            private Expression CurrentParameter
            {
                get
                {
                    return ParameterStack.Count > 0 ? ParameterStack.Peek() : Parent.CurrentParameter;
                }
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                return node;
            }

            private QuerySourceReferenceExpression _currentIncludeSource;
            protected override Expression VisitExtension(Expression node)
            {
                if (node is NullConditionalExpression)
                {
                    var caller = ((NullConditionalExpression)node).Caller;
                    var access = ((NullConditionalExpression)node).AccessOperation;
                    if (caller is QuerySourceReferenceExpression)
                    {
                        var refExpr = caller as QuerySourceReferenceExpression;
                        var fromClause = refExpr.ReferencedQuerySource as FromClauseBase;
                        if (!QuerySourceMapping.ContainsMapping(refExpr.ReferencedQuerySource))
                        {
                            QuerySourceMapping.AddMapping(refExpr.ReferencedQuerySource, MakeItemNameProperty(CurrentParameter, refExpr.ReferencedQuerySource.ItemName, true));
							if (ParameterStack.Count > 0 && fromClause?.FromExpression is SubQueryExpression)
							{
								SubQueryTargetNames.Push(refExpr.ReferencedQuerySource.ItemName.Substring(refExpr.ReferencedQuerySource.ItemName.IndexOf('.') + 1));
								LiftSubQueryExpression(fromClause?.FromExpression as SubQueryExpression);
								SubQueryTargetNames.Pop();
							}
							else
								LiftClausesFromSubQueryExpression(refExpr, TopQueryModel, fromClause?.FromExpression as SubQueryExpression);
                        }
                    }
                }
                return node;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                CurrentParameters = node.Parameters;
                return base.VisitLambda(node);
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                var sourceStackCount = SourceStack.Count;
                var parameterStackCount = ParameterStack.Count;
                var nameStackCount = SubQueryTargetNames.Count;
                var result = base.VisitMemberInit(node);
                if (SourceStack.Count > sourceStackCount)
                {
                    var peeked = SourceStack.Peek();
                    //if we didnt process the querysource
                    if (SubQueryTargetNames.Count == nameStackCount && peeked.ReferencedQuerySource is AdditionalFromClause && ParameterStack.Count > 0)
                    {
                        var additionalFromClause = (peeked.ReferencedQuerySource as AdditionalFromClause).FromExpression as SubQueryExpression;
                        if (additionalFromClause != null)
                        {
                            if (!QuerySourceMapping.ContainsMapping(peeked.ReferencedQuerySource))
                            {
                                QuerySourceMapping.AddMapping(peeked.ReferencedQuerySource, MakeItemNameProperty(CurrentParameter, peeked.ReferencedQuerySource.ItemName, true));
								if (ParameterStack.Count > 0)
								{
									SubQueryTargetNames.Push(peeked.ReferencedQuerySource.ItemName.Substring(peeked.ReferencedQuerySource.ItemName.IndexOf('.') + 1));
									LiftSubQueryExpression(additionalFromClause);
									SubQueryTargetNames.Pop();
								}
								else
									LiftClausesFromSubQueryExpression(peeked, TopQueryModel, additionalFromClause);
                            }
                        }
                    }
                    SourceStack.Pop();
                }

                if (SubQueryTargetNames.Count > nameStackCount)
                    SubQueryTargetNames.Pop();

                if (ParameterStack.Count > parameterStackCount)
                    ParameterStack.Pop();

                return result;
            }

            private void LiftClausesFromSubQueryExpression(QuerySourceReferenceExpression liftTarget, QueryModel targetModel, SubQueryExpression expression)
            {
                if (expression != null)
                {
                    //TODO: this isnt dealing with select operators
                    foreach (var bodyClause in expression.QueryModel.BodyClauses)
                    {
                        //TODO: this isnt dealing with joins and additional froms
                        if (bodyClause is WhereClause)
                        {
                            var whereClause = bodyClause as WhereClause;
                            LiftInOperator(whereClause);
                            AddOrUpdateSelector(new QuerySourceReferenceExpression(expression.QueryModel.MainFromClause), liftTarget);
                            var rewrite = new SelectorRewriter()
                            {
                                Replacements = SelectorRewriterLookup
                            };
                            whereClause.Predicate = rewrite.Visit(whereClause.Predicate) as Expression;
							AddClauseIfUnique(targetModel.BodyClauses, whereClause);
                        }
                        else if (bodyClause is OrderByClause)
                        {
                            LiftOrderByOperator(bodyClause as OrderByClause);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
            //this is a really basic ranking that may eventually need to examine the actual properties in question to determine quality
            private int WhereAsJoinClauseQuality(WhereClause clause)
            {
                return WhereAsJoinClauseExpressionQuality(clause.Predicate);
            }

            private static int WhereAsJoinClauseExpressionQuality(Expression expression)
            {
                var binaryExpression = expression as BinaryExpression;
                var nullSafeExpression = expression as NullSafeEqualExpression;
                var blockExpression = expression as BlockExpression;
                if (binaryExpression != null)
                {
                    if (binaryExpression.Left is ParameterExpression || binaryExpression.Left is ConstantExpression ||
                       binaryExpression.Right is ParameterExpression || binaryExpression.Right is ConstantExpression)
                    {
                        return -1;
                    }
                    else
                    {
                        if ((binaryExpression.Left is MethodCallExpression || binaryExpression.Left is MemberExpression) &&
                            (binaryExpression.Right is MethodCallExpression || binaryExpression.Right is MemberExpression))
                        {
                            return 1;
                        }
                        else
                            return WhereAsJoinClauseExpressionQuality(binaryExpression.Left);
                    }
                }
                else if (nullSafeExpression != null)
                {
                    return WhereAsJoinClauseExpressionQuality(nullSafeExpression.EqualExpression);
                }
                else if (blockExpression != null && blockExpression.Expressions.Count == 1)
                {
                    return WhereAsJoinClauseExpressionQuality(blockExpression.Expressions.First());
                }
                else if (expression is InExpression)
                {
                    return -1;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            private void AddOrUpdateSelector(Expression key, Expression value)
            {
                if (SelectorRewriterLookup.ContainsKey(key))
                {
                    SelectorRewriterLookup[key] = value;
                }
                else
                {
                    SelectorRewriterLookup.Add(key, value);
                }
            }

            private static char[] DotArray = new char[] { '.' };

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                //check if we're putting a subquery into an entity type
                if (typeof(DataObjectBase).IsAssignableFrom(node.Member.DeclaringType))
                {
                    var subQueryExpression = node.Expression as SubQueryExpression;
                    var creatingType = node.Member.DeclaringType;
                    try
                    {
                        SubQueryTargetNames.Push(node.Member.Name);
                        var updatedExpression = LiftSubQueryExpression(subQueryExpression);
                        return node.Update(updatedExpression);
                    }
                    finally
                    {
                        SubQueryTargetNames.Pop();
                    }
                }
                else
                {
                    //this is for things like SelectAllAndExpand from OData
                    //we should be able to shake things into this pattern if they have a similar purpose with a subquery
                    if (node.Member.Name == "Instance")
                    {
                        SourceStack.Push(node.Expression as QuerySourceReferenceExpression);
                    }
                    else if (node.Member.Name == "Name")
                    {
                        SubQueryTargetNames.Push(((ConstantExpression)node.Expression).Value as string);
                    }
                    else if (node.Member.Name.StartsWith("Next") && node.Expression is MemberInitExpression)
                    {
                    }
                    else if (node.Member.Name == "IsNull")
                    {
                        var propCall = (node.Expression as BinaryExpression)?.Left as MethodCallExpression;
                        if (propCall != null && propCall.Method.Name == "Property" && (propCall.Arguments[0] is QuerySourceReferenceExpression))
                        {
                            var targetQuerySource = (propCall.Arguments[0] as QuerySourceReferenceExpression).ReferencedQuerySource;
                            var targetParameter = QuerySourceMapping.ContainsMapping(targetQuerySource) ? QuerySourceMapping.GetExpression(targetQuerySource) : CurrentParameter;
                            Expression targetProperty = Expression.Property(targetParameter, SubQueryTargetName);
                            return node.Update(Expression.Equal(targetProperty, Expression.Default(targetProperty.Type)));
                        }
                    }
                    else if (node.Member.Name == "Value")
                    {
                        if (node.Expression is SubQueryExpression)
                        {
                            var updatedExpression = LiftSubQueryExpression(node.Expression as SubQueryExpression);
                            return node.Update(updatedExpression);
                        }
                    }
                }
                return base.VisitMemberAssignment(node);
            }

            private Expression LiftSubQueryExpression(SubQueryExpression node)
            {
                DebugLogSession.Logging.LogDebug("HarmonyCoreEF: Rewriting SubQueryExpression -> {0}", node.QueryModel);

                var queryModel = node.QueryModel;
                var propValue = MakeItemNameProperty(CurrentParameter, SubQueryTargetName, false);
                var resultExpressionElementType = propValue.Type.IsGenericType ? propValue.Type.GenericTypeArguments.First() : propValue.Type;
                Expression resultExpression = (propValue.Type.IsGenericType && propValue.Type.GetGenericTypeDefinition() == typeof(ICollection<>)) ?
                    Expression.Condition(Expression.Equal(propValue, Expression.Constant(null)), Expression.Convert(Expression.New(typeof(List<>).MakeGenericType(new Type[] { resultExpressionElementType })), propValue.Type), propValue) :
                    (Expression)propValue;

                //LiftClausesFromSubQueryExpression(new QuerySourceReferenceExpression(queryModel.MainFromClause), TopQueryModel, queryModel.MainFromClause?.FromExpression as SubQueryExpression);
                var fromSubQuery = queryModel.MainFromClause?.FromExpression as SubQueryExpression;
                var allBodyClauses = fromSubQuery?.QueryModel?.BodyClauses != null ?
                    queryModel.BodyClauses.Concat(fromSubQuery.QueryModel.BodyClauses).ToList() :
                    queryModel.BodyClauses.ToList();
                //some of the query sources are pointless SQLism around their inability to return collections outside of grouping
                //we need to replace those sources with the materialized properties to allow Selectors to operate on a consistant model
                foreach (var bodyClause in allBodyClauses)
                {
                    var groupJoin = bodyClause as GroupJoinClause;
                    var additionalFrom = bodyClause as AdditionalFromClause;
                    if (groupJoin != null && groupJoin.ItemName != groupJoin.JoinClause.ItemName)
                    {
                        if (!QuerySourceAliases.ContainsKey(groupJoin))
                            QuerySourceAliases.Add(groupJoin, groupJoin.JoinClause);
                    }

                    if (additionalFrom != null)
                    {
                        var flatGroupJoin = additionalFrom.TryGetFlattenedGroupJoinClause();
                        if (flatGroupJoin != null && QuerySourceAliases.ContainsKey(flatGroupJoin))
                            QuerySourceAliases.Add(additionalFrom, flatGroupJoin.JoinClause);
                    }

                    if (bodyClause is WhereClause)
                    {
                        var whereClause = bodyClause as WhereClause;
                        LiftInOperator(whereClause);
                        if (fromSubQuery?.QueryModel?.BodyClauses.Contains(bodyClause) ?? false)
                        {
                            AddOrUpdateSelector(new QuerySourceReferenceExpression(fromSubQuery.QueryModel.MainFromClause),
                                new QuerySourceReferenceExpression(queryModel.MainFromClause));
                        }
                    }
                    else if (bodyClause is OrderByClause)
                    {
                        LiftOrderByOperator(bodyClause as OrderByClause);
                    }
                }

                Type resultTypeParameter = queryModel.ResultTypeOverride?.ForceSequenceType() ?? queryModel.SelectClause?.Selector?.Type;
                var currentParameter = Expression.Parameter(resultExpressionElementType);
                if (Source != null && Source.ReferencedQuerySource.ItemType == CurrentParameter.Type && !QuerySourceMapping.ContainsMapping(Source.ReferencedQuerySource))
                    QuerySourceMapping.AddMapping(Source.ReferencedQuerySource, CurrentParameter);

                try
                {
                    ParameterStack.Push(currentParameter);

                    if (!QuerySourceMapping.ContainsMapping(queryModel.MainFromClause) && queryModel.MainFromClause.ItemType == currentParameter.Type)
                    {
                        QuerySourceMapping.AddMapping(queryModel.MainFromClause, currentParameter);

                        //we only attach one predicate to a join clause, the rest need to be lifted into a regular where clause
                        //this may need additional work to pick the best join field
                        var orderedWhereClauses = allBodyClauses.OfType<WhereClause>().OrderByDescending(WhereAsJoinClauseQuality).ToList();
                        var whereClause = orderedWhereClauses.FirstOrDefault();
                        var rewrite = new SelectorRewriter()
                        {
                            Replacements = SelectorRewriterLookup
                        };

                        if (whereClause != null)
                        {
                            var joinOnLambda = whereClause.Predicate;
                            var navPropInfo = Source.Type.GetProperty(SubQueryTargetName);
                            var madeJoin = new JoinClause(Source.ReferencedQuerySource.ItemName + "." + SubQueryTargetName,
                                navPropInfo.PropertyType.GenericTypeArguments[0], Source, Expression.Constant(true), Expression.Constant(true));

                            var madeGroupJoin = new GroupJoinClause(Source.ReferencedQuerySource.ItemName + "." + SubQueryTargetName, typeof(IEnumerable<>).MakeGenericType(madeJoin.ItemType), madeJoin);
                            var newQuerySource = new QuerySourceReferenceExpression(madeGroupJoin);
                            QuerySourceAliases.Add(queryModel.MainFromClause, madeGroupJoin);
                            AddOrUpdateSelector(new QuerySourceReferenceExpression(queryModel.MainFromClause), newQuerySource);
                            var rewrittenJoinLambda = rewrite.Visit(joinOnLambda) as Expression;
                            BinaryExpression simpleJoinCondition = rewrittenJoinLambda as BinaryExpression;
                            if(simpleJoinCondition == null)
                                simpleJoinCondition = (rewrittenJoinLambda as NullSafeEqualExpression)?.EqualExpression;
                            if (simpleJoinCondition == null)
                                simpleJoinCondition = (rewrittenJoinLambda as BlockExpression)?.Expressions.LastOrDefault() as BinaryExpression;

                            madeJoin.InnerKeySelector = simpleJoinCondition.Right;
                            madeJoin.OuterKeySelector = simpleJoinCondition.Left;
							AddClauseIfUnique(TopQueryModel.BodyClauses, madeGroupJoin);
                        }

                        foreach (var additionalWhereClause in orderedWhereClauses.Skip(1))
                        {
                            additionalWhereClause.Predicate = rewrite.Visit(additionalWhereClause.Predicate);
							AddClauseIfUnique(TopQueryModel.BodyClauses, additionalWhereClause);
                        }

                        foreach (var joinClause in queryModel.BodyClauses.OfType<JoinClause>())
                        {
                            Expression targetExpr = MakeItemNameProperty(currentParameter, joinClause.ItemName, true);

                            QuerySourceMapping.AddMapping(joinClause, targetExpr);

							joinClause.InnerKeySelector = CleanKeySelector(joinClause.InnerKeySelector, rewrite);
                            joinClause.OuterKeySelector = CleanKeySelector(joinClause.OuterKeySelector, rewrite);
							AddClauseIfUnique(TopQueryModel.BodyClauses, joinClause);
                        }

						foreach (var groupJoinClause in queryModel.BodyClauses.OfType<GroupJoinClause>())
						{
							Expression targetExpr = MakeItemNameProperty(currentParameter, groupJoinClause.JoinClause.ItemName, true);

							QuerySourceMapping.AddMapping(groupJoinClause, targetExpr);

							groupJoinClause.JoinClause.InnerKeySelector = CleanKeySelector(groupJoinClause.JoinClause.InnerKeySelector, rewrite);
							groupJoinClause.JoinClause.OuterKeySelector = CleanKeySelector(groupJoinClause.JoinClause.OuterKeySelector, rewrite);
							AddClauseIfUnique(TopQueryModel.BodyClauses, groupJoinClause);
						}

						foreach (var orderByClause in queryModel.BodyClauses.OfType<OrderByClause>())
                        {
                            var targetOrderByResult = TopQueryModel.BodyClauses.OfType<OrderByClause>().FirstOrDefault();
                            if (targetOrderByResult == null)
                            {
                                targetOrderByResult = new OrderByClause();
								AddClauseIfUnique(TopQueryModel.BodyClauses, targetOrderByResult);
                            }

                            LiftOrderByOperator(orderByClause);
                            foreach (var ordering in orderByClause.Orderings)
                            {
                                ordering.Expression = rewrite.Visit(ordering.Expression);
                                targetOrderByResult.Orderings.Add(ordering);
                            }
                        }
                    }

                    if (!(from ro in queryModel.ResultOperators
                          select ro.GetType()).Any(delegate (Type t)
                          {
                              if (!(t == typeof(GroupResultOperator)))
                              {
                                  return t == typeof(AllResultOperator);
                              }
                              return true;
                          }))
                    {

                        var subQueryVisitor = new SubQueryRewriter { QueryModel = queryModel, TopQueryModel = TopQueryModel, Parent = Parent, QuerySourceMapping = QuerySourceMapping, QuerySourceAliases = QuerySourceAliases, ParameterStack = ParameterStack, SelectorRewriterLookup = SelectorRewriterLookup };
                        //Expression expression = selectorRewriter.Visit(queryModel.SelectClause.Selector);
                        var selector = subQueryVisitor.Visit(queryModel.SelectClause.Selector);
                        var memberAccessVisitor = new HarmonyMemberAccessBindingExpressionVisitor(QuerySourceMapping, Parent, false);
                        // querySourceMapping.AddMapping(kvp.Key, propertyNode);

                        foreach (var queryAlias in QuerySourceAliases)
                        {
                            if (QuerySourceMapping.ContainsMapping(queryAlias.Value))
                            {
                                if (!QuerySourceMapping.ContainsMapping(queryAlias.Key))
                                {
                                    QuerySourceMapping.AddMapping(queryAlias.Key, QuerySourceMapping.GetExpression(queryAlias.Value));
                                }
                                else
                                {
                                    QuerySourceMapping.ReplaceMapping(queryAlias.Key, QuerySourceMapping.GetExpression(queryAlias.Value));
                                }
                            }
                        }


                        var expression = memberAccessVisitor.Visit(selector);
                        Expression expression2 = expression;
                        TaskLiftingExpressionVisitor taskLiftingExpressionVisitor = new TaskLiftingExpressionVisitor();
                        if (resultExpression.Type.TryGetElementType(typeof(IAsyncEnumerable<>)) != (Type)null)
                        {
                            expression2 = taskLiftingExpressionVisitor.LiftTasks(expression);
                        }

                        if (resultExpressionElementType != resultTypeParameter)
                        {
                            resultExpression = ((expression2 == expression) ?
                                Expression.Call(Parent.LinqOperatorProvider.Select.MakeGenericMethod(resultExpressionElementType, resultTypeParameter), resultExpression, Expression.Lambda(expression, currentParameter)) :
                                Expression.Call(AsyncLinqOperatorProvider.SelectAsyncMethod.MakeGenericMethod(resultExpressionElementType, resultTypeParameter), resultExpression, Expression.Lambda(expression2, currentParameter, taskLiftingExpressionVisitor.CancellationTokenParameter)));
                        }
                        else
                        {
                            resultExpression = expression;
                        }
                    }
                    return resultExpression;
                }
                finally
                {
                    ParameterStack.Pop();
                }
            }

			private static Expression CleanKeySelector(Expression selector, SelectorRewriter rewriter)
			{
				if (selector is NullConditionalExpression)
				{
					return selector.RemoveNullConditional();
				}
				else
				{
					return selector;
				}
			}

            private static Expression MakeItemNameProperty(Expression currentParameter, string itemName, bool skipOne)
            {
				//grab each item join and make a query source replacement that points to the actual property instead of a query source
				var joinItemNameParts = (IEnumerable<string>)itemName.Split(DotArray, StringSplitOptions.RemoveEmptyEntries);
				if(skipOne)
					joinItemNameParts = joinItemNameParts.Skip(1);

                Expression targetExpr = currentParameter;
                foreach (var item in joinItemNameParts)
                {
                    var targetProperty = Expression.Property(targetExpr, item);
                    targetExpr = Expression.Condition(Expression.Equal(targetExpr, Expression.Constant(null, targetExpr.Type)), Expression.Constant(null, targetProperty.Type), targetProperty);
                }

                return targetExpr;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (IncludeCompiler.IsIncludeMethod(node))
                {
                    _currentIncludeSource = node.Arguments[1] as QuerySourceReferenceExpression;
                }

                Visit(node.Arguments.Last());

                //we fold the include directly into the query this is a nop
                if (IncludeCompiler.IsIncludeMethod(node))
                {
                    _currentIncludeSource = null;
                    return node.Arguments[1];
                }
                else if(node.Method.Name == "IncludeCollection")
                {
                    //get the target navigation property
                    var navProp = node.Arguments[1] as ConstantExpression;
                    var realNavProp = navProp.Value as INavigation;
                    var joinOnLambda = node.Arguments.Last() as LambdaExpression;
                    var entitySourceType = typeof(EntityQueryable<>).MakeGenericType(joinOnLambda.Parameters[0].Type);
                    var entityQueryable = Expression.Constant(Activator.CreateInstance(entitySourceType, ((dynamic)_currentIncludeSource.ReferencedQuerySource).FromExpression.Value.Provider as IQueryProvider));
                    var madeJoin = new JoinClause(_currentIncludeSource.ReferencedQuerySource.ItemName + "." + realNavProp.Name,
                        realNavProp.PropertyInfo.PropertyType.GenericTypeArguments[0], entityQueryable, Expression.Constant(true), Expression.Constant(true));
                    var madeGroupJoin = new GroupJoinClause(_currentIncludeSource.ReferencedQuerySource.ItemName + "." + realNavProp.Name, typeof(IEnumerable<>).MakeGenericType(madeJoin.ItemType), madeJoin);
                    var newQuerySource = new QuerySourceReferenceExpression(madeGroupJoin);
                    var rewrite = new SelectorRewriter()
                    {
                        Replacements = new Dictionary<Expression, Expression>
                        {
                            { joinOnLambda.Parameters[1], newQuerySource },
                            { joinOnLambda.Parameters[0], _currentIncludeSource }
                        }
                    };
                    var rewrittenJoinLambda = rewrite.Visit(joinOnLambda) as LambdaExpression;
                    BinaryExpression simpleJoinCondition = null;
                    if (rewrittenJoinLambda.Body is BinaryExpression)
                        simpleJoinCondition = rewrittenJoinLambda.Body as BinaryExpression;
                    else if (rewrittenJoinLambda.Body is ConditionalExpression)
                    {
                        var condExpr = rewrittenJoinLambda.Body as ConditionalExpression;
                        if (condExpr.IfFalse is BinaryExpression)
                            simpleJoinCondition = condExpr.IfFalse as BinaryExpression;
                        else if(condExpr.IfTrue is BinaryExpression)
                            simpleJoinCondition = condExpr.IfTrue as BinaryExpression;
                        else
                            throw new NotImplementedException(string.Format("Failed to translate conditional expression {0}\r\ninto join condition", condExpr));
                    }
                    else
                    {
                        throw new NotImplementedException(string.Format("Failed to translate expression {0}\r\ninto join condition", rewrittenJoinLambda.Body));
                    }


                    madeJoin.InnerKeySelector = simpleJoinCondition.Right;
                    madeJoin.OuterKeySelector = simpleJoinCondition.Left;
					AddClauseIfUnique(TopQueryModel.BodyClauses, madeGroupJoin);
                }

                return node;
            }
        }

        public class SelectorRewriter : System.Linq.Expressions.ExpressionVisitor
        {
            public Dictionary<Expression, Expression> Replacements;

            protected override Expression VisitExtension(Expression node)
            {
                if (node is NullSafeEqualExpression)
                {
                    return Visit(((NullSafeEqualExpression)node).EqualExpression);
                }
                else if (Replacements.TryGetValue(node, out var replacement))
                {
                    return replacement;
                }
                else
                {
                    return node;
                }
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (Replacements.TryGetValue(node.Expression, out var replacement))
                {
                    return Expression.Call(typeof(Microsoft.EntityFrameworkCore.EF), "Property", new Type[] { ((PropertyInfo) node.Member).PropertyType }, replacement, Expression.Constant(node.Member.Name));
                }
                else
                    return node;
            }
        }

        public PreparedQueryPlan QueryPlan { get; set; }
        public Type MainQueryType { get; set; }
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static readonly MethodInfo EntityQueryMethodInfo
            = typeof(HarmonyQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(EntityQuery));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static readonly MethodInfo OfTypeMethodInfo
            = typeof(Enumerable).GetTypeInfo()
                .GetDeclaredMethod(nameof(Enumerable.OfType));


        private static IEnumerable<TEntity> EntityQuery<TEntity>(
            QueryContext queryContext,
            PreparedQueryPlan queryPlan,
            bool isTrackingQuery)
            where TEntity : DataObjectBase
        {
            return queryPlan.ExecuteCollectionPlan<TEntity>(
                (obj) => 
                {
                    if (isTrackingQuery)
                        queryContext.QueryBuffer.StartTracking(obj, (((HarmonyQueryContext)queryContext).GetEntityType(obj.GetType())));
                    return obj;
                },
                queryContext.ParameterValues, 
                (((HarmonyQueryContext)queryContext).Store));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static readonly MethodInfo ProjectionQueryMethodInfo
            = typeof(HarmonyQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(ProjectionQuery));


        private static IEnumerable<ValueBuffer> ProjectionQuery(
            QueryContext queryContext,
            PreparedQueryPlan queryPlan,
            bool isTrackingQuery)

        {
            throw new NotImplementedException();
            //QueryModelVisitor.ExecuteSelectInternal((expr, propName, outerQuery, innerQuery) =>
            //    {
            //        var joiningType = compilationContext.Model.FindEntityType(outerQuery.ItemType);
            //        var mainEntityType = compilationContext.Model.FindEntityType(innerQuery.ItemType);
            //        var maybeForeignKeys = joiningType.FindForeignKeys(joiningType.FindProperty(propName));
            //        var targetKey = maybeForeignKeys.FirstOrDefault((key) => key.PrincipalEntityType == mainEntityType);

            //        if (targetKey == null)
            //            throw new NotImplementedException();

            //        return Expression.And(expr, Expression.Call(typeof(Microsoft.EntityFrameworkCore.EF), "Property", new Type[] { targetKey.PrincipalKey.Properties.First().ClrType }, Expression.Constant(outerQuery), Expression.Constant(targetKey.PrincipalKey.Properties.First().Name)));
            //    }, queryModel, (obj) => { queryContext.QueryBuffer.StartTracking(obj, entityType); return obj; }, queryContext.ParameterValues, (((HarmonyQueryContext)queryContext).Store)).OfType<DataObjectBase>()
            //        .Select(t => new ValueBuffer(t.InternalGetValues()));
        }

        internal class HarmonyMemberAccessBindingExpressionVisitor : MemberAccessBindingExpressionVisitor
        {
            private static readonly MethodInfo _getValueMethodInfo = typeof(MemberAccessBindingExpressionVisitor).GetMethod("GetValue", BindingFlags.Static | BindingFlags.NonPublic);
            public HarmonyMemberAccessBindingExpressionVisitor(QuerySourceMapping querySourceMapping, EntityQueryModelVisitor queryModelVisitor, bool inProjection) : base(querySourceMapping, queryModelVisitor, inProjection)
            {
            }

            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                var result = base.VisitMethodCall(methodCallExpression);
                if ((result as MethodCallExpression)?.Method?.GetGenericMethodDefinition() == _getValueMethodInfo)
                {
                    var callExpr = result as MethodCallExpression;
                    var propConst = callExpr.Arguments[2] as ConstantExpression;
                    var propValue = propConst?.Value as IProperty;
                    var baseExpr = callExpr.Arguments[1];
                    if(propValue != null)
                        return Expression.Convert(Expression.Property(baseExpr, propValue.Name), callExpr.Method.ReturnType);
                }
                return result;
            }
        }
    }
}
