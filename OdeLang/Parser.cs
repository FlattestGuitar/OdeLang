﻿using System;
using System.Collections.Generic;
using static OdeLang.Helpers;
using static OdeLang.Tokens;

namespace OdeLang
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _i;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public Statement Parse()
        {
            return CompoundStatement(0);
        }

        private Token PopCurrentToken()
        {
            _i++;
            return _tokens[_i - 1];
        }

        private Token CurrentToken()
        {
            try
            {
                return _tokens[_i];
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

        private StringStatement String()
        {
            var currentToken = PopCurrentToken();
            if (currentToken.TokenType != TokenType.String)
            {
                throw UnexpectedTokenException();
            }

            return new StringStatement((string) currentToken.Value);
        }

        private BooleanStatement Boolean()
        {
            var currentToken = PopCurrentToken();
            if (currentToken.TokenType != TokenType.Boolean)
            {
                throw UnexpectedTokenException();
            }

            if ((string) currentToken.Value == Language.BooleanTrue)
            {
                return new BooleanStatement(true);
            }

            return new BooleanStatement(false);
        }

        private Statement Expression()
        {
            var statement = Term();
            while (CurrentToken().TokenType is TokenType.Plus or TokenType.Minus)
            {
                var operation = PopCurrentToken();
                var right = Term();

                statement = new BinaryArithmeticStatement(statement, right,
                    ArithmeticTokenToOperation(operation.TokenType));
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
                    ArithmeticTokenToOperation(operation.TokenType));
            }

            return statement;
        }

        private Statement Factor()
        {
            if (CurrentToken().TokenType == TokenType.String)
            {
                return String();
            }

            if (CurrentToken().TokenType == TokenType.Boolean)
            {
                return Boolean();
            }

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

        private ConditionalStatement ConditionalStatement(int nestingLevel)
        {
            EatAndAdvance(TokenType.If);
            EatAndAdvance(TokenType.OpenParenthesis);
            BooleanStatement boolean = Boolean();
            EatAndAdvance(TokenType.CloseParenthesis);
            EatAndAdvance(TokenType.Newline);
            var compound = CompoundStatement(nestingLevel + 1);

            return new ConditionalStatement(boolean, compound);
        }

        private Statement Statement(int nestingLevel)
        {
            for (int i = 0; i < nestingLevel; i++)
            {
                try
                {
                    EatAndAdvance(TokenType.Whitespace);
                }
                catch (ArgumentException e)
                { 
                    //todo better line+col number
                    throw new ArgumentException($"Incorrect indentation, expected {nestingLevel} levels.");
                }
                
            }
            
            if (CurrentToken().TokenType == TokenType.If)
            {
                return ConditionalStatement(nestingLevel);
            }
            
            var identifier = PopCurrentToken();

            if (CurrentToken().TokenType == TokenType.OpenParenthesis)
            {
                EatAndAdvance(TokenType.OpenParenthesis);
                var expression = Expression();
                EatAndAdvance(TokenType.CloseParenthesis);
                var result = new FunctionCallStatement(expression, (string) identifier.Value);
                EatAndAdvance(TokenType.Newline);
                return result;
            }

            if (CurrentToken().TokenType == TokenType.Assignment)
            {
                EatAndAdvance(TokenType.Assignment);
                var expression = Expression();
                var result = new VariableAssignmentStatement(expression, (string) identifier.Value);
                EatAndAdvance(TokenType.Newline);
                return result;
            }

            throw UnexpectedTokenException();
        }

        private CompoundStatement CompoundStatement(int nestingLevel)
        {
            List<Statement> statements = new List<Statement>();
            while (CurrentToken().TokenType != TokenType.EndOfFile)
            {
                var nextWhitespace = WhitespaceTokensNextInQueue();
                if (nextWhitespace < nestingLevel)
                {
                    break;
                }

                if (nextWhitespace > nestingLevel)
                {
                    throw new ArgumentException("Inconsistent indentation!"); //todo better exception
                }

                statements.Add(Statement(nestingLevel));

            }

            return new CompoundStatement(statements);
        }

        private int WhitespaceTokensNextInQueue()
        {
            var iterator = _i;
            while (_tokens[iterator].TokenType == TokenType.Whitespace)
            {
                iterator++;
            }
            
            return iterator - _i;
        }

        private ArgumentException UnexpectedTokenException()
        {
            var token = CurrentToken();
            return new ArgumentException($"Unexpected token {token.Value} at {token.Line}:{token.Column}");
        }
    }
}