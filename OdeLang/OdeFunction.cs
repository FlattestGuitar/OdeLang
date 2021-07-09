using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OdeLang.ErrorExceptions;
using static OdeLang.Language;

namespace OdeLang
{
    public class OdeFunction
    {
        public string Name { get; }
        public Delegate Function { get; }

        public OdeFunction(string name, Delegate function)
        {
            Function = function;
            Name = name;
        }

        public Value Eval(List<Value> args)
        {
            var requiredArgCount = Function.GetMethodInfo().GetParameters().Length;
            if (args.Count != requiredArgCount)
            {
                throw new ArgumentException($"Function {Name} called with the wrong number of arguments. Required: {requiredArgCount}, received: {args.Count}");
            }

            return AutoMapToOdeType(Function.DynamicInvoke(ForgeArguments(args)));
        }

        private object[] ForgeArguments(List<Value> args)
        {
            ArrayList res = new ArrayList();

            var parameters = Function.Method.GetParameters();

            if (parameters.Length != args.Count)
            {
                throw new ArgumentException("Parameter count not equal");
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                res.Add(AutoTypeFromValue(args[i], parameters[i].ParameterType));
            }
            
            return res.ToArray();
        }

    }
}