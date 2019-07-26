// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Harmony.Core.Context;
using Harmony.Core.Interface;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq;

namespace Harmony.Core.EF.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyDatabase : Database, IHarmonyDatabase
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Update> _updateLogger;
        private readonly IDataObjectProvider _dataObjectProvider;
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyDatabase(
            DatabaseDependencies dependencies,
            IDataObjectProvider dataProvider,
            IServiceProvider serviceProvider,
            IDbContextOptions options,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
            : base(dependencies)
        {
            //_store = storeCache.GetStore(options);
            _updateLogger = updateLogger;
            _dataObjectProvider = dataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IDataObjectProvider Store => _dataObjectProvider;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override int SaveChanges(IReadOnlyList<IUpdateEntry> entries)
            => DispatchTransactionFromEntries(entries);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Task<int> SaveChangesAsync(
            IReadOnlyList<IUpdateEntry> entries,
            CancellationToken cancellationToken = default)
            => Task.FromResult(DispatchTransactionFromEntries(entries));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool EnsureDatabaseCreated(StateManagerDependencies stateManagerDependencies)
            => true; //do nothing we dont support this

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Func<QueryContext, IAsyncEnumerable<TResult>> CompileAsyncQuery<TResult>(QueryModel queryModel)
        {
            var syncQueryExecutor = CompileQuery<TResult>(queryModel);
            return qc => syncQueryExecutor(qc).ToAsyncEnumerable();
        }

        public void AddPrimaryKeygeneratorToTransaction(IPrimaryKeyFactory keyFactory)
        {

        }

        public void AddConflictResolverToTransaction(IDataObjectConflictResolver conflictResolver)
        {

        }

        public void Patch(DataObjectBase obj, Func<DataObjectBase, DataObjectBase> applyPatch)
        {

        }

        public void Upsert(DataObjectBase obj)
        {
            
        }

        private int DispatchTransactionFromEntries(IReadOnlyList<IUpdateEntry> entries)
        {
            List<DataObjectBase> created = new List<DataObjectBase>();
            List<DataObjectBase> updated = new List<DataObjectBase>();
            List<DataObjectBase> deleted = new List<DataObjectBase>();

            foreach (var entry in entries)
            {
                if (entry.EntityState == EntityState.Added)
                    created.Add(entry.ToEntityEntry().Entity as DataObjectBase);
                if (entry.EntityState == EntityState.Modified)
                    updated.Add(entry.ToEntityEntry().Entity as DataObjectBase);
                if (entry.EntityState == EntityState.Deleted)
                    deleted.Add(entry.ToEntityEntry().Entity as DataObjectBase);
            }

            try
            {
                _dataObjectProvider.ExecuteTransaction(new FileIOServiceProvider(_serviceProvider, created, updated, deleted), created, updated, deleted);
            }
            catch (Synergex.SynergyDE.RecordNotSameException)
            {
                throw new DbUpdateConcurrencyException("", entries);
            }

            return created.Count + updated.Count + deleted.Count;
        }

        private class FileIOServiceProvider : IServiceProvider
        {
            public IServiceProvider Context { get; set; }
            public IDataObjectTransactionContext TransactionContext {get; set;}
            public FileIOServiceProvider(IServiceProvider sp, IEnumerable<DataObjectBase> created, IEnumerable<DataObjectBase> updated, IEnumerable<DataObjectBase> deleted)
            {
                TransactionContext = new TransactionContext { Created = created, Updated = updated, Deleted = deleted };
                Context = sp;
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(DbContext))
                {
                    return Context.GetService<ICurrentDbContext>().Context;
                }
                else if (serviceType == typeof(IDataObjectTransactionContext))
                {
                    return TransactionContext;
                }
                else
                {
                    return Context.GetService(serviceType);
                }
            }
        }
        private class TransactionContext : IDataObjectTransactionContext
        {
            public IEnumerable<DataObjectBase> Created { get; set; }

            public IEnumerable<DataObjectBase> Updated { get; set; }

            public IEnumerable<DataObjectBase> Deleted { get; set; }
        }
    }
}
