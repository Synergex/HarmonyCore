using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Harmony.Core.EF.Infrastructure.Internal
{
    internal class HarmonyStateManager : StateManager
    {
        public HarmonyStateManager([NotNullAttribute] StateManagerDependencies dependencies) : base(dependencies)
        {
            
        }
    }
}
