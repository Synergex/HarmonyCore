// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Harmony.Core.EF.Storage;
using Microsoft.EntityFrameworkCore;
using Harmony.Core.EF.Extensions.Internal;

namespace Harmony.Core.EF.Query.Internal
{
    public class HarmonyQueryableMethodTranslatingExpressionVisitor : QueryableMethodTranslatingExpressionVisitor
    {
        private static readonly MethodInfo _efPropertyMethod = typeof(Microsoft.EntityFrameworkCore.EF).GetTypeInfo().GetDeclaredMethod(nameof(Microsoft.EntityFrameworkCore.EF.Property));

        private readonly HarmonyExpressionTranslatingExpressionVisitor _expressionTranslator;
        private readonly WeakEntityExpandingExpressionVisitor _weakEntityExpandingExpressionVisitor;
        private readonly HarmonyProjectionBindingExpressionVisitor _projectionBindingExpressionVisitor;
        private readonly IModel _model;
        internal readonly Dictionary<Expression, HarmonyQueryExpression> _parameterToQueryMapping;

        public HarmonyQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            IModel model)
            : base(dependencies, subquery: false)
        {
            _parameterToQueryMapping = new Dictionary<Expression, HarmonyQueryExpression>();
            _expressionTranslator = new HarmonyExpressionTranslatingExpressionVisitor(this);
            _weakEntityExpandingExpressionVisitor = new WeakEntityExpandingExpressionVisitor(_parameterToQueryMapping, _expressionTranslator);
            _projectionBindingExpressionVisitor = new HarmonyProjectionBindingExpressionVisitor(this, _expressionTranslator);
            _model = model;
        }

        protected HarmonyQueryableMethodTranslatingExpressionVisitor(
            HarmonyQueryableMethodTranslatingExpressionVisitor parentVisitor)
            : base(parentVisitor.Dependencies, subquery: true)
        {
            _parameterToQueryMapping = new Dictionary<Expression, HarmonyQueryExpression>();
            _expressionTranslator = parentVisitor._expressionTranslator;
            _weakEntityExpandingExpressionVisitor = parentVisitor._weakEntityExpandingExpressionVisitor;
            _projectionBindingExpressionVisitor = new HarmonyProjectionBindingExpressionVisitor(this, _expressionTranslator);
            _model = parentVisitor._model;
        }

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
            => new HarmonyQueryableMethodTranslatingExpressionVisitor(this);

        protected override ShapedQueryExpression CreateShapedQueryExpression(Type elementType)
        {
            return CreateShapedQueryExpression(_model.FindEntityType(elementType), _parameterToQueryMapping);
        }

        private static ShapedQueryExpression CreateShapedQueryExpression(IEntityType entityType, Dictionary<Expression, HarmonyQueryExpression> mapping)
        {
            var queryExpression = new HarmonyQueryExpression(entityType);
            mapping.Add(queryExpression.CurrentParameter, queryExpression);
            return new ShapedQueryExpression(
                queryExpression,
                Expression.Convert(queryExpression.CurrentParameter, entityType.ClrType));
        }

        protected override ShapedQueryExpression TranslateAll(ShapedQueryExpression source, LambdaExpression predicate)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            predicate = TranslateLambdaExpression(source, predicate, preserveType: true);
            if (predicate == null)
            {
                return null;
            }

            inMemoryQueryExpression.ServerQueryExpression =
                Expression.Call(
                    EnumerableMethods.All.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression,
                    predicate);

            source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

            return source;
        }

        protected override ShapedQueryExpression TranslateAny(ShapedQueryExpression source, LambdaExpression predicate)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;

            if (predicate == null)
            {
                inMemoryQueryExpression.ServerQueryExpression = Expression.Call(
                    EnumerableMethods.AnyWithoutPredicate.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression);
            }
            else
            {
                predicate = TranslateLambdaExpression(source, predicate, preserveType: true);
                if (predicate == null)
                {
                    return null;
                }

                inMemoryQueryExpression.ServerQueryExpression = Expression.Call(
                    EnumerableMethods.AnyWithPredicate.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression,
                    predicate);
            }

            source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

            return source;
        }

        protected override ShapedQueryExpression TranslateAverage(ShapedQueryExpression source, LambdaExpression selector, Type resultType)
            => TranslateScalarAggregate(source, selector, nameof(Enumerable.Average));

        protected override ShapedQueryExpression TranslateCast(ShapedQueryExpression source, Type resultType)
        {
            if (source.ShaperExpression.Type == resultType)
            {
                return source;
            }

            source.ShaperExpression = Expression.Convert(source.ShaperExpression, resultType);

            return source;
        }

        protected override ShapedQueryExpression TranslateConcat(ShapedQueryExpression source1, ShapedQueryExpression source2)
            => TranslateSetOperation(EnumerableMethods.Concat, source1, source2);

        protected override ShapedQueryExpression TranslateContains(ShapedQueryExpression source, Expression item)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            item = TranslateExpression(item, preserveType: true);
            if (item == null)
            {
                return null;
            }

            inMemoryQueryExpression.ServerQueryExpression =
                Expression.Call(
                    EnumerableMethods.Contains.MakeGenericMethod(item.Type),
                    Expression.Call(
                        EnumerableMethods.Select.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type, item.Type),
                        inMemoryQueryExpression.ServerQueryExpression,
                        Expression.Lambda(
                            inMemoryQueryExpression.GetMappedProjection(new ProjectionMember()), inMemoryQueryExpression.CurrentParameter)),
                    item);

            source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

            return source;
        }

        protected override ShapedQueryExpression TranslateCount(ShapedQueryExpression source, LambdaExpression predicate)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;

            if (predicate == null)
            {
                inMemoryQueryExpression.ServerQueryExpression =
                    Expression.Call(
                        EnumerableMethods.CountWithoutPredicate.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                        inMemoryQueryExpression.ServerQueryExpression);
            }
            else
            {
                predicate = TranslateLambdaExpression(source, predicate, preserveType: true);
                if (predicate == null)
                {
                    return null;
                }

                inMemoryQueryExpression.ServerQueryExpression =
                    Expression.Call(
                        EnumerableMethods.CountWithPredicate.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                        inMemoryQueryExpression.ServerQueryExpression,
                        predicate);
            }

            source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

            return source;
        }

        protected override ShapedQueryExpression TranslateDefaultIfEmpty(ShapedQueryExpression source, Expression defaultValue)
        {
            if (defaultValue == null)
            {
                ((HarmonyQueryExpression)source.QueryExpression).ApplyDefaultIfEmpty();
                source.ShaperExpression = MarkShaperNullable(source.ShaperExpression);

                return source;
            }

            return null;
        }

        protected override ShapedQueryExpression TranslateDistinct(ShapedQueryExpression source)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;

            inMemoryQueryExpression.PushdownIntoSubquery();
            inMemoryQueryExpression.ServerQueryExpression
                = Expression.Call(
                    EnumerableMethods.Distinct.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression);

            return source;
        }

        protected override ShapedQueryExpression TranslateElementAtOrDefault(
            ShapedQueryExpression source, Expression index, bool returnDefault)
            => null;

        protected override ShapedQueryExpression TranslateExcept(ShapedQueryExpression source1, ShapedQueryExpression source2)
            => TranslateSetOperation(EnumerableMethods.Except, source1, source2);

        protected override ShapedQueryExpression TranslateFirstOrDefault(
            ShapedQueryExpression source, LambdaExpression predicate, Type returnType, bool returnDefault)
        {
            return TranslateSingleResultOperator(
                source,
                predicate,
                returnType,
                returnDefault
                    ? EnumerableMethods.FirstOrDefaultWithoutPredicate
                    : EnumerableMethods.FirstWithoutPredicate);
        }

        protected override ShapedQueryExpression TranslateGroupBy(
            ShapedQueryExpression source, LambdaExpression keySelector, LambdaExpression elementSelector, LambdaExpression resultSelector)
        {
            var remappedKeySelector = RemapLambdaBody(source, keySelector);

            var translatedKey = TranslateGroupingKey(remappedKeySelector);
            if (translatedKey != null)
            {
                if (elementSelector != null)
                {
                    source = TranslateSelect(source, elementSelector);
                }

                var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
                source.ShaperExpression = inMemoryQueryExpression.ApplyGrouping(translatedKey, source.ShaperExpression);

                if (resultSelector == null)
                {
                    return source;
                }

                var original1 = resultSelector.Parameters[0];
                var original2 = resultSelector.Parameters[1];

                var newResultSelectorBody = new ReplacingExpressionVisitor(
                    new Dictionary<Expression, Expression>
                    {
                        { original1, ((GroupByShaperExpression)source.ShaperExpression).KeySelector },
                        { original2, source.ShaperExpression }
                    }).Visit(resultSelector.Body);

                newResultSelectorBody = ExpandWeakEntities(inMemoryQueryExpression, newResultSelectorBody);

                source.ShaperExpression = _projectionBindingExpressionVisitor.Translate(inMemoryQueryExpression, newResultSelectorBody);

                inMemoryQueryExpression.PushdownIntoSubquery();

                return source;
            }

            return null;
        }

        private Expression TranslateGroupingKey(Expression expression)
        {
            switch (expression)
            {
                case NewExpression newExpression:
                    if (newExpression.Arguments.Count == 0)
                    {
                        return newExpression;
                    }

                    var newArguments = new Expression[newExpression.Arguments.Count];
                    for (var i = 0; i < newArguments.Length; i++)
                    {
                        newArguments[i] = TranslateGroupingKey(newExpression.Arguments[i]);
                        if (newArguments[i] == null)
                        {
                            return null;
                        }
                    }

                    return newExpression.Update(newArguments);

                case MemberInitExpression memberInitExpression:
                    var updatedNewExpression = (NewExpression)TranslateGroupingKey(memberInitExpression.NewExpression);
                    if (updatedNewExpression == null)
                    {
                        return null;
                    }

                    var newBindings = new MemberAssignment[memberInitExpression.Bindings.Count];
                    for (var i = 0; i < newBindings.Length; i++)
                    {
                        var memberAssignment = (MemberAssignment)memberInitExpression.Bindings[i];
                        var visitedExpression = TranslateGroupingKey(memberAssignment.Expression);
                        if (visitedExpression == null)
                        {
                            return null;
                        }

                        newBindings[i] = memberAssignment.Update(visitedExpression);
                    }

                    return memberInitExpression.Update(updatedNewExpression, newBindings);

                default:
                    var translation = _expressionTranslator.Translate(expression);
                    if (translation == null)
                    {
                        return null;
                    }

                    return translation.Type == expression.Type
                        ? translation
                        : Expression.Convert(translation, expression.Type);
            }
        }

        protected override ShapedQueryExpression TranslateGroupJoin(
            ShapedQueryExpression outer, ShapedQueryExpression inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector,
            LambdaExpression resultSelector)
            => null;

        protected override ShapedQueryExpression TranslateIntersect(ShapedQueryExpression source1, ShapedQueryExpression source2)
            => TranslateSetOperation(EnumerableMethods.Intersect, source1, source2);

        protected override ShapedQueryExpression TranslateJoin(
            ShapedQueryExpression outer, ShapedQueryExpression inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector,
            LambdaExpression resultSelector)
        {
            outerKeySelector = TranslateLambdaExpression(outer, outerKeySelector);
            innerKeySelector = TranslateLambdaExpression(inner, innerKeySelector);
            if (outerKeySelector == null
                || innerKeySelector == null)
            {
                return null;
            }

            (outerKeySelector, innerKeySelector) = AlignKeySelectorTypes(outerKeySelector, innerKeySelector);

            var transparentIdentifierType = TransparentIdentifierFactory.Create(
                resultSelector.Parameters[0].Type,
                resultSelector.Parameters[1].Type);

            ((HarmonyQueryExpression)outer.QueryExpression).AddInnerJoin(
                (HarmonyQueryExpression)inner.QueryExpression,
                outerKeySelector,
                innerKeySelector,
                transparentIdentifierType);

            return TranslateResultSelectorForJoin(
                outer,
                resultSelector,
                inner.ShaperExpression,
                transparentIdentifierType);
        }

        private static (LambdaExpression OuterKeySelector, LambdaExpression InnerKeySelector)
            AlignKeySelectorTypes(LambdaExpression outerKeySelector, LambdaExpression innerKeySelector)
        {
            static bool isConvertedToNullable(Expression outer, Expression inner)
                => outer.Type.IsNullableType()
                    && !inner.Type.IsNullableType()
                    && outer.Type.UnwrapNullableType() == inner.Type;

            if (outerKeySelector.Body.Type != innerKeySelector.Body.Type)
            {
                if (isConvertedToNullable(outerKeySelector.Body, innerKeySelector.Body))
                {
                    innerKeySelector = Expression.Lambda(
                        Expression.Convert(innerKeySelector.Body, outerKeySelector.Body.Type), innerKeySelector.Parameters);
                }
                else if (isConvertedToNullable(innerKeySelector.Body, outerKeySelector.Body))
                {
                    outerKeySelector = Expression.Lambda(
                        Expression.Convert(outerKeySelector.Body, innerKeySelector.Body.Type), outerKeySelector.Parameters);
                }
            }

            return (outerKeySelector, innerKeySelector);
        }

        protected override ShapedQueryExpression TranslateLastOrDefault(
            ShapedQueryExpression source, LambdaExpression predicate, Type returnType, bool returnDefault)
        {
            return TranslateSingleResultOperator(
                source,
                predicate,
                returnType,
                returnDefault
                    ? EnumerableMethods.LastOrDefaultWithoutPredicate
                    : EnumerableMethods.LastWithoutPredicate);
        }

        protected override ShapedQueryExpression TranslateLeftJoin(
            ShapedQueryExpression outer, ShapedQueryExpression inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector,
            LambdaExpression resultSelector)
        {
            outerKeySelector = TranslateLambdaExpression(outer, outerKeySelector);
            innerKeySelector = TranslateLambdaExpression(inner, innerKeySelector);
            if (outerKeySelector == null
                || innerKeySelector == null)
            {
                return null;
            }

            (outerKeySelector, innerKeySelector) = AlignKeySelectorTypes(outerKeySelector, innerKeySelector);

            ((HarmonyQueryExpression)outer.QueryExpression).AddLeftJoin(
                (HarmonyQueryExpression)inner.QueryExpression,
                outerKeySelector.Body,
                innerKeySelector.Body);

            INavigation innerNav = null;
            if (resultSelector.Body is BlockExpression block && 
                block.Expressions[0] is ConstantExpression navConstant &&
                navConstant.Value is INavigation navValue)
            {
                innerNav = navValue;
                HarmonyQueryExpression outerQueryExpression;
                if (outer is JoinedShapedQueryExpression joinedQuery)
                {
                    var topOuterQueryExpression = (HarmonyQueryExpression)joinedQuery.QueryExpression;
                    var outerSelectorBody = outerKeySelector.Body as MemberExpression;
                    if (outerKeySelector.Parameters[0] == topOuterQueryExpression.CurrentParameter && (outerSelectorBody == null || !(outerSelectorBody.Expression is MemberExpression)))
                    {
                        outerQueryExpression = topOuterQueryExpression;
                    }
                    else
                    {
                        outerQueryExpression = (HarmonyQueryExpression)joinedQuery.Inner.QueryExpression;
                    }
                }
                else
                {
                    outerQueryExpression = (HarmonyQueryExpression)outer.QueryExpression;
                }
                var outerTable = outerQueryExpression.ServerQueryExpression as HarmonyTableExpression;
                var innerQuery = inner.QueryExpression as HarmonyQueryExpression;
                var innerTable = innerQuery.ServerQueryExpression as HarmonyTableExpression;
                innerTable.Name = string.IsNullOrWhiteSpace(outerTable.Name) ? navValue.Name : outerTable.Name + "." + navValue.Name;
                //Add outerQuery.ConvertedParameter + innerNav to map to innerTable
                var innerExpr = Expression.PropertyOrField(outerQueryExpression.ConvertedParameter, navValue.Name);
                //this should fit into our table as an alias so we can make a mapping later on when processing the On objects
                innerTable.Aliases.Add(innerExpr);
            }
            //make custom shaped Query expression to keep track of the added left join
            return new JoinedShapedQueryExpression(outer.QueryExpression, outer.ShaperExpression, inner, true, innerNav);
        }

        protected override ShapedQueryExpression TranslateLongCount(ShapedQueryExpression source, LambdaExpression predicate)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;

            if (predicate == null)
            {
                inMemoryQueryExpression.ServerQueryExpression =
                    Expression.Call(
                        EnumerableMethods.LongCountWithoutPredicate.MakeGenericMethod(
                            inMemoryQueryExpression.CurrentParameter.Type),
                        inMemoryQueryExpression.ServerQueryExpression);
            }
            else
            {
                predicate = TranslateLambdaExpression(source, predicate, preserveType: true);
                if (predicate == null)
                {
                    return null;
                }

                inMemoryQueryExpression.ServerQueryExpression =
                    Expression.Call(
                        EnumerableMethods.LongCountWithPredicate.MakeGenericMethod(
                            inMemoryQueryExpression.CurrentParameter.Type),
                        inMemoryQueryExpression.ServerQueryExpression,
                        predicate);
            }

            source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

            return source;
        }

        protected override ShapedQueryExpression TranslateMax(ShapedQueryExpression source, LambdaExpression selector, Type resultType)
            => TranslateScalarAggregate(source, selector, nameof(Enumerable.Max));

        protected override ShapedQueryExpression TranslateMin(ShapedQueryExpression source, LambdaExpression selector, Type resultType)
            => TranslateScalarAggregate(source, selector, nameof(Enumerable.Min));

        protected override ShapedQueryExpression TranslateOfType(ShapedQueryExpression source, Type resultType)
        {
            if (source.ShaperExpression is EntityShaperExpression entityShaperExpression)
            {
                var entityType = entityShaperExpression.EntityType;
                if (entityType.ClrType == resultType)
                {
                    return source;
                }

                var baseType = entityType.GetAllBaseTypes().SingleOrDefault(et => et.ClrType == resultType);
                if (baseType != null)
                {
                    source.ShaperExpression = entityShaperExpression.WithEntityType(baseType);

                    return source;
                }

                var derivedType = entityType.GetDerivedTypes().SingleOrDefault(et => et.ClrType == resultType);
                if (derivedType != null)
                {
                    var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
                    var discriminatorProperty = entityType.GetDiscriminatorProperty();
                    var parameter = Expression.Parameter(entityType.ClrType);

                    var callEFProperty = Expression.Call(
                        _efPropertyMethod.MakeGenericMethod(
                            discriminatorProperty.ClrType),
                        parameter,
                        Expression.Constant(discriminatorProperty.Name));

                    var equals = Expression.Equal(
                        callEFProperty,
                        Expression.Constant(derivedType.GetDiscriminatorValue(), discriminatorProperty.ClrType));

                    foreach (var derivedDerivedType in derivedType.GetDerivedTypes())
                    {
                        equals = Expression.OrElse(
                            equals,
                            Expression.Equal(
                                callEFProperty,
                                Expression.Constant(derivedDerivedType.GetDiscriminatorValue(), discriminatorProperty.ClrType)));
                    }

                    var discriminatorPredicate = TranslateLambdaExpression(source, Expression.Lambda(equals, parameter));
                    if (discriminatorPredicate == null)
                    {
                        return null;
                    }

                    inMemoryQueryExpression.ServerQueryExpression = Expression.Call(
                        EnumerableMethods.Where.MakeGenericMethod(typeof(DataObjectBase)),
                        inMemoryQueryExpression.ServerQueryExpression,
                        discriminatorPredicate);

                    var projectionBindingExpression = (ProjectionBindingExpression)entityShaperExpression.ValueBufferExpression;
                    var projectionMember = projectionBindingExpression.ProjectionMember;
                    var entityProjection = (EntityProjectionExpression)inMemoryQueryExpression.GetMappedProjection(projectionMember);

                    inMemoryQueryExpression.ReplaceProjectionMapping(
                        new Dictionary<ProjectionMember, Expression>
                        {
                            { projectionMember, entityProjection.UpdateEntityType(derivedType) }
                        });

                    source.ShaperExpression = entityShaperExpression.WithEntityType(derivedType);

                    return source;
                }
            }

            return null;
        }

        protected override ShapedQueryExpression TranslateOrderBy(
            ShapedQueryExpression source, LambdaExpression keySelector, bool ascending)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;

            keySelector = TranslateLambdaExpression(source, keySelector);
            if (keySelector == null)
            {
                return null;
            }

            var orderBy = ascending ? EnumerableMethods.OrderBy : EnumerableMethods.OrderByDescending;
            inMemoryQueryExpression.ServerQueryExpression
                = Expression.Call(
                    orderBy.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type, keySelector.ReturnType),
                    inMemoryQueryExpression.ServerQueryExpression,
                    keySelector);

            return source;
        }

        protected override ShapedQueryExpression TranslateReverse(ShapedQueryExpression source)
            => null;


        private static bool ComparePastConvert(Expression expr1, Expression expr2)
        {
            if (expr1 == expr2)
                return true;
            else if ((expr1 as UnaryExpression)?.Operand == expr2)
                return true;
            else if ((expr2 as UnaryExpression)?.Operand == expr1)
                return true;
            else if (expr1 is UnaryExpression && expr2 is UnaryExpression && 
                (expr1 as UnaryExpression)?.Operand == (expr2 as UnaryExpression)?.Operand)
                return true;
            else 
                return false;
        }

        private static bool ExtractKeySelectors(BinaryExpression expr, Expression inner, Expression outer, ref Expression innerSelector, ref Expression outerSelector)
        {
            var foundSelector = false;
            var leftBinary = expr.Left as BinaryExpression;
            var rightBinary = expr.Right as BinaryExpression;

            var leftMember = expr.Left as MemberExpression;
            var rightMember = expr.Right as MemberExpression;

            if (leftMember != null && rightMember != null)
            {
                if (ComparePastConvert(leftMember.Expression, inner))
                {
                    innerSelector = leftMember;
                    outerSelector = rightMember;
                    foundSelector = true;
                }
                else if (ComparePastConvert(leftMember.Expression, outer))
                {
                    outerSelector = leftMember;
                    innerSelector = rightMember;
                    foundSelector = true;
                }
                else if (ComparePastConvert(rightMember.Expression, inner))
                {
                    innerSelector = rightMember;
                    outerSelector = leftMember;
                    foundSelector = true;
                }
                else if (ComparePastConvert(rightMember.Expression, outer))
                {
                    outerSelector = rightMember;
                    innerSelector = leftMember;
                    foundSelector = true;
                }
            }
    
            if (leftBinary != null)
            {
                foundSelector |= ExtractKeySelectors(leftBinary, inner, outer, ref innerSelector, ref outerSelector);
            }
            if (rightBinary != null)
            {
                foundSelector |= ExtractKeySelectors(rightBinary, inner, outer, ref innerSelector, ref outerSelector);
            }
            return foundSelector;
        }

        private static bool ExtractKeySelectors(LambdaExpression expr, Expression inner, Expression outer, out Expression innerSelector, out Expression outerSelector)
        {
            innerSelector = null;
            outerSelector = null;
            var binaryExpr = expr.Body as BinaryExpression;
            if (binaryExpr != null)
            {
                return ExtractKeySelectors(binaryExpr, inner, outer, ref innerSelector, ref outerSelector);
            }
            else
                return false;
        }

        protected override ShapedQueryExpression TranslateSelect(ShapedQueryExpression source, LambdaExpression selector)
        {
            if (selector.Body == selector.Parameters[0])
            {
                return source;
            }

            var cleanSelector = selector;
            var selectorBody = selector.Body;
            var includeExpression = selectorBody as IncludeExpression;

            if (includeExpression != null)
            {
                while (includeExpression != null)
                {
                    var queryExpr = source.QueryExpression as HarmonyQueryExpression;
                    var navExpression = includeExpression.NavigationExpression as MaterializeCollectionNavigationExpression;
                    if (navExpression != null)
                    {
                        var cleanSubQuery = navExpression.Subquery;
                        var replacementVisitor = new IdentifierReplacingExpressionVisitor() { ReplacementSource = source as JoinedShapedQueryExpression, Target = selector.Parameters.First() };
                        var subQueryTable = ((HarmonyQueryExpression)LiftSubquery(queryExpr, cleanSubQuery, replacementVisitor).QueryExpression).ServerQueryExpression as HarmonyTableExpression;

                        subQueryTable.Name = includeExpression.Navigation.PropertyInfo.Name;
                        subQueryTable.IsCollection = includeExpression.Navigation.IsCollection();
                    }
                    else
                    {
                        var joinSource = source as JoinedShapedQueryExpression;
                        if (joinSource != null)
                        {
                            var innerExpr = joinSource.Inner.QueryExpression as HarmonyQueryExpression;
                            var innerTableExpression = queryExpr.RootExpressions[innerExpr.CurrentParameter];
                            innerTableExpression.Name = includeExpression.Navigation.PropertyInfo.Name;
                            innerTableExpression.IsCollection = includeExpression.Navigation.IsCollection();
                        }
                    }

                    includeExpression = includeExpression.EntityExpression as IncludeExpression;
                }
            }
            else if(selectorBody is MemberInitExpression)
            {
                var subqueryVisitor = new SubqueryReplacingExpressionVisitor() { CurrentVisitor = this, ReplacementSource = source, Outer = selector.Parameters.First()};
                source.ShaperExpression = subqueryVisitor.Visit(selector);
            }

            var groupByQuery = source.ShaperExpression is GroupByShaperExpression;
            var queryExpression = (HarmonyQueryExpression)source.QueryExpression;

            if (groupByQuery)
            {
                queryExpression.PushdownIntoSubquery();
            }

            return source;
        }

        internal ShapedQueryExpression LiftSubquery(HarmonyQueryExpression queryExpr, Expression cleanSubQuery, IdentifierReplacingExpressionVisitor replacementVisitor, Expression outerOverride = null)
        {
            var subquery = TranslateSubquery(cleanSubQuery);
            var subQueryExpr = subquery.QueryExpression as HarmonyQueryExpression;
            var subQueryTable = subQueryExpr.ServerQueryExpression as HarmonyTableExpression;
            for (int i = 0; i < subQueryTable.WhereExpressions.Count; i++)
            {
                if(replacementVisitor != null)
                    subQueryTable.WhereExpressions[i] = replacementVisitor.Visit(subQueryTable.WhereExpressions[i]);

                if (ExtractKeySelectors(subQueryTable.WhereExpressions[i] as LambdaExpression, subQueryExpr.ConvertedParameter, outerOverride ?? queryExpr.ConvertedParameter, out var innerSelector, out var outerSelector))
                {
                    if (innerSelector == null || outerSelector == null)
                        throw new NotImplementedException("failed to find key selector expression");
                    //to avoid remapping this later just swap it out for the actual query expression before we add it to the join list
                    if (outerOverride != null)
                    {
                        outerSelector = ReplacingExpressionVisitor.Replace(outerOverride, queryExpr.ConvertedParameter, outerSelector);
                    }
                    queryExpr.AddLeftJoin(subQueryExpr, outerSelector, innerSelector);
                    //this expression was really a join expression, dont treat it like a where
                    subQueryTable.WhereExpressions.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            return subquery;
        }

        public override ShapedQueryExpression TranslateSubquery(Expression expression)
            => CreateSubqueryVisitor().Visit(expression) as ShapedQueryExpression;

        protected override ShapedQueryExpression TranslateSelectMany(
            ShapedQueryExpression source, LambdaExpression collectionSelector, LambdaExpression resultSelector)
        {
            var defaultIfEmpty = new DefaultIfEmptyFindingExpressionVisitor().IsOptional(collectionSelector);
            var collectionSelectorBody = RemapLambdaBody(source, collectionSelector);

            if (Visit(collectionSelectorBody) is ShapedQueryExpression inner)
            {
                var transparentIdentifierType = TransparentIdentifierFactory.Create(
                    resultSelector.Parameters[0].Type,
                    resultSelector.Parameters[1].Type);

                var innerShaperExpression = defaultIfEmpty
                    ? MarkShaperNullable(inner.ShaperExpression)
                    : inner.ShaperExpression;

                ((HarmonyQueryExpression)source.QueryExpression).AddSelectMany(
                    (HarmonyQueryExpression)inner.QueryExpression, transparentIdentifierType, defaultIfEmpty);

                return TranslateResultSelectorForJoin(
                    source,
                    resultSelector,
                    innerShaperExpression,
                    transparentIdentifierType);
            }

            return null;
        }

        private sealed class DefaultIfEmptyFindingExpressionVisitor : ExpressionVisitor
        {
            private bool _defaultIfEmpty;

            public bool IsOptional(LambdaExpression lambdaExpression)
            {
                _defaultIfEmpty = false;

                Visit(lambdaExpression.Body);

                return _defaultIfEmpty;
            }

            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.IsGenericMethod
                    && methodCallExpression.Method.GetGenericMethodDefinition() == QueryableMethods.DefaultIfEmptyWithoutArgument)
                {
                    _defaultIfEmpty = true;
                }

                return base.VisitMethodCall(methodCallExpression);
            }
        }

        protected override ShapedQueryExpression TranslateSelectMany(ShapedQueryExpression source, LambdaExpression selector)
        {
            var innerParameter = Expression.Parameter(selector.ReturnType.TryGetSequenceType(), "i");
            var resultSelector = Expression.Lambda(
                innerParameter, Expression.Parameter(source.Type.TryGetSequenceType()), innerParameter);

            return TranslateSelectMany(source, selector, resultSelector);
        }

        protected override ShapedQueryExpression TranslateSingleOrDefault(
            ShapedQueryExpression source, LambdaExpression predicate, Type returnType, bool returnDefault)
        {
            return TranslateSingleResultOperator(
                source,
                predicate,
                returnType,
                returnDefault
                    ? EnumerableMethods.SingleOrDefaultWithoutPredicate
                    : EnumerableMethods.SingleWithoutPredicate);
        }

        protected override ShapedQueryExpression TranslateSkip(ShapedQueryExpression source, Expression count)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            count = TranslateExpression(count);
            if (count == null)
            {
                return null;
            }

            inMemoryQueryExpression.ServerQueryExpression
                = Expression.Call(
                    EnumerableMethods.Skip.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression,
                    count);

            return source;
        }

        protected override ShapedQueryExpression TranslateSkipWhile(ShapedQueryExpression source, LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression TranslateSum(ShapedQueryExpression source, LambdaExpression selector, Type resultType)
            => TranslateScalarAggregate(source, selector, nameof(Enumerable.Sum));

        protected override ShapedQueryExpression TranslateTake(ShapedQueryExpression source, Expression count)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            count = TranslateExpression(count);
            if (count == null)
            {
                return null;
            }

            inMemoryQueryExpression.ServerQueryExpression
                = Expression.Call(
                    EnumerableMethods.Take.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression,
                    count);

            return source;
        }

        protected override ShapedQueryExpression TranslateTakeWhile(ShapedQueryExpression source, LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression TranslateThenBy(ShapedQueryExpression source, LambdaExpression keySelector, bool ascending)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            keySelector = TranslateLambdaExpression(source, keySelector);
            if (keySelector == null)
            {
                return null;
            }

            inMemoryQueryExpression.ServerQueryExpression
                = Expression.Call(
                    (ascending ? EnumerableMethods.ThenBy : EnumerableMethods.ThenByDescending)
                    .MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type, keySelector.ReturnType),
                    inMemoryQueryExpression.ServerQueryExpression,
                    keySelector);

            return source;
        }

        protected override ShapedQueryExpression TranslateUnion(ShapedQueryExpression source1, ShapedQueryExpression source2)
            => TranslateSetOperation(EnumerableMethods.Union, source1, source2);

        protected override ShapedQueryExpression TranslateWhere(ShapedQueryExpression source, LambdaExpression predicate)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            predicate = TranslateLambdaExpression(source, predicate, preserveType: true);
            if (predicate == null)
            {
                return null;
            }
            inMemoryQueryExpression.RootExpressions[inMemoryQueryExpression.CurrentParameter].WhereExpressions.Add(predicate);
            return source;
        }

        private Expression TranslateExpression(Expression expression, bool preserveType = false)
        {
            var result = _expressionTranslator.Translate(expression);

            if (expression != null
                && result != null
                && preserveType
                && expression.Type != result.Type)
            {
                result = expression.Type == typeof(bool)
                    ? Expression.Equal(result, Expression.Constant(true, result.Type))
                    : (Expression)Expression.Convert(result, expression.Type);
            }

            return result;
        }

        private LambdaExpression TranslateLambdaExpression(
            ShapedQueryExpression shapedQueryExpression,
            LambdaExpression lambdaExpression,
            bool preserveType = false)
        {
            var lambdaBody = TranslateExpression(RemapLambdaBody(shapedQueryExpression, lambdaExpression), preserveType);

            return lambdaBody != null
                ? Expression.Lambda(
                    lambdaBody,
                    ((HarmonyQueryExpression)shapedQueryExpression.QueryExpression).CurrentParameter)
                : null;
        }

        private Expression RemapLambdaBody(ShapedQueryExpression shapedQueryExpression, LambdaExpression lambdaExpression)
        {
            var lambdaBody = ReplacingExpressionVisitor.Replace(
                lambdaExpression.Parameters.Single(), shapedQueryExpression.ShaperExpression, lambdaExpression.Body);

            return ExpandWeakEntities((HarmonyQueryExpression)shapedQueryExpression.QueryExpression, lambdaBody);
        }

        internal Expression ExpandWeakEntities(HarmonyQueryExpression queryExpression, Expression lambdaBody)
            => _weakEntityExpandingExpressionVisitor.Expand(queryExpression, lambdaBody);

        private sealed class WeakEntityExpandingExpressionVisitor : ExpressionVisitor
        {
            private HarmonyQueryExpression _queryExpression;
            private readonly HarmonyExpressionTranslatingExpressionVisitor _expressionTranslator;
            private readonly Dictionary<Expression, HarmonyQueryExpression> _parameterToQueryMapping;
            public WeakEntityExpandingExpressionVisitor(Dictionary<Expression, HarmonyQueryExpression> parameterToQueryMapping, HarmonyExpressionTranslatingExpressionVisitor expressionTranslator)
            {
                _parameterToQueryMapping = parameterToQueryMapping;
                _expressionTranslator = expressionTranslator;
            }

            public Expression Expand(HarmonyQueryExpression queryExpression, Expression lambdaBody)
            {
                _queryExpression = queryExpression;

                return Visit(lambdaBody);
            }

            protected override Expression VisitMember(MemberExpression memberExpression)
            {
                var innerExpression = Visit(memberExpression.Expression);

                return TryExpand(innerExpression, MemberIdentity.Create(memberExpression.Member))
                    ?? memberExpression.Update(innerExpression);
            }

            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.TryGetEFPropertyArguments(out var source, out var navigationName))
                {
                    source = Visit(source);

                    return TryExpand(source, MemberIdentity.Create(navigationName))
                        ?? methodCallExpression.Update(null, new[] { source, methodCallExpression.Arguments[1] });
                }

                return base.VisitMethodCall(methodCallExpression);
            }

            protected override Expression VisitExtension(Expression extensionExpression)
                => extensionExpression is EntityShaperExpression
                    ? extensionExpression
                    : base.VisitExtension(extensionExpression);

            private Expression TryExpand(Expression source, MemberIdentity member)
            {
                source = source.UnwrapTypeConversion(out var convertedType);
                if (!(source is EntityShaperExpression entityShaperExpression))
                {
                    return null;
                }

                var entityType = entityShaperExpression.EntityType;
                if (convertedType != null)
                {
                    entityType = entityType.GetRootType().GetDerivedTypesInclusive()
                        .FirstOrDefault(et => et.ClrType == convertedType);

                    if (entityType == null)
                    {
                        return null;
                    }
                }

                var navigation = member.MemberInfo != null
                    ? entityType.FindNavigation(member.MemberInfo)
                    : entityType.FindNavigation(member.Name);

                if (navigation == null)
                {
                    return null;
                }

                var targetEntityType = navigation.GetTargetType();
                if (targetEntityType == null
                    || (!targetEntityType.HasDefiningNavigation()
                        && !targetEntityType.IsOwned()))
                {
                    return null;
                }

                var foreignKey = navigation.ForeignKey;
                if (navigation.IsCollection())
                {
                    var innerShapedQuery = CreateShapedQueryExpression(targetEntityType, _parameterToQueryMapping);
                    var innerQueryExpression = (HarmonyQueryExpression)innerShapedQuery.QueryExpression;

                    var makeNullable = foreignKey.PrincipalKey.Properties
                        .Concat(foreignKey.Properties)
                        .Select(p => p.ClrType)
                        .Any(t => t.IsNullableType());

                    var outerKey = entityShaperExpression.CreateKeyAccessExpression(
                        navigation.IsDependentToPrincipal()
                            ? foreignKey.Properties
                            : foreignKey.PrincipalKey.Properties,
                        makeNullable);
                    var innerKey = innerShapedQuery.ShaperExpression.CreateKeyAccessExpression(
                        navigation.IsDependentToPrincipal()
                            ? foreignKey.PrincipalKey.Properties
                            : foreignKey.Properties,
                        makeNullable);

                    var outerKeyFirstProperty = outerKey is NewExpression newExpression
                        ? ((UnaryExpression)((NewArrayExpression)newExpression.Arguments[0]).Expressions[0]).Operand
                        : outerKey;

                    var predicate = outerKeyFirstProperty.Type.IsNullableType()
                        ? Expression.AndAlso(
                            Expression.NotEqual(outerKeyFirstProperty, Expression.Constant(null, outerKeyFirstProperty.Type)),
                            Expression.Equal(outerKey, innerKey))
                        : Expression.Equal(outerKey, innerKey);

                    var correlationPredicate = _expressionTranslator.Translate(predicate);
                    innerQueryExpression.ServerQueryExpression = Expression.Call(
                        EnumerableMethods.Where.MakeGenericMethod(innerQueryExpression.CurrentParameter.Type),
                        innerQueryExpression.ServerQueryExpression,
                        Expression.Lambda(correlationPredicate, innerQueryExpression.CurrentParameter));

                    return innerShapedQuery;
                }

                var entityProjectionExpression
                    = (EntityProjectionExpression)(entityShaperExpression.ValueBufferExpression is
                        ProjectionBindingExpression projectionBindingExpression
                        ? _queryExpression.GetMappedProjection(projectionBindingExpression.ProjectionMember)
                        : entityShaperExpression.ValueBufferExpression);

                var innerShaper = entityProjectionExpression.BindNavigation(navigation);
                if (innerShaper == null)
                {
                    var innerShapedQuery = CreateShapedQueryExpression(targetEntityType, _parameterToQueryMapping);
                    var innerQueryExpression = (HarmonyQueryExpression)innerShapedQuery.QueryExpression;

                    var makeNullable = foreignKey.PrincipalKey.Properties
                        .Concat(foreignKey.Properties)
                        .Select(p => p.ClrType)
                        .Any(t => t.IsNullableType());

                    var outerKey = entityShaperExpression.CreateKeyAccessExpression(
                        navigation.IsDependentToPrincipal()
                            ? foreignKey.Properties
                            : foreignKey.PrincipalKey.Properties,
                        makeNullable);
                    var innerKey = innerShapedQuery.ShaperExpression.CreateKeyAccessExpression(
                        navigation.IsDependentToPrincipal()
                            ? foreignKey.PrincipalKey.Properties
                            : foreignKey.Properties,
                        makeNullable);

                    var outerKeySelector = Expression.Lambda(_expressionTranslator.Translate(outerKey), _queryExpression.CurrentParameter);
                    var innerKeySelector = Expression.Lambda(
                        _expressionTranslator.Translate(innerKey), innerQueryExpression.CurrentParameter);
                    (outerKeySelector, innerKeySelector) = AlignKeySelectorTypes(outerKeySelector, innerKeySelector);
                    innerShaper = _queryExpression.AddNavigationToWeakEntityType(
                        entityProjectionExpression, navigation, innerQueryExpression, outerKeySelector, innerKeySelector);
                }

                return innerShaper;
            }
        }

        private ShapedQueryExpression TranslateScalarAggregate(
            ShapedQueryExpression source, LambdaExpression selector, string methodName)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;

            selector = selector == null
                || selector.Body == selector.Parameters[0]
                    ? Expression.Lambda(
                        inMemoryQueryExpression.GetMappedProjection(new ProjectionMember()),
                        inMemoryQueryExpression.CurrentParameter)
                    : TranslateLambdaExpression(source, selector, preserveType: true);

            if (selector == null)
            {
                return null;
            }

            var method = GetMethod();
            method = method.GetGenericArguments().Length == 2
                ? method.MakeGenericMethod(typeof(DataObjectBase), selector.ReturnType)
                : method.MakeGenericMethod(typeof(DataObjectBase));

            inMemoryQueryExpression.ServerQueryExpression
                = Expression.Call(
                    method,
                    inMemoryQueryExpression.ServerQueryExpression,
                    selector);

            source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

            return source;

            MethodInfo GetMethod()
                => methodName switch
                {
                    nameof(Enumerable.Average) => EnumerableMethods.GetAverageWithSelector(selector.ReturnType),
                    nameof(Enumerable.Max) => EnumerableMethods.GetMaxWithSelector(selector.ReturnType),
                    nameof(Enumerable.Min) => EnumerableMethods.GetMinWithSelector(selector.ReturnType),
                    nameof(Enumerable.Sum) => EnumerableMethods.GetSumWithSelector(selector.ReturnType),
                    _ => throw new InvalidOperationException("Invalid Aggregate Operator encountered."),
                };
        }

        private ShapedQueryExpression TranslateSingleResultOperator(
            ShapedQueryExpression source, LambdaExpression predicate, Type returnType, MethodInfo method)
        {
            var inMemoryQueryExpression = (HarmonyQueryExpression)source.QueryExpression;
            inMemoryQueryExpression.RootExpressions[inMemoryQueryExpression.CurrentParameter].IsCollection = false;
            if (predicate != null)
            {
                source = TranslateWhere(source, predicate);
                if (source == null)
                {
                    return null;
                }
            }

            inMemoryQueryExpression.ServerQueryExpression =
                Expression.Call(
                    method.MakeGenericMethod(inMemoryQueryExpression.CurrentParameter.Type),
                    inMemoryQueryExpression.ServerQueryExpression);

            inMemoryQueryExpression.ConvertToEnumerable();

            if (source.ShaperExpression.Type != returnType)
            {
                source.ShaperExpression = Expression.Convert(source.ShaperExpression, returnType);
            }

            return source;
        }

        private ShapedQueryExpression TranslateSetOperation(
            MethodInfo setOperationMethodInfo,
            ShapedQueryExpression source1,
            ShapedQueryExpression source2)
        {
            var inMemoryQueryExpression1 = (HarmonyQueryExpression)source1.QueryExpression;
            var inMemoryQueryExpression2 = (HarmonyQueryExpression)source2.QueryExpression;

            // Apply any pending selectors, ensuring that the shape of both expressions is identical
            // prior to applying the set operation.
            inMemoryQueryExpression1.PushdownIntoSubquery();
            inMemoryQueryExpression2.PushdownIntoSubquery();

            inMemoryQueryExpression1.ServerQueryExpression = Expression.Call(
                setOperationMethodInfo.MakeGenericMethod(typeof(DataObjectBase)),
                inMemoryQueryExpression1.ServerQueryExpression,
                inMemoryQueryExpression2.ServerQueryExpression);

            return source1;
        }

        internal class JoinedShapedQueryExpression : ShapedQueryExpression
        {
            public ShapedQueryExpression Inner;
            public INavigation InnerNavigation;
            public bool LeftJoin;
            public JoinedShapedQueryExpression(Expression queryExpression, Expression shaperExpression, ShapedQueryExpression inner, bool leftJoin, INavigation innerNavigation) : base(queryExpression, shaperExpression)
            {
                Inner = inner;
                LeftJoin = leftJoin;
                InnerNavigation = innerNavigation;
            }
        }

        internal class IdentifierReplacingExpressionVisitor : ExpressionVisitor
        {
            public Expression Target;
            public ShapedQueryExpression ReplacementSource;
            protected override Expression VisitMember(MemberExpression node)
            {
                if ((node.Expression as MemberExpression)?.Member?.Name == "Outer")
                {
                    return Expression.PropertyOrField((ReplacementSource.QueryExpression as HarmonyQueryExpression).ConvertedParameter, node.Member.Name);
                }
                else if (ReplacementSource is JoinedShapedQueryExpression && (node.Expression as MemberExpression)?.Member?.Name == "Inner")
                {
                    return Expression.PropertyOrField((((JoinedShapedQueryExpression)ReplacementSource).Inner.QueryExpression as HarmonyQueryExpression).ConvertedParameter, node.Member.Name);
                }
                return base.VisitMember(node);
            }
        }

        internal class SubqueryReplacingExpressionVisitor : ExpressionVisitor
        {
            public ShapedQueryExpression ReplacementSource;
            public HarmonyQueryableMethodTranslatingExpressionVisitor CurrentVisitor;
            public ParameterExpression Outer;
            public Stack<string> SubQueryTargetNames = new Stack<string>();
            public Stack<Expression> SourceStack = new Stack<Expression>();

            public Expression CurrentParameter
            {
                get
                {
                    return SourceStack.Count > 0 ? SourceStack.Peek() : Outer;
                }
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                var nameStackCount = SubQueryTargetNames.Count;
                var parameterStackCount = SourceStack.Count;

                var result = base.VisitMemberInit(node);

                while (SubQueryTargetNames.Count > nameStackCount)
                    SubQueryTargetNames.Pop();

                while (SourceStack.Count > parameterStackCount)
                    SourceStack.Pop();

                return result;
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                //check if we're putting a subquery into an entity type
                if (typeof(DataObjectBase).IsAssignableFrom(node.Member.DeclaringType))
                {
                    var creatingType = node.Member.DeclaringType;
                    try
                    {
                        SubQueryTargetNames.Push(node.Member.Name);
                        var updatedExpression = CurrentVisitor.LiftSubquery(ReplacementSource.QueryExpression as HarmonyQueryExpression, node.Expression, null, CurrentParameter);
                        if(updatedExpression != null)
                            return node.Update(updatedExpression);
                    }
                    finally
                    {
                        SubQueryTargetNames.Pop();
                    }
                }
                else
                {
                    //this is for things like SelectAllAndExpand from OData
                    //we should be able to shake things into this pattern if they have a similar purpose with a subquery
                    if (node.Member.Name == "Instance")
                    {
                        SourceStack.Push(node.Expression);
                    }
                    else if (node.Member.Name == "Name")
                    {
                        SubQueryTargetNames.Push(((ConstantExpression)node.Expression).Value as string);
                    }
                    else if (node.Member.Name.StartsWith("Next") && node.Expression is MemberInitExpression)
                    {
                    }
                    else if (node.Member.Name == "IsNull")
                    {
                        var propCall = (node.Expression as BinaryExpression)?.Left as MethodCallExpression;
                        if (propCall != null && propCall.Method.Name == "Property")
                        {
                            var propTarget = (propCall.Arguments[1] as ConstantExpression)?.Value as string;
                            if (propTarget == null)
                                throw new NotImplementedException("EF Property call has an invalid 2nd argument, check debug tree");
                            var targetProperty = Expression.Property(propCall.Arguments[0], propTarget);
                            var nullsafeProperty = Expression.Condition(Expression.Equal(Expression.Constant(null), propCall.Arguments[0]), Expression.Default(targetProperty.Type), targetProperty, targetProperty.Type);
                            return node.Update(Expression.Equal(nullsafeProperty, Expression.Default(targetProperty.Type)));
                        }
                    }
                    else if (node.Member.Name == "Value")
                    {
                        var selectCall = node.Expression as MethodCallExpression;
                        if (selectCall != null && selectCall.Method.Name == "Select" && selectCall.Method.DeclaringType == typeof(Queryable))
                        {
                            var updatedExpression = CurrentVisitor.LiftSubquery(ReplacementSource.QueryExpression as HarmonyQueryExpression, node.Expression, null, CurrentParameter);
                            if (updatedExpression != null)
                            {
                                var tableExpression = ((HarmonyQueryExpression)updatedExpression.QueryExpression).ServerQueryExpression as HarmonyTableExpression;
                                tableExpression.Name = SubQueryTargetNames.Peek();
                                tableExpression.IsCollection = true;
                                var queryableType = typeof(Queryable);
                                var asQueryableMethod = queryableType.GetMethods().FirstOrDefault(mi => mi.Name == nameof(Queryable.AsQueryable) && mi.IsGenericMethod);
                                var emptyMethod = typeof(Enumerable).GetMethod("Empty").MakeGenericMethod(tableExpression.ItemType);
                                var enumerableResult = Expression.PropertyOrField(CurrentParameter, SubQueryTargetNames.Peek());
                                var nullsafeEnumerable = Expression.Condition(Expression.Equal(Expression.Default(enumerableResult.Type), enumerableResult), Expression.Call(null, emptyMethod), enumerableResult, emptyMethod.ReturnType);
                                var asQueryableCall = Expression.Call(null, asQueryableMethod.MakeGenericMethod(tableExpression.ItemType), nullsafeEnumerable);
                                return node.Update(selectCall.Update(null, new Expression[] { asQueryableCall, updatedExpression.ShaperExpression }));
                            }
                        }
                    }
                }
                return base.VisitMemberAssignment(node);
            }
        

            private static readonly MethodInfo _getParameterValueMethodInfo
                = typeof(HarmonyExpressionTranslatingExpressionVisitor)
                    .GetTypeInfo().GetDeclaredMethod(nameof(GetParameterValue));
            //protected override Expression VisitMethodCall(MethodCallExpression node)
            //{
            //    if (node.Method.Name == "Select" && node.Method.DeclaringType == typeof(Queryable))
            //    {
            //        var subQueryExpr = CurrentVisitor.LiftSubquery(ReplacementSource.QueryExpression as HarmonyQueryExpression, node, null, CurrentParameter);
            //        //return subQueryExpr;
            //    }
            //    return base.VisitMethodCall(node);
            //}

            private static T GetParameterValue<T>(QueryContext queryContext, string parameterName) => (T)queryContext.ParameterValues[parameterName];

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node.Name?.StartsWith("__") ?? false)
                {
                    return Expression.Call(
                    _getParameterValueMethodInfo.MakeGenericMethod(node.Type),
                    QueryCompilationContext.QueryContextParameter,
                    Expression.Constant(node.Name));
                }
                return base.VisitParameter(node);
            }
        }
    }
}
