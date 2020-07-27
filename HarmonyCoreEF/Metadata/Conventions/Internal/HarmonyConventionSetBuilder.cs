// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Harmony.Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.Core.EF.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyConventionSetBuilder : ProviderConventionSetBuilder
    {
        /// <summary>
        ///     Creates a new <see cref="InMemoryConventionSetBuilder" /> instance.
        /// </summary>
        /// <param name="dependencies"> The core dependencies for this service. </param>
        public HarmonyConventionSetBuilder(
            ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     <para>
        ///         Call this method to build a <see cref="ConventionSet" /> for the in-memory provider when using
        ///         the <see cref="ModelBuilder" /> outside of <see cref="DbContext.OnModelCreating" />.
        ///     </para>
        ///     <para>
        ///         Note that it is unusual to use this method.
        ///         Consider using <see cref="DbContext" /> in the normal way instead.
        ///     </para>
        /// </summary>
        /// <returns> The convention set. </returns>
        public static ConventionSet Build()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkHarmonyDatabase()
                .AddDbContext<DbContext>(
                    (p, o) =>
                        o.UseHarmonyDatabase(p.GetService(typeof(IDataObjectProvider)) as IDataObjectProvider))
                .BuildServiceProvider();

            using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<DbContext>();
            return ConventionSet.CreateConventionSet(context);
        }
    }
}
