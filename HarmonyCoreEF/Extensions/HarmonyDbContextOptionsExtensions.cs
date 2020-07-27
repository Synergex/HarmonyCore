// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Harmony.Core.EF.Infrastructure.Internal;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Harmony.Core.Context;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class HarmonyDbContextOptionsExtensions
    {
        private const string LegacySharedName = "___Shared_Database___";

        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider. To use the same in-memory database across service providers, call
        ///     <see cref="UseHarmonyDatabase{TContext}(DbContextOptionsBuilder{TContext},string,HarmonyDatabaseRoot,Action{HarmonyDbContextOptionsBuilder})" />
        ///     passing a shared <see cref="HarmonyDatabaseRoot"/> on which to root the database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        //public static DbContextOptionsBuilder<TContext> UseHarmonyDatabase<TContext>(
        //    this DbContextOptionsBuilder<TContext> optionsBuilder,
        //    string databaseName,
        //     Action<HarmonyDbContextOptionsBuilder> inMemoryOptionsAction = null)
        //    where TContext : DbContext
        //    => (DbContextOptionsBuilder<TContext>)UseHarmonyDatabase(
        //        (DbContextOptionsBuilder)optionsBuilder, databaseName, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider. To use the same in-memory database across service providers, call
        ///     <see cref="UseHarmonyDatabase(DbContextOptionsBuilder,string,HarmonyDatabaseRoot,Action{HarmonyDbContextOptionsBuilder})" />
        ///     passing a shared <see cref="HarmonyDatabaseRoot"/> on which to root the database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        //public static DbContextOptionsBuilder UseHarmonyDatabase(
        //    this DbContextOptionsBuilder optionsBuilder,
        //    string databaseName,
        //     Action<HarmonyDbContextOptionsBuilder> inMemoryOptionsAction = null)
        //    => UseHarmonyDatabase(optionsBuilder, databaseName, null, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        //public static DbContextOptionsBuilder<TContext> UseHarmonyDatabase<TContext>(
        //    this DbContextOptionsBuilder<TContext> optionsBuilder,
        //    string databaseName,
        //     HarmonyDatabaseRoot databaseRoot,
        //     Action<HarmonyDbContextOptionsBuilder> inMemoryOptionsAction = null)
        //    where TContext : DbContext
        //    => (DbContextOptionsBuilder<TContext>)UseHarmonyDatabase(
        //        (DbContextOptionsBuilder)optionsBuilder, databaseName, databaseRoot, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used, but only for a given
        ///     service provider.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="databaseRoot">
        ///     All in-memory databases will be rooted in this object, allowing the application
        ///     to control their lifetime. This is useful when sometimes the context instance
        ///     is created explicitly with <c>new</c> while at other times it is resolved using dependency injection.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseHarmonyDatabase(
            this DbContextOptionsBuilder optionsBuilder,
            IDataObjectProvider dataProvider)
        {
            var extension = optionsBuilder.Options.FindExtension<HarmonyOptionsExtension>() ?? new HarmonyOptionsExtension(dataProvider);
            ConfigureWarnings(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            return optionsBuilder;
        }

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            // Set warnings defaults
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    HarmonyEventId.TransactionIgnoredWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
