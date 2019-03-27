// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Harmony.Core.Context;
using Harmony.Core.Interface;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Harmony.Core.EF.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IHarmonyDatabase : IDatabase
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IDataObjectProvider Store { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        bool EnsureDatabaseCreated(StateManagerDependencies stateManagerDependencies);

        void AddPrimaryKeygeneratorToTransaction(IPrimaryKeyFactory keyFactory);
        void AddConflictResolverToTransaction(IDataObjectConflictResolver conflictResolver);
        void Patch(DataObjectBase obj, Func<DataObjectBase, DataObjectBase> applyPatch);
        void Upsert(DataObjectBase obj);
    }
}
