using Harmony.Core.EF.Extensions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Harmony.Core.EF.Query.Internal
{
    internal class HarmonyNavigationExpandingExpressionVisitor : NavigationExpandingExpressionVisitor
    {
        internal static readonly MethodInfo LeftJoinMethodInfo = typeof(QueryableExtensions).GetTypeInfo().GetDeclaredMethods("LeftJoin").Single((MethodInfo mi) => mi.GetParameters().Length == 5);
        public HarmonyNavigationExpandingExpressionVisitor(
            QueryTranslationPreprocessor queryTranslationPreprocessor,
            QueryCompilationContext queryCompilationContext,
            IEvaluatableExpressionFilter evaluatableExpressionFilter,
            INavigationExpansionExtensibilityHelper helper) : base(queryTranslationPreprocessor, queryCompilationContext, evaluatableExpressionFilter, helper)
        {

        }

        public override Expression Expand(Expression query)
        {
            //return query;//
            var expandedResult = base.Expand(query);
            return expandedResult;
        }

        //protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        //{
        //    var method = methodCallExpression.Method;
        //    if (method.DeclaringType == typeof(Queryable)
        //        || method.DeclaringType == typeof(QueryableExtensions)
        //        || method.DeclaringType == typeof(EntityFrameworkQueryableExtensions))
        //    {
        //        var genericMethod = method.IsGenericMethod ? method.GetGenericMethodDefinition() : null;
        //        var firstArgument = Visit(methodCallExpression.Arguments[0]);
        //        if (firstArgument is NavigationExpansionExpression source)
        //        {
        //            if (source.PendingOrderings.Any()
        //                && genericMethod != QueryableMethods.ThenBy
        //                && genericMethod != QueryableMethods.ThenByDescending)
        //            {
        //                //TODO
        //                //ApplyPendingOrderings(source);
        //            }

        //            switch (method.Name)
        //            {
        //                case nameof(QueryableExtensions.LeftJoin)
        //                    when genericMethod == LeftJoinMethodInfo:
        //                    {
        //                        var secondArgument = Visit(methodCallExpression.Arguments[1]);
        //                        if (secondArgument is NavigationExpansionExpression innerSource)
        //                        {
        //                            //return ProcessLeftJoin(
        //                            //    source,
        //                            //    innerSource,
        //                            //    methodCallExpression.Arguments[2].UnwrapLambdaFromQuote(),
        //                            //    methodCallExpression.Arguments[3].UnwrapLambdaFromQuote(),
        //                            //    methodCallExpression.Arguments[4].UnwrapLambdaFromQuote());
        //                        }

        //                        break;
        //                    }
        //            }
        //        }
        //    }
        //    return base.VisitMethodCall(methodCallExpression);
        //}

        //private Expression ProcessLeftJoin(
        //    NavigationExpansionExpression outerSource,
        //    NavigationExpansionExpression innerSource,
        //    LambdaExpression outerKeySelector,
        //    LambdaExpression innerKeySelector,
        //    LambdaExpression resultSelector)
        //{
        //    if (innerSource.PendingOrderings.Any())
        //    {
        //        ApplyPendingOrderings(innerSource);
        //    }

        //    var outerKey = ExpandNavigationsInLambdaExpression(outerSource, outerKeySelector);
        //    var innerKey = ExpandNavigationsInLambdaExpression(innerSource, innerKeySelector);

        //    outerKeySelector = GenerateLambda(outerKey, outerSource.CurrentParameter);
        //    innerKeySelector = GenerateLambda(innerKey, innerSource.CurrentParameter);

        //    var transparentIdentifierType = TransparentIdentifierFactory.Create(
        //        outerSource.SourceElementType, innerSource.SourceElementType);

        //    var transparentIdentifierOuterMemberInfo = transparentIdentifierType.GetTypeInfo().GetDeclaredField("Outer");
        //    var transparentIdentifierInnerMemberInfo = transparentIdentifierType.GetTypeInfo().GetDeclaredField("Inner");

        //    var newResultSelector = Expression.Lambda(
        //        Expression.New(
        //            transparentIdentifierType.GetConstructors().Single(),
        //            new[] { outerSource.CurrentParameter, innerSource.CurrentParameter }, transparentIdentifierOuterMemberInfo,
        //            transparentIdentifierInnerMemberInfo),
        //        outerSource.CurrentParameter,
        //        innerSource.CurrentParameter);

        //    var source = Expression.Call(
        //        LeftJoinMethodInfo.MakeGenericMethod(
        //            outerSource.SourceElementType, innerSource.SourceElementType, outerKeySelector.ReturnType,
        //            newResultSelector.ReturnType),
        //        outerSource.Source,
        //        innerSource.Source,
        //        Expression.Quote(outerKeySelector),
        //        Expression.Quote(innerKeySelector),
        //        Expression.Quote(newResultSelector));

        //    var innerPendingSelector = innerSource.PendingSelector;
        //    innerPendingSelector = _entityReferenceOptionalMarkingExpressionVisitor.Visit(innerPendingSelector);

        //    var currentTree = new NavigationTreeNode(outerSource.CurrentTree, innerSource.CurrentTree);
        //    var pendingSelector = new ReplacingExpressionVisitor(
        //        new Dictionary<Expression, Expression>
        //        {
        //            { resultSelector.Parameters[0], outerSource.PendingSelector },
        //            { resultSelector.Parameters[1], innerPendingSelector }
        //        }).Visit(resultSelector.Body);
        //    var parameterName = GetParameterName("ti");

        //    return new NavigationExpansionExpression(source, currentTree, pendingSelector, parameterName);
        //}
    }
}
