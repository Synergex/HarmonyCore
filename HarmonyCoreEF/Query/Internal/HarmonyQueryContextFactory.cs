// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Harmony.Core.EF.Storage.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Harmony.Core.Context;

namespace Harmony.Core.EF.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyQueryContextFactory : IQueryContextFactory
    {
        private readonly IDataObjectProvider _provider;
        private readonly QueryContextDependencies _dependencies;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyQueryContextFactory(
            QueryContextDependencies dependencies,
            IDataObjectProvider provider,
            IDbContextOptions contextOptions)
        {
            _provider = provider;
            _dependencies = dependencies;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual QueryContext Create()
            => new HarmonyQueryContext(_dependencies, _provider);
    }
}
