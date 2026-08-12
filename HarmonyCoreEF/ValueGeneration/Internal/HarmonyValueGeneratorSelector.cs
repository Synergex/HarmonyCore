// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Harmony.Core.EF.Extensions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Harmony.Core.EF.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class HarmonyValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly HarmonyIntegerValueGeneratorFactory _inMemoryFactory = new HarmonyIntegerValueGeneratorFactory();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public HarmonyValueGeneratorSelector(ValueGeneratorSelectorDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     EF Core 10's ValueGenerationManager requires TrySelect to succeed for every
        ///     value-generating single-column key property — even when the entity already has an
        ///     explicit value — so integer keys need a generator here (mirrors the InMemory provider).
        /// </summary>
        public override bool TrySelect(IProperty property, ITypeBase typeBase, out ValueGenerator valueGenerator)
        {
            if (property.GetValueGeneratorFactory() == null
                && property.ClrType.IsInteger()
                && property.ClrType.UnwrapNullableType() != typeof(char))
            {
                valueGenerator = Cache.GetOrAdd(property, typeBase, (p, t) => _inMemoryFactory.Create(p, t));
                return true;
            }

            return base.TrySelect(property, typeBase, out valueGenerator);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property, ITypeBase entityType)
        {
            return base.Create(property, entityType);
        }
    }
}
