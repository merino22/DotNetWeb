using DotNetWeb.Core.Expresiones;
using DotNetWeb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Environment = System.Environment;

namespace DotNetWeb.Core.Statements
{
    public class AssignationStatement : Statement
    {
        public AssignationStatement(Id id, TypedExpression expression)
        {
            Id = id;
            Expression = expression;
        }

        public Id Id { get; }
        public TypedExpression Expression { get; }

        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"<h1>{Id.Generate()} = {Expression.Generate()}</h1>{'\n'}";
            return code;
        }

        public override void Interpret()
        {
            EnvironmentManager.UpdateVariable(Id.Token.Lexeme, Expression.Evaluate());
        }

        public override void ValidateSemantic()
        {
            if (Id.GetExpressionType() != Expression.GetExpressionType())
            {
                throw new ApplicationException($"Type {Id.GetExpressionType()} is not assignable to {Expression.GetExpressionType()}");
            }
        }
        //statements
    }
}
