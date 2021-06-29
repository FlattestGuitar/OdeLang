using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace OdeLang
{
    public class InterpretingContext
    {
        //todo add support for non-float types
        //todo add support for context-based variable resolution
        private Dictionary<string, Value> _variables = new();
        private Dictionary<string, Func<List<Value>, Value>> _functions = new();

        private string _output = "";

        public InterpretingContext()
        {
            _functions["print"] = number =>
            {
                Print(number);
                return Value.NullValue();
            };
            _functions["println"] = number =>
            {
                Print(number);
                PrintNewline();
                return Value.NullValue();
            };
        }

        public void SetVariable(string name, Value value)
        {
            _variables[name] = value;
        }

        public Value GetVariable(string name)
        {
            return _variables[name];
        }

        public Value CallFunction(string name, List<Value> arguments)
        {
            try
            {
                return _functions[name].Invoke(arguments);
            }
            catch (Exception e) //todo catch better exceptions
            {
                throw new ArgumentException($"No such function {name}");
            }
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
    }
}