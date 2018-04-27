// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Harmony.Core.EF.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyTableFactory : IdentityMapFactoryFactoryBase, IHarmonyTableFactory
    {
        private readonly bool _sensitiveLoggingEnabled = false;

        private readonly ConcurrentDictionary<IKey, Func<IHarmonyTable>> _factories
            = new ConcurrentDictionary<IKey, Func<IHarmonyTable>>();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyTableFactory(ILoggingOptions loggingOptions)
        {
            _sensitiveLoggingEnabled = loggingOptions.IsSensitiveDataLoggingEnabled;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IHarmonyTable Create(IEntityType entityType)
            => _factories.GetOrAdd(entityType.FindPrimaryKey(), Create)();

        private Func<IHarmonyTable> Create(IKey key)
            => (Func<IHarmonyTable>)typeof(HarmonyTableFactory).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateFactory))
                .MakeGenericMethod(GetKeyType(key))
                .Invoke(null, new object[] { key, _sensitiveLoggingEnabled });

        
        private static Func<IHarmonyTable> CreateFactory<TKey>(IKey key, bool sensitiveLoggingEnabled)
            => () => new HarmonyTable<TKey>(key.GetPrincipalKeyValueFactory<TKey>(), sensitiveLoggingEnabled);
    }
}
