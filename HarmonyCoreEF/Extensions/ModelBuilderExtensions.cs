using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Harmony.Core.EF.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void AddOneToOneRelation(this ModelBuilder builder)
        {

        }

        public static void AddOneToOneToOneRelation(this ModelBuilder builder)
        {

        }

        public static void AddOneToOneToManyRelation(this ModelBuilder builder)
        {

        }

        public static void AddOneToManyToOneRelation(this ModelBuilder builder)
        {

        }

        public static void AddOneToManyRelation(this ModelBuilder builder)
        {

        }

        public static void AddManyToOneRelation(this ModelBuilder builder)
        {

        }


        public static void AddGlobalTagFilter<T>(this ModelBuilder builder, ParameterExpression paramExpr, params Expression[] tagExpressions)
        {
            Expression resultExpression = null;

            foreach (var expr in tagExpressions)
            {
                if (resultExpression == null)
                    resultExpression = expr;
                else
                    resultExpression = Expression.AndAlso(resultExpression, expr);
            }

            builder.Entity(typeof(T))
              .HasQueryFilter(
                Expression.Lambda(
                  Expression.Block(resultExpression), paramExpr));

        }
    }
}
