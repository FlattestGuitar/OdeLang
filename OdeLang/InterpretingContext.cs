using System;
using System.Collections.Generic;
using System.Linq;

namespace OdeLang
{
    public class InterpretingContext
    {
        //todo add support for non-float types
        //todo add support for context-based variable resolution
        private Dictionary<string, float> _variables = new();
        private Dictionary<string, Func<float, float?>> _functions = new();
        
        private string output = "";

        public InterpretingContext()
        {
            _functions["print"] = number => print(number.ToString());
            _functions["println"] = number => println(number.ToString());
        }

        public void setVariable(string name, float value)
        {
            _variables[name] = value;
        }

        public float getVariable(string name)
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

        private float? print(string output)
        {
            this.output += output;
            return null;
        }
        
        private float? println(string output)
        {
            this.output += output + "\n";
            return null;
        }

        public string getOutput()
        {
            return this.output;
        }
        
    }

}