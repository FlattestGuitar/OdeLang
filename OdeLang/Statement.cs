using System;
using System.Collections.Generic;
using System.Linq;
using static OdeLang.Value;

namespace OdeLang
{
    /// <summary>
    /// Statements are stateless building blocks for the syntax tree.
    /// The top level statement is always a compound statement, but anything can happen below that.
    /// </summary>
    internal abstract class Statement
    {
        internal abstract Value Eval(InterpretingContext context);
    }

    internal class CompoundStatement : Statement
    {
        private readonly List<Statement> _children;

        internal CompoundStatement(List<Statement> children)
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

        internal BinaryStatement(Statement left, Statement right, Func<Value, Value, Value> operation)
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

        internal UnaryStatement(Statement value, Func<Value, Value> operation)
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

        internal ObjectFunctionCallStatement(List<Statement> arguments, Statement obj, string functionName)
        {
            _arguments = arguments;
            _obj = obj;
            _functionName = functionName;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return _obj.Eval(context).GetObjectValue().CallFunction(_functionName,
                new List<Value>(_arguments.Select(arg => arg.Eval(context))));
        }
    }

    internal class GlobalFunctionCallStatement : Statement
    {
        private readonly List<Statement> _arguments;
        private readonly string _functionName;

        internal GlobalFunctionCallStatement(List<Statement> arguments, string functionName)
        {
            _arguments = arguments;
            _functionName = functionName;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return context.CallGlobalFunction(_functionName, _arguments.Select(arg => arg.Eval(context)).ToList());
        }
    }

    internal class FunctionDefinitionStatement : Statement
    {
        private readonly string _functionName;
        private readonly List<string> _argumentNames;
        private readonly Statement _body;

        internal FunctionDefinitionStatement(string functionName, List<string> argumentNames, Statement body)
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

        internal VariableAssignmentStatement(Statement value, string variableName)
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

        internal VariableReadStatement(string variableName)
        {
            _variableName = variableName;
        }

        internal override Value Eval(InterpretingContext context)
        {
            return context.GetVariable(_variableName);
        }
    }

    internal class NumberStatement : Statement
    {
        private readonly float _number;

        internal NumberStatement(float number)
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

        internal StringStatement(string stringy)
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

        internal BooleanStatement(bool booly)
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

        internal ConditionalStatement(List<Statement> conditionals, List<CompoundStatement> bodies)
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

        internal WhileLoopStatement(Statement condition, CompoundStatement body)
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
                    throw new ArgumentException(
                        $"Possible infinite loop. Loop ran for over {MaxLoopRuns} runs. Aborting.");
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
                catch (LoopBreakException e)
                {
                    return NullValue();
                }
                catch (LoopContinueException e)
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

        internal ForLoopStatement(string iteratorName, Statement iterable, CompoundStatement body)
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
                throw new ArgumentException("Object is not a collection. Cannot be used as for loop iterable.");
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
        internal override Value Eval(InterpretingContext context)
        {
            throw new LoopBreakException();
        }
    }

    internal class LoopContinueStatement : Statement
    {
        internal override Value Eval(InterpretingContext context)
        {
            throw new LoopContinueException();
        }
    }

    internal class NoopStatement : Statement
    {
        internal override Value Eval(InterpretingContext context)
        {
            return NullValue();
        }
    }

    internal class FunctionReturnStatement : Statement
    {
        private readonly Statement returnValue;

        internal FunctionReturnStatement(Statement returnValue)
        {
            this.returnValue = returnValue;
        }

        internal override Value Eval(InterpretingContext context)
        {
            context.ValidateCanReturn();
            throw new FunctionReturnException(returnValue.Eval(context));
        }
    }

    internal class ArrayStatement : Statement
    {
        private readonly List<Statement> _values;

        internal ArrayStatement(List<Statement> values)
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

        internal DictionaryStatement(List<Tuple<Statement, Statement>> values)
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

    internal class ManipulateBeforeReturnStatement : Statement
    {
        private readonly bool _increment;
        private readonly string _identifier;

        public ManipulateBeforeReturnStatement(string identifier, bool increment)
        {
            _identifier = identifier;
            _increment = increment;
        }

        internal override Value Eval(InterpretingContext context)
        {
            var value = context.GetVariable(_identifier);
            context.SetVariable(_identifier,
                _increment
                    ? NumericalValue(value.GetNumericalValue() + 1)
                    : NumericalValue(value.GetNumericalValue() - 1));

            return context.GetVariable(_identifier);
        }
    }
    internal class ManipulateAfterReturnStatement : Statement
    {
        private readonly bool _increment;
        private readonly string _identifier;

        public ManipulateAfterReturnStatement(string identifier, bool increment)
        {
            _identifier = identifier;
            _increment = increment;
        }

        internal override Value Eval(InterpretingContext context)
        {
            var value = context.GetVariable(_identifier);
            context.SetVariable(_identifier,
                _increment
                    ? NumericalValue(value.GetNumericalValue() + 1)
                    : NumericalValue(value.GetNumericalValue() - 1));

            return value;
        }
    }
}