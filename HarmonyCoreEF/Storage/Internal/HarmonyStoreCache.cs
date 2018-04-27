// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using Harmony.Core.EF.Infrastructure.Internal;

namespace Harmony.Core.EF.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyStoreCache : IHarmonyStoreCache
    {
        private readonly IHarmonyTableFactory _tableFactory;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<string, IHarmonyStore> _namedStores;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Obsolete("Use the constructor that also accepts options.")]
        public HarmonyStoreCache(IHarmonyTableFactory tableFactory)
            : this(tableFactory, null)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyStoreCache(
            IHarmonyTableFactory tableFactory,
             IHarmonySingletonOptions options)
        {
            _tableFactory = tableFactory;

            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<string, IHarmonyStore>());

                _namedStores = (ConcurrentDictionary<string, IHarmonyStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<string, IHarmonyStore>();
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IHarmonyStore GetStore(string name)
            => _namedStores.GetOrAdd(name, n => new HarmonyStore(_tableFactory, _useNameMatching));
    }
}
