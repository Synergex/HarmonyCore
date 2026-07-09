using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Core.EF.Extensions
{
    class HarmonyQueryableExtensions
    {
        internal static readonly MethodInfo LeftJoinMethodInfo = typeof(Queryable).GetTypeInfo().GetDeclaredMethods("LeftJoin").Single((MethodInfo mi) => mi.GetParameters().Length == 5);
    }
}
