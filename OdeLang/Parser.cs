using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static OdeLang.Helpers;
using static OdeLang.Tokens;

namespace OdeLang
{
    //todo errors should be far more verbose and less copypasty
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
            if (token.GetTokenType() != type)
            {
                throw new ArgumentException("Unexpected token");
            }
        }

        private NumberStatement Number()
        {
            var currentToken = PopCurrentToken();
            if (currentToken.GetTokenType() != TokenType.Number)
            {
                throw new ArgumentException("Expected number here");
            }
            return new NumberStatement((float) currentToken.GetValue());
        }

        private Statement Expression()
        {
            var statement = Term();
            while (CurrentToken().GetTokenType() == TokenType.Plus || CurrentToken().GetTokenType() == TokenType.Minus)
            {
                var operation = PopCurrentToken();
                var right = Term();

                statement = new BinaryArithmeticStatement(statement, right,
                    arithmeticTokenToOperation(operation.GetTokenType()));
            }

            return statement;
        }

        private Statement Term()
        {
            var statement = Factor();
            while (CurrentToken().GetTokenType() == TokenType.Asterisk || CurrentToken().GetTokenType() == TokenType.Slash)
            {
                var operation = PopCurrentToken();

                var right = Factor();

                statement = new BinaryArithmeticStatement(statement, right,
                    arithmeticTokenToOperation(operation.GetTokenType()));
            }

            return statement;
        }

        private Statement Factor()
        {

            if (CurrentToken().GetTokenType() == TokenType.OpenParenthesis)
            {
                EatAndAdvance(TokenType.OpenParenthesis);
                var expression = Expression();
                EatAndAdvance(TokenType.CloseParenthesis);
                return expression;
            }

            if (CurrentToken().GetTokenType() == TokenType.Number)
            {
                return Number();
            }

            throw new ArgumentException("Unexpected token");
        }


        public Statement Parse()
        {
            return new LoggingStatement(Expression());
        }
    }
}