using System;
using System.Collections.Generic;

namespace OdeLang
{
    public class InterpretingContext
    {
        //todo add support for non-float types
        //todo add support for context-based variable resolution
        private Dictionary<string, float> _variables = new();
        private Dictionary<string, Func<float, float?>> _functions = new();

        private string _output = "";

        public InterpretingContext()
        {
            _functions["print"] = number => Print(number.ToString());
            _functions["println"] = number => Println(number.ToString());
        }

        public void SetVariable(string name, float value)
        {
            _variables[name] = value;
        }

        public float GetVariable(string name)
        {
            return _variables[name];
        }

        public float? CallFunction(string name, float argument)
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
            this._output += output;
            return null;
        }

        private float? Println(string output)
        {
            this._output += output + "\n";
            return null;
        }

        public string GetOutput()
        {
            return this._output;
        }
    }
}