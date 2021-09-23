using System;
using System.Collections.Generic;
using System.Text;
using DotNetWeb.Core;
using DotNetWeb.Core.Interfaces;

namespace DotNetWeb.Core.Expresiones
{
    public class Id : TypedExpression
    {
        public Id(Token token, Type type) : base(token, type)
        {
        }

        public override dynamic Evaluate()
        {
            return EnvironmentManager.GetSymbolForEvaluation(Token.Lexeme).Value;
        }

        public override Type GetExpressionType()
        {
            return Type;
        }

        public override string Generate()
        {
            return Token.Lexeme;
        }
    }
}
