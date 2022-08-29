// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Harmony.Core.EF.Query.Internal
{
    public class HarmonyGroupByShaperExpression : GroupByShaperExpression
    {
        public HarmonyGroupByShaperExpression(
            Expression keySelector,
            ShapedQueryExpression groupingEnumerable,
            ParameterExpression groupingParameter,
            ParameterExpression valueBufferParameter)
            : base(keySelector, groupingEnumerable)
        {
            GroupingParameter = groupingParameter;
            ValueBufferParameter = valueBufferParameter;
        }

        public virtual ParameterExpression GroupingParameter { get; }
        public virtual ParameterExpression ValueBufferParameter { get; }
    }
}
