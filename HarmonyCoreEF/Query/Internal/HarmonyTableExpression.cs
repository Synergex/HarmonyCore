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
    public class HarmonyTableExpression : Expression, IPrintableExpression, IHarmonyQueryTable
    {
        public HarmonyTableExpression(IEntityType entityType, string name, HarmonyQueryExpression rootExpr)
        {
            EntityType = entityType;
            Type = typeof(IEnumerable<>).MakeGenericType(new Type[] { EntityType.ClrType });
            Name = name;
            RootExpression = rootExpr;
            WhereExpressions = new List<Expression>();
            OrderByExpressions = new List<Tuple<Expression, bool>>();
            OnExpressions = new List<Expression>();
            Aliases = new List<Expression>();
        }

        public override Type Type { get; } 
        public virtual IEntityType EntityType { get; }
        public HarmonyQueryExpression RootExpression { get; }
        public List<Expression> Aliases { get; }
        public List<Expression> WhereExpressions { get; }
        public List<Tuple<Expression, bool>> OrderByExpressions { get; }
        public List<Expression> OnExpressions { get; }
        public Expression Top { get; set; }
        public Expression Skip { get; set; }
        public bool IsCaseSensitive { get; set; }
        public bool IsCollection { get; set; }
        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public string Name { get; set; }

        public Type ItemType => EntityType.ClrType;

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
