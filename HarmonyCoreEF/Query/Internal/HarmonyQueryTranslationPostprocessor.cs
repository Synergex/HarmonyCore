// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;

namespace Harmony.Core.EF.Query.Internal
{
    public class HarmonyQueryTranslationPostprocessor : QueryTranslationPostprocessor
    {
        HarmonyQueryCompilationContext _compilationContext;
        public HarmonyQueryTranslationPostprocessor(HarmonyQueryCompilationContext compilationContext, QueryTranslationPostprocessorDependencies dependencies)
            : base(dependencies)
        {
            _compilationContext = compilationContext;
        }

        public override Expression Process(Expression query)
        {
            query = base.Process(query);
            var visitor = new ValueBufferParameterVisitor(_compilationContext);
            query = visitor.Visit(query);


            return query;
        }

        private class ValueBufferParameterVisitor : ExpressionVisitor
        {
            private HarmonyQueryCompilationContext _compilationContext;
            private Stack<ShapedQueryExpression> _queryStack = new Stack<ShapedQueryExpression>();

            public ValueBufferParameterVisitor(HarmonyQueryCompilationContext compilationContext)
            {
                _compilationContext = compilationContext;
            }

            protected override Expression VisitExtension(Expression node)
            {
                if (node is ShapedQueryExpression shapedNode)
                {
                    _queryStack.Push(shapedNode);
                    if (shapedNode.ShaperExpression is LambdaExpression shaperLambda)
                    {
                        _compilationContext.MapQueryExpression(shapedNode, shaperLambda.Parameters[0]);
                    }
                    else
                    {
                        _compilationContext.MapQueryExpression(shapedNode, shapedNode.ShaperExpression);
                    }

                    Visit(shapedNode.ShaperExpression);
                    _queryStack.Pop();
                    return shapedNode;
                }
                return base.VisitExtension(node);
            }
        }
    }
}
