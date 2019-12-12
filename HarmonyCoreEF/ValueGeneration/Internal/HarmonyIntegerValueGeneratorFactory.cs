// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Harmony.Core.EF.Extensions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Harmony.Core.EF.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyIntegerValueGeneratorFactory : ValueGeneratorFactory
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property)
        {
            var type = property.ClrType.UnwrapNullableType().UnwrapEnumType();

            if (type == typeof(long))
            {
                return new HarmonyIntegerValueGenerator<long>();
            }

            if (type == typeof(int))
            {
                return new HarmonyIntegerValueGenerator<int>();
            }

            if (type == typeof(short))
            {
                return new HarmonyIntegerValueGenerator<short>();
            }

            if (type == typeof(byte))
            {
                return new HarmonyIntegerValueGenerator<byte>();
            }

            if (type == typeof(ulong))
            {
                return new HarmonyIntegerValueGenerator<ulong>();
            }

            if (type == typeof(uint))
            {
                return new HarmonyIntegerValueGenerator<uint>();
            }

            if (type == typeof(ushort))
            {
                return new HarmonyIntegerValueGenerator<ushort>();
            }

            if (type == typeof(sbyte))
            {
                return new HarmonyIntegerValueGenerator<sbyte>();
            }

            throw new ArgumentException(
                CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(HarmonyIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
