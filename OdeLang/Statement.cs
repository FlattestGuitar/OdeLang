#nullable enable
using System;
using System.Collections.Generic;
using static OdeLang.Value;

namespace OdeLang
{
    public abstract class Statement
    {
        public abstract Value Eval(InterpretingContext context);
    }

    //a compound statement is just a parent for a list of statements, it does not have a value
    public class CompoundStatement : Statement
    {
        private readonly List<Statement> _children;

        public CompoundStatement(List<Statement> children)
        {
            _children = children;
        }

        public override Value Eval(InterpretingContext context)
        {
            _children.ForEach(child => child.Eval(context));
            return NullValue();
        }
    }

    //any arithmetic operation on two numbers
    public class BinaryStatement : Statement
    {
        private readonly Statement _left;
        private readonly Statement _right;
        private readonly Func<Value, Value, Value> _operation;

        public BinaryStatement(Statement left, Statement right, Func<Value, Value, Value> operation)
        {
            _left = left;
            _right = right;
            _operation = operation;
        }

        public override Value Eval(InterpretingContext context)
        {
            return _operation.Invoke(_left.Eval(context), _right.Eval(context));
        }
    }

    public class FunctionCallStatement : Statement
    {
        private readonly Statement _argument;
        private readonly string _functionName;

        public FunctionCallStatement(Statement argument, string functionName)
        {
            _argument = argument;
            _functionName = functionName;
        }

        public override Value Eval(InterpretingContext context)
        {
            return context.CallFunction(_functionName, _argument.Eval(context));
        }
    }

    public class VariableAssignmentStatement : Statement
    {
        private readonly Statement _value;
        private readonly string _variableName;

        public VariableAssignmentStatement(Statement value, string variableName)
        {
            _value = value;
            _variableName = variableName;
        }


        public override Value Eval(InterpretingContext context)
        {
            context.SetVariable(_variableName, _value.Eval(context));
            return null;
        }
    }

    public class VariableReadStatement : Statement
    {
        private readonly string _variableName;

        public VariableReadStatement(string variableName)
        {
            _variableName = variableName;
        }

        public override Value Eval(InterpretingContext context)
        {
            return context.GetVariable(_variableName);
        }
    }

    public class NumberStatement : Statement
    {
        private readonly float _number;

        public NumberStatement(float number)
        {
            _number = number;
        }

        public override Value Eval(InterpretingContext context)
        {
            return NumericalValue(_number);
        }
    }
    public class StringStatement : Statement
    {
        private readonly string _string;

        public StringStatement(string stringy)
        {
            _string = stringy;
        }

        public override Value Eval(InterpretingContext context)
        {
            return StringValue(_string);
        }
    }
    public class BooleanStatement : Statement
    {
        private readonly bool _bool;

        public BooleanStatement(bool booly)
        {
            _bool = booly;
        }

        public override Value Eval(InterpretingContext context)
        {
            return BooleanValue(_bool);
        }
    }

    public class ConditionalStatement : Statement
    {
        private readonly Statement _if;
        private readonly CompoundStatement _compoundStatement;

        public ConditionalStatement(Statement @if, CompoundStatement compoundStatement)
        {
            _if = @if;
            _compoundStatement = compoundStatement;
        }


        public override Value Eval(InterpretingContext context)
        {
            if (_if.Eval(context).GetBoolValue())
            {
                return _compoundStatement.Eval(context);
            }

            return NullValue();
        }
    }
}