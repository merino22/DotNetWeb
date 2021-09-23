using DotNetWeb.Core.Expresiones;
using DotNetWeb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Statements
{
    public class ForEachStatement : Statement, ISemanticValidation
    {
        public ForEachStatement(TypedExpression expression, Statement statement)
        {
            Expression = expression;
            Statement = statement;
        }

        public TypedExpression Expression { get; }
        public Statement Statement { get; }
        public override void Interpret()
        {
            if (Expression.Evaluate())
            {
                Statement.Interpret();
            }
        }

        public override void ValidateSemantic()
        {

        }

        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"foreach({Expression?.Evaluate()}){'\n'}{{{Statement.Generate(tabs)}}}";
            return code;
        }
    }
}
