
using DotNetWeb.Core.Expresiones;
using System;
using Type = DotNetWeb.Core.Type;

namespace DotNetWeb.Core.Statements
{
    public class IfStatement : Statement
    {
        public IfStatement(TypedExpression expression, Statement statement)
        {
            Expression = expression;
            Statement = statement;
        }

        public TypedExpression Expression { get; }
        public Statement Statement { get; }
        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"if({Expression.Generate()}){{{'\n'}";
            return code;
        }
        public override void Interpret()
        {
            if (Expression.Evaluate())
            {
                Statement.Interpret();
            }
        }

        public override void ValidateSemantic()
        {
            Statement.ValidateSemantic();
        }
    }
}
