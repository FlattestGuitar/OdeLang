﻿using System;
using System.Collections.Generic;
using System.Linq;
using static OdeLang.Language;

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
        private readonly string _objectName;
        private readonly Dictionary<string, FunctionDefinition> _functions;

        public OdeObject(string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc)
        {
            _objectName = objectName;
            _functions = functions.ToDictionary(def => def.Name);
            _functions[ToStringFunctionName] = new FunctionDefinition(ToStringFunctionName, 0,
                _ => Value.StringValue(toStringFunc.Invoke()));
        }

        public Value CallFunction(string name, List<Value> args)
        {
            if (!_functions.ContainsKey(name))
            {
                throw new ArgumentException($"Object does not contain function {name}!");
            }

            return _functions[name].Eval(args);
        }

        public string CallToStringFunc()
        {
            return CallFunction(ToStringFunctionName, new List<Value>()).GetStringValue();
        }

        public class FunctionDefinition
        {
            public string Name { get; }
            public int NumberOfArguments { get; } //-1 for unlimited, like print()
            public Func<List<Value>, Value> Operation { get; }

            public FunctionDefinition(string name, int numberOfArguments, Func<List<Value>, Value> operation)
            {
                Name = name;
                NumberOfArguments = numberOfArguments;
                Operation = operation;
            }

            public FunctionDefinition(string name, int numberOfArguments, Action<List<Value>> operation)
            {
                Name = name;
                NumberOfArguments = numberOfArguments;
                Operation = args =>
                {
                    operation.Invoke(args);
                    return Value.NullValue();
                };
            }

            public Value Eval(List<Value> args)
            {
                if (NumberOfArguments > -1 && args.Count != NumberOfArguments)
                {
                    throw new ArgumentException(
                        $"Wrong number of arguments. {Name} needs exactly {NumberOfArguments} arguments and received {args.Count}!");
                }

                return Operation.Invoke(args);
            }
        }

    }
}