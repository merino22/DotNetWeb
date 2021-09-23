using System;
using System.Collections.Generic;
using System.Text;


namespace DotNetWeb.Core.Expresiones
{
    public abstract class Expression : Node
    {
        public Type Type { get; }

        public Token Token { get; }

        public Expression(Token token, Type type)
        {
            Token = token;
            this.Type = type;
        }
        public abstract string Generate();
    }
}
