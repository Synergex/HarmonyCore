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
        private class ExpressionValueComparer : IEqualityComparer<Expression>
        {
            private void StringifyExpression(Expression expr, StringBuilder sb)
            {
                if (expr is ParameterExpression parm)
                {
                    sb.AppendFormat("({0}:{1})", parm.Name, parm.Type);

                }
                else if (expr is UnaryExpression unary)
                {
                    StringifyExpression(unary.Operand, sb);
                }
                else if (expr is MemberExpression member)
                {
                    StringifyExpression(member.Expression, sb);
                    sb.AppendFormat("({0})", member.Member.Name);
                }
                else
                    throw new NotImplementedException();
            }
            public bool Equals(Expression x, Expression y)
            {
                var sb = new StringBuilder();
                StringifyExpression(x, sb);
                var xString = sb.ToString();
                sb.Clear();
                StringifyExpression(y, sb);
                var yString = sb.ToString();

                return yString == xString;
            }

            public int GetHashCode(Expression obj)
            {
                var sb = new StringBuilder();
                StringifyExpression(obj, sb);
                return sb.ToString().GetHashCode();
            }
        }


        public Dictionary<Expression, INavigation> ParameterToNavigationLookup = new Dictionary<Expression, INavigation>(new ExpressionValueComparer());
        public Dictionary<Expression, ICollection<INavigation>> NavigationsForParameterLookup = new Dictionary<Expression, ICollection<INavigation>>(new ExpressionValueComparer());
        public Dictionary<Expression, Expression> ValueBufferToParameterLookup = new Dictionary<Expression, Expression>(new ExpressionValueComparer());
        public Dictionary<Expression, HarmonyQueryExpression> ParameterToQueryExpressionLookup = new Dictionary<Expression, HarmonyQueryExpression>(new ExpressionValueComparer());

        public HarmonyQueryCompilationContext(QueryCompilationContextDependencies dependencies, bool async) : base(dependencies, async)
        {
        }

        public void AddNavigationToParameter(Expression innerExpr, Expression outerExpr, INavigation nav)
        {
            if (NavigationsForParameterLookup.TryGetValue(outerExpr, out var navCollection))
            {
                if(!navCollection.Contains(nav))
                    navCollection.Add(nav);
            }
            else
            {
                NavigationsForParameterLookup.Add(outerExpr, new List<INavigation> { nav });
            }

            ParameterToNavigationLookup.Add(innerExpr, nav);
        }

        public void MapQueryExpression(ShapedQueryExpression shapedExpression, Expression selectParameter)
        {
            var queryExpression = shapedExpression.QueryExpression as HarmonyQueryExpression;
            if (ParameterToQueryExpressionLookup.TryGetValue(selectParameter, out var existingQuery) && existingQuery != queryExpression)
                throw new Exception("query expression was mapped to multiple select paramters");
            else
                ParameterToQueryExpressionLookup.Add(selectParameter, queryExpression);

            if (shapedExpression is JoinedShapedQueryExpression joinedExpr)
            {
                var innerSelectParameter = Expression.PropertyOrField(selectParameter, joinedExpr.InnerNavigation.Name);
                MapQueryExpression(joinedExpr.Inner, innerSelectParameter);
            }
        }

    }
}
