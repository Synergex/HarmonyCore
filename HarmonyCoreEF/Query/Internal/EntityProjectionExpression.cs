// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Harmony.Core.EF.Query.Internal
{
    public class EntityProjectionExpression : Expression, IPrintableExpression
    {
        private readonly IDictionary<IProperty, Expression> _readExpressionMap;

        private readonly IDictionary<INavigation, StructuralTypeShaperExpression> _navigationExpressionsCache
            = new Dictionary<INavigation, StructuralTypeShaperExpression>();

        public EntityProjectionExpression(
            IEntityType entityType, IDictionary<IProperty, Expression> readExpressionMap)
        {
            EntityType = entityType;
            _readExpressionMap = readExpressionMap;
        }

        public virtual IEntityType EntityType { get; }
        public override Type Type => EntityType.ClrType;
        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public virtual EntityProjectionExpression UpdateEntityType(IEntityType derivedType)
        {
            var readExpressionMap = new Dictionary<IProperty, Expression>();
            foreach (var kvp in _readExpressionMap)
            {
                var property = kvp.Key;
                if (derivedType.IsAssignableFrom(property.DeclaringType)
                    || property.DeclaringType.IsAssignableFrom(derivedType))
                {
                    readExpressionMap[property] = kvp.Value;
                }
            }

            return new EntityProjectionExpression(derivedType, readExpressionMap);
        }

        public virtual Expression BindProperty(IProperty property)
        {
            if (!EntityType.IsAssignableFrom(property.DeclaringType)
                && !property.DeclaringType.IsAssignableFrom(EntityType))
            {
                throw new InvalidOperationException(
                    $"Called EntityProjectionExpression.BindProperty() with incorrect IProperty. EntityType:{EntityType.DisplayName()}, Property:{property.Name}");
            }

            return _readExpressionMap[property];
        }

        public virtual void AddNavigationBinding(INavigation navigation, StructuralTypeShaperExpression shaper)
        {
            if (!EntityType.IsAssignableFrom(navigation.DeclaringEntityType)
                && !navigation.DeclaringEntityType.IsAssignableFrom(EntityType))
            {
                throw new InvalidOperationException(
                    "Called EntityProjectionExpression.AddNavigationBinding() with incorrect INavigation. "
                    + $"EntityType:{EntityType.DisplayName()}, Property:{navigation.Name}");
            }

            _navigationExpressionsCache[navigation] = shaper;
        }

        public virtual StructuralTypeShaperExpression? BindNavigation(INavigation navigation)
        {
            if (!EntityType.IsAssignableFrom(navigation.DeclaringEntityType)
                && !navigation.DeclaringEntityType.IsAssignableFrom(EntityType))
            {
                throw new InvalidOperationException(
                    "Called EntityProjectionExpression.BindNavigation() with incorrect INavigation. "
                    + $"EntityType:{EntityType.DisplayName()}, Property:{navigation.Name}");
            }

            return _navigationExpressionsCache.TryGetValue(navigation, out var expression)
                ? expression
                : null;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        void IPrintableExpression.Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.AppendLine(nameof(EntityProjectionExpression) + ":");
            using (expressionPrinter.Indent())
            {
                foreach (var (property, methodCallExpression) in _readExpressionMap)
                {
                    expressionPrinter.Append(property + " -> ");
                    expressionPrinter.Visit(methodCallExpression);
                    expressionPrinter.AppendLine();
                }
            }
        }
    }
}
