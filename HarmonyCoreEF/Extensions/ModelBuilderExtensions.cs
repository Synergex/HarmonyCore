using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Harmony.Core.EF.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void AddOneToOneToOneRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinedProperty, string joinedKey)
        {
            builder.Entity(typeof(D))
               .HasOne(typeof(J), drivingProperty)
               .WithOne(joinedProperty)
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
               .HasForeignKey(typeof(J), joinFK)
               .HasPrincipalKey(typeof(D), drivingKey);
        }

        public static void AddOneToManyRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinFK)
        {
            builder.Entity(typeof(D))
               .HasMany(typeof(J), drivingProperty)
               .WithOne(null)
               .HasForeignKey(joinFK)
               .HasPrincipalKey(drivingKey);
        }

        public static void AddManyToOneRelation<D, J>(this ModelBuilder builder, string drivingProperty, string drivingKey, string joinFK)
        {
            builder.Entity(typeof(D))
               .HasOne(typeof(J), drivingProperty)
               .WithOne(null)
               .HasForeignKey(typeof(J), joinFK)
               .HasPrincipalKey(typeof(D), drivingKey);
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
