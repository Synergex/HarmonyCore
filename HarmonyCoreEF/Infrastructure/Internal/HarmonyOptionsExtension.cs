// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using Harmony.Core.Context;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.Core.EF.Infrastructure.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyOptionsExtension : IDbContextOptionsExtension
    {
        private string _storeName;
        private HarmonyDatabaseRoot _databaseRoot;
        private string _logFragment;
        private IDataObjectProvider _objectProvider;
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyOptionsExtension()
        {
        }

        public HarmonyOptionsExtension(IDataObjectProvider objectProvider)
        {
            _objectProvider = objectProvider;
            _databaseRoot = new HarmonyDatabaseRoot { Instance = _objectProvider };
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected HarmonyOptionsExtension(HarmonyOptionsExtension copyFrom)
        {
            _storeName = copyFrom._storeName;
            _databaseRoot = copyFrom._databaseRoot;
            _objectProvider = copyFrom._objectProvider;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual HarmonyOptionsExtension Clone() => new HarmonyOptionsExtension(this);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string StoreName => _storeName;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual HarmonyOptionsExtension WithStoreName(string storeName)
        {
            var clone = Clone();

            clone._storeName = storeName;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual HarmonyDatabaseRoot DatabaseRoot => _databaseRoot;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual HarmonyOptionsExtension WithDatabaseRoot(HarmonyDatabaseRoot databaseRoot)
        {
            var clone = Clone();

            clone._databaseRoot = databaseRoot;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool ApplyServices(IServiceCollection services)
        {
            services.AddSingleton<IDataObjectProvider>(_objectProvider);
            services.AddEntityFrameworkHarmonyDatabase();

            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual long GetServiceProviderHashCode() => _databaseRoot?.GetHashCode() ?? 0L;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    var builder = new StringBuilder();

                    builder.Append("StoreName=").Append(_storeName).Append(' ');

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }
    }
}
