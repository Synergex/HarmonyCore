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
            var queryModel = QueryModelVisitor.ActiveQueryModel;
            var model = _model;
            var entityType = context.FindEntityType(_querySource) ?? model.FindEntityType(elementType.FullName);
            if (context.QuerySourceRequiresMaterialization(_querySource))
            {
                //Func<Type, EntityTrackingInfo> tracking = null;
                //if (context.IsTrackingQuery)
                //{
                //    var querySourceReference = new QuerySourceReferenceExpression(_querySource);//new QuerySourceReferenceExpression(queryModel.MainFromClause);
                //    var mainTrackingInfo = new EntityTrackingInfo(context, querySourceReference, entityType);
                //    var trackingInfoLookup = new Dictionary<Type, EntityTrackingInfo> { { elementType, mainTrackingInfo } };
                //    tracking = ((ty, lookup) =>
                //    {
                //        EntityTrackingInfo
                //        EntityTrackingInfo result;
                //        if (!trackingInfoLookup.TryGetValue(ty, out result))
                //        {
                //            result = new EntityTrackingInfo(context, querySourceReference, model.FindEntityType(ty.FullName));
                //            trackingInfoLookup.Add(ty, result);
                //        }

                //        return result;
                //    });
                //}
                //else
                //{
                //    tracking = ((ty) => null);
                //}
                return Expression.Call(typeof(System.Linq.Enumerable), "Empty", new Type[] { elementType });
                //return Expression.Call(
                //    HarmonyQueryModelVisitor.EntityQueryMethodInfo.MakeGenericMethod(elementType),
                //    EntityQueryModelVisitor.QueryContextParameter,
                //    Expression.Constant(entityType),
                //    Expression.Constant(queryModel));
            }
            else
            {
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
