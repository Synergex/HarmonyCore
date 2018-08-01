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
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
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
                        queryModel.BodyClauses.Add(orderByClause);
                        this.VisitOrderByClause(orderByClause, queryModel, queryModel.BodyClauses.IndexOf(orderByClause));
                    }
                }
            }
        }

        //public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        //{
        //    QuerySourceMapping
        //    Microsoft.EntityFrameworkCore.Utilities.Check.NotNull(joinClause, "joinClause");
        //    Microsoft.EntityFrameworkCore.Utilities.Check.NotNull(queryModel, "queryModel");
        //    Expression expression = this.ReplaceClauseReferences(joinClause.OuterKeySelector, joinClause, false);
        //    Expression expression2 = this.CompileJoinClauseInnerSequenceExpression(joinClause, queryModel);
        //    ParameterExpression parameterExpression = Expression.Parameter(expression2.Type.GetSequenceType(), joinClause.ItemName);
        //    this.QueryCompilationContext.AddOrUpdateMapping(joinClause, parameterExpression);
        //    Expression body = this.ReplaceClauseReferences(joinClause.InnerKeySelector, joinClause, false);
        //    Type type = typeof(TransparentIdentifier<,>).MakeGenericType(this.CurrentParameter.Type, parameterExpression.Type);
        //    this._expression = Expression.Call(this.LinqOperatorProvider.Join.MakeGenericMethod(this.CurrentParameter.Type, parameterExpression.Type, expression.Type, type), this._expression, expression2, Expression.Lambda(expression, this.CurrentParameter), Expression.Lambda(body, parameterExpression), Expression.Lambda(this.CallCreateTransparentIdentifier(type, this.CurrentParameter, parameterExpression), this.CurrentParameter, parameterExpression));
        //    this.IntroduceTransparentScope(joinClause, queryModel, index, type);
        //}

        //public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        //{
        //    if (selectClause.Selector.Type == Expression.Type.GetSequenceType() && selectClause.Selector is QuerySourceReferenceExpression)
        //    {
        //        return;
        //    }
        //    if (this.CanOptimizeCorrelatedCollections())
        //    {
        //        this.TryOptimizeCorrelatedCollections(queryModel);
        //    }

        //    QueryPlan = QueryModelVisitor.PrepareQuery(queryModel, ProcessWeirdJoin, out var querySourceBuffer);
        //    Expression = Expression.Call(
        //            HarmonyQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(MainQueryType),
        //            EntityQueryModelVisitor.QueryContextParameter,
        //            Expression.Constant(QueryPlan),
        //            Expression.Constant(QueryCompilationContext.IsTrackingQuery));

        //    Expression expression = this.ReplaceClauseReferences(this._projectionExpressionVisitorFactory.Create(this, queryModel.MainFromClause).Visit(selectClause.Selector), null, true);
        //    if (!(expression.Type != Expression.Type.GetSequenceType()) && selectClause.Selector is QuerySourceReferenceExpression)
        //    {
        //        return;
        //    }
        //    if (!(from ro in queryModel.ResultOperators
        //          select ro.GetType()).Any(delegate (Type t)
        //          {
        //              if (!(t == typeof(GroupResultOperator)))
        //              {
        //                  return t == typeof(AllResultOperator);
        //              }
        //              return true;
        //          }))
        //    {
        //        Expression expression2 = expression;
        //        TaskLiftingExpressionVisitor taskLiftingExpressionVisitor = new TaskLiftingExpressionVisitor();
        //        if (Expression.Type.TryGetElementType(typeof(IAsyncEnumerable<>)) != (Type)null)
        //        {
        //            expression2 = taskLiftingExpressionVisitor.LiftTasks(expression);
        //        }
        //        Expression = ((expression2 == expression) ? 
        //            Expression.Call(this.LinqOperatorProvider.Select.MakeGenericMethod(this.CurrentParameter.Type, expression.Type), Expression, Expression.Lambda(expression, this.CurrentParameter)) :
        //            Expression.Call(EntityQueryModelVisitor.SelectAsyncMethod.MakeGenericMethod(this.CurrentParameter.Type, expression.Type), Expression, Expression.Lambda(expression2, this.CurrentParameter, taskLiftingExpressionVisitor.CancellationTokenParameter)));
        //    }
        //}

        public override void VisitQueryModel(QueryModel queryModel)
        {
            MainQueryType = queryModel.MainFromClause.ItemType;
            this.TryOptimizeCorrelatedCollections(queryModel);
            QueryPlan = QueryModelVisitor.PrepareQuery(queryModel, ProcessWeirdJoin, out var querySourceBuffer);
            //base.VisitQueryModel(queryModel);
            Expression = Expression.Call(
                    HarmonyQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(MainQueryType),
                    EntityQueryModelVisitor.QueryContextParameter,
                    Expression.Constant(QueryPlan),
                    Expression.Constant(QueryCompilationContext.IsTrackingQuery));

            this.CurrentParameter = Expression.Parameter(MainQueryType, queryModel.MainFromClause.ItemName);

            Type resultTypeParameter = queryModel.ResultTypeOverride;
            if (queryModel.ResultTypeOverride.IsGenericType && (queryModel.ResultTypeOverride.GetGenericTypeDefinition() == typeof(IQueryable<>)
                || queryModel.ResultTypeOverride.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                resultTypeParameter = queryModel.ResultTypeOverride.GenericTypeArguments.First();
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
                //Dictionary<IQuerySource, Expression> sourceMapping = new Dictionary<IQuerySource, Expression>();
                var buffers = QueryPlan.GetQueryBuffer().TypeBuffers;
                var queryMapping = new QuerySourceMapping();
                foreach (var kvp in querySourceBuffer)
                {
                    if (!string.IsNullOrWhiteSpace(buffers[kvp.Value].ParentFieldName))
                    {
                        var propertyNode = Expression.Property(this.CurrentParameter, MainQueryType.GetProperty(buffers[kvp.Value].ParentFieldName));
                        //queryMapping.AddMapping(kvp.Key, Expression.Condition(Expression.Equal(this.CurrentParameter, Expression.Constant(null)), Expression.Constant(null, propertyNode.Type), propertyNode, propertyNode.Type));
                        queryMapping.AddMapping(kvp.Key, propertyNode);
                    }
                    else
                    {
                        queryMapping.AddMapping(kvp.Key, this.CurrentParameter);
                    }

                }
                if (queryModel.SelectClause.Selector is MethodCallExpression && ((MethodCallExpression)queryModel.SelectClause.Selector).Method.DeclaringType == typeof(IncludeCompiler))
                    return;
                else
                {
                    //var selectorRewriter = new ReplaceQuerySource(sourceMapping);
                    //Expression expression = selectorRewriter.Visit(queryModel.SelectClause.Selector);
                    var memberAccessVisitor = new MemberAccessBindingExpressionVisitor(queryMapping, this, false);
                    var expression = memberAccessVisitor.Visit(queryModel.SelectClause.Selector);
                    Expression expression2 = expression;
                    TaskLiftingExpressionVisitor taskLiftingExpressionVisitor = new TaskLiftingExpressionVisitor();
                    if (Expression.Type.TryGetElementType(typeof(IAsyncEnumerable<>)) != (Type)null)
                    {
                        expression2 = taskLiftingExpressionVisitor.LiftTasks(expression);
                    }

                    Expression = ((expression2 == expression) ?
                        Expression.Call(this.LinqOperatorProvider.Select.MakeGenericMethod(this.CurrentParameter.Type, resultTypeParameter), Expression, Expression.Lambda(expression, this.CurrentParameter)) :
                        Expression.Call(EntityQueryModelVisitor.SelectAsyncMethod.MakeGenericMethod(this.CurrentParameter.Type, resultTypeParameter), Expression, Expression.Lambda(expression2, this.CurrentParameter, taskLiftingExpressionVisitor.CancellationTokenParameter)));
                }
            }
        }
        protected override Func<QueryContext, TResults> CreateExecutorLambda<TResults>()
        {
            return base.CreateExecutorLambda<TResults>();
        }

        public class ReplaceQuerySource : System.Linq.Expressions.ExpressionVisitor
        {
            private readonly Dictionary<IQuerySource, Expression> fromMapping;
            public ReplaceQuerySource(Dictionary<IQuerySource, Expression> fromMapping)
            {
                this.fromMapping = fromMapping;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var updatedBody = this.Visit(node.Body);        // which will convert parameters to 'to'
                return Expression.Lambda(updatedBody, node.Parameters);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var newNode = base.VisitMethodCall(node) as MethodCallExpression;
                if (newNode.Method.Name == "Property")
                {
                    var resultExpr = Expression.Property(newNode.Arguments[0], (((ConstantExpression)newNode.Arguments[1]).Value as string));
                    if (resultExpr.Type != newNode.Type)
                        return Expression.Convert(resultExpr, newNode.Type);
                    else
                        return resultExpr;
                }
                else
                    return newNode;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                return node;
            }

            protected override Expression VisitExtension(Expression node)
            {
                if (node is QuerySourceReferenceExpression)
                    return fromMapping[((QuerySourceReferenceExpression)node).ReferencedQuerySource];
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
    }
}
