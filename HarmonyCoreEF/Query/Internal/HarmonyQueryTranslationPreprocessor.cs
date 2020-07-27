﻿using Microsoft.EntityFrameworkCore.Query;
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
            query = new EnumerableToQueryableMethodConvertingExpressionVisitor().Visit(query);
            query = new QueryMetadataExtractingExpressionVisitor(_queryCompilationContext).Visit(query);
            query = new InvocationExpressionRemovingExpressionVisitor().Visit(query);
            query = new AllAnyToContainsRewritingExpressionVisitor().Visit(query);
            query = new GroupJoinFlatteningExpressionVisitor().Visit(query);
            query = new NullCheckRemovingExpressionVisitor().Visit(query);
            query = new EntityEqualityRewritingExpressionVisitor(_queryCompilationContext).Rewrite(query);
            query = new SubqueryMemberPushdownExpressionVisitor().Visit(query);
            query = new NavigationExpandingExpressionVisitor(_queryCompilationContext, Dependencies.EvaluatableExpressionFilter).Expand(query);
            query = new FunctionPreprocessingExpressionVisitor().Visit(query);
            new EnumerableVerifyingExpressionVisitor().Visit(query);
            return query;
        }
    }
}
