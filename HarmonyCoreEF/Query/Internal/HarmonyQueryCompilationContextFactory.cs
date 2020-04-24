using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harmony.Core.EF.Query.Internal
{
    class HarmonyQueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        private readonly QueryCompilationContextDependencies _dependencies;

        public HarmonyQueryCompilationContextFactory(QueryCompilationContextDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public virtual QueryCompilationContext Create(bool async)
        {
            return new HarmonyQueryCompilationContext(_dependencies, async);
        }
    }
}
