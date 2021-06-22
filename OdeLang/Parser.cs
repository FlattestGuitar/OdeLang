using System;
using System.Collections.Generic;
using static OdeLang.Helpers;
using static OdeLang.Tokens;

namespace OdeLang
{
    public class Parser
    {
        private List<Token> tokens;
        private int i;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Statement Parse()
        {
            return CompoundStatement();
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
            while (CurrentToken().TokenType is TokenType.Asterisk or TokenType.Slash)
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

            if (CurrentToken().TokenType == TokenType.Identifier)
            {
                return Variable();
            }

            throw UnexpectedTokenException();
        }

        private Statement Variable()
        {
            var identifier = PopCurrentToken();
            return new VariableReadStatement((string) identifier.Value);
        }

        private Statement Statement()
        {
            var identifier = PopCurrentToken();
            if (CurrentToken().TokenType == TokenType.OpenParenthesis)
            {
                EatAndAdvance(TokenType.OpenParenthesis);
                var expression = Expression();
                EatAndAdvance(TokenType.CloseParenthesis);
                return new FunctionCallStatement(expression, (string) identifier.Value);
            }

            if (CurrentToken().TokenType == TokenType.Assignment)
            {
                EatAndAdvance(TokenType.Assignment);
                var expression = Expression();
                return new VariableAssignmentStatement(expression, (string) identifier.Value);
            }

            throw UnexpectedTokenException();
        }

        private Statement CompoundStatement()
        {
            List<Statement> statements = new List<Statement>();
            while (CurrentToken().TokenType != TokenType.EndOfFile)
            {
                statements.Add(Statement());
                EatAndAdvance(TokenType.Newline);
            }
            
            return new CompoundStatement(statements);
        }


        private ArgumentException UnexpectedTokenException()
        {
            var token = CurrentToken();
            return new ArgumentException($"Unexpected token {token.Value} at {token.Line}:{token.Column}");
        }
    }
}