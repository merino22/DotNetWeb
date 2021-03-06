using DotNetWeb.Core;
using DotNetWeb.Core.Expresiones;
using DotNetWeb.Core.Interfaces;
using DotNetWeb.Core.Statements;
using System;
using System.IO;
using System.Text;
using Type = DotNetWeb.Core.Type;

namespace DotNetWeb.Parser
{
    public class Parser : IParser
    {
        private readonly IScanner scanner;
        private Token lookAhead;
        public Parser(IScanner scanner)
        {
            this.scanner = scanner;
            this.Move();
        }
        public Statement Parse()
        {
            
            EnvironmentManager.PushContext();
            var prog = Program();
            prog.ValidateSemantic();
            prog.Interpret();
            var code = "<!DOCTYPE html>\n";
            code += "<html lang=\"en\">\n";
            code += "<head>\n";
            code += "<meta charset=\"UTF-8\">\n";
            code += "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n";
            code += "<meta name=\"viewport\" content=\"width=device-wdith, initial-scale=1.0\">\n";
            code += "<title>Document</title>\n";
            code += "</head>\n";
            code += "<body>\n";
            code += prog.Generate(1);
            code += "</body>\n";
            code += "</html>\n";
            System.Console.WriteLine(code);
            string filename = @"C:\Users\Aaron\Desktop\htmlTest.html";
            try
            {
                if(File.Exists(filename))
                {
                    File.Delete(filename);
                }

                using(FileStream fs = File.Create(filename))
                {
                    Byte[] codex = new UTF8Encoding(true).GetBytes(code);
                    fs.Write(codex, 0, codex.Length);
                }
            }
            catch
            {
                throw new ApplicationException("Error Creating HTML file");
            }
            return prog;
        }

        private Statement Program()
        {
            return new SequenceStatement(Init(), Template());
        }

        private Statement Template()
        {
            var state1 = Tag();
            var state2 = InnerTemplate();
            var stateFinal = new SequenceStatement(state1, state2);
            return stateFinal;
        }
        
        private Statement InnerTemplate()
        {
            if (this.lookAhead.TokenType == TokenType.LessThan)
            {
                var statement = Template();
                return statement;
            }
            return null;
        }
        private Statement Tag()
        {
            Match(TokenType.LessThan);
            Match(TokenType.Identifier);
            Match(TokenType.GreaterThan);
            var statement = Stmts();
            Match(TokenType.LessThan);
            Match(TokenType.Slash);
            Match(TokenType.Identifier);
            Match(TokenType.GreaterThan);
            return statement;
        }

        private Statement Stmts()
        {
            if (this.lookAhead.TokenType == TokenType.OpenBrace)
            {
                return new SequenceStatement(Stmt(), Stmts());
            }
            return null; 
        }

        private Statement Stmt()
        {
            Expression expression;
            Match(TokenType.OpenBrace);
            switch (this.lookAhead.TokenType)
            {
                case TokenType.OpenBrace:
                    Match(TokenType.OpenBrace);
                    expression = Eq();
                    //var x = EnvironmentManager._contexts.Contains(value);
                    Match(TokenType.CloseBrace);
                    Match(TokenType.CloseBrace);
                    return new InterpolationStatement(null, expression as TypedExpression);
                case TokenType.Percentage:
                    var statement = IfStmt();
                    return statement;
                case TokenType.Hyphen:
                    return ForeachStatement();
                default:
                    throw new ApplicationException("Unrecognized statement");
            }
        }

        private Statement ForeachStatement()
        {
            Match(TokenType.Hyphen);
            Match(TokenType.Percentage);
            Match(TokenType.ForEeachKeyword);
            var TokenX = lookAhead;
            Match(TokenType.Identifier);
            var id = new Id(TokenX, Type.FloatList);
            EnvironmentManager.AddVariable(TokenX.Lexeme, id);
            Match(TokenType.InKeyword);
            var TokenY = lookAhead;
            Match(TokenType.Identifier);
            /*if (EnvironmentManager.GetSymbolForEvaluation(TokenY.Lexeme) == null)
            {
                throw new ApplicationException($"Variable {TokenY.Lexeme} no existe en el contexto actual.");
            }*/
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            var statement = Template();
            Match(TokenType.OpenBrace);
            Match(TokenType.Percentage);
            Match(TokenType.EndForEachKeyword);
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            //foreach
            return new ForEachStatement(TokenX, TokenY, statement);
        }

        public Statement ForEachParse(DualExpression expression)
        {
            return new SequenceStatement(new AssignationStatement(new Id(expression.Expr1.Token, expression.Expr1.Type),expression.Expr1 as TypedExpression),expression.Expr2 is DualExpression dualExpression? ForEachParse(dualExpression): new AssignationStatement(new Id(expression.Expr2.Token, expression.Expr2.Type),expression.Expr2 as TypedExpression));
        }

        private Statement IfStmt()
        {
            Expression expression;
            Statement statement;
            Match(TokenType.Percentage);
            Match(TokenType.IfKeyword);
            expression = Eq();
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            statement = Template();
            Match(TokenType.OpenBrace);
            Match(TokenType.Percentage);
            Match(TokenType.EndIfKeyword);
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);

            return new IfStatement(expression as TypedExpression, statement);
        }

        private Expression Eq()
        {
            var expression = Rel();
            while (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
            {
                var token = lookAhead;
                Move();
                expression = new RelationalExpression(token, expression as TypedExpression, Rel() as TypedExpression);
            }
            return expression;
        }

        private Expression Rel()
        {
            var expression = Expr();
            if (this.lookAhead.TokenType == TokenType.LessThan
                || this.lookAhead.TokenType == TokenType.GreaterThan)
            {
                var token = lookAhead;
                Move();
                expression = new RelationalExpression(token, expression as TypedExpression, Expr() as TypedExpression);
            }

            /*if (this.lookAhead.TokenType == TokenType.InKeyword)
            {
                var token = lookAhead;
                Move();
                Match(TokenType.Identifier);
                expression = new ArgumentExpression(token, expression as TypedExpression, null);
            }*/
            return expression;
        }

        private Expression Expr()
        {
            var expression = Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Hyphen)
            {
                var token = lookAhead;
                Move();
                expression = new ArithmeticOperator(token, expression as TypedExpression, Term() as TypedExpression);
            }
            return expression;
        }

        private Expression Term()
        {
            var expression = Factor();
            while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Slash)
            {
                var token = lookAhead;
                Move();
                expression = new ArithmeticOperator(token, expression as TypedExpression, Factor() as TypedExpression);
            }
            return expression;
        }

        private Expression Factor()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    {
                        Match(TokenType.LeftParens);
                        var expression = Eq();
                        Match(TokenType.RightParens);
                        return expression;
                    }
                case TokenType.IntConstant:
                    var constant = new Constant(lookAhead, Type.Int);
                    Match(TokenType.IntConstant);
                    return constant;
                case TokenType.FloatConstant:
                    constant = new Constant(lookAhead, Type.Float);
                    Match(TokenType.FloatConstant);
                    return constant;
                case TokenType.StringConstant:
                    constant = new Constant(lookAhead, Type.String);
                    Match(TokenType.StringConstant);
                    return constant;
                case TokenType.OpenBracket:
                    Match(TokenType.OpenBracket);
                    var exprlist = ExprList();
                    Match(TokenType.CloseBracket);
                    return exprlist;
                default:
                    var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
                    Match(TokenType.Identifier);
                    return symbol.Id;
            }
        }

        private Expression ExprList()
        {
            var expression = Eq();
            if (this.lookAhead.TokenType != TokenType.Comma)
            {
                return expression;
            }
            Match(TokenType.Comma);
            return new DualExpression(expression, ExprList());
        }


        private Statement Init()
        {
            Match(TokenType.OpenBrace);
            Match(TokenType.Percentage);
            Match(TokenType.InitKeyword);
            var code = Code();
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            return code;
        }

        private Statement Code()
        {
            Decls();
            var assigns = Assignations();
            return assigns;
        }

        private Statement Assignations()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
                return new SequenceStatement(Assignation(symbol.Id), Assignations());
            }
            return null;
        }

        private Statement Assignation(Id id)
        {
            Match(TokenType.Identifier);
            Match(TokenType.Assignation);
            var expression = Eq();
            Match(TokenType.SemiColon);
            if(expression is DualExpression dualExpression)
            {
                return ForEachParse(dualExpression);
            }
            else
            {
                return new AssignationStatement(id, expression as TypedExpression);
            }
        }

        private void Decls()
        {
            Decl();
            InnerDecls();
        }

        private void InnerDecls()
        {
            if (this.LookAheadIsType())
            {
                Decls();
            }
        }

        private void Decl()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.FloatKeyword:
                    Match(TokenType.FloatKeyword);
                    var token = this.lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    var id = new Id(token, Type.Float);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.StringKeyword:
                    Match(TokenType.StringKeyword);
                    token = this.lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.String);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.IntKeyword:
                    Match(TokenType.IntKeyword);
                    token = this.lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.Int);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.FloatListKeyword:
                    Match(TokenType.FloatListKeyword);
                    token = this.lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.FloatList);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.IntListKeyword:
                    Match(TokenType.IntListKeyword);
                    token = this.lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.IntList);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.StringListKeyword:
                    Match(TokenType.StringListKeyword);
                    token = this.lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.StringList);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                default:
                    throw new ApplicationException($"Unsupported type {this.lookAhead.Lexeme}");
            }
        }

        private void Move()
        {
            this.lookAhead = this.scanner.GetNextToken();
        }

        private void Match(TokenType tokenType)
        {
            if (this.lookAhead.TokenType != tokenType)
            {
                throw new ApplicationException($"Syntax error! expected token {tokenType} but found {this.lookAhead.TokenType}. Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}");
            }
            this.Move();
        }

        private bool LookAheadIsType()
        {
            return this.lookAhead.TokenType == TokenType.IntKeyword ||
                this.lookAhead.TokenType == TokenType.StringKeyword ||
                this.lookAhead.TokenType == TokenType.FloatKeyword ||
                this.lookAhead.TokenType == TokenType.IntListKeyword ||
                this.lookAhead.TokenType == TokenType.FloatListKeyword ||
                this.lookAhead.TokenType == TokenType.StringListKeyword;

        }

        void IParser.Parse()
        {
            throw new NotImplementedException();
        }
    }
}
