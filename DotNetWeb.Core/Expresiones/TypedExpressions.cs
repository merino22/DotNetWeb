using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Expresiones
{
    public abstract class TypedExpression : Expression
    {
        public TypedExpression(Token token, Type type)
            : base(token, type)
        {
        }
        public abstract dynamic Evaluate();
        public abstract Type GetExpressionType();
    }
}
