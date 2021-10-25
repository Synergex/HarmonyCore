using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Harmony.Core.EF.Query.Internal
{
    internal class HarmonyQueryTranslationPreprocessor : QueryTranslationPreprocessor
    {
        private class EnumerableVerifyingExpressionVisitor : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(Enumerable) && node.Arguments[0].Type.IsGenericType && node.Arguments[0].Type.GetGenericTypeDefinition() == typeof(IQueryable<>) && !string.Equals(node.Method.Name, "ToList") && !string.Equals(node.Method.Name, "ToArray"))
                {
                    throw new Exception("Invalid filter condition");
                }
                return base.VisitMethodCall(node);
            }
        }

        QueryCompilationContext _queryCompilationContext;
        public HarmonyQueryTranslationPreprocessor(QueryTranslationPreprocessorDependencies dependencies, QueryCompilationContext queryCompilationContext) : base(dependencies, queryCompilationContext)
        {
            _queryCompilationContext = queryCompilationContext;
        }

        public override Expression Process(Expression query)
        {
            query = new InvocationExpressionRemovingExpressionVisitor().Visit(query);
            query = NormalizeQueryableMethod(query);
            query = new NullCheckRemovingExpressionVisitor().Visit(query);
            query = new SubqueryMemberPushdownExpressionVisitor(QueryCompilationContext.Model).Visit(query);
            query = new NavigationExpandingExpressionVisitor(this, QueryCompilationContext, Dependencies.EvaluatableExpressionFilter)
                .Expand(query);
            query = new QueryOptimizingExpressionVisitor().Visit(query);
            query = new NullCheckRemovingExpressionVisitor().Visit(query);
            return query;
        }
    }
}
