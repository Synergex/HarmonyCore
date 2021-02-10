using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Harmony.Core.EF.Query.Internal.HarmonyQueryableMethodTranslatingExpressionVisitor;

namespace Harmony.Core.EF.Query.Internal
{
    public class HarmonyQueryCompilationContext : QueryCompilationContext
    {
        public Dictionary<Expression, ICollection<INavigation>> NavigationsForParameterLookup = new Dictionary<Expression, ICollection<INavigation>>(new ExpressionValueComparer());
        public Dictionary<Expression, Expression> ParameterToValueBufferLookup = new Dictionary<Expression, Expression>(new ExpressionValueComparer());
        public Dictionary<Expression, HarmonyQueryExpression> ParameterToQueryExpressionLookup = new Dictionary<Expression, HarmonyQueryExpression>(new ExpressionValueComparer());

        public HarmonyQueryCompilationContext(QueryCompilationContextDependencies dependencies, bool async) : base(dependencies, async)
        {
        }

        public void AddNavigationToParameter(Expression innerExpr, Expression outerExpr, INavigation nav)
        {
            if (NavigationsForParameterLookup.TryGetValue(outerExpr, out var navCollection))
            {
                if (!navCollection.Contains(nav))
                    navCollection.Add(nav);
            }
            else
            {
                NavigationsForParameterLookup.Add(outerExpr, new List<INavigation> { nav });
            }
        }

        public void MapQueryExpression(ShapedQueryExpression shapedExpression, Expression selectParameter)
        {
            var queryExpression = shapedExpression.QueryExpression as HarmonyQueryExpression;
            if (ParameterToValueBufferLookup.TryGetValue(selectParameter, out var existing) && existing != queryExpression.ConvertedParameter)
                throw new Exception("queryExpression was mapped to multiple value buffers");
            else
                ParameterToValueBufferLookup.Add(selectParameter, queryExpression.ConvertedParameter);

            if (ParameterToQueryExpressionLookup.TryGetValue(selectParameter, out var existingQuery) && existingQuery != queryExpression)
                throw new Exception("query expression was mapped to multiple select parameters");
            else
                ParameterToQueryExpressionLookup.Add(selectParameter, queryExpression);

            if (shapedExpression is JoinedShapedQueryExpression joinedExpr && selectParameter.Type == joinedExpr.InnerNavigation.ClrType)
            {
                var innerSelectParameter = Expression.PropertyOrField(selectParameter, joinedExpr.InnerNavigation.Name);
                MapQueryExpression(joinedExpr.Inner, innerSelectParameter);
            }
        }

    }
}
