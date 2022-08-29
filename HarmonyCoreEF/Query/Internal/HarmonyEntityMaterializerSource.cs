using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Core.EF.Query.Internal
{
    class HarmonyEntityMaterializerSource
    {
        public static readonly MethodInfo TryReadValueMethod
            = typeof(EntityMaterializerSource).GetTypeInfo()
                .GetDeclaredMethod(nameof(TryReadValue));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TValue TryReadValue<TValue>(
            in ValueBuffer valueBuffer, int index, IPropertyBase property)
            => valueBuffer[index] is TValue value ? value : default;
    }
}
