using DotNetWeb.Core.Expresiones;
using DotNetWeb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Statements
{
    public class InterpolationStatement : Statement
    {
        public InterpolationStatement(Symbol symbol, TypedExpression expression)
        {
            Expression = expression;
        }

        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"\t<div>{Expression.Evaluate()}</div>{'\n'}";
            return code;
        }

        public TypedExpression Expression { get; }
        public Symbol Symbol { get; }
        public override void Interpret()
        {
            //interpret
        }

        public override void ValidateSemantic()
        {
        }
    }
}
