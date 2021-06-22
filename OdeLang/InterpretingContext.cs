using System;
using System.Collections.Generic;
using System.Linq;

namespace OdeLang
{
    public class InterpretingContext
    {
        //todo add support for non-float types
        //todo add support for context-based variable resolution
        private Dictionary<string, float> _variables = new Dictionary<string, float>();

        private string output = "";

        public void setVariable(string name, float value)
        {
            _variables[name] = value;
        }

        public float getVariable(string name)
        {
            return _variables[name];
        }

        public void addOutput(string output)
        {
            this.output += "\n" + output;
        }

        public string getOutput()
        {
            return this.output;
        }
        
    }

}