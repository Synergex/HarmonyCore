// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Harmony.Core.EF.Infrastructure.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonySingletonOptions : IHarmonySingletonOptions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Initialize(IDbContextOptions options)
        {
            var inMemoryOptions = options.FindExtension<HarmonyOptionsExtension>();

            if (inMemoryOptions != null)
            {
                DatabaseRoot = inMemoryOptions.DatabaseRoot;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
            var inMemoryOptions = options.FindExtension<HarmonyOptionsExtension>();

            if (inMemoryOptions != null
                && DatabaseRoot != inMemoryOptions.DatabaseRoot)
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(HarmonyDbContextOptionsExtensions.UseHarmonyDatabase),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual HarmonyDatabaseRoot DatabaseRoot { get; private set; }
    }
}
