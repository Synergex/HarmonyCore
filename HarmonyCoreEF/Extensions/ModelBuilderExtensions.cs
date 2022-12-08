using Harmony.Core.FileIO.Queryable.Expressions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;

namespace Harmony.Core.EF.Extensions
{
    public enum TagConnector
    {
        None,
        AndExpression,
        OrExpression
    }

    public static class ModelBuilderExtensions
    {
        public static void AddOneToOneToOneRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinedProperty, string joinedKey)
        {
            builder.Entity(typeof(D))
               .HasOne(typeof(J), drivingProperty)
               .WithOne(joinedProperty)
               .IsRequired(false)
               .HasForeignKey(typeof(J), joinedKey)
               .HasPrincipalKey(typeof(D), drivingKey);
        }

        public static void AddOneToOneToManyRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinedProperty, string joinedKey)
        {
            builder.Entity(typeof(D))
               .HasOne(typeof(J), drivingProperty)
               .WithMany(joinedProperty)
               .HasPrincipalKey(joinedKey)
               .HasForeignKey(drivingKey);
        }

        public static void AddOneToManyToOneRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinedProperty, string joinedKey)
        {
            builder.Entity(typeof(J))
               .HasOne(typeof(D), joinedProperty)
               .WithMany(drivingProperty)
               .HasPrincipalKey(drivingKey)
               .HasForeignKey(joinedKey);
        }

        public static void AddOneToOneRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinFK)
        {
            builder.Entity(typeof(D))
               .HasOne(typeof(J), drivingProperty)
               .WithOne(null)
               .HasForeignKey(typeof(D), drivingKey)
               .HasPrincipalKey(typeof(J), joinFK);
        }

        public static void AddOneToManyRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinFK)
        {
            //builder.Entity(typeof(D))
            //   .HasMany(typeof(J), drivingProperty)
            //   .WithOne(null)
            //   .HasPrincipalKey(drivingKey)
            //   .HasForeignKey(joinFK);

            builder.Entity(typeof(J))
               .HasOne(typeof(D))
               .WithMany(drivingProperty)
               .HasPrincipalKey(drivingKey)
               .HasForeignKey(joinFK);
        }

        public static void AddManyToOneRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinFK)
        {
            builder.Entity(typeof(D))
               .HasOne(typeof(J), drivingProperty)
               .WithOne(null)
               .HasPrincipalKey(typeof(D), drivingKey)
               .HasForeignKey(typeof(J), joinFK);
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

        public static void AddGlobalTagFilterAndOr<T>(this ModelBuilder builder, ParameterExpression paramExpr, List<Tuple<Expression, TagConnector>> tagExpressions)
        {
            Expression resultExpression = null;

            foreach (Tuple<Expression, TagConnector> exprTuple in tagExpressions)
            {
                Expression newExpressionPart = exprTuple.Item1;
                TagConnector connector = exprTuple.Item2;

                if (resultExpression == null)
                {
                    resultExpression = newExpressionPart;
                }
                else
                {
                    switch (connector)
                    {
                        case TagConnector.None:
                        case TagConnector.AndExpression:
                            resultExpression = Expression.AndAlso(resultExpression, newExpressionPart);
                            break;
                        case TagConnector.OrExpression:
                            resultExpression = Expression.OrElse(resultExpression, newExpressionPart);
                            break;
                    }
                }
            }

            builder.Entity(typeof(T))
                .HasQueryFilter(
                    Expression.Lambda(
                        Expression.Block(resultExpression), paramExpr));
        }
    }
}
