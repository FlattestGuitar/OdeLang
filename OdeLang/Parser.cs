using System;
using System.Collections.Generic;
using OdeLang.ErrorExceptions;
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

        private TokenType NextTokenTypeAfterWhitespace()
        {
            int i = _i;
            while (i < _tokens.Count)
            {
                if (_tokens[i].TokenType != TokenType.Whitespace)
                {
                    return _tokens[i].TokenType;
                }

                i++;
            }
            
            return EndOfFile;
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

            return new NumberStatement((float) currentToken.Value, currentToken);
        }

        private StringStatement String()
        {
            var currentToken = PopCurrentToken();
            if (currentToken.TokenType != TokenType.String)
            {
                throw UnexpectedTokenException();
            }

            return new StringStatement((string) currentToken.Value, currentToken);
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
                return new BooleanStatement(true, currentToken);
            }

            return new BooleanStatement(false, currentToken);
        }

        private Statement Expression()
        {
            var startToken = CurrentToken();
            var statement = Term();
            while (CurrentToken().TokenType == TokenType.Plus ||
                   CurrentToken().TokenType == TokenType.Minus ||
                   CurrentToken().TokenType == TokenType.Or ||
                   CurrentToken().TokenType == TokenType.And)
            {
                var operation = PopCurrentToken();
                var right = Term();

                statement = new BinaryStatement(statement, right,
                    ArithmeticTokenToOperation(operation), startToken);
            }

            return statement;
        }

        private Statement Term()
        {
            var startToken = CurrentToken();
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
                    ArithmeticTokenToOperation(operation), startToken);
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

            
            return new UnaryStatement(value, ArithmeticTokenToUnaryOperation(token), token);
        }

        private Statement Variable()
        {
            var startToken = CurrentToken();
            if (CurrentToken().TokenType == TokenType.Increment)
            {
                EatAndAdvance(TokenType.Increment);
                var identifier = PopCurrentToken();
                return new ManipulateBeforeReturnStatement((string) identifier.Value, true, startToken);
            }

            if (CurrentToken().TokenType == TokenType.Decrement)
            {
                EatAndAdvance(TokenType.Decrement);
                var identifier = PopCurrentToken();
                return new ManipulateBeforeReturnStatement((string) identifier.Value, false, startToken);
            }

            var id = PopCurrentToken();
            if (CurrentToken().TokenType == TokenType.Increment)
            {
                EatAndAdvance(TokenType.Increment);
                return new ManipulateAfterReturnStatement((string) id.Value, true, startToken);
            }

            if (CurrentToken().TokenType == TokenType.Decrement)
            {
                EatAndAdvance(TokenType.Decrement);
                return new ManipulateAfterReturnStatement((string) id.Value, false, startToken);
            }

            return new VariableReadStatement((string) id.Value, startToken);
        }

        private ConditionalStatement ConditionalStatement(int nestingLevel)
        {
            List<Statement> conditionals = new List<Statement>();
            List<CompoundStatement> bodies = new List<CompoundStatement>();

            var ifToken = PopCurrentToken();

            conditionals.Add(Expression());
            EatAndAdvance(TokenType.Newline);
            bodies.Add(CompoundStatement(nestingLevel + 1));

            while (WhitespaceTokensNextInQueue() == nestingLevel)
            {
                
                if (NextTokenTypeAfterWhitespace() == TokenType.Elif)
                {
                    EatWhitespace(nestingLevel);
                    EatAndAdvance(TokenType.Elif);
                    conditionals.Add(Expression());
                    EatAndAdvance(TokenType.Newline);
                    bodies.Add(CompoundStatement(nestingLevel + 1));        
                } else if (NextTokenTypeAfterWhitespace() == TokenType.Else)
                {
                    EatWhitespace(nestingLevel);
                    EatAndAdvance(TokenType.Else);
                    EatAndAdvance(TokenType.Newline);
                    bodies.Add(CompoundStatement(nestingLevel + 1));
                }
                else
                {
                    break;
                }
            }

            return new ConditionalStatement(conditionals, bodies, ifToken);
        }


        private Statement WhileStatement(int nestingLevel)
        {
            var whileToken = PopCurrentToken();

            Statement condition = Expression();

            EatAndAdvance(TokenType.Newline);
            var body = CompoundStatement(nestingLevel + 1);

            return new WhileLoopStatement(condition, body, whileToken);
        }
        
        private Statement ForStatement(int nestingLevel)
        {
            var forToken = PopCurrentToken();

            var iterationId = (string) PopCurrentToken().Value;
            
            EatAndAdvance(TokenType.In);

            var iterable = Value();
            
            EatAndAdvance(TokenType.Newline);
            var body = CompoundStatement(nestingLevel + 1);

            return new ForLoopStatement(iterationId, iterable, body, forToken);
        }

        private ArrayStatement ArrayStatement()
        {
            var openSquareBracket = PopCurrentToken();
            EatCollectionDeadspace();

            List<Statement> values = new List<Statement>();
            while (CurrentToken().TokenType != TokenType.ClosedSquareBracket)
            {
                values.Add(Expression());

                EatCollectionDeadspace();
            }

            EatAndAdvance(TokenType.ClosedSquareBracket);
            return new ArrayStatement(values, openSquareBracket);
        }

        private DictionaryStatement DictionaryStatement()
        {
            var openCurlyBracket = PopCurrentToken();
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
            return new DictionaryStatement(values, openCurlyBracket);
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

            EatWhitespace(nestingLevel);

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

        private void EatWhitespace(int nestingLevel)
        {
            var whitespaceNextInQueue = WhitespaceTokensNextInQueue();
            if (whitespaceNextInQueue != nestingLevel)
            {
                throw new OdeException(
                    $"Inconsistent indentation. Expected {nestingLevel} whitespace, actual is {whitespaceNextInQueue}",
                    CurrentToken());
            }
            
            for (int i = 0; i < nestingLevel; i++)
            {
                try
                {
                    EatAndAdvance(TokenType.Whitespace);
                }
                catch (ArgumentException e)
                {
                    throw new OdeException($"Bad indentation. Expected {nestingLevel} levels.", CurrentToken().Line, 0);
                }
            }
        }

        private Statement Assignment()
        {
            var firstToken = PopCurrentToken();
            var identifier = (string) firstToken.Value;
            var assignmentType = PopCurrentToken().TokenType;
            if (assignmentType == TokenType.Assignment)
            {
                var expression = Expression();
                return new VariableAssignmentStatement(expression, identifier, firstToken);
            }

            if (assignmentType == TokenType.PlusAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier, firstToken), expr,
                        PlusOperation, firstToken), identifier, firstToken);
            }

            if (assignmentType == TokenType.MinusAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier, firstToken), expr,
                        MinusOperation, firstToken), identifier, firstToken);
            }

            if (assignmentType == TokenType.AsteriskAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier, firstToken), expr,
                        AsteriskOperation, firstToken), identifier, firstToken);
            }

            if (assignmentType == TokenType.SlashAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier, firstToken), expr,
                        SlashOperation, firstToken), identifier, firstToken);
            }

            if (assignmentType == TokenType.ModuloAssignment)
            {
                var expr = Expression();
                return new VariableAssignmentStatement(
                    new BinaryStatement(new VariableReadStatement(identifier, firstToken), expr,
                        ModuloOperation, firstToken), identifier, firstToken);
            }

            throw new ArgumentException("Incorrect assignment type");
        }

        private Statement GlobalFunctionCall()
        {
            var identifier = PopCurrentToken();
            var arguments = SlurpArguments();
            var result = new GlobalFunctionCallStatement(arguments, (string) identifier.Value, identifier);
            return result;
        }

        private Statement ObjectFunctionCall(Statement obj)
        {
            var nameToken = PopCurrentToken();
            var arguments = SlurpArguments();
            var result = new ObjectFunctionCallStatement(arguments, obj, (string) nameToken.Value, nameToken);
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
            var returnToken = PopCurrentToken();
            Statement returnValue;
            if (OnlyWhitespaceBeforeNextNewline())
            {
                returnValue = new NoopStatement(returnToken);
                EatWhitespaceAndNewline();
            }
            else
            {
                returnValue = Expression();
                EatAndAdvance(TokenType.Newline);
            }

            return new FunctionReturnStatement(returnValue, returnToken);
        }

        private Statement FunctionDefinition(int nestingLevel)
        {
            var functionToken = PopCurrentToken();
            
            if (nestingLevel > 0)
            {
                throw new OdeException("Bad function definition. Functions can only be defined at no nesting level.", functionToken);
            }

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

            return new FunctionDefinitionStatement(functionName, argumentNames, body, functionToken);
        }

        private void EatWhitespaceAndNewline()
        {
            while (CurrentToken().TokenType == TokenType.Whitespace)
            {
                EatAndAdvance(TokenType.Whitespace);
            }

            EatAndAdvance(TokenType.Newline);
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
            var breakToken = PopCurrentToken();
            EatAndAdvance(TokenType.Newline);
            return new LoopBreakStatement(breakToken);
        }

        private Statement LoopContinueStatement()
        {
            var continueToken = PopCurrentToken();
            EatAndAdvance(TokenType.Newline);
            return new LoopContinueStatement(continueToken);
        }

        private CompoundStatement CompoundStatement(int nestingLevel)
        {
            var start = CurrentToken();
            List<Statement> statements = new List<Statement>();
            while (CurrentToken().TokenType != EndOfFile)
            {
                //whitespace doesn't matter if there's nothing in the line
                if (OnlyWhitespaceBeforeNextNewline())
                {
                    EatWhitespaceAndNewline();
                    continue;
                }

                if (WhitespaceTokensNextInQueue() < nestingLevel)
                {
                    break; //end of the compound statement
                } 

                statements.Add(Statement(nestingLevel));
            }

            return new CompoundStatement(statements, start);
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