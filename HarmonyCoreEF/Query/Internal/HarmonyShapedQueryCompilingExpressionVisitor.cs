// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore;
using Harmony.Core.FileIO.Queryable;

namespace Harmony.Core.EF.Query.Internal
{
    public partial class HarmonyShapedQueryCompilingExpressionVisitor : ShapedQueryCompilingExpressionVisitor
    {
        private readonly Type _contextType;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _logger;

        public HarmonyShapedQueryCompilingExpressionVisitor(
            ShapedQueryCompilingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
            _contextType = queryCompilationContext.ContextType;
            _logger = queryCompilationContext.Logger;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            switch (extensionExpression)
            {
                case HarmonyQueryExpression inMemoryQueryExpression:
                    inMemoryQueryExpression.ApplyProjection();
                    return Visit(inMemoryQueryExpression.ServerQueryExpression);

                case HarmonyTableExpression inMemoryTableExpression:
                    return Expression.Call(
                        _tableMethodInfo,
                        QueryCompilationContext.QueryContextParameter,
                        Expression.Constant(inMemoryTableExpression.EntityType),
                        Expression.Constant(inMemoryTableExpression.RootExpression.PrepareQuery()));
            }

            return base.VisitExtension(extensionExpression);
        }

        protected override Expression VisitShapedQueryExpression(ShapedQueryExpression shapedQueryExpression)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)shapedQueryExpression.QueryExpression;

            var shaper = new ShaperExpressionProcessingExpressionVisitor(
                    inMemoryQueryExpression, inMemoryQueryExpression.CurrentParameter)
                .Inject(shapedQueryExpression.ShaperExpression);

            shaper = InjectEntityMaterializers(shaper);

            var innerEnumerable = Visit(inMemoryQueryExpression);

            shaper = new HarmonyProjectionBindingRemovingExpressionVisitor().Visit(shaper);

            shaper = new CustomShaperCompilingExpressionVisitor(IsTracking).Visit(shaper);

            var shaperLambda = (LambdaExpression)shaper;

            return Expression.New(
                typeof(QueryingEnumerable<>).MakeGenericType(shaperLambda.ReturnType).GetConstructors()[0],
                QueryCompilationContext.QueryContextParameter,
                innerEnumerable,
                Expression.Constant(shaperLambda.Compile()),
                Expression.Constant(_contextType),
                Expression.Constant(_logger));
        }

        private static readonly MethodInfo _tableMethodInfo
            = typeof(HarmonyShapedQueryCompilingExpressionVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(Table));

        private static IEnumerable<DataObjectBase> Table(
            QueryContext queryContext,
            IEntityType entityType,
            PreparedQueryPlan queryPlan)
        {
            Func<DataObjectBase, DataObjectBase> track = (obj) =>
            {
                queryContext.StartTracking(entityType, obj, default(Microsoft.EntityFrameworkCore.Storage.ValueBuffer));
                return obj;
            };

            if (queryPlan.IsCollection)
            {
                return typeof(PreparedQueryPlan)
                    .GetMethod("ExecuteCollectionPlan")
                    .MakeGenericMethod(new Type[] { entityType.ClrType })
                    .Invoke(queryPlan, new object[] { track, ((HarmonyQueryContext)queryContext).ParameterValues, ((HarmonyQueryContext)queryContext).Store }) as IEnumerable<DataObjectBase>;
            }
            else
            {
                var singleResult = typeof(PreparedQueryPlan)
                    .GetMethod("ExecutePlan")
                    .MakeGenericMethod(new Type[] { entityType.ClrType })
                    .Invoke(queryPlan, new object[] { track, ((HarmonyQueryContext)queryContext).ParameterValues, ((HarmonyQueryContext)queryContext).Store }) as DataObjectBase;
                var arrayResult = Array.CreateInstance(entityType.ClrType, 1);
                arrayResult.SetValue(singleResult, 0);
                return arrayResult as IEnumerable<DataObjectBase>;
            }
        }
    }
}
