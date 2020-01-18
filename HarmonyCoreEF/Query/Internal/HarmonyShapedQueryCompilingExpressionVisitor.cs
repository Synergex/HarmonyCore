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
        private readonly HarmonyQueryCompilationContext _compilationContext;
        public HarmonyShapedQueryCompilingExpressionVisitor(
            ShapedQueryCompilingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
            _contextType = queryCompilationContext.ContextType;
            _logger = queryCompilationContext.Logger; 
            _compilationContext = queryCompilationContext as HarmonyQueryCompilationContext;
            if (_compilationContext == null)
                throw new Exception("invalid compilation context");
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
                        Expression.Constant(inMemoryTableExpression.RootExpression.PrepareQuery(_compilationContext)));
            }

            return base.VisitExtension(extensionExpression);
        }

        protected override Expression VisitShapedQueryExpression(ShapedQueryExpression shapedQueryExpression)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)shapedQueryExpression.QueryExpression;

            //var shaper = new ShaperExpressionProcessingExpressionVisitor(
            //        inMemoryQueryExpression, inMemoryQueryExpression.CurrentParameter)
            //    .Inject(shapedQueryExpression.ShaperExpression);

            var shaper = InjectEntityMaterializers(shapedQueryExpression.ShaperExpression);

            var innerEnumerable = Visit(inMemoryQueryExpression);

            shaper = new CustomShaperCompilingExpressionVisitor(IsTracking).Visit(shaper);
            var queryExpr = shapedQueryExpression.QueryExpression as HarmonyQueryExpression;
            LambdaExpression shaperLambda = null;
            if (shaper is LambdaExpression)
                shaperLambda = (LambdaExpression)shaper;
            else
                shaperLambda = Expression.Lambda(shaper, new ParameterExpression[] { queryExpr.CurrentParameter });

            var shaperArg = Expression.Parameter(typeof(DataObjectBase));
            var capturedShaper = Expression.Lambda(
                Expression.Invoke(shaperLambda, 
                    Expression.Convert(shaperArg, shaperLambda.Parameters[0].Type)), 
                new ParameterExpression[] { QueryCompilationContext.QueryContextParameter, shaperArg });


            return Expression.New(
                typeof(QueryingEnumerable<>).MakeGenericType(shaperLambda.ReturnType).GetConstructors()[0],
                QueryCompilationContext.QueryContextParameter,
                innerEnumerable,
                Expression.Constant(capturedShaper.Compile()),
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
