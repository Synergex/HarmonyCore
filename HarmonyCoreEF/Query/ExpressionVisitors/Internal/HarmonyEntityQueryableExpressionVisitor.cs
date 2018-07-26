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
using System.Collections.Generic;

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

        protected override Expression VisitEntityQueryable(Type elementType)
        {
            var context = QueryModelVisitor.QueryCompilationContext;
            var queryModel = QueryModelVisitor.QueryPlan;
            if (context.QuerySourceRequiresMaterialization(_querySource))
            {
                return Expression.Call(
                    HarmonyQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(elementType),
                    EntityQueryModelVisitor.QueryContextParameter,
                    Expression.Constant(QueryModelVisitor.QueryPlan),
                    Expression.Constant(context.IsTrackingQuery));
            }
            else
            {
                var entityType = context.FindEntityType(_querySource) ?? _model.FindEntityType(elementType.FullName);
                return Expression.Call(
                HarmonyQueryModelVisitor.ProjectionQueryMethodInfo,
                EntityQueryModelVisitor.QueryContextParameter,
                Expression.Constant(entityType),
                Expression.Constant(context));
            }
        }

        private new HarmonyQueryModelVisitor QueryModelVisitor
            => (HarmonyQueryModelVisitor)base.QueryModelVisitor;

    }
}
