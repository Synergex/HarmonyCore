// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Harmony.Core.FileIO.Queryable;

namespace Harmony.Core.EF.Query.Internal
{
    public class HarmonyTableExpression : Expression, IPrintableExpression
    {
        public HarmonyTableExpression(IEntityType entityType)
        {
            EntityType = entityType;
            Type = typeof(IEnumerable<>).MakeGenericType(new Type[] { EntityType.ClrType });
        }

        public override Type Type { get; } 
        public virtual IEntityType EntityType { get; }
        public PreparedQueryPlan QueryPlan { get; set; }
        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        public virtual void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append(nameof(HarmonyTableExpression) + ": Entity: " + EntityType.DisplayName());
        }
    }
}
