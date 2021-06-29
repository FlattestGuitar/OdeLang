using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace OdeLang
{
    public class InterpretingContext
    {
        private Dictionary<string, Value> _globalVariables = new();

        private Stack<Dictionary<string, Value>>
            _functionContextVariables = new(); //only the latest entry is visible at all times

        private Dictionary<string, Func<List<Value>, Value>> _builtInFunctions = new();
        private Dictionary<string, Function> _userDefinedFunctions = new();

        private string _output = "";

        public InterpretingContext()
        {
            _builtInFunctions["print"] = number =>
            {
                Print(number);
                return Value.NullValue();
            };
            _builtInFunctions["println"] = number =>
            {
                Print(number);
                PrintNewline();
                return Value.NullValue();
            };
        }


        public void SetVariable(string name, Value value)
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

        public Value GetVariable(string name)
        {
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

        public Value CallFunction(string name, List<Value> arguments)
        {
            if (_userDefinedFunctions.ContainsKey(name))
            {
                var definedFunc = _userDefinedFunctions[name];
                SeedArguments(name, definedFunc, arguments);
                var result = definedFunc.Statement.Eval(this);
                ClearArguments();
                return result;
            }

            if (_builtInFunctions.ContainsKey(name))
            {
                return _builtInFunctions[name].Invoke(arguments);
            }

            throw new ArgumentException($"No such function {name}");
        }

        private void SeedArguments(string name, Function definedFunc, List<Value> arguments)
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

            Dictionary<string, Value> argumentsToAdd = new();

            for (var i = 0; i < requiredArgCount; i++)
            {
                argumentsToAdd[definedFunc.Arguments[i]] = arguments[i];
            }

            _functionContextVariables.Push(argumentsToAdd);
        }

        private void ClearArguments()
        {
            _functionContextVariables.Pop();
        }

        public void RegisterFunction(string name, List<string> argumentNames, Statement statement)
        {
            if (_builtInFunctions.ContainsKey(name) || _userDefinedFunctions.ContainsKey(name))
            {
                throw new ArgumentException($"Function {name} is already defined!");
            }

            _userDefinedFunctions[name] = new Function(argumentNames, statement);
        }

        private void Print(List<Value> output)
        {
            _output += String.Join(" ", output.Select(val => val.GetStringValue()));
        }

        private void PrintNewline()
        {
            _output += "\n";
        }

        public string GetOutput()
        {
            return _output;
        }

        private bool CurrentlyInFunctionContext()
        {
            return _functionContextVariables.Count > 0;
        }
    }
}