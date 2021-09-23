using DotNetWeb.Core.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core
{
    public class Symbol
    {
        public Symbol(SymbolType symbolType, Id id, dynamic value)
        {
            SymbolType = symbolType;
            Id = id;
            Value = value;
        }

        public Symbol(SymbolType symbolType, Id id, Expression attributes)
        {
            Attributes = attributes;
            SymbolType = symbolType;
            Id = id;
        }

        public SymbolType SymbolType { get; }
        public Id Id { get; }
        public dynamic Value { get; set; }
        public Expression Attributes { get; }
    }
}
