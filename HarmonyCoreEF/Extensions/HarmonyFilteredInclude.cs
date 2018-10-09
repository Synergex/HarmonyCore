using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Harmony.Core.EF.Extensions
{
    public class HarmonyFilteredInclude
    {
        private class QueryBlock
        {
            public string TargetField;
            public string WhereExpression;
            public object[] Parameters;
            public string OrderByField;
            public bool OrderByAsc;
        }

        private class SelectorBlock
        {
            public Expression Item;
            public Expression QueryableExpression;
            public string ParentField;
            public Dictionary<string, SelectorBlock> Selectors;
        }

        //this is a lookup of the filter arguments and filter query
        private List<QueryBlock> _queryBlocks = new List<QueryBlock>();
        public Expression<ResultExpr> MakeJoinSelectorExpression<ResultExpr, T>(DbContext context, Expression queryableExpression, ParameterExpression[] parameters) 
            where T : class 
            where ResultExpr : Delegate
        {
            var rootSelectors = new Dictionary<string, SelectorBlock>();
            //order by the depth of the join to make things sane
            var orderedBlocks = _queryBlocks.OrderBy(block => block.TargetField.Count(ch => ch == '.'));
            foreach (var block in orderedBlocks)
            {
                // block.TargetField
            }
            Expression selectorExpr = null;
            //public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);
            var selectCall = Expression.Call(queryableExpression, typeof(Queryable).GetMethod("Select"), selectorExpr);
            return Expression.Lambda<ResultExpr>(selectCall, parameters);
        }

        private SelectorBlock MakeSelectorBlock(IModel targetModel, QueryBlock queryBlock, Expression parentItem, string parentField)
        {
            var targetField = Expression.Property(parentItem, parentField);
            //get the element type if this is IEnumerable
            var targetFieldElementType = targetField.Type;
            var targetSelectorParameter = Expression.Parameter(targetFieldElementType, "item");
            return new SelectorBlock { Item = targetSelectorParameter, ParentField = parentField, QueryableExpression = MakeQueryableExpression(targetModel, parentItem, queryBlock), Selectors = new Dictionary<string, SelectorBlock>() };
        }
        //get the main queryable
        //for each level we have to produce a single selector expression
        //if the field is singular join it
        //if the field is a collection we need to apply a selector to the entity collection with the join condition and any additional conditions

        private Expression MakeQueryableExpression(IModel targetModel, Expression parentItem, QueryBlock block)
        {

            return null;
        }
    }
}
