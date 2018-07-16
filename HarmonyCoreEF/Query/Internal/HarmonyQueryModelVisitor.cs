// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Harmony.Core.FileIO.Queryable;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace Harmony.Core.EF.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyQueryModelVisitor : EntityQueryModelVisitor
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyQueryModelVisitor(
            EntityQueryModelVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            ActiveQueryModel = queryModel;
            base.VisitQueryModel(queryModel);
            
        }

        public QueryModel ActiveQueryModel { get; set; }
        public IQuerySource QuerySource { get; internal set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static readonly MethodInfo EntityQueryMethodInfo
            = typeof(HarmonyQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(EntityQuery));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static readonly MethodInfo OfTypeMethodInfo
            = typeof(Enumerable).GetTypeInfo()
                .GetDeclaredMethod(nameof(Enumerable.OfType));

        Dictionary<Type, IList<object>> _results = null;

        

        private IEnumerable<TEntity> EntityQuery<TEntity>(
            QueryContext queryContext,
            IEntityType entityType,
            Func<Type, EntityTrackingInfo> trackingInfo,
            QueryModel queryModel,
            QueryCompilationContext compilationContext)
            where TEntity : class
        {
            if (_results == null)
            {
                //compilationContext.FindEntityType(null).FindForeignKeys((IProperty)null).FirstOrDefault().PrincipalKey.Properties.First().ClrType
                _results = new Dictionary<Type, IList<object>>();
                var resultList = new List<object>();
                _results.Add(typeof(TEntity), resultList);
                var selectInternalResult = QueryModelVisitor.ExecuteSelectInternal((expr, propName, outerQuery, innerQuery) =>
                {
                    
                    var joiningType = compilationContext.Model.FindEntityType(outerQuery.ItemType);
                    var mainEntityType = compilationContext.Model.FindEntityType(innerQuery.ItemType);
                    var targetProperty = joiningType.FindProperty(propName);
                    var maybeForeignKeys = joiningType.FindForeignKeys(targetProperty);
                    var targetKey = maybeForeignKeys.FirstOrDefault((key) => key.PrincipalEntityType == mainEntityType);

                    if (targetKey == null)
                    {
                        var primaryKey = joiningType.FindKey(targetProperty);
                        if (primaryKey == null)
                            throw new NotImplementedException();
                        targetKey = primaryKey.GetReferencingForeignKeys().FirstOrDefault((key) => key.DeclaringEntityType == mainEntityType);
                        if (targetKey == null)
                            throw new NotImplementedException();
                    }

                    return Expression.Equal(expr, Expression.Call(typeof(Microsoft.EntityFrameworkCore.EF), "Property", new Type[] { expr.Type }, new QuerySourceReferenceExpression(outerQuery), Expression.Constant(targetKey.PrincipalKey.Properties.First().Name)));
                }, queryModel, (obj) =>
                {
                    var objType = obj.GetType();
                    trackingInfo(obj.GetType()).StartTracking(queryContext.StateManager, obj, new ValueBuffer(obj.InternalGetValues()));
                    IList<object> targetList;
                    if (!_results.TryGetValue(objType, out targetList))
                    {
                        targetList = new List<object>();
                        _results.Add(objType, targetList);
                    }
                    targetList.Add(obj);
                    return obj;
                }, queryContext.ParameterValues, (((HarmonyQueryContext)queryContext).Store)).OfType<TEntity>();
            }
            return _results[typeof(TEntity)].OfType<TEntity>();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static readonly MethodInfo ProjectionQueryMethodInfo
            = typeof(HarmonyQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(ProjectionQuery));


        private static IEnumerable<ValueBuffer> ProjectionQuery(
            QueryContext queryContext,
            QueryModel queryModel,
            IEntityType entityType,
            QueryCompilationContext compilationContext)
            => QueryModelVisitor.ExecuteSelectInternal((expr, propName, outerQuery, innerQuery) =>
            {
                var joiningType = compilationContext.Model.FindEntityType(outerQuery.ItemType);
                var mainEntityType = compilationContext.Model.FindEntityType(innerQuery.ItemType);
                var maybeForeignKeys = joiningType.FindForeignKeys(joiningType.FindProperty(propName));
                var targetKey = maybeForeignKeys.FirstOrDefault((key) => key.PrincipalEntityType == mainEntityType);

                if (targetKey == null)
                    throw new NotImplementedException();

                return Expression.And(expr, Expression.Call(typeof(Microsoft.EntityFrameworkCore.EF), "Property", new Type[] { targetKey.PrincipalKey.Properties.First().ClrType }, Expression.Constant(outerQuery), Expression.Constant(targetKey.PrincipalKey.Properties.First().Name)));
            }, queryModel, (obj) => { queryContext.QueryBuffer.StartTracking(obj, entityType); return obj; }, queryContext.ParameterValues, (((HarmonyQueryContext)queryContext).Store)).OfType<DataObjectBase>()
                .Select(t => new ValueBuffer(t.InternalGetValues()));
    }
}
