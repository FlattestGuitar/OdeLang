﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

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
            _functionContextVariables = new Stack<Dictionary<string, Value>>(); //only the latest entry is visible at all times

        private Dictionary<string, Func<List<Value>, Value>> _builtInFunctions = new Dictionary<string, Func<List<Value>, Value>>();
        private Dictionary<string, CustomFunction> _userDefinedFunctions = new Dictionary<string, CustomFunction>();

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

        public void InjectObject(OdeObject obj)
        {
            _globalVariables[obj.Name] = Value.ReferenceValue(obj);
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

        internal Value CallGlobalFunction(string name, List<Value> arguments)
        {
            if (_userDefinedFunctions.ContainsKey(name))
            {
                var definedFunc = _userDefinedFunctions[name];
                SeedArguments(name, definedFunc, arguments);

                Value result = Value.NullValue();

                try
                {
                    definedFunc.Statement.Eval(this);
                }
                catch (FunctionReturnException e)
                {
                    result = e.ReturnValue;
                }
                
                ClearArguments();
                return result;
            }

            if (_builtInFunctions.ContainsKey(name))
            {
                return _builtInFunctions[name].Invoke(arguments);
            }

            throw new ArgumentException($"No such function {name}");
        }

        private void SeedArguments(string name, CustomFunction definedFunc, List<Value> arguments)
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

        private void ClearArguments()
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

        internal void ValidateCanReturn()
        {
            if (!CurrentlyInFunctionContext())
            {
                throw new ArgumentException("Cannot return when not in a function!");
            }
        }
    }
}