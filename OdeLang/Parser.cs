using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static OdeLang.Helpers;
using static OdeLang.Tokens;

namespace OdeLang
{
    public class Parser
    {
        private List<Token> tokens;
        private int i = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Token PopCurrentToken()
        {
            i++;
            return tokens[i - 1];
        }
        private Token CurrentToken()
        {
            try
            {
                return tokens[i];
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
            
        }

        private void EatAndAdvance(TokenType type)
        {
            var token = PopCurrentToken();
            if (token.TokenType != type)
            {
                throw UnexpectedTokenException();
            }
        }

        private NumberStatement Number()
        {
            var currentToken = PopCurrentToken();
            if (currentToken.TokenType != TokenType.Number)
            {
                throw UnexpectedTokenException();
            }
            return new NumberStatement((float) currentToken.Value);
        }

        private Statement Expression()
        {
            var statement = Term();
            while (CurrentToken().TokenType == TokenType.Plus || CurrentToken().TokenType == TokenType.Minus)
            {
                var operation = PopCurrentToken();
                var right = Term();

                statement = new BinaryArithmeticStatement(statement, right,
                    arithmeticTokenToOperation(operation.TokenType));
            }

            return statement;
        }

        private Statement Term()
        {
            var statement = Factor();
            while (CurrentToken().TokenType == TokenType.Asterisk || CurrentToken().TokenType == TokenType.Slash)
            {
                var operation = PopCurrentToken();

                var right = Factor();

                statement = new BinaryArithmeticStatement(statement, right,
                    arithmeticTokenToOperation(operation.TokenType));
            }

            return statement;
        }

        private Statement Factor()
        {

            if (CurrentToken().TokenType == TokenType.OpenParenthesis)
            {
                EatAndAdvance(TokenType.OpenParenthesis);
                var expression = Expression();
                EatAndAdvance(TokenType.CloseParenthesis);
                return expression;
            }

            if (CurrentToken().TokenType == TokenType.Number)
            {
                return Number();
            }

            throw UnexpectedTokenException();
        }

        private Statement CompoundStatement()
        {
            List<Statement> statements = new List<Statement>();
            while (CurrentToken().TokenType != TokenType.EndOfFile)
            {
                statements.Add(Expression());
            }
            
            return new CompoundStatement(statements);
        }


        public Statement Parse()
        {
            return new LoggingStatement(CompoundStatement());
        }

        private ArgumentException UnexpectedTokenException()
        {
            var token = CurrentToken();
            return new ArgumentException($"Unexpected token {token.Value} at {token.Line}:{token.Column}");
        }
    }
}