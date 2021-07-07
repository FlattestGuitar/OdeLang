﻿using System;
using System.Collections.Generic;
using System.Linq;
using static OdeLang.Language;
using static OdeLang.Value;

namespace OdeLang
{
    /// <summary>
    /// OdeLang does not support the creation of custom objects. Objects can only be created by plugging them into
    /// the interpreting context from outside.
    ///
    /// One exception are collections, which behave like objects for all intents and purposes.
    /// </summary>
    public class OdeObject
    {
        public string Name { get; set; }

        protected Dictionary<string, FunctionDefinition> Functions;

        public OdeObject(string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc)
        {
            Name = objectName;
            Functions = functions.ToDictionary(def => def.Name);
            Functions[ToStringFunctionName] = new FunctionDefinition(
                ToStringFunctionName, 
                new List<FunctionDefinition.ArgumentType>(),
                _ => StringValue(toStringFunc.Invoke()));
        }

        internal OdeObject()
        {
        }

        public Value CallFunction(string name, List<Value> args)
        {
            if (!Functions.ContainsKey(name))
            {
                throw new ArgumentException($"Object does not contain function {name}!");
            }

            return Functions[name].Eval(args);
        }

        public string CallToStringFunc()
        {
            return CallFunction(ToStringFunctionName, new List<Value>()).GetStringValue();
        }

    }
}