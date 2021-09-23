using DotNetWeb.Core.Expresiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetWeb.Core
{
    public static class EnvironmentManager
    {
        private static List<Environment> _contexts = new List<Environment>();
        private static List<Environment> _interpretContexts = new List<Environment>();
        private static int _currentIndex = -1;

        public static Environment PushContext()
        {
            var env = new Environment();
            _contexts.Add(env);
            _interpretContexts.Add(env);
            return env;
        }

        public static Environment PopContext()
        {
            var lastContext = _contexts.Last();
            _contexts.Remove(lastContext);
            return lastContext;
        }


        public static Symbol GetSymbol(string lexeme)
        {
            for (int i = _contexts.Count - 1; i >= 0; i--)
            {
                var context = _contexts[i];
                var symbol = context.Get(lexeme);
                if (symbol != null)
                {
                    return symbol;
                }
            }

            throw new ApplicationException($"Symbol {lexeme} doesn't exist in current context");
        }

        public static Symbol GetSymbolForEvaluation(string lexeme)
        {
            foreach (var context in _interpretContexts)
            {
                var symbol = context.Get(lexeme);
                if (symbol != null)
                {
                    return symbol;
                }
            }

            throw new ApplicationException($"Symbol {lexeme} doesn't exist in current context");
        }

        public static void AddVariable(string lexeme, Id id) => _contexts.Last().AddVariable(lexeme, id);

        public static void UpdateVariable(string lexeme, dynamic value)
        {
            for (int i = _contexts.Count - 1; i >= 0; i--)
            {
                var context = _contexts[i];
                var symbol = context.Get(lexeme);
                if (symbol != null)
                {
                    context.UpdateVariable(lexeme, value);
                }
            }
        }
    }

}
