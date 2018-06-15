// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Harmony.Core.EF.Query.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Harmony.Core.EF.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private readonly IModel _model;
        private readonly IHarmonyMaterializerFactory _materializerFactory;
        private readonly IQuerySource _querySource;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyEntityQueryableExpressionVisitor(
            IModel model,
            IHarmonyMaterializerFactory materializerFactory,
            EntityQueryModelVisitor entityQueryModelVisitor,
             IQuerySource querySource)
            : base(entityQueryModelVisitor)
        {
            _model = model;
            _materializerFactory = materializerFactory;
            _querySource = querySource;
        }


        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            return base.VisitSubQuery(expression);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        private new HarmonyQueryModelVisitor QueryModelVisitor
            => (HarmonyQueryModelVisitor)base.QueryModelVisitor;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitEntityQueryable(Type elementType)
        {
            var entityType = QueryModelVisitor.QueryCompilationContext.FindEntityType(_querySource)
                             ?? _model.FindEntityType(elementType.FullName);

            if (QueryModelVisitor.QueryCompilationContext
                .QuerySourceRequiresMaterialization(_querySource))
            {
                var materializer = _materializerFactory.CreateMaterializer(entityType);

                return Expression.Call(
                    HarmonyQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(elementType),
                    EntityQueryModelVisitor.QueryContextParameter,
                    Expression.Constant(entityType),
                    Expression.Constant(entityType.FindPrimaryKey()),
                    Expression.Constant(new EntityTrackingInfo(QueryModelVisitor.QueryCompilationContext, new QuerySourceReferenceExpression(_querySource), entityType)),
                    Expression.Constant(QueryModelVisitor.ActiveQueryModel),
                    materializer,
                    Expression.Constant(
                        QueryModelVisitor.QueryCompilationContext.IsTrackingQuery
                        && !entityType.IsQueryType));
            }

            return Expression.Call(
                HarmonyQueryModelVisitor.ProjectionQueryMethodInfo,
                EntityQueryModelVisitor.QueryContextParameter,
                Expression.Constant(QueryModelVisitor.ActiveQueryModel),
                Expression.Constant(entityType));
        }
    }
}
