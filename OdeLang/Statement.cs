﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using OdeLang.ErrorExceptions;
using static OdeLang.Tokens;
using static OdeLang.Value;

namespace OdeLang
{
    /// <summary>
    /// Statements are stateless building blocks for the syntax tree.
    /// The top level statement is always a compound statement, but anything can happen below that.
    /// </summary>
    internal abstract class Statement
    {
        internal Statement(Token firstToken)
        {
            Line = firstToken.Line;
            Column = firstToken.Column;
        }

        internal int Line { get; }
        internal int Column { get; }

        internal abstract Value Eval(InterpretingContext context);
    }

    internal class CompoundStatement : Statement
    {
        private readonly List<Statement> _children;

        internal CompoundStatement(List<Statement> children, Token firstToken) : base(firstToken)
        {
            _children = children;
        }

        internal override Value Eval(InterpretingContext context)
        {
            _children.ForEach(child => child.Eval(context));
            return NullValue();
        }
    }

    //any arithmetic operation on two numbers
    internal class BinaryStatement : Statement
    {
        private readonly Statement _left;
        private readonly Statement _right;
        private readonly Func<Value, Value, Value> _operation;

        internal BinaryStatement(Statement left, Statement right, Func<Value, Value, Value> operation, Token firstToken)
            : base(firstToken)
        {
            _left = left;
            _right = right;
            _operation = operation;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return _operation.Invoke(_left.Eval(context), _right.Eval(context));
        }
    }

    internal class UnaryStatement : Statement
    {
        private readonly Statement _value;
        private readonly Func<Value, Value> _operation;

        internal UnaryStatement(Statement value, Func<Value, Value> operation, Token firstToken) : base(firstToken)
        {
            _operation = operation;
            _value = value;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return _operation.Invoke(_value.Eval(context));
        }
    }

    internal class ObjectFunctionCallStatement : Statement
    {
        private readonly List<Statement> _arguments;
        private readonly Statement _obj;
        private readonly string _functionName;

        internal ObjectFunctionCallStatement(List<Statement> arguments, Statement obj, string functionName,
            Token firstToken) : base(firstToken)
        {
            _arguments = arguments;
            _obj = obj;
            _functionName = functionName;
        }

        internal override Value Eval(InterpretingContext context)
        {
            try
            {
                return _obj.Eval(context).GetObjectValue().CallFunction(_functionName,
                    new List<Value>(_arguments.Select(arg => arg.Eval(context))));
            }
            catch (ArgumentException)
            {
                throw new OdeException($"Object does not contain function {_functionName}", this);
            }
            catch (ArgumentCountException e)
            {
                throw new OdeException(
                    $"Function {{name}} called with wrong argument count. Required: {e.Required}, Actual: {e.Actual}",
                    this);
            }
        }
    }

    internal class GlobalFunctionCallStatement : Statement
    {
        private readonly List<Statement> _arguments;
        private readonly string _functionName;

        internal GlobalFunctionCallStatement(List<Statement> arguments, string functionName, Token firstToken) :
            base(firstToken)
        {
            _arguments = arguments;
            _functionName = functionName;
        }

        internal override Value Eval(InterpretingContext context)
        {
            try
            {
                return context.CallGlobalFunction(_functionName, _arguments.Select(arg => arg.Eval(context)).ToList());
            }
            catch (ArgumentException)
            {
                throw new OdeException($"No such function {_functionName}", this);
            }
            catch (ArgumentCountException e)
            {
                throw new OdeException(
                    $"Function {{name}} called with wrong argument count. Required: {e.Required}, Actual: {e.Actual}",
                    this);
            }
        }
    }

    internal class FunctionDefinitionStatement : Statement
    {
        private readonly string _functionName;
        private readonly List<string> _argumentNames;
        private readonly Statement _body;

        internal FunctionDefinitionStatement(string functionName, List<string> argumentNames, Statement body,
            Token firstToken) : base(firstToken)
        {
            _functionName = functionName;
            _argumentNames = argumentNames;
            _body = body;
        }

        internal override Value Eval(InterpretingContext context)
        {
            context.RegisterFunction(_functionName, _argumentNames, _body);
            return NullValue();
        }
    }

    internal class VariableAssignmentStatement : Statement
    {
        private readonly Statement _value;
        private readonly string _variableName;

        internal VariableAssignmentStatement(Statement value, string variableName, Token firstToken) : base(firstToken)
        {
            _value = value;
            _variableName = variableName;
        }


        internal override Value Eval(InterpretingContext context)
        {
            context.SetVariable(_variableName, _value.Eval(context));
            return null;
        }
    }

    internal class VariableReadStatement : Statement
    {
        private readonly string _variableName;

        internal VariableReadStatement(string variableName, Token firstToken) : base(firstToken)
        {
            _variableName = variableName;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return context.GetVariable(_variableName, Column, Line);
        }
    }

    internal class NumberStatement : Statement
    {
        private readonly float _number;

        internal NumberStatement(float number, Token firstToken) : base(firstToken)
        {
            _number = number;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return NumericalValue(_number);
        }
    }

    internal class StringStatement : Statement
    {
        private readonly string _string;

        internal StringStatement(string stringy, Token firstToken) : base(firstToken)
        {
            _string = stringy;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return StringValue(_string);
        }
    }

    internal class BooleanStatement : Statement
    {
        private readonly bool _bool;

        internal BooleanStatement(bool booly, Token firstToken) : base(firstToken)
        {
            _bool = booly;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return BoolValue(_bool);
        }
    }

    internal class ConditionalStatement : Statement
    {
        private readonly List<Statement> _conditionals; //if, elif conditionals
        private readonly List<CompoundStatement> _bodies; //if, elif, else bodies. Can be one more than above.

        internal ConditionalStatement(List<Statement> conditionals, List<CompoundStatement> bodies, Token firstToken) :
            base(firstToken)
        {
            _conditionals = conditionals;
            _bodies = bodies;
        }

        internal override Value Eval(InterpretingContext context)
        {
            for (int i = 0; i < _conditionals.Count; i++)
            {
                if (_conditionals[i].Eval(context).GetBoolValue())
                {
                    return _bodies[i].Eval(context);
                }
            }

            if (_bodies.Count > _conditionals.Count)
            {
                return _bodies[_bodies.Count - 1].Eval(context);
            }
            else
            {
                return NullValue();
            }
        }
    }

    internal class WhileLoopStatement : Statement
    {
        private static readonly int MaxLoopRuns = 10000;

        private readonly Statement _condition;
        private readonly CompoundStatement _body;

        internal WhileLoopStatement(Statement condition, CompoundStatement body, Token firstToken) : base(firstToken)
        {
            _condition = condition;
            _body = body;
        }


        internal override Value Eval(InterpretingContext context)
        {
            int runs = 0;
            while (true)
            {
                if (runs > MaxLoopRuns)
                {
                    throw new OdeException($"Possible infinite loop. Ran for over {MaxLoopRuns} runs.", this);
                }

                if (!_condition.Eval(context).GetBoolValue())
                {
                    return NullValue();
                }

                try
                {
                    runs++;
                    _body.Eval(context);
                }
                catch (LoopBreakException)
                {
                    return NullValue();
                }
                catch (LoopContinueException)
                {
                    //noop
                }
            }
        }
    }

    internal class ForLoopStatement : Statement
    {
        private readonly string _iteratorName;
        private readonly Statement _iterable;
        private readonly CompoundStatement _body;

        internal ForLoopStatement(string iteratorName, Statement iterable, CompoundStatement body, Token firstToken) :
            base(firstToken)
        {
            _iteratorName = iteratorName;
            _iterable = iterable;
            _body = body;
        }

        internal override Value Eval(InterpretingContext context)
        {
            var evaluatedIterable = _iterable.Eval(context).GetObjectValue();

            if (!(evaluatedIterable is OdeCollection))
            {
                throw new OdeException("Only a list or dictionary can be used as an iterable in a for loop.", this);
            }

            var evaluatedCollection = (OdeCollection) evaluatedIterable;
            var size = evaluatedCollection.Length();

            for (int i = 0; i < size; i++)
            {
                try
                {
                    context.ForLoopIteration(i, evaluatedCollection, _body, _iteratorName);
                }
                catch (LoopBreakException)
                {
                    break;
                }
                catch (LoopContinueException)
                {
                    continue;
                }
            }

            return NullValue();
        }
    }


    internal class LoopBreakStatement : Statement
    {
        public LoopBreakStatement(Token firstToken) : base(firstToken)
        {
        }

        internal override Value Eval(InterpretingContext context)
        {
            throw new LoopBreakException();
        }
    }

    internal class LoopContinueStatement : Statement
    {
        public LoopContinueStatement(Token firstToken) : base(firstToken)
        {
        }

        internal override Value Eval(InterpretingContext context)
        {
            throw new LoopContinueException();
        }
    }

    internal class NoopStatement : Statement
    {
        public NoopStatement(Token firstToken) : base(firstToken)
        {
        }

        internal override Value Eval(InterpretingContext context)
        {
            return NullValue();
        }
    }

    internal class FunctionReturnStatement : Statement
    {
        private readonly Statement returnValue;

        internal FunctionReturnStatement(Statement returnValue, Token firstToken) : base(firstToken)
        {
            this.returnValue = returnValue;
        }

        internal override Value Eval(InterpretingContext context)
        {
            try
            {
                context.ValidateCanReturn();
            }
            catch (ArgumentException)
            {
                throw new OdeException("Cannot use return statement when not inside of a function!", this);
            }
            
            throw new FunctionReturnException(returnValue.Eval(context));
        }
    }

    internal class ArrayStatement : Statement
    {
        private readonly List<Statement> _values;

        internal ArrayStatement(List<Statement> values, Token firstToken) : base(firstToken)
        {
            _values = values;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return ObjectValue(Objects.Array(new List<Value>(_values.Select(statement => statement.Eval(context)))));
        }
    }

    internal class DictionaryStatement : Statement
    {
        private readonly List<Tuple<Statement, Statement>> _values;

        internal DictionaryStatement(List<Tuple<Statement, Statement>> values, Token firstToken) : base(firstToken)
        {
            _values = values;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return ObjectValue(Objects.Dictionary(
                _values.Select(tuple =>
                    new Tuple<Value, Value>(
                        tuple.Item1.Eval(context),
                        tuple.Item2.Eval(context)
                    )
                ).ToList()
            ));
        }
    }
    internal class CollectionAccessStatement : Statement
    {
        private readonly Statement _collection;
        private readonly Statement _index;

        internal CollectionAccessStatement(Statement collection, Statement indexToAccess, Token firstToken) : base(firstToken)
        {
            _collection = collection;
            _index = indexToAccess;
        }

        internal override Value Eval(InterpretingContext context)
        {
            try
            {
                return _collection
                    .Eval(context)
                    .GetObjectValue()
                    .CallFunction(
                        "get",
                        new List<Value> {_index.Eval(context)});
            }
            catch (ArgumentException)
            {
                throw new OdeException("The [] operator must be used on an array or dictionary.", this);
            }
        }
    }
    
    internal class CollectionAssignmentStatement : Statement
    {
        private readonly Statement _collection;
        private readonly Statement _index;
        private readonly Statement _value;

        internal CollectionAssignmentStatement(Statement collection, Statement indexToAccess, Statement value, Token firstToken) : base(firstToken)
        {
            _collection = collection;
            _index = indexToAccess;
            _value = value;
        }

        internal override Value Eval(InterpretingContext context)
        {
            var collection = _collection
                .Eval(context)
                .GetObjectValue();

            var index = _index.Eval(context);

            var value = _value.Eval(context);

            if (collection.GetType() == typeof(OdeArray))
            {
                return ((OdeArray) collection).CallFunction("set", new List<Value>() {index, value});
            }if (collection.GetType() == typeof(OdeDictionary))
            {
                return ((OdeDictionary) collection).CallFunction("put", new List<Value>() {index, value});
            }

            throw new OdeException("The [] operator must be used on an array or dictionary.", this);
        }
    }

    internal class ManipulateBeforeReturnStatement : Statement
    {
        private readonly bool _increment;
        private readonly string _identifier;

        public ManipulateBeforeReturnStatement(string identifier, bool increment, Token firstToken) : base(firstToken)
        {
            _identifier = identifier;
            _increment = increment;
        }

        internal override Value Eval(InterpretingContext context)
        {
            var value = context.GetVariable(_identifier, Column, Line);
            context.SetVariable(_identifier,
                _increment
                    ? NumericalValue(value.GetNumericalValue() + 1)
                    : NumericalValue(value.GetNumericalValue() - 1));

            return context.GetVariable(_identifier, Column, Line);
        }
    }

    internal class ManipulateAfterReturnStatement : Statement
    {
        private readonly bool _increment;
        private readonly string _identifier;

        public ManipulateAfterReturnStatement(string identifier, bool increment, Token firstToken) : base(firstToken)
        {
            _identifier = identifier;
            _increment = increment;
        }

        internal override Value Eval(InterpretingContext context)
        {
            var value = context.GetVariable(_identifier, Column, Line);
            context.SetVariable(_identifier,
                _increment
                    ? NumericalValue(value.GetNumericalValue() + 1)
                    : NumericalValue(value.GetNumericalValue() - 1));

            return value;
        }
    }
}