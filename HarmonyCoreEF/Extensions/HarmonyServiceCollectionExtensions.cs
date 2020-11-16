// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Harmony.Core.EF.Infrastructure.Internal;
using Harmony.Core.EF.Metadata.Conventions.Internal;
using Harmony.Core.EF.Query.Internal;
using Harmony.Core.EF.Storage;
using Harmony.Core.EF.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Storage;
using Harmony.Core.EF.Storage.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class HarmonyServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Adds the services required by the in-memory database provider for Entity Framework
        ///         to an <see cref="IServiceCollection" />. You use this method when using dependency injection
        ///         in your application, such as with ASP.NET. For more information on setting up dependency
        ///         injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        ///     </para>
        ///     <para>
        ///         You only need to use this functionality when you want Entity Framework to resolve the services it uses
        ///         from an external dependency injection container. If you are not using an external
        ///         dependency injection container, Entity Framework will take care of creating the services it requires.
        ///     </para>
        /// </summary>
        /// <example>
        ///     <code>
        ///         public void ConfigureServices(IServiceCollection services)
        ///         {
        ///             services
        ///                 .AddEntityFrameworkHarmonyDatabase()
        ///                 .AddDbContext&lt;MyContext&gt;((serviceProvider, options) =>
        ///                     options.UseHarmonyDatabase("MyDatabase")
        ///                            .UseInternalServiceProvider(serviceProvider));
        ///         }
        ///     </code>
        /// </example>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkHarmonyDatabase(this IServiceCollection serviceCollection)
        {
            //

            var builder = new EntityFrameworkServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, HarmonyLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<HarmonyOptionsExtension>>()
                .TryAdd<IValueGeneratorSelector, HarmonyValueGeneratorSelector>()
                .TryAdd<IDatabase>(p => p.GetService<IHarmonyDatabase>())
                .TryAdd<IDbContextTransactionManager, HarmonyTransactionManager>()
                .TryAdd<IDatabaseCreator, HarmonyDatabaseCreator>()
                .TryAdd<IQueryContextFactory, HarmonyQueryContextFactory>()
                .TryAdd<IQueryCompilationContextFactory, HarmonyQueryCompilationContextFactory>()
                .TryAdd<IProviderConventionSetBuilder, HarmonyConventionSetBuilder>()
                .TryAdd<ITypeMappingSource, HarmonyTypeMappingSource>()
                .TryAdd<IStateManager, HarmonyStateManager>()
                .TryAdd<IEntityFinderSource, HarmonyEntityFinderSource>()

                // New Query pipeline
                .TryAdd<IQueryTranslationPreprocessorFactory, HarmonyQueryTranslationPreprocessorFactory>()
                .TryAdd<IShapedQueryCompilingExpressionVisitorFactory, HarmonyShapedQueryCompilingExpressionVisitorFactory>()
                .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, HarmonyQueryableMethodTranslatingExpressionVisitorFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, HarmonyQueryTranslationPostprocessorFactory>()
                .TryAdd<ISingletonOptions, IHarmonySingletonOptions>(p => p.GetService<IHarmonySingletonOptions>())
                .TryAddProviderSpecificServices(
                    b => b
                        .TryAddSingleton<IHarmonySingletonOptions, HarmonySingletonOptions>()
                        .TryAddScoped<IHarmonyDatabase, HarmonyDatabase>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }

    }
}
