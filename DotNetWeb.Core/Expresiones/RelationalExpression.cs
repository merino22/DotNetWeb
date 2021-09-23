using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Expresiones
{
    public class RelationalExpression : TypedBinaryOperator
    {
        private readonly Dictionary<(Type, Type), Type> _typeRules;
        public RelationalExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
            : base(token, leftExpression, rightExpression, null)
        {
            _typeRules = new Dictionary<(Type, Type), Type>
            {
                { (Type.Float, Type.Float), Type.Bool },
                { (Type.Int, Type.Int), Type.Bool },
                { (Type.String, Type.String), Type.Bool },
                { (Type.Float, Type.Int), Type.Bool },
                { (Type.Int, Type.Float), Type.Bool }
            };
        }

        public override dynamic Evaluate()
        {
            return Token.TokenType switch
            {
                TokenType.GreaterThan => LeftExpression.Evaluate() > RightExpression.Evaluate(),
                TokenType.LessThan => LeftExpression.Evaluate() < RightExpression.Evaluate(),
                TokenType.Equal => LeftExpression.Evaluate() == RightExpression.Evaluate(),
                TokenType.NotEqual => LeftExpression.Evaluate() != RightExpression.Evaluate(),
                TokenType.InKeyword => null,
                _ => throw new NotImplementedException()
            };
        }

        public override Type GetExpressionType()
        {
            if (_typeRules.TryGetValue((LeftExpression.GetExpressionType(), RightExpression.GetExpressionType()), out var res))
            {
                return res;
            }
            throw new ApplicationException($"No relational operation on {LeftExpression.GetExpressionType()}, {RightExpression.GetExpressionType()} made.");
        }

        public override string Generate()
        {
            if (Token.TokenType == TokenType.Equal)
                return $"{LeftExpression.Generate()} == {RightExpression.Generate()}";

            return $"{LeftExpression.Generate()} {Token.Lexeme} {RightExpression.Generate()}";
        }
        //Relational
    }
}
