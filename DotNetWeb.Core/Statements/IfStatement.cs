
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
            if (Expression.Evaluate())
            {
                code += $"{Statement.Generate(tabs)}{' '}";  
            }
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
            if(Expression.GetExpressionType() != Type.Bool)
            {
                throw new ApplicationException("Invalid if statement.");
            }
        }
    }
}
