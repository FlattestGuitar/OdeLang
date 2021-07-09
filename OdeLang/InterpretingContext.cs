using System;
using System.Collections.Generic;

namespace OdeLang
{
    /// <summary>
    /// This class is mostly responsible for holding the current interpretation state.
    /// All the variables and the current execution context are referenced here. 
    /// </summary>
    public class InterpretingContext
    {
        private Dictionary<string, Value> _globalVariables = new Dictionary<string, Value>();

        private Stack<Dictionary<string, Value>>
            _functionContextVariables =
                new Stack<Dictionary<string, Value>>(); //only the latest entry is visible at all times

        private Dictionary<string, OdeFunction> _builtInFunctions = new Dictionary<string, OdeFunction>();
        private Dictionary<string, CustomFunction> _userDefinedFunctions = new Dictionary<string, CustomFunction>();
        private List<Dictionary<string, Value>> _loopIterators = new List<Dictionary<string, Value>>();

        private string _output = "";
        private Action<string> _outputHandler;

        public InterpretingContext(Action<string> outputHandler)
        {
            seedDefaultMethods();
            _outputHandler = outputHandler;
        }

        public InterpretingContext()
        {
            seedDefaultMethods();
            _outputHandler = _ => { };
        }

        public void InjectObject(OdeObject obj)
        {
            _globalVariables[obj.Name] = Value.ObjectValue(obj);
        }

        public void InjectGlobalFunction(OdeFunction functionDefinition)
        {
            _builtInFunctions[functionDefinition.Name] = functionDefinition;
        }

        public string GetOutput()
        {
            return _output;
        }

        internal void SetVariable(string name, Value value)
        {
            if (CurrentlyInFunctionContext() && !_globalVariables.ContainsKey(name))
            {
                var contextVars = _functionContextVariables.Peek();
                contextVars[name] = value;
            }
            else
            {
                _globalVariables[name] = value;
            }
        }

        internal Value GetVariable(string name)
        {
            for (var i = _loopIterators.Count; i > 0; i--)
            {
                if (_loopIterators[i - 1].ContainsKey(name))
                {
                    return _loopIterators[i - 1][name];
                }
            }

            if (CurrentlyInFunctionContext())
            {
                var variablesInContext = _functionContextVariables.Peek();
                if (variablesInContext.ContainsKey(name))
                {
                    return variablesInContext[name];
                }
            }

            if (_globalVariables.ContainsKey(name))
            {
                return _globalVariables[name];
            }

            throw new ArgumentException($"Variable undefined: {name}");
        }

        public Value CallGlobalFunction(string name, List<Value> arguments)
        {
            if (_userDefinedFunctions.ContainsKey(name))
            {
                var definedFunc = _userDefinedFunctions[name];
                SeedFunctionArguments(name, definedFunc, arguments);

                Value result = Value.NullValue();

                try
                {
                    definedFunc.Statement.Eval(this);
                }
                catch (FunctionReturnException e)
                {
                    result = e.ReturnValue;
                }

                ClearFunctionArguments();
                return result;
            }

            if (_builtInFunctions.ContainsKey(name))
            {
                return _builtInFunctions[name].Eval(arguments);
            }

            throw new ArgumentException($"No such function {name}");
        }

        internal void ForLoopIteration(int iterationNumber, OdeCollection collection, CompoundStatement body,
            string iteratorName)
        {
            SeedLoopArguments(new Dictionary<string, Value>() {{iteratorName, collection.GetAtIndex(iterationNumber)}});
            body.Eval(this);
            ClearLoopArguments();
        }

        private void SeedFunctionArguments(string name, CustomFunction definedFunc, List<Value> arguments)
        {
            var requiredArgCount = definedFunc.Arguments.Count;

            if (requiredArgCount < arguments.Count)
            {
                throw new ArgumentException($"Too many arguments. {name} needs exactly {requiredArgCount}!");
            }

            if (requiredArgCount > arguments.Count)
            {
                throw new ArgumentException(
                    $"Too few arguments. {name} requires exactly {requiredArgCount}!");
            }

            Dictionary<string, Value> argumentsToAdd = new Dictionary<string, Value>();

            for (var i = 0; i < requiredArgCount; i++)
            {
                argumentsToAdd[definedFunc.Arguments[i]] = arguments[i];
            }

            _functionContextVariables.Push(argumentsToAdd);
        }

        private void ClearFunctionArguments()
        {
            _functionContextVariables.Pop();
        }

        private void SeedLoopArguments(Dictionary<string, Value> args)
        {
            _functionContextVariables.Push(args);
        }

        private void ClearLoopArguments()
        {
            _functionContextVariables.Pop();
        }

        internal void RegisterFunction(string name, List<string> argumentNames, Statement statement)
        {
            if (_builtInFunctions.ContainsKey(name) || _userDefinedFunctions.ContainsKey(name))
            {
                throw new ArgumentException($"Function {name} is already defined!");
            }

            _userDefinedFunctions[name] = new CustomFunction(argumentNames, statement);
        }

        internal void ValidateCanReturn()
        {
            if (!CurrentlyInFunctionContext())
            {
                throw new ArgumentException("Cannot return when not in a function!");
            }
        }

        private void seedDefaultMethods()
        {
            InjectGlobalFunction(new OdeFunction(
                "print",
                new Action<string>(s => Print(s))));

            InjectGlobalFunction(new OdeFunction(
                "println",
                new Action<string>(s =>
                {
                    Print(s);
                    Print("\n");
                })));
        }

        private void Print(string output)
        {
            _output += output;
            _outputHandler(output);
        }

        private bool CurrentlyInFunctionContext()
        {
            return _functionContextVariables.Count > 0;
        }
    }
}