using System;
using System.Collections.Generic;
using static OdeLang.Operators;
using static OdeLang.Tokens;
using static OdeLang.TokenType;

namespace OdeLang
{
    /// <summary>
    /// This class is at the heart of parsing tokens in OdeLang.
    /// Each method roughly corresponds to a grammar rule in the language.
    /// The entire program is one big compound statement, but these statements get broken down further
    /// to create a complete syntax tree.
    /// </summary>
    internal class Parser
    {
        private static readonly List<TokenType> AssignmentTypes = new List<TokenType>()
        {
            TokenType.Assignment,
            TokenType.PlusAssignment,
            TokenType.MinusAssignment,
            TokenType.AsteriskAssignment,
            TokenType.SlashAssignment,
            TokenType.ModuloAssignment,
        };

        private List<Token> _tokens;
        private int _i;

        internal Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        internal Statement Parse()
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
            while (CurrentToken().TokenType == TokenType.Plus ||
                   CurrentToken().TokenType == TokenType.Minus ||
                   CurrentToken().TokenType == TokenType.Or ||
                   CurrentToken().TokenType == TokenType.And)
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
            while (CurrentToken().TokenType == TokenType.Asterisk ||
                   CurrentToken().TokenType == TokenType.Slash ||
                   CurrentToken().TokenType == TokenType.Modulo ||
                   CurrentToken().TokenType == TokenType.LessThan ||
                   CurrentToken().TokenType == TokenType.MoreThan ||
                   CurrentToken().TokenType == TokenType.Equal ||
                   CurrentToken().TokenType == TokenType.NotEqual)
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

            if (CurrentToken().TokenType == TokenType.Minus ||
                CurrentToken().TokenType == TokenType.Plus ||
                CurrentToken().TokenType == TokenType.Not)
            {
                return UnaryOperatorFactor();
            }

            if (CurrentToken().TokenType == TokenType.OpenSquareBracket)
            {
                return ArrayStatement();
            }

            if (CurrentToken().TokenType == TokenType.OpenCurlyBracket)
            {
                return DictionaryStatement();
            }

            if (CurrentToken().TokenType == TokenType.Identifier || CurrentToken().TokenType == TokenType.Increment ||
                CurrentToken().TokenType == TokenType.Decrement)
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

        private Token PeekNextToken(int offset)
        {
            try
            {
                return _tokens[_i + offset + 1];
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }

        private Token PeekNextToken()
        {
            return PeekNextToken(0);
        }

        private UnaryStatement UnaryOperatorFactor()
        {
            var token = PopCurrentToken();
            var value = Factor();

            return new UnaryStatement(value, ArithmeticTokenToUnaryOperation(token.TokenType));
        }

        private Statement Variable()
        {
            if (CurrentToken().TokenType == TokenType.Increment)
            {
                EatAndAdvance(TokenType.Increment);
                var identifier = PopCurrentToken();
                return new ManipulateBeforeReturnStatement((string) identifier.Value, true);
            }

            if (CurrentToken().TokenType == TokenType.Decrement)
            {
                EatAndAdvance(TokenType.Decrement);
                var identifier = PopCurrentToken();
                return new ManipulateBeforeReturnStatement((string) identifier.Value, false);
            }

            var id = PopCurrentToken();
            if (CurrentToken().TokenType == TokenType.Increment)
            {
                EatAndAdvance(TokenType.Increment);
                return new ManipulateAfterReturnStatement((string) id.Value, true);
            }

            if (CurrentToken().TokenType == TokenType.Decrement)
            {
                EatAndAdvance(TokenType.Decrement);
                return new ManipulateAfterReturnStatement((string) id.Value, false);
            }

            return new VariableReadStatement((string) id.Value);
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

            return new WhileLoopStatement(condition, body);
        }
        
        private Statement ForStatement(int nestingLevel)
        {
            EatAndAdvance(TokenType.For);

            var iterationId = (string) PopCurrentToken().Value;
            
            EatAndAdvance(TokenType.In);

            var iterable = Value();
            
            EatAndAdvance(TokenType.Newline);
            var body = CompoundStatement(nestingLevel + 1);

            return new ForLoopStatement(iterationId, iterable, body);
        }

        private ArrayStatement ArrayStatement()
        {
            EatAndAdvance(TokenType.OpenSquareBracket);
            EatCollectionDeadspace();

            List<Statement> values = new List<Statement>();
            while (CurrentToken().TokenType != TokenType.ClosedSquareBracket)
            {
                values.Add(Expression());

                EatCollectionDeadspace();
            }

            EatAndAdvance(TokenType.ClosedSquareBracket);
            return new ArrayStatement(values);
        }

        private DictionaryStatement DictionaryStatement()
        {
            EatAndAdvance(TokenType.OpenCurlyBracket);
            EatCollectionDeadspace();

            var values = new List<Tuple<Statement, Statement>>();
            while (CurrentToken().TokenType != TokenType.ClosedCurlyBracket)
            {
                var key = Expression();
                EatAndAdvance(TokenType.Colon);
                var value = Expression();
                values.Add(new Tuple<Statement, Statement>(key, value));

                EatCollectionDeadspace();
            }

            EatAndAdvance(TokenType.ClosedCurlyBracket);
            return new DictionaryStatement(values);
        }

        private void EatCollectionDeadspace()
        {
            while (CurrentToken().TokenType == TokenType.Comma ||
                   CurrentToken().TokenType == TokenType.Whitespace ||
                   CurrentToken().TokenType == TokenType.Newline)
            {
                PopCurrentToken();
            }
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

            if (CurrentToken().TokenType == TokenType.Fn)
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

            if (CurrentToken().TokenType == TokenType.For)
            {
                return ForStatement(nestingLevel);
            }

            if (CurrentToken().TokenType == TokenType.Identifier)
            {
                if (AssignmentTypes.Contains(PeekNextToken().TokenType))
                {
                    return Assignment();
                }

                return Expression();
            }

            throw UnexpectedTokenException();
        }

        private Statement Assignment()
        {
            var identifier = (string) PopCurrentToken().Value;
            var assignmentType = PopCurrentToken().TokenType;
            if (assignmentType == TokenType.Assignment)
            {
                var expression = Expression();
                return new VariableAssignmentStatement(expression, identifier);
            }

            if (assignmentType == TokenType.PlusAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier), expr,
                        ArithmeticTokenToOperation(TokenType.Plus)), identifier);
            }

            if (assignmentType == TokenType.MinusAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier), expr,
                        ArithmeticTokenToOperation(TokenType.Minus)), identifier);
            }

            if (assignmentType == TokenType.AsteriskAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier), expr,
                        ArithmeticTokenToOperation(TokenType.Asterisk)), identifier);
            }

            if (assignmentType == TokenType.SlashAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier), expr,
                        ArithmeticTokenToOperation(TokenType.Slash)), identifier);
            }

            if (assignmentType == TokenType.ModuloAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier), expr,
                        ArithmeticTokenToOperation(TokenType.ModuloAssignment)), identifier);
            }

            //todo this will never be reached, bad structure
            throw new ArgumentException("Incorrect assignment type");
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
            List<Statement> arguments = new List<Statement>();
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

            EatAndAdvance(TokenType.Fn);
            var functionName = (string) PopCurrentToken().Value;
            EatAndAdvance(TokenType.OpenParenthesis);
            List<string> argumentNames = new List<string>();
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