using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harmony.Core.EF.Query.Internal
{
    internal class HarmonyQueryTranslationPreprocessorFactory : QueryTranslationPreprocessorFactory
    {
        QueryTranslationPreprocessorDependencies _dependencies;
        public HarmonyQueryTranslationPreprocessorFactory(
            QueryTranslationPreprocessorDependencies dependencies) : base(dependencies)
        {
            _dependencies = dependencies;
        }


        public override QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        {
            return new HarmonyQueryTranslationPreprocessor(_dependencies, queryCompilationContext);
        }
    }
}
