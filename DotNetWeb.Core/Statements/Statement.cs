using DotNetWeb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Statements
{
    public abstract class Statement : Node, ISemanticValidation
    {
        public abstract void Interpret();

        public abstract void ValidateSemantic();

        public abstract string Generate(int tabs);

        public virtual string GetCodeInit(int tabs)
        {
            var code = string.Empty;
            for (int i = 0; i < tabs; i++)
            {
                code += "\t";
            }
            return code;
        }
    }
}
