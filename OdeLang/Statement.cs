#nullable enable
using System;
using System.Collections.Generic;

namespace OdeLang
{
    public abstract class Statement
    {
        public abstract object? Eval(InterpretingContext context);
    }

    public class NoOpStatement : Statement
    {
        public override object? Eval(InterpretingContext context)
        {
            return null;
        }
    }

    //a compound statement is just a parent for a list of statements, it does not have a value
    public class CompoundStatement : Statement
    {
        private readonly List<Statement> _children;

        public CompoundStatement(List<Statement> children)
        {
            _children = children;
        }

        public override object? Eval(InterpretingContext context)
        {
            _children.ForEach(child => child.Eval(context));
            return null;
        }
    }
    
    //any arithmetic operation on two numbers
    public class BinaryArithmeticStatement : Statement
    {
        private readonly Statement _left;
        private readonly Statement _right;
        private readonly Func<float, float, float> _operation;

        public BinaryArithmeticStatement(Statement left, Statement right, Func<float, float, float> operation)
        {
            _left = left;
            _right = right;
            _operation = operation;
        }

        public override object? Eval(InterpretingContext context)
        {
            return _operation.Invoke((float) _left.Eval(context), (float)_right.Eval(context));
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

        public override object? Eval(InterpretingContext context)
        {
            return context.CallFunction(_functionName, (float)_argument.Eval(context));
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


        public override object? Eval(InterpretingContext context)
        {
            context.setVariable(_variableName, (float)_value.Eval(context));
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

        public override object? Eval(InterpretingContext context)
        {
            return context.getVariable(_variableName);
        }
    }
    
    public class UnaryArithmeticStatement : Statement
    {

        private readonly Statement _number;
        private readonly bool _negation;


        public UnaryArithmeticStatement(Statement number, bool negation)
        {
            _number = number;
            _negation = negation;
        }

        public override object? Eval(InterpretingContext context)
        {
            var num = (float)_number.Eval(context);
            
            return _negation
                ? -num
                : num;
        }
    }

    public class NumberStatement : Statement
    {
        private readonly float _number;

        public NumberStatement(float number)
        {
            _number = number;
        }

        public override object? Eval(InterpretingContext context)
        {
            return _number;
        }
    }
}