using System;
using System.Collections.Generic;
using static OdeLang.Helpers;
using static OdeLang.Tokens;
using static OdeLang.TokenType;

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
            while (CurrentToken().TokenType is TokenType.Plus or TokenType.Minus or TokenType.Or or TokenType.And)
            {
                var operation = PopCurrentToken();
                var right = Term();

                statement = new BinaryStatement(statement, right,
                    ArithmeticTokenToOperation(operation.TokenType));
            }

            return statement;
        }

        private Statement Term()
        {
            var statement = Factor();
            while (CurrentToken().TokenType is TokenType.Asterisk or TokenType.Slash or TokenType.Modulo or
                TokenType.LessThan or TokenType.MoreThan or TokenType.Equal or TokenType.NotEqual)
            {
                var operation = PopCurrentToken();

                var right = Factor();

                statement = new BinaryStatement(statement, right,
                    ArithmeticTokenToOperation(operation.TokenType));
            }

            return statement;
        }

        private Statement Value()
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

            if (CurrentToken().TokenType is TokenType.Minus or TokenType.Plus or TokenType.Not)
            {
                return UnaryOperatorFactor();
            }
            
            if (CurrentToken().TokenType == TokenType.OpenSquareBracket)
            {
                return ArrayStatement();
            }

            if (CurrentToken().TokenType == TokenType.Identifier)
            {
                if (PeekNextToken().TokenType == TokenType.OpenParenthesis)
                {
                    return GlobalFunctionCall();
                }

                return Variable();
            }
            
            throw UnexpectedTokenException();
        }

        private Statement Factor()
        {
            var statement = Value();

            while (CurrentToken().TokenType == TokenType.Period)
            {
                EatAndAdvance(TokenType.Period);
                statement = ObjectFunctionCall(statement);
            }

            return statement;
        }

        private Token PeekNextToken()
        {
            try
            {
                return _tokens[_i + 1];
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }

        private UnaryStatement UnaryOperatorFactor()
        {
            var token = PopCurrentToken();
            var value = Factor();

            return new UnaryStatement(value, ArithmeticTokenToUnaryOperation(token.TokenType));
        }

        private Statement Variable()
        {
            var identifier = PopCurrentToken();
            return new VariableReadStatement((string) identifier.Value);
        }

        private ConditionalStatement ConditionalStatement(int nestingLevel)
        {
            EatAndAdvance(TokenType.If);

            Statement boolean = Expression();

            EatAndAdvance(TokenType.Newline);
            var compound = CompoundStatement(nestingLevel + 1);

            return new ConditionalStatement(boolean, compound);
        }


        private Statement WhileStatement(int nestingLevel)
        {
            EatAndAdvance(TokenType.While);

            Statement condition = Expression();

            EatAndAdvance(TokenType.Newline);
            var body = CompoundStatement(nestingLevel + 1);

            return new LoopStatement(condition, body);
        }

        private ArrayStatement ArrayStatement()
        {
            EatAndAdvance(TokenType.OpenSquareBracket);
            List<Statement> values = new();
            while (CurrentToken().TokenType != TokenType.ClosedSquareBracket)
            {
                values.Add(Expression());
                //ignore all whitespace while we're eating an array
                while (CurrentToken().TokenType is TokenType.Comma or TokenType.Whitespace or TokenType.Newline)
                {
                    PopCurrentToken();
                }
            }

            EatAndAdvance(TokenType.ClosedSquareBracket);
            return new ArrayStatement(values);
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

            if (CurrentToken().TokenType == TokenType.Return)
            {
                return FunctionReturn();
            }

            if (CurrentToken().TokenType == TokenType.Def)
            {
                return FunctionDefinition(nestingLevel);
            }

            if (CurrentToken().TokenType == TokenType.If)
            {
                return ConditionalStatement(nestingLevel);
            }

            if (CurrentToken().TokenType == TokenType.Continue)
            {
                return LoopContinueStatement();
            }

            if (CurrentToken().TokenType == TokenType.Break)
            {
                return LoopBreakStatement();
            }

            if (CurrentToken().TokenType == TokenType.While)
            {
                return WhileStatement(nestingLevel);
            }

            if (CurrentToken().TokenType == TokenType.Identifier)
            {
                if (PeekNextToken().TokenType == TokenType.Assignment)
                {
                    return Assignment();
                }

                return Expression();
            }

            throw UnexpectedTokenException();
        }

        private Statement Assignment()
        {
            var identifier = PopCurrentToken();
            EatAndAdvance(TokenType.Assignment);
            var expression = Expression();
            var result = new VariableAssignmentStatement(expression, (string) identifier.Value);
            EatAndAdvance(TokenType.Newline);
            return result;
        }

        private Statement GlobalFunctionCall()
        {
            var identifier = PopCurrentToken();
            var arguments = SlurpArguments();
            var result = new GlobalFunctionCallStatement(arguments, (string) identifier.Value);
            return result;
        }

        private Statement ObjectFunctionCall(Statement obj)
        {
            var nameToken = PopCurrentToken();
            var arguments = SlurpArguments();
            var result = new ObjectFunctionCallStatement(arguments, obj, (string) nameToken.Value);
            return result;
        }

        private List<Statement> SlurpArguments()
        {
            EatAndAdvance(TokenType.OpenParenthesis);
            List<Statement> arguments = new();
            while (CurrentToken().TokenType != TokenType.CloseParenthesis)
            {
                arguments.Add(Expression());
                if (CurrentToken().TokenType == TokenType.Comma)
                {
                    EatAndAdvance(TokenType.Comma);
                }
                else
                {
                    break;
                }
            }

            EatAndAdvance(TokenType.CloseParenthesis);
            return arguments;
        }


        private Statement FunctionReturn()
        {
            EatAndAdvance(TokenType.Return);
            Statement returnValue;
            if (OnlyWhitespaceBeforeNextNewline())
            {
                returnValue = NoopStatement();
            }
            else
            {
                returnValue = Expression();
                EatAndAdvance(TokenType.Newline);
            }

            return new FunctionReturnStatement(returnValue);
        }

        private Statement FunctionDefinition(int nestingLevel)
        {
            if (nestingLevel > 0)
            {
                throw new ArgumentException("Functions can only be defined at the base level");
            }

            EatAndAdvance(TokenType.Def);
            var functionName = (string) PopCurrentToken().Value;
            EatAndAdvance(TokenType.OpenParenthesis);
            List<string> argumentNames = new();
            while (CurrentToken().TokenType != TokenType.CloseParenthesis)
            {
                argumentNames.Add((string) PopCurrentToken().Value);
                if (CurrentToken().TokenType == TokenType.Comma)
                {
                    EatAndAdvance(TokenType.Comma);
                }
            }

            EatAndAdvance(TokenType.CloseParenthesis);
            EatAndAdvance(TokenType.Newline);
            var body = CompoundStatement(nestingLevel + 1);

            return new FunctionDefinitionStatement(functionName, argumentNames, body);
        }

        private Statement NoopStatement()
        {
            while (CurrentToken().TokenType == TokenType.Whitespace)
            {
                EatAndAdvance(TokenType.Whitespace);
            }

            EatAndAdvance(TokenType.Newline);
            return new NoopStatement();
        }

        private bool OnlyWhitespaceBeforeNextNewline()
        {
            int iterator = _i;
            while (_tokens[iterator].TokenType == TokenType.Whitespace)
            {
                iterator++;
            }

            if (_tokens[iterator].TokenType == TokenType.Newline)
            {
                return true;
            }

            return false;
        }

        private Statement LoopBreakStatement()
        {
            EatAndAdvance(TokenType.Break);
            EatAndAdvance(TokenType.Newline);
            return new LoopBreakStatement();
        }

        private Statement LoopContinueStatement()
        {
            EatAndAdvance(TokenType.Continue);
            EatAndAdvance(TokenType.Newline);
            return new LoopContinueStatement();
        }

        private CompoundStatement CompoundStatement(int nestingLevel)
        {
            List<Statement> statements = new List<Statement>();
            while (CurrentToken().TokenType != EndOfFile)
            {
                //whitespace doesn't matter if there's nothing in the line
                if (OnlyWhitespaceBeforeNextNewline())
                {
                    statements.Add(NoopStatement());
                    continue;
                }

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