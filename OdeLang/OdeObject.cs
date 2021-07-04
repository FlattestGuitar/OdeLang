using System;
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
            Functions[ToStringFunctionName] = new FunctionDefinition(ToStringFunctionName, 0,
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

        public class FunctionDefinition
        {
            private static readonly Dictionary<Type, Func<object, Value>> Types = new Dictionary<Type, Func<object, Value>>()
            {
                {typeof(Value), obj => (Value) obj},
                {typeof(bool), obj => BooleanValue((bool) obj)},
                {typeof(int), obj => NumericalValue((int) obj)},
                {typeof(float), obj => NumericalValue((float) obj)},
                {typeof(double), obj => NumericalValue(Convert.ToSingle(obj))},
                {typeof(string), obj => StringValue((string) obj)},
                {typeof(OdeObject), obj => ReferenceValue((OdeObject) obj)}
            };

            internal string Name { get; }
            internal int NumberOfArguments { get; } //-1 for unlimited, like print()
            internal Func<List<Value>, object> Operation { get; }

            public FunctionDefinition(string name, int numberOfArguments, Func<List<Value>, object> operation)
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
                    return null;
                };
            }

            internal Value Eval(List<Value> args)
            {
                if (NumberOfArguments > -1 && args.Count != NumberOfArguments)
                {
                    throw new ArgumentException(
                        $"Wrong number of arguments. {Name} needs exactly {NumberOfArguments} arguments and received {args.Count}!");
                }

                var result = Operation.Invoke(args);

                return AutoMapToOdeType(result);
            }

            private Value AutoMapToOdeType(object result)
            {
                try
                {
                    return Types[result.GetType()].Invoke(result);
                }
                catch
                {
                    return NullValue();
                }
            }
        }

    }
}