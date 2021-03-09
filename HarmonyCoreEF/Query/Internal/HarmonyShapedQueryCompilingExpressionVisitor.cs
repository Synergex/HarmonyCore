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
using Harmony.Core.EF.Extensions.Internal;
using Harmony.Core.Utility;
using System.Runtime.CompilerServices;

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
                    return inMemoryTableExpression.RootExpression.PrepareQuery(_tableMethodInfo, QueryCompilationContext.QueryContextParameter, _compilationContext);
                    //return Expression.Call(
                    //    _tableMethodInfo,
                    //    QueryCompilationContext.QueryContextParameter,
                    //    Expression.Constant(inMemoryTableExpression.EntityType),
                    //    Expression.Constant(inMemoryTableExpression.RootExpression.PrepareQuery(_compilationContext)),
                    //    Expression.Constant(_compilationContext.IsTracking));
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

            var innerEnumerableType = innerEnumerable.Type.TryGetSequenceType() ?? typeof(DataObjectBase);
            var shaperArg = Expression.Parameter(innerEnumerableType);
            var capturedShaper = Expression.Lambda(
                Expression.Invoke(shaperLambda, 
                    Expression.Convert(shaperArg, shaperLambda.Parameters[0].Type)), 
                new ParameterExpression[] { QueryCompilationContext.QueryContextParameter, shaperArg });

            //TODO: we still need to pass the query context along here
            if (shapedQueryExpression.ResultCardinality == ResultCardinality.Single && shaperLambda.Parameters[0].Type == innerEnumerable.Type)
            {
                return Expression.NewArrayInit(shaperLambda.ReturnType, Expression.Invoke(shaperLambda, innerEnumerable));
            }
            else
            {
                //if trace logging is turned on, compile the shaper expression with debug info
                Delegate shaperInstance = null;
                //put this back in when its supported in .Net Core
                //if (DebugLogSession.Logging.Level == Interface.LogLevel.Trace)
                //    shaperInstance = capturedShaper.Compile(DebugInfoGenerator.CreatePdbGenerator());
                //else
                shaperInstance = capturedShaper.Compile();

                return Expression.New(
                typeof(QueryingEnumerable<,>).MakeGenericType(shaperLambda.ReturnType, innerEnumerableType).GetConstructors()[0],
                QueryCompilationContext.QueryContextParameter,
                innerEnumerable,
                Expression.Constant(shaperInstance),
                Expression.Constant(_contextType),
                Expression.Constant(_logger));
            }

            
        }

        private static readonly MethodInfo _tableMethodInfo
            = typeof(HarmonyShapedQueryCompilingExpressionVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(Table));

        private static IEnumerable<DataObjectBase> Table(
            QueryContext queryContext,
            IEntityType entityType,
            PreparedQueryPlan queryPlan,
            bool isTracking)
        {
            Func<DataObjectBase, DataObjectBase> track = (obj) =>
            {
                if (isTracking)
                {
                    var localType = entityType;
                    if (entityType.ClrType != obj.GetType())
                    {
                        localType = queryContext.Context.Model.FindEntityType(obj.GetType());
                    }
                    var keyValues = localType.FindPrimaryKey().Properties.Select(prop => prop.GetGetter().GetClrValue(obj)).ToArray();
                    var foundEntry = queryContext.StateManager.TryGetEntry(localType.FindPrimaryKey(), keyValues);
                    if (foundEntry != null && foundEntry.EntityState != EntityState.Detached)
                        return foundEntry.Entity as DataObjectBase;
                    else
                        queryContext.StartTracking(localType, obj, default(Microsoft.EntityFrameworkCore.Storage.ValueBuffer));
                }

                return obj;
            };

            if (queryPlan.IsCollection)
            {
                return typeof(PreparedQueryPlan)
                    .GetMethod("ExecuteCollectionPlan")
                    .MakeGenericMethod(new Type[] { entityType.ClrType })
                    .Invoke(queryPlan, new object[] { track, ((HarmonyQueryContext)queryContext).ParameterValues, ((HarmonyQueryContext)queryContext).Store, queryContext }) as IEnumerable<DataObjectBase>;
            }
            else
            {
                var singleResult = typeof(PreparedQueryPlan)
                    .GetMethod("ExecutePlan")
                    .MakeGenericMethod(new Type[] { entityType.ClrType })
                    .Invoke(queryPlan, new object[] { track, ((HarmonyQueryContext)queryContext).ParameterValues, ((HarmonyQueryContext)queryContext).Store, queryContext }) as DataObjectBase;
                var arrayResult = Array.CreateInstance(entityType.ClrType, 1);
                arrayResult.SetValue(singleResult, 0);
                return arrayResult as IEnumerable<DataObjectBase>;
            }
        }
    }
}
