using DotNetWeb.Core.Expresiones;
using DotNetWeb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Statements
{
    public class ForEachStatement : Statement
    {
        public ForEachStatement(Token tokx, Token toky, Statement statement)
        {
            TokX = tokx;
            TokY = toky;
            Statement = statement;
        }

        public override void Interpret()
        {
            Statement?.Interpret();
        }

        public override void ValidateSemantic()
        {
            Statement?.ValidateSemantic();
        }

        public override string Generate(int tabs)
        {
            var code = Statement.Generate(tabs);
            code += Statement.Generate(tabs + 1);
            return code;
        }

        public Token TokX { get; }
        public Token TokY { get; }
        public Statement Statement { get; }
    }
}
