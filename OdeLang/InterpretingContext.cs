using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace OdeLang
{
    public class InterpretingContext
    {
        //todo add support for non-float types
        //todo add support for context-based variable resolution
        private Dictionary<string, Value> _variables = new();
        private Dictionary<string, Func<Value, Value>> _functions = new();

        private string _output = "";

        public InterpretingContext()
        {
            _functions["print"] = number =>
            {
                Print(number.GetStringValue());
                return Value.NullValue();
            };
            _functions["println"] = number =>
            {
                Println(number.GetStringValue());
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

        public Value CallFunction(string name, Value argument)
        {
            try
            {
                return _functions[name].Invoke(argument);
            }
            catch (Exception e) //todo catch better exceptions
            {
                throw new ArgumentException($"No such function {name}");
            }
        }

        private float? Print(string output)
        {
            _output += output;
            return null;
        }

        private float? Println(string output)
        {
            _output += output + "\n";
            return null;
        }

        public string GetOutput()
        {
            return _output;
        }
    }
}