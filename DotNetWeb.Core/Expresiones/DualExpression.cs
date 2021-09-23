using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Expresiones
{
    public class DualExpression : Expression
    {
        public DualExpression(Expression expr1, Expression expr2) : base(null, null)
        {
            this.Expr1 = expr1;
            this.Expr2 = expr2;
        }

        public Expression Expr1 { get; }
        public Expression Expr2 { get; }

        public override string Generate()
        {
            return $"{Expr1?.Generate() + Expr2?.Generate()}";
        }
        //dual
    }
}
