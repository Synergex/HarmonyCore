// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Harmony.Core.EF.Properties;
using Microsoft.EntityFrameworkCore.Update;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class HarmonyLoggerExtensions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void TransactionIgnoredWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> diagnostics)
        {
            //var definition = HarmonyStrings.LogTransactionsNotSupported;

            //var warningBehavior = definition.GetLogBehavior(diagnostics);
            //if (warningBehavior != WarningBehavior.Ignore)
            //{
            //    definition.Log(diagnostics, warningBehavior);
            //}

            //if (diagnostics.DiagnosticSource.IsEnabled(definition.EventId.Name))
            //{
            //    diagnostics.DiagnosticSource.Write(
            //        definition.EventId.Name,
            //        new EventData(
            //            definition,
            //            (d, p) => ((EventDefinition)d).GenerateMessage()));
            //}
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void ChangesSaved(
            this IDiagnosticsLogger<DbLoggerCategory.Update> diagnostics,
            IEnumerable<IUpdateEntry> entries,
            int rowsAffected)
        {
            //var definition = HarmonyStrings.LogSavedChanges;

            //var warningBehavior = definition.GetLogBehavior(diagnostics);
            //if (warningBehavior != WarningBehavior.Ignore)
            //{
            //    definition.Log(
            //        diagnostics,
            //        warningBehavior,
            //        rowsAffected);
            //}

            //if (diagnostics.DiagnosticSource.IsEnabled(definition.EventId.Name))
            //{
            //    diagnostics.DiagnosticSource.Write(
            //        definition.EventId.Name,
            //        new SaveChangesEventData(
            //            definition,
            //            ChangesSaved,
            //            entries,
            //            rowsAffected));
            //}
        }

        private static string ChangesSaved(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<int>)definition;
            var p = (SaveChangesEventData)payload;
            return d.GenerateMessage(p.RowsAffected);
        }
    }
}
