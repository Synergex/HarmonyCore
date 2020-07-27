// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Harmony.Core.EF.Storage.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class HarmonyTypeMapping : CoreTypeMapping
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public HarmonyTypeMapping(
             Type clrType,
             ValueComparer comparer = null,
             ValueComparer keyComparer = null,
             ValueComparer structuralComparer = null)
            : base(
                new CoreTypeMappingParameters(
                    clrType,
                    converter: null,
                    comparer,
                    keyComparer,
                    structuralComparer))
        {
        }

        private HarmonyTypeMapping(CoreTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override CoreTypeMapping Clone(ValueConverter converter)
            => new HarmonyTypeMapping(Parameters.WithComposedConverter(converter));
    }
}
