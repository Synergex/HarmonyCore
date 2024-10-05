// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.Json;

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
             ValueComparer keyComparer = null)
            : base(
                new CoreTypeMappingParameters(
                    clrType,
                    converter: null,
                    comparer,
                    keyComparer))
        {
        }

        private HarmonyTypeMapping(CoreTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        public override CoreTypeMapping WithComposedConverter(
            ValueConverter converter,
            ValueComparer comparer = null,
            ValueComparer keyComparer = null,
            CoreTypeMapping elementMapping = null,
            JsonValueReaderWriter jsonValueReaderWriter = null)
            => new HarmonyTypeMapping(Parameters.WithComposedConverter(converter, comparer, keyComparer, elementMapping, jsonValueReaderWriter));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override CoreTypeMapping Clone(CoreTypeMappingParameters parameters)
            => new HarmonyTypeMapping(parameters);
    }
}
