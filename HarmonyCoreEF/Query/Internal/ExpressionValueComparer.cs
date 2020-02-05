using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Harmony.Core.EF.Query.Internal
{
    internal class ExpressionValueComparer : IEqualityComparer<Expression>
    {
        private void StringifyExpression(Expression expr, StringBuilder sb)
        {
            if (expr is ParameterExpression parm)
            {
                //this type of parameter will be referentially unique and needs to be unambiguous between different multiple instances of the same table
                if (parm.Name == "valueBuffer" && parm.Type == typeof(DataObjectBase))
                    sb.AppendFormat("{0}:{1}:{2}", parm.Name, parm.Type, parm.GetHashCode());
                else
                    sb.AppendFormat("({0}:{1})", parm.Name, parm.Type);

            }
            else if (expr is UnaryExpression unary)
            {
                StringifyExpression(unary.Operand, sb);
            }
            else if (expr is MemberExpression member)
            {
                StringifyExpression(member.Expression, sb);
                sb.AppendFormat("({0})", member.Member.Name);
            }
            else
                throw new NotImplementedException();
        }
        public bool Equals(Expression x, Expression y)
        {
            var sb = new StringBuilder();
            StringifyExpression(x, sb);
            var xString = sb.ToString();
            sb.Clear();
            StringifyExpression(y, sb);
            var yString = sb.ToString();

            return yString == xString;
        }

        public int GetHashCode(Expression obj)
        {
            var sb = new StringBuilder();
            StringifyExpression(obj, sb);
            return sb.ToString().GetHashCode();
        }
    }
}
