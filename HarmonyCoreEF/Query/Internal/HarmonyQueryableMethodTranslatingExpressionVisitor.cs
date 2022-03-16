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
using Harmony.Core.FileIO.Queryable.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Harmony.Core.EF.Query.Internal
{
    public class HarmonyQueryableMethodTranslatingExpressionVisitor : QueryableMethodTranslatingExpressionVisitor
    {
        private static readonly MethodInfo _efPropertyMethod = typeof(Microsoft.EntityFrameworkCore.EF).GetTypeInfo().GetDeclaredMethod(nameof(Microsoft.EntityFrameworkCore.EF.Property));

        private readonly HarmonyExpressionTranslatingExpressionVisitor _expressionTranslator;
        private readonly WeakEntityExpandingExpressionVisitor _weakEntityExpandingExpressionVisitor;
        private readonly HarmonyProjectionBindingExpressionVisitor _projectionBindingExpressionVisitor;
        private readonly QueryCompilationContext _context;
        internal readonly Dictionary<Expression, HarmonyQueryExpression> _parameterToQueryMapping;
        private readonly bool _subquery;

        public QueryCompilationContext Context => _context;

        public HarmonyQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            QueryCompilationContext context)
            : base(dependencies, context, subquery: false)
        {
            _subquery = false;
            _parameterToQueryMapping = new Dictionary<Expression, HarmonyQueryExpression>();
            _expressionTranslator = new HarmonyExpressionTranslatingExpressionVisitor(this);
            _weakEntityExpandingExpressionVisitor = new WeakEntityExpandingExpressionVisitor(_parameterToQueryMapping, _expressionTranslator);
            _projectionBindingExpressionVisitor = new HarmonyProjectionBindingExpressionVisitor(this, _expressionTranslator);
            _context = context;
        }

        protected HarmonyQueryableMethodTranslatingExpressionVisitor(
            HarmonyQueryableMethodTranslatingExpressionVisitor parentVisitor)
            : base(parentVisitor.Dependencies, parentVisitor._context, subquery: true)
        {
            _subquery = true;
            _parameterToQueryMapping = new Dictionary<Expression, HarmonyQueryExpression>();
            _expressionTranslator = parentVisitor._expressionTranslator;
            _weakEntityExpandingExpressionVisitor = parentVisitor._weakEntityExpandingExpressionVisitor;
            _projectionBindingExpressionVisitor = new HarmonyProjectionBindingExpressionVisitor(this, _expressionTranslator);
            _context = parentVisitor._context;
        }

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
            => new HarmonyQueryableMethodTranslatingExpressionVisitor(this);

        protected override ShapedQueryExpression CreateShapedQueryExpression(Type elementType)
        {
            return CreateShapedQueryExpression(_context.Model.FindEntityType(elementType), _parameterToQueryMapping);
        }
        protected override ShapedQueryExpression CreateShapedQueryExpression(IEntityType elementType)
        {
            return CreateShapedQueryExpression(elementType, _parameterToQueryMapping);
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

            return source.UpdateShaperExpression(inMemoryQueryExpression.GetSingleScalarProjection());
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

            return source.UpdateShaperExpression(inMemoryQueryExpression.GetSingleScalarProjection());
        }

        protected override ShapedQueryExpression TranslateAverage(ShapedQueryExpression source, LambdaExpression selector, Type resultType)
            => TranslateScalarAggregate(source, selector, nameof(Enumerable.Average));

        protected override ShapedQueryExpression TranslateCast(ShapedQueryExpression source, Type resultType)
        {
            if (source.ShaperExpression.Type == resultType)
            {
                return source;
            }

            return source.UpdateShaperExpression(Expression.Convert(source.ShaperExpression, resultType));
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

            return source.UpdateShaperExpression(inMemoryQueryExpression.GetSingleScalarProjection());
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

            return source.UpdateShaperExpression(inMemoryQueryExpression.GetSingleScalarProjection());
        }

        protected override ShapedQueryExpression TranslateDefaultIfEmpty(ShapedQueryExpression source, Expression defaultValue)
        {
            if (defaultValue == null)
            {
                ((HarmonyQueryExpression)source.QueryExpression).ApplyDefaultIfEmpty();
                return source.UpdateShaperExpression(MarkShaperNullable(source.ShaperExpression));
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
                source = source.UpdateShaperExpression(inMemoryQueryExpression.ApplyGrouping(translatedKey, source));

                if (resultSelector == null)
                {
                    return source;
                }

                var original1 = resultSelector.Parameters[0];
                var original2 = resultSelector.Parameters[1];

                var newResultSelectorBody = new ReplacingExpressionVisitor(
                    new List<Expression> { original1, original2 }, 
                    new List<Expression> { ((GroupByShaperExpression)source.ShaperExpression).KeySelector, source.ShaperExpression })
                    .Visit(resultSelector.Body);

                newResultSelectorBody = ExpandWeakEntities(inMemoryQueryExpression, newResultSelectorBody);

                source = source.UpdateShaperExpression(_projectionBindingExpressionVisitor.Translate(inMemoryQueryExpression, newResultSelectorBody));

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
                var outerTable = outerQueryExpression.FindServerExpression();
                var innerQuery = inner.QueryExpression as HarmonyQueryExpression;
                var innerTable = innerQuery.FindServerExpression();
                innerTable.Name = string.IsNullOrWhiteSpace(outerTable.Name) ? navValue.Name : outerTable.Name + "." + navValue.Name;
                //Add outerQuery.ConvertedParameter + innerNav to map to innerTable
                var innerExpr = Expression.PropertyOrField(outerQueryExpression.ConvertedParameter, navValue.Name);
                //this should fit into our table as an alias so we can make a mapping later on when processing the On objects
                innerTable.Aliases.Add(innerExpr);
            }
            //make custom shaped Query expression to keep track of the added left join
            return new JoinedShapedQueryExpression(outer.QueryExpression, outer.ShaperExpression, inner, false, innerNav);
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
                var outerTable = outerQueryExpression.FindServerExpression();
                var innerQuery = inner.QueryExpression as HarmonyQueryExpression;
                var innerTable = innerQuery.FindServerExpression();
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
                var funcType = typeof(Func<IEnumerable<DataObjectBase>, long>);//.MakeGenericType(inMemoryQueryExpression.ServerQueryExpression.Type, typeof(long));
                var seqParameter = Expression.Parameter(typeof(IEnumerable<DataObjectBase>));//inMemoryQueryExpression.ServerQueryExpression.Type);
                source = source.UpdateShaperExpression(
                    Expression.Lambda(funcType, Expression.Call(
                        EnumerableMethods.LongCountWithoutPredicate.MakeGenericMethod(typeof(DataObjectBase)), seqParameter), seqParameter));
                    
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

            //source.ShaperExpression = inMemoryQueryExpression.GetSingleScalarProjection();

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
                    return source.UpdateShaperExpression(entityShaperExpression.WithEntityType(baseType));
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

                    return source.UpdateShaperExpression(entityShaperExpression.WithEntityType(derivedType));
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

            inMemoryQueryExpression.FindServerExpression().OrderByExpressions.Add(Tuple.Create<Expression, bool>(keySelector.Body, ascending));
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
            return ExtractKeySelectors(inner, outer, ref innerSelector, ref outerSelector, ref foundSelector, expr.Left, expr.Right);
        }

        private static bool ExtractKeySelectors(Expression expr, Expression inner, Expression outer, ref Expression innerSelector, ref Expression outerSelector)
        {
            var foundSelector = false;
            if (expr is BinaryExpression binaryExpression)
                return ExtractKeySelectors(binaryExpression, inner, outer, ref innerSelector, ref outerSelector);
            else if (expr is MethodCallExpression mCall && mCall.Method.Name == "Equals")
                return ExtractKeySelectors(inner, outer, ref innerSelector, ref outerSelector, ref foundSelector, mCall.Arguments[0], mCall.Arguments[1]);
            else
                return false;
        }

        private static bool ExtractKeySelectors(Expression inner, Expression outer, ref Expression innerSelector, ref Expression outerSelector, ref bool foundSelector, Expression leftExpression, Expression rightExpression)
        {
            var leftMember = leftExpression as MemberExpression;
            var rightMember = rightExpression as MemberExpression;

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

            if (leftMember == null && leftExpression != null)
            {
                foundSelector |= ExtractKeySelectors(leftExpression, inner, outer, ref innerSelector, ref outerSelector);
            }
            if (rightMember == null && rightExpression != null)
            {
                foundSelector |= ExtractKeySelectors(leftExpression, inner, outer, ref innerSelector, ref outerSelector);
            }

            return foundSelector;
        }

        private static bool ExtractKeySelectors(LambdaExpression expr, Expression inner, Expression outer, out Expression innerSelector, out Expression outerSelector)
        {
            innerSelector = null;
            outerSelector = null;
            return ExtractKeySelectors(expr.Body, inner, outer, ref innerSelector, ref outerSelector);
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
            var includeParameterReplacements = new Dictionary<Expression, Expression>(new ExpressionValueComparer());
            includeParameterReplacements.Add(selector.Parameters[0], ((HarmonyQueryExpression)source.QueryExpression).ConvertedParameter);
            var replacementVisitor = new IdentifierReplacingExpressionVisitor() { ReplacementExpressions = includeParameterReplacements };
            var referencedFieldDefs = new List<FieldDataDefinition>();
            
            var subqueryVisitor = new SubqueryReplacingExpressionVisitor() { CurrentVisitor = this, ReplacementSource = source, ReplacementVisitor = replacementVisitor, Outer = selector.Parameters.First()};
            
            var newResultSelectorBody = subqueryVisitor.Visit(selector);
            source = source.UpdateShaperExpression(newResultSelectorBody);

            var groupByQuery = source.ShaperExpression is GroupByShaperExpression;
            var queryExpression = (HarmonyQueryExpression)source.QueryExpression;
            var tableExpression = queryExpression.ServerQueryExpression as HarmonyTableExpression;
            if (tableExpression != null)
            {
                foreach (var memberKvp in subqueryVisitor.ReferencedMembers)
                {
                    foreach (var member in memberKvp.Value)
                    {
                        var metadataObject = DataObjectMetadataBase.LookupType(member.DeclaringType);
                        var endOfParentPath = memberKvp.Key.LastIndexOf('.');
                        var targetName = endOfParentPath > -1 ? memberKvp.Key.Remove(endOfParentPath) : "";
                        //dont rely on the reference actually belonging to the current target table
                        //Collection -> single -> Single will result in REL_Single.REL_Single as the member key
                        //this needs to be assigned to the correct parent expression instead
                        var targetRefTable = tableExpression.RootExpression.RootExpressions.Values.FirstOrDefault(table => table.Name == targetName);
                        if (targetRefTable != null)
                        {
                            var refName = endOfParentPath > -1 ? memberKvp.Key.Substring(endOfParentPath + 1) : memberKvp.Key;
                            targetRefTable.ReferencedFields.Add(Tuple.Create(refName, metadataObject.GetFieldByName(member.Name)));
                        }
                    }
                }
                tableExpression.IsCollection = true;
            }
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
            var subQueryTable = subQueryExpr.FindServerExpression();
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
                else
                {
                    //lift this all the way up to the root so it can be processed later by split expression
                    var targetExpr = subQueryTable.WhereExpressions[i];
                    HarmonyQueryExpression.CombineExpressionIntoList(queryExpr.FindServerExpression().WhereExpressions, targetExpr);
                    subQueryTable.WhereExpressions.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            return subquery;
        }

        //protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        //{
        //    MethodCallExpression methodCallExpression2 = methodCallExpression;
        //   // Microsoft.EntityFrameworkCore.Utilities.Check.NotNull(methodCallExpression2, "methodCallExpression");
        //    MethodInfo method = methodCallExpression2.Method;
        //    if (method.DeclaringType == typeof(Queryable) || method.DeclaringType == typeof(QueryableExtensions))
        //    {
        //        Expression expression = Visit(methodCallExpression2.Arguments[0]);
        //        ShapedQueryExpression shapedQueryExpression = expression as ShapedQueryExpression;
        //        if (shapedQueryExpression != null)
        //        {
        //            MethodInfo left = method.IsGenericMethod ? method.GetGenericMethodDefinition() : null;
        //            switch (method.Name)
        //            {
        //                case "All":
        //                    if (left == QueryableMethods.All)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateAll(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "Any":
        //                    if (left == QueryableMethods.AnyWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateAny(shapedQueryExpression, null));
        //                    }

        //                    if (left == QueryableMethods.AnyWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateAny(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "AsQueryable":
        //                    if (left == QueryableMethods.AsQueryable)
        //                    {
        //                        return expression;
        //                    }

        //                    break;
        //                case "Average":
        //                    if (QueryableMethods.IsAverageWithoutSelector(method))
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateAverage(shapedQueryExpression, null, methodCallExpression2.Type));
        //                    }

        //                    if (QueryableMethods.IsAverageWithSelector(method))
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateAverage(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type));
        //                    }

        //                    break;
        //                case "Cast":
        //                    if (left == QueryableMethods.Cast)
        //                    {
        //                        return CheckTranslated(TranslateCast(shapedQueryExpression, method.GetGenericArguments()[0]));
        //                    }

        //                    break;
        //                case "Concat":
        //                    if (left == QueryableMethods.Concat)
        //                    {
        //                        ShapedQueryExpression shapedQueryExpression5 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                        if (shapedQueryExpression5 != null)
        //                        {
        //                            return CheckTranslated(TranslateConcat(shapedQueryExpression, shapedQueryExpression5));
        //                        }
        //                    }

        //                    break;
        //                case "Contains":
        //                    if (left == QueryableMethods.Contains)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateContains(shapedQueryExpression, methodCallExpression2.Arguments[1]));
        //                    }

        //                    break;
        //                case "Count":
        //                    if (left == QueryableMethods.CountWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateCount(shapedQueryExpression, null));
        //                    }

        //                    if (left == QueryableMethods.CountWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateCount(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "DefaultIfEmpty":
        //                    if (left == QueryableMethods.DefaultIfEmptyWithoutArgument)
        //                    {
        //                        return CheckTranslated(TranslateDefaultIfEmpty(shapedQueryExpression, null));
        //                    }

        //                    if (left == QueryableMethods.DefaultIfEmptyWithArgument)
        //                    {
        //                        return CheckTranslated(TranslateDefaultIfEmpty(shapedQueryExpression, methodCallExpression2.Arguments[1]));
        //                    }

        //                    break;
        //                case "Distinct":
        //                    if (left == QueryableMethods.Distinct)
        //                    {
        //                        return CheckTranslated(TranslateDistinct(shapedQueryExpression));
        //                    }

        //                    break;
        //                case "ElementAt":
        //                    if (left == QueryableMethods.ElementAt)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateElementAtOrDefault(shapedQueryExpression, methodCallExpression2.Arguments[1], returnDefault: false));
        //                    }

        //                    break;
        //                case "ElementAtOrDefault":
        //                    if (left == QueryableMethods.ElementAtOrDefault)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateElementAtOrDefault(shapedQueryExpression, methodCallExpression2.Arguments[1], returnDefault: true));
        //                    }

        //                    break;
        //                case "Except":
        //                    if (left == QueryableMethods.Except)
        //                    {
        //                        ShapedQueryExpression shapedQueryExpression6 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                        if (shapedQueryExpression6 != null)
        //                        {
        //                            return CheckTranslated(TranslateExcept(shapedQueryExpression, shapedQueryExpression6));
        //                        }
        //                    }

        //                    break;
        //                case "First":
        //                    if (left == QueryableMethods.FirstWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateFirstOrDefault(shapedQueryExpression, null, methodCallExpression2.Type, returnDefault: false));
        //                    }

        //                    if (left == QueryableMethods.FirstWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateFirstOrDefault(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type, returnDefault: false));
        //                    }

        //                    break;
        //                case "FirstOrDefault":
        //                    if (left == QueryableMethods.FirstOrDefaultWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateFirstOrDefault(shapedQueryExpression, null, methodCallExpression2.Type, returnDefault: true));
        //                    }

        //                    if (left == QueryableMethods.FirstOrDefaultWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateFirstOrDefault(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type, returnDefault: true));
        //                    }

        //                    break;
        //                case "GroupBy":
        //                    if (left == QueryableMethods.GroupByWithKeySelector)
        //                    {
        //                        return CheckTranslated(TranslateGroupBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), null, null));
        //                    }

        //                    if (left == QueryableMethods.GroupByWithKeyElementSelector)
        //                    {
        //                        return CheckTranslated(TranslateGroupBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), GetLambdaExpressionFromArgument(2), null));
        //                    }

        //                    if (left == QueryableMethods.GroupByWithKeyElementResultSelector)
        //                    {
        //                        return CheckTranslated(TranslateGroupBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), GetLambdaExpressionFromArgument(2), GetLambdaExpressionFromArgument(3)));
        //                    }

        //                    if (left == QueryableMethods.GroupByWithKeyResultSelector)
        //                    {
        //                        return CheckTranslated(TranslateGroupBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), null, GetLambdaExpressionFromArgument(2)));
        //                    }

        //                    break;
        //                case "GroupJoin":
        //                    if (left == QueryableMethods.GroupJoin)
        //                    {
        //                        ShapedQueryExpression shapedQueryExpression4 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                        if (shapedQueryExpression4 != null)
        //                        {
        //                            return CheckTranslated(TranslateGroupJoin(shapedQueryExpression, shapedQueryExpression4, GetLambdaExpressionFromArgument(2), GetLambdaExpressionFromArgument(3), GetLambdaExpressionFromArgument(4)));
        //                        }
        //                    }

        //                    break;
        //                case "Intersect":
        //                    if (left == QueryableMethods.Intersect)
        //                    {
        //                        ShapedQueryExpression shapedQueryExpression3 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                        if (shapedQueryExpression3 != null)
        //                        {
        //                            return CheckTranslated(TranslateIntersect(shapedQueryExpression, shapedQueryExpression3));
        //                        }
        //                    }

        //                    break;
        //                case "Join":
        //                    if (left == QueryableMethods.Join)
        //                    {
        //                        ShapedQueryExpression shapedQueryExpression8 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                        if (shapedQueryExpression8 != null)
        //                        {
        //                            return CheckTranslated(TranslateJoin(shapedQueryExpression, shapedQueryExpression8, GetLambdaExpressionFromArgument(2), GetLambdaExpressionFromArgument(3), GetLambdaExpressionFromArgument(4)));
        //                        }
        //                    }

        //                    break;
        //                //case "LeftJoin":
        //                //    if (left == QueryableExtensions.LeftJoinMethodInfo)
        //                //    {
        //                //        ShapedQueryExpression shapedQueryExpression7 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                //        if (shapedQueryExpression7 != null)
        //                //        {
        //                //            return CheckTranslated(TranslateLeftJoin(shapedQueryExpression, shapedQueryExpression7, GetLambdaExpressionFromArgument(2), GetLambdaExpressionFromArgument(3), GetLambdaExpressionFromArgument(4)));
        //                //        }
        //                //    }

        //                //    break;
        //                case "Last":
        //                    if (left == QueryableMethods.LastWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateLastOrDefault(shapedQueryExpression, null, methodCallExpression2.Type, returnDefault: false));
        //                    }

        //                    if (left == QueryableMethods.LastWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateLastOrDefault(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type, returnDefault: false));
        //                    }

        //                    break;
        //                case "LastOrDefault":
        //                    if (left == QueryableMethods.LastOrDefaultWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateLastOrDefault(shapedQueryExpression, null, methodCallExpression2.Type, returnDefault: true));
        //                    }

        //                    if (left == QueryableMethods.LastOrDefaultWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateLastOrDefault(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type, returnDefault: true));
        //                    }

        //                    break;
        //                case "LongCount":
        //                    if (left == QueryableMethods.LongCountWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateLongCount(shapedQueryExpression, null));
        //                    }

        //                    if (left == QueryableMethods.LongCountWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateLongCount(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "Max":
        //                    if (left == QueryableMethods.MaxWithoutSelector)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateMax(shapedQueryExpression, null, methodCallExpression2.Type));
        //                    }

        //                    if (left == QueryableMethods.MaxWithSelector)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateMax(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type));
        //                    }

        //                    break;
        //                case "Min":
        //                    if (left == QueryableMethods.MinWithoutSelector)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateMin(shapedQueryExpression, null, methodCallExpression2.Type));
        //                    }

        //                    if (left == QueryableMethods.MinWithSelector)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateMin(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type));
        //                    }

        //                    break;
        //                case "OfType":
        //                    if (left == QueryableMethods.OfType)
        //                    {
        //                        return CheckTranslated(TranslateOfType(shapedQueryExpression, method.GetGenericArguments()[0]));
        //                    }

        //                    break;
        //                case "OrderBy":
        //                    if (left == QueryableMethods.OrderBy)
        //                    {
        //                        return CheckTranslated(TranslateOrderBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), ascending: true));
        //                    }

        //                    break;
        //                case "OrderByDescending":
        //                    if (left == QueryableMethods.OrderByDescending)
        //                    {
        //                        return CheckTranslated(TranslateOrderBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), ascending: false));
        //                    }

        //                    break;
        //                case "Reverse":
        //                    if (left == QueryableMethods.Reverse)
        //                    {
        //                        return CheckTranslated(TranslateReverse(shapedQueryExpression));
        //                    }

        //                    break;
        //                case "Select":
        //                    if (left == QueryableMethods.Select)
        //                    {
        //                        return CheckTranslated(TranslateSelect(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "SelectMany":
        //                    if (left == QueryableMethods.SelectManyWithoutCollectionSelector)
        //                    {
        //                        return CheckTranslated(TranslateSelectMany(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    if (left == QueryableMethods.SelectManyWithCollectionSelector)
        //                    {
        //                        return CheckTranslated(TranslateSelectMany(shapedQueryExpression, GetLambdaExpressionFromArgument(1), GetLambdaExpressionFromArgument(2)));
        //                    }

        //                    break;
        //                case "Single":
        //                    if (left == QueryableMethods.SingleWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateSingleOrDefault(shapedQueryExpression, null, methodCallExpression2.Type, returnDefault: false));
        //                    }

        //                    if (left == QueryableMethods.SingleWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateSingleOrDefault(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type, returnDefault: false));
        //                    }

        //                    break;
        //                case "SingleOrDefault":
        //                    if (left == QueryableMethods.SingleOrDefaultWithoutPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateSingleOrDefault(shapedQueryExpression, null, methodCallExpression2.Type, returnDefault: true));
        //                    }

        //                    if (left == QueryableMethods.SingleOrDefaultWithPredicate)
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.SingleOrDefault);
        //                        return CheckTranslated(TranslateSingleOrDefault(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type, returnDefault: true));
        //                    }

        //                    break;
        //                case "Skip":
        //                    if (left == QueryableMethods.Skip)
        //                    {
        //                        return CheckTranslated(TranslateSkip(shapedQueryExpression, methodCallExpression2.Arguments[1]));
        //                    }

        //                    break;
        //                case "SkipWhile":
        //                    if (left == QueryableMethods.SkipWhile)
        //                    {
        //                        return CheckTranslated(TranslateSkipWhile(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "Sum":
        //                    if (QueryableMethods.IsSumWithoutSelector(method))
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateSum(shapedQueryExpression, null, methodCallExpression2.Type));
        //                    }

        //                    if (QueryableMethods.IsSumWithSelector(method))
        //                    {
        //                        shapedQueryExpression = shapedQueryExpression.UpdateResultCardinality(ResultCardinality.Single);
        //                        return CheckTranslated(TranslateSum(shapedQueryExpression, GetLambdaExpressionFromArgument(1), methodCallExpression2.Type));
        //                    }

        //                    break;
        //                case "Take":
        //                    if (left == QueryableMethods.Take)
        //                    {
        //                        return CheckTranslated(TranslateTake(shapedQueryExpression, methodCallExpression2.Arguments[1]));
        //                    }

        //                    break;
        //                case "TakeWhile":
        //                    if (left == QueryableMethods.TakeWhile)
        //                    {
        //                        return CheckTranslated(TranslateTakeWhile(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //                case "ThenBy":
        //                    if (left == QueryableMethods.ThenBy)
        //                    {
        //                        return CheckTranslated(TranslateThenBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), ascending: true));
        //                    }

        //                    break;
        //                case "ThenByDescending":
        //                    if (left == QueryableMethods.ThenByDescending)
        //                    {
        //                        return CheckTranslated(TranslateThenBy(shapedQueryExpression, GetLambdaExpressionFromArgument(1), ascending: false));
        //                    }

        //                    break;
        //                case "Union":
        //                    if (left == QueryableMethods.Union)
        //                    {
        //                        ShapedQueryExpression shapedQueryExpression2 = Visit(methodCallExpression2.Arguments[1]) as ShapedQueryExpression;
        //                        if (shapedQueryExpression2 != null)
        //                        {
        //                            return CheckTranslated(TranslateUnion(shapedQueryExpression, shapedQueryExpression2));
        //                        }
        //                    }

        //                    break;
        //                case "Where":
        //                    if (left == QueryableMethods.Where)
        //                    {
        //                        return CheckTranslated(TranslateWhere(shapedQueryExpression, GetLambdaExpressionFromArgument(1)));
        //                    }

        //                    break;
        //            }
        //        }
        //    }

        //    if (!_subquery)
        //    {
        //        throw new InvalidOperationException(CoreStrings.TranslationFailed(methodCallExpression2.Print()));
        //    }

        //    return Microsoft.EntityFrameworkCore.Query.QueryCompilationContext.NotTranslatedExpression;
        //    ShapedQueryExpression CheckTranslated(ShapedQueryExpression? translated)
        //    {
        //        return translated ?? throw new InvalidOperationException((TranslationErrorDetails == null) ? CoreStrings.TranslationFailed(methodCallExpression2.Print()) : CoreStrings.TranslationFailedWithDetails(methodCallExpression2.Print(), TranslationErrorDetails));
        //    }

        //    LambdaExpression GetLambdaExpressionFromArgument(int argumentIndex)
        //    {
        //        return methodCallExpression2.Arguments[argumentIndex].UnwrapLambdaFromQuote();
        //    }
        //}

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

            var serverExpr = inMemoryQueryExpression.FindServerExpression();
            serverExpr.IsCollection = true;
            serverExpr.Skip = count;

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

            var serverExpr = inMemoryQueryExpression.FindServerExpression();
            serverExpr.IsCollection = true;
            serverExpr.Top = count;

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

            inMemoryQueryExpression.FindServerExpression().OrderByExpressions.Add(Tuple.Create<Expression, bool>(keySelector.Body, ascending));
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
            AddWherePredicateToQueryExpression(inMemoryQueryExpression.CurrentParameter, inMemoryQueryExpression, predicate);
            return source;
        }

        private void AddWherePredicateToQueryExpression(ParameterExpression currentParameter, HarmonyQueryExpression queryExpression, LambdaExpression predicate)
        {
            //break predicate into one or more expressions, swapping the specified current parameter for the actual navigation's parameter
            var parameterPairings = SimplifyPredicate(queryExpression, predicate);
            foreach(var paramPair in parameterPairings)
            {
                if (paramPair.Key != currentParameter)
                {
                    //if we're doing a top level filter, the table must have a value in order to eval true
                    queryExpression.RootExpressions[paramPair.Key].IsInnerJoin = true;
                }
                
                queryExpression.RootExpressions[currentParameter].WhereExpressions.Add(paramPair.Value);
            }
            
        }

        private class JoinOnClause : Expression, IHasInnerExpression
        {
            public HarmonyTableExpression TargetTable;
            public ParameterExpression CurrentParameter;

            public override ExpressionType NodeType => InnerExpression.NodeType;
            public override Type Type => InnerExpression.Type;

            public Expression InnerExpression { get; set; }
        }

        class LambdaSimplifier : ExpressionVisitor
        {
            public HarmonyQueryExpression QueryExpression;
            public Dictionary<string, HarmonyTableExpression> _navigationMappings = new Dictionary<string, HarmonyTableExpression>();
            public Dictionary<HarmonyTableExpression, ParameterExpression> _tableMapping = new Dictionary<HarmonyTableExpression, ParameterExpression>();

            public LambdaSimplifier(HarmonyQueryExpression queryExpression)
            {
                QueryExpression = queryExpression;
                foreach (var rootExpr in queryExpression.RootExpressions)
                {
                    if (rootExpr.Value.Name.Contains("."))
                        throw new NotImplementedException();
                    else
                    {
                        _navigationMappings.Add(rootExpr.Value.Name, rootExpr.Value);
                        _tableMapping.Add(rootExpr.Value, rootExpr.Key as ParameterExpression);
                    }
                }
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                //remove nullable checks
                //remove parameter empty checks
                //fold upwards AndAlso binary ops with collapsed left or right sides

                var simplifiedLeft = Visit(node.Left);
                var simplifiedRight = Visit(node.Right);

                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:  
                        if (simplifiedLeft == null)
                        {
                            if (simplifiedRight is ConstantExpression)
                                return null;
                        }
                        if (simplifiedRight == null)
                        {
                            if (simplifiedLeft is ConstantExpression)
                                return null;
                        }

                        if (IsParameter(simplifiedLeft) && simplifiedRight is ConstantExpression)
                            return null;

                        if (IsParameter(simplifiedRight) && simplifiedLeft is ConstantExpression)
                            return null;

                        if (simplifiedLeft is ConstantExpression constLeft && constLeft.Value == null)
                            return null;

                        if (simplifiedRight is ConstantExpression constRight && constRight.Value == null)
                            return null;

                        break;
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        if (simplifiedLeft == null || simplifiedRight == null)
                            return simplifiedLeft ?? simplifiedRight;
                        break;
                    default:
                        if (simplifiedLeft == null || simplifiedRight == null)
                        {
                            throw new NotImplementedException("Unsupported binary expression while simplifying where predicate");
                        }
                        break;
                }


                var leftNav = FindParameterOrNavigation(simplifiedLeft);
                var rightNav = FindParameterOrNavigation(simplifiedRight);

                var updatedNode = node.Update(simplifiedLeft, node.Conversion, simplifiedRight) as Expression;

                if (leftNav.Item1 is HarmonyTableExpression || rightNav.Item1 is HarmonyTableExpression)
                {

                    if (simplifiedLeft is ConstantExpression lce && lce.Value == null)
                        updatedNode = simplifiedRight;
                    if (simplifiedRight is ConstantExpression rce && rce.Value == null)
                        updatedNode = simplifiedLeft;
                    if (updatedNode is ConstantExpression)
                        throw new NotImplementedException();


                    var targetTable = (leftNav.Item1 as HarmonyTableExpression) ?? (rightNav.Item1 as HarmonyTableExpression);
                    var replacements = new Dictionary<Expression, Expression>();
                    if (leftNav.Item1 is HarmonyTableExpression leftTable && leftNav.Item2 != null)
                    {
                        replacements.Add(leftNav.Item2, Expression.Convert(_tableMapping[leftTable], leftTable.ItemType));
                    }
                    
                    if (rightNav.Item1 is HarmonyTableExpression rightTable && rightNav.Item2 != null)
                    {
                        replacements.Add(rightNav.Item2, Expression.Convert(_tableMapping[rightTable], rightTable.ItemType));
                    }
                    
                    if (replacements.Count > 0)
                    {
                        var paramReplacer = new IdentifierReplacingExpressionVisitor
                        {
                            ReplacementExpressions = replacements
                        };
                        updatedNode = paramReplacer.Visit(updatedNode);
                    }

                    if ((node.NodeType == ExpressionType.OrElse || node.NodeType == ExpressionType.AndAlso) &&
                        !(rightNav.Item1 is HarmonyTableExpression && leftNav.Item1 is HarmonyTableExpression))
                    {
                        //if only one side is a JoinOnClause then dont bubble up any further
                        rightTable = rightNav.Item1 as HarmonyTableExpression;
                        leftTable = leftNav.Item1 as HarmonyTableExpression;

                        if (updatedNode is BinaryExpression binaryUpdatedNode)
                        {
                            if (rightTable != null)
                            {
                                var rightReplacement = new JoinOnClause
                                {
                                    InnerExpression = binaryUpdatedNode.Right,
                                    TargetTable = rightTable,
                                    CurrentParameter = _tableMapping[rightTable]
                                };
                                return binaryUpdatedNode.Update(binaryUpdatedNode.Left, binaryUpdatedNode.Conversion, rightReplacement);
                            }
                            else if (leftTable != null)
                            {
                                var leftReplacement = new JoinOnClause
                                {
                                    InnerExpression = binaryUpdatedNode.Left,
                                    TargetTable = leftTable,
                                    CurrentParameter = _tableMapping[leftTable]
                                };
                                return binaryUpdatedNode.Update(leftReplacement, binaryUpdatedNode.Conversion, binaryUpdatedNode.Right);
                            }
                            else
                                throw new NotImplementedException("found table expression but missing both left and right?");
                        }
                        else
                            return updatedNode;
                    }
                    else
                    {
                        if (updatedNode is JoinOnClause)
                            return updatedNode;
                        else
                        {
                            return new JoinOnClause
                            {
                                InnerExpression = updatedNode,
                                TargetTable = targetTable,
                                CurrentParameter = _tableMapping[targetTable]
                            };
                        }
                    }
                }
                else
                {
                    return updatedNode;
                }
            }

            internal (Expression, Expression) FindParameterOrNavigation(Expression expr)
            {
                if (expr is ParameterExpression)
                    return (expr, expr);

                else if (expr is MemberExpression memberExpr)
                {
                    if (_navigationMappings.TryGetValue(memberExpr.Member.Name, out var harmonyTableExpression))
                    {
                        return (harmonyTableExpression, expr);
                    }
                    var nestedFind = FindParameterOrNavigation(memberExpr.Expression);
                    if (nestedFind.Item1 is HarmonyTableExpression || nestedFind.Item1 is ParameterExpression)
                        return nestedFind;
                    else
                        return (expr, null);

                }
                else if(expr is InExpression inexpr)
                {
                    return FindParameterOrNavigation(inexpr.Predicate);
                }
                else if (expr is JoinOnClause joc)
                    return (joc.TargetTable, null);
                else if (expr is MethodCallExpression mce)
                {
                    var nestedFind = FindParameterOrNavigation(mce.Object);
                    if (nestedFind.Item1 is HarmonyTableExpression || nestedFind.Item1 is ParameterExpression)
                        return nestedFind;

                    var argFind = mce.Arguments.Select(expr => FindParameterOrNavigation(expr)).FirstOrDefault(exprTpl => exprTpl.Item1 is HarmonyTableExpression || exprTpl.Item1 is ParameterExpression);
                    if (argFind.Item1 != null || argFind.Item2 != null)
                        return argFind;
                    else
                        return (expr, null);
                }
                else
                    return (expr, null);
            }

            private static bool IsParameter(Expression expression)
            {
                if (expression is ParameterExpression)
                {
                    return true;
                }
                else if (expression is MethodCallExpression methodExpression && methodExpression.Method.Name == "GetParameterValue")
                {
                    return true;
                }

                return false;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(Nullable<>) && node.Value != null)
                {
                    return Expression.Constant(node.Value, node.Type.GetGenericArguments()[0]);
                }
                return base.VisitConstant(node);
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                //remove null checks here return the non null condition side
                var testExpression = node.Test as BinaryExpression; 
                switch (node.Test.NodeType)
                {
                    case ExpressionType.OrElse:
                        {
                            if (testExpression.Right is ConstantExpression rightConst && (rightConst.Value as Nullable<bool>) == false)
                            {
                                if(testExpression.Left is BinaryExpression leftBinary && leftBinary.Right is ConstantExpression downLeftConst && downLeftConst.Value == null)
                                {
                                    return Visit(node.IfFalse);
                                }
                            }
                            break;
                        }
                    case ExpressionType.Equal:
                        {
                            if (testExpression.Left is ConstantExpression leftConst && leftConst.Value == null)
                            {
                                return Visit(node.IfFalse);
                            }
                            if (testExpression.Right is ConstantExpression rightConst && rightConst.Value == null)
                            {
                                return Visit(node.IfFalse);
                            }
                            break;
                        }
                    case ExpressionType.NotEqual:
                        {
                            if (testExpression.Left is ConstantExpression leftConst && leftConst.Value == null)
                            {
                                return Visit(node.IfTrue);
                            }
                            if (testExpression.Right is ConstantExpression rightConst && rightConst.Value == null)
                            {
                                return Visit(node.IfTrue);
                            }
                            break;
                        }
                }
                return base.VisitConditional(node);
            }

            protected override Expression VisitExtension(Expression node)
            {
                if (node is InExpression inExpr)
                {
                    Visit(inExpr.Predicate);
                    return node;
                }

                return base.VisitExtension(node);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Not)
                {
                    var body = Visit(node.Operand);
                    return Expression.Not(body);
                }
                else if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return Visit(node.Operand);
                else if (node.Type == node.Operand.Type)
                    return Visit(node.Operand);
                else
                    return base.VisitUnary(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                return base.VisitMethodCall(node);
            }
        }

        private Dictionary<ParameterExpression, LambdaExpression> SimplifyPredicate(HarmonyQueryExpression queryExpression, LambdaExpression predicate)
        {
            var simplifier = new LambdaSimplifier(queryExpression);
            var visitorResult = simplifier.Visit(predicate) as LambdaExpression;
            var result = new Dictionary<ParameterExpression, LambdaExpression>();
            var expressionBody = ExtractJoins(result, visitorResult.Body, simplifier);

            if(expressionBody != null)
            {
                result.Add(queryExpression.CurrentParameter, Expression.Lambda(expressionBody, queryExpression.CurrentParameter));
            }

            return result;
        }

        private Expression ExtractJoins(Dictionary<ParameterExpression, LambdaExpression> predicates, Expression node, LambdaSimplifier simplifier)
        {
            if (node is BinaryExpression be)
            {
                var ljc = PeekPastJoinClause(be.Left as JoinOnClause);
                var rjc = PeekPastJoinClause(be.Right as JoinOnClause);
                if (be.NodeType == ExpressionType.OrElse && (ljc != null || rjc != null))
                {
                    return be.Update(ljc ?? be.Left, be.Conversion, rjc ?? be.Right);
                }
                else
                {
                    var left = ExtractJoins(predicates, be.Left, simplifier);
                    var right = ExtractJoins(predicates, be.Right, simplifier);
                    if (left == null || right == null)
                        return left ?? right;
                    else
                        return be.Update(left, be.Conversion, right);
                }
            }
            else if (node is UnaryExpression ue)
            {
                var operand = ExtractJoins(predicates, ue.Operand, simplifier);
                if (operand != null)
                    return ue.Update(operand);
                else
                    return null;
            }
            else if (node is MethodCallExpression mce)
            {
                var obj = ExtractJoins(predicates, mce.Object, simplifier);
                bool hasExtracted = obj != null;
                var arguments = mce.Arguments.Select(expr =>
                {
                    var extracted = ExtractJoins(predicates, expr, simplifier);
                    if (extracted != null)
                        hasExtracted = true;
                    return extracted ?? expr;
                }).ToList();
                if (hasExtracted)
                    return mce.Update(obj ?? mce.Object, arguments);
                else
                    return null;
            }
            else if(node is InExpression inexpr)
            {
                var (param, atNode) = simplifier.FindParameterOrNavigation(inexpr);

                if (param is MemberExpression)
                    return node;

                var cleanParam = param as ParameterExpression ?? simplifier._tableMapping[param as HarmonyTableExpression];
                if (cleanParam == null || cleanParam == simplifier.QueryExpression.CurrentParameter)
                {
                    return node;
                }
                else
                {
                    if (predicates.TryGetValue(cleanParam, out var currentPredicate))
                    {
                        predicates[cleanParam] = Expression.Lambda(Expression.AndAlso(currentPredicate.Body, node), cleanParam);
                    }
                    else
                    {
                        predicates.Add(cleanParam, Expression.Lambda(node, cleanParam));
                    }
                    return null;
                }
            }
            else if (node is JoinOnClause joc)
            {
                if (predicates.TryGetValue(joc.CurrentParameter, out var currentPredicate))
                {
                    predicates[joc.CurrentParameter] = Expression.Lambda(Expression.AndAlso(currentPredicate.Body, PeekPastJoinClause(joc)), joc.CurrentParameter);
                }
                else
                {
                    predicates.Add(joc.CurrentParameter, Expression.Lambda(PeekPastJoinClause(joc), joc.CurrentParameter));
                }
                return null;
            }

            else
                return node;
        }

        Expression PeekPastJoinClause(JoinOnClause joc)
        {
            if (joc == null)
                return null;

            if (joc.InnerExpression is JoinOnClause joc2)
                return PeekPastJoinClause(joc2);
            else if(joc.InnerExpression is BinaryExpression be)
            {
                return be.Update(PeekPastJoinClause(be.Left as JoinOnClause) ?? be.Left, be.Conversion, PeekPastJoinClause(be.Right as JoinOnClause) ?? be.Right);
            }
            else
                return joc.InnerExpression;
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
                var entityShaperExpression = source as EntityShaperExpression;
                if (entityShaperExpression == null && convertedType != null)
                    entityShaperExpression = new EntityShaperExpression(_expressionTranslator.Context.Model.FindEntityType(convertedType), source, false);


                if (entityShaperExpression == null)
                    return null;

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

                    var outerKey = entityShaperExpression.CreateKeyValuesExpression(
                        navigation.IsDependentToPrincipal()
                            ? foreignKey.Properties
                            : foreignKey.PrincipalKey.Properties,
                        makeNullable);
                    var innerKey = innerShapedQuery.ShaperExpression.CreateKeyValuesExpression(
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

                    var outerKey = entityShaperExpression.CreateKeyValuesExpression(
                        navigation.IsDependentToPrincipal()
                            ? foreignKey.Properties
                            : foreignKey.PrincipalKey.Properties,
                        makeNullable);
                    var innerKey = innerShapedQuery.ShaperExpression.CreateKeyValuesExpression(
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

            return source.UpdateShaperExpression(inMemoryQueryExpression.GetSingleScalarProjection());

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

            if (!(source.ShaperExpression is LambdaExpression) && source.ShaperExpression.Type != returnType)
            {
                return source.UpdateShaperExpression(Expression.Convert(source.ShaperExpression, returnType));
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
            public Dictionary<Expression, Expression> ReplacementExpressions;
            protected override Expression VisitMember(MemberExpression node)
            {
                if (ReplacementExpressions.TryGetValue(node.Expression, out var mappedExpression))
                { 
                    return node.Update(mappedExpression);
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitExtension(Expression node)
            {
                if(node is InExpression inExpr)
                {
                    return node;
                }
                else if(node is IHasInnerExpression innerExpr)
                {
                    return Visit(innerExpr.InnerExpression);
                }
                return base.VisitExtension(node);
            }
        }

        internal class SubqueryReplacingExpressionVisitor : ExpressionVisitor
        {
            public ShapedQueryExpression ReplacementSource;
            public HarmonyQueryableMethodTranslatingExpressionVisitor CurrentVisitor;
            public ParameterExpression Outer;
            public Stack<string> SubQueryTargetNames = new Stack<string>();
            public Stack<Expression> SourceStack = new Stack<Expression>();
            public Dictionary<string, HashSet<MemberInfo>> ReferencedMembers = new Dictionary<string, HashSet<MemberInfo>>();
            public IdentifierReplacingExpressionVisitor ReplacementVisitor;

            public Expression CurrentParameter
            {
                get
                {
                    return SourceStack.Count > 0 ? SourceStack.Peek() : Outer;
                }
            }

            private Expression ProcessNav(MaterializeCollectionNavigationExpression matColExpr, HarmonyQueryExpression queryExpr)
            {
                var cleanSubQuery = matColExpr.Subquery;
                var subQueryTable = ((HarmonyQueryExpression)CurrentVisitor.LiftSubquery(queryExpr, cleanSubQuery, ReplacementVisitor).QueryExpression).FindServerExpression();

                subQueryTable.Name = matColExpr.Navigation.PropertyInfo.Name;
                subQueryTable.IsCollection = matColExpr.Navigation.IsCollection;
                return Expression.PropertyOrField(CurrentParameter, matColExpr.Navigation.PropertyInfo.Name);
            }

            protected override Expression VisitExtension(Expression node)
            {
                switch (node)
                {
                    case MaterializeCollectionNavigationExpression matColExpr:
                        return ProcessNav(matColExpr, ReplacementSource.QueryExpression as HarmonyQueryExpression);
                    case IncludeExpression includeExpression:
                        while (includeExpression != null)
                        {
                            var queryExpr = ReplacementSource.QueryExpression as HarmonyQueryExpression;
                            var navExpression = includeExpression.NavigationExpression as MaterializeCollectionNavigationExpression;
                            if (navExpression != null)
                            {
                                ProcessNav(navExpression, ReplacementSource.QueryExpression as HarmonyQueryExpression);
                            }
                            else
                            {
                                var joinSource = ReplacementSource as JoinedShapedQueryExpression;
                                if (joinSource != null)
                                {
                                    var innerExpr = joinSource.Inner.QueryExpression as HarmonyQueryExpression;
                                    var innerTableExpression = queryExpr.RootExpressions[innerExpr.CurrentParameter];
                                    innerTableExpression.Name = includeExpression.Navigation.PropertyInfo.Name;
                                    innerTableExpression.IsCollection = includeExpression.Navigation.IsCollection;
                                }
                            }

                            includeExpression = includeExpression.EntityExpression as IncludeExpression;
                        }
                        return CurrentParameter;
                }


                return base.VisitExtension(node);
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

            protected override Expression VisitMember(MemberExpression node)
            {
                var (memberInfo, memberType) = ProcessNodeForDataReferences(node);

                //check if member expression is a dangerous dereference and should be made nullable
                //as a pattern, parameters here should be null safe
                if (!(node.Expression is ParameterExpression) && !node.Type.IsValueType)
                {
                    if (memberType != null && !memberType.IsValueType)
                    {
                        return MakeNullSafeExpression(Visit(node.Expression), memberInfo.Name, false).Item1;
                    }
                }

                return base.VisitMember(node);
            }

            private string PathFromNode(Expression node)
            {
                if (node is MemberExpression me)
                {
                    var parent = PathFromNode(me.Expression);
                    if (string.IsNullOrWhiteSpace(parent))
                        return me.Member.Name;
                    else
                        return parent + "." + me.Member.Name;
                }
                else
                    return "";
            }

            private (MemberInfo, Type) ProcessNodeForDataReferences(MemberExpression node)
            {
                var memberInfo = node?.Member;
                var memberType = (memberInfo as FieldInfo)?.FieldType ?? (memberInfo as PropertyInfo)?.PropertyType;

                var parentPath = PathFromNode(node.Expression);
                if(!ReferencedMembers.TryGetValue(parentPath, out var pathReferencedMembers))
                {
                    pathReferencedMembers = new HashSet<MemberInfo>();
                    ReferencedMembers.Add(parentPath, pathReferencedMembers);
                }
                if (memberInfo != null && typeof(DataObjectBase).IsAssignableFrom(memberInfo.DeclaringType) && !pathReferencedMembers.Contains(memberInfo))
                {
                    if (!typeof(DataObjectBase).IsAssignableFrom(memberType))
                    {
                        pathReferencedMembers.Add(memberInfo);
                    }
                }

                return (memberInfo, memberType);
            }

            private (Expression, MemberExpression) MakeNullSafeExpression(Expression baseExpression, string propOrFieldName, bool nullable)
            {
                var typedTarget = Expression.PropertyOrField(baseExpression, propOrFieldName);
                var target = typedTarget as Expression;
                var nullableType = target.Type.IsValueType ? typeof(Nullable<>).MakeGenericType(target.Type) : null;
                if (baseExpression.Type.IsValueType)
                    return (nullable ? Expression.Convert(target, nullableType) : target, typedTarget);
                else if(!nullable || nullableType == null)
                    return (Expression.Condition(Expression.Equal(Expression.Constant(null), baseExpression), Expression.Default(target.Type), target, target.Type), typedTarget);
                else
                    return (Expression.Condition(Expression.Equal(Expression.Constant(null), baseExpression), Expression.Constant(null, nullableType), Expression.Convert(target, nullableType), nullableType), typedTarget);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                var (memberInfo, memberType) = ProcessNodeForDataReferences(node.Operand as MemberExpression);

                if (node.NodeType == ExpressionType.Convert && node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (node.Operand is MemberExpression memberExpr)
                    {
                        if (memberInfo != null)
                        {
                            return MakeNullSafeExpression(Visit(memberExpr.Expression), memberInfo.Name, true).Item1;
                        }
                    }
                }
                return base.VisitUnary(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression methodCall)
            {
                if (methodCall.Method.Name == "Property")
                {
                    var propTarget = (methodCall.Arguments[1] as ConstantExpression)?.Value as string;
                    if (propTarget == null)
                        throw new NotImplementedException("EF Property call has an invalid 2nd argument, check debug tree");

                    var targetArgs = Visit(methodCall.Arguments[0]);
                    var (result, memberExpression) = MakeNullSafeExpression(targetArgs, propTarget, true);
                    if(memberExpression != null)
                        ProcessNodeForDataReferences(memberExpression);
                    return result;
                }
                else if (methodCall.Method.Name == "Select" && methodCall.Method.DeclaringType == typeof(Queryable))
                {
                    var updatedExpression = CurrentVisitor.LiftSubquery(ReplacementSource.QueryExpression as HarmonyQueryExpression, methodCall, null, CurrentParameter);
                    if (updatedExpression != null)
                    {
                        var tableExpression = ((HarmonyQueryExpression)updatedExpression.QueryExpression).FindServerExpression();
                        tableExpression.Name = SubQueryTargetNames.Peek();
                        tableExpression.IsCollection = true;
                        var rootSource = ReplacementSource.QueryExpression as HarmonyQueryExpression;
                        var queryableType = typeof(Queryable);
                        var asQueryableMethod = queryableType.GetMethods().FirstOrDefault(mi => mi.Name == nameof(Queryable.AsQueryable) && mi.IsGenericMethod);
                        var emptyMethod = typeof(Enumerable).GetMethod("Empty").MakeGenericMethod(tableExpression.ItemType);
                        var enumerableResult = Expression.PropertyOrField(CurrentParameter, SubQueryTargetNames.Peek());
                        ProcessNodeForDataReferences(enumerableResult);
                        var nullsafeEnumerable = Expression.Condition(Expression.Equal(Expression.Default(enumerableResult.Type), enumerableResult), Expression.Call(null, emptyMethod), enumerableResult, emptyMethod.ReturnType);
                        var asQueryableCall = Expression.Call(null, asQueryableMethod.MakeGenericMethod(tableExpression.ItemType), nullsafeEnumerable);
                        return methodCall.Update(null, new Expression[] { asQueryableCall, updatedExpression.ShaperExpression });
                    }
                }
                
                return base.VisitMethodCall(methodCall);
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
                }

                var expr = Visit(node.Expression);
                if (node.Expression.Type.IsValueType && !expr.Type.IsValueType)
                {
                    return node.Update(Expression.Convert(expr, node.Expression.Type));
                }
                else
                    return node.Update(expr);
            }
        

            internal static readonly MethodInfo _getParameterValueMethodInfo
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
