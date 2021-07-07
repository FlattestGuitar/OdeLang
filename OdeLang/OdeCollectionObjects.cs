using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using static OdeLang.FunctionDefinition;
using static OdeLang.Language;

namespace OdeLang
{
    public abstract class OdeCollection : OdeObject
    {
        protected OdeCollection(string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc) :
            base(objectName, functions, toStringFunc)
        {
        }

        internal OdeCollection()
        {
        }

        internal abstract Value GetAtIndex(int index);

        internal abstract int Length();
    }

    public class OdeArray : OdeCollection
    {
        private readonly List<Value> _internalList;

        public OdeArray(List<Value> internalList)
        {
            _internalList = internalList;
            Name = "array";
            Func<List<Value>, string> toString = _ =>
                "[" + String.Join(",", internalList.Select(val => val.GetStringValue())) + "]";

            Functions = new Dictionary<string, FunctionDefinition>
            {
                {"append", new FunctionDefinition("append", new List<ArgumentType>{AnyArgument()}, args => internalList.Add(args[0]))},
                {"get", new FunctionDefinition("get", new List<ArgumentType>{NumericalArgument()}, args => internalList[(int) args[0].GetNumericalValue()])},
                {
                    "remove_at",
                    new FunctionDefinition("remove_at", new List<ArgumentType>{NumericalArgument()},
                        args => internalList.RemoveAt((int) args[0].GetNumericalValue()))
                },
                {
                    "insert",
                    new FunctionDefinition("insert", new List<ArgumentType>{NumericalArgument(), AnyArgument()},
                        args => internalList.Insert((int) args[0].GetNumericalValue(), args[1]))
                },
                {"clear", new FunctionDefinition("clear", new List<ArgumentType>(), _ => internalList.Clear())},
                {"length", new FunctionDefinition("length", new List<ArgumentType>(), _ => Value.NumericalValue(internalList.Count))},
                {ToStringFunctionName, new FunctionDefinition("to_string", new List<ArgumentType>(), toString)}
            };
        }

        internal override Value GetAtIndex(int index)
        {
            return _internalList[index];
        }

        internal override int Length()
        {
            return _internalList.Count;
        }
    }

    public class OdeDictionary : OdeCollection
    {
        private readonly OrderedDictionary _internalDictionary;

        public OdeDictionary(OrderedDictionary internalDictionary)
        {
            _internalDictionary = internalDictionary;
            Name = "array";
            Func<List<Value>, string> toString = _ => "{" + String.Join(",",
                _internalDictionary.Keys.Cast<string>()
                    .Select(key => key + ":" + ((Value) _internalDictionary[key]).GetStringValue())) + "}";

            Functions = new Dictionary<string, FunctionDefinition>
            {
                {
                    "put",
                    new FunctionDefinition("put", new List<ArgumentType> {StringArgument(), AnyArgument()}, args => internalDictionary.Add(args[0].GetStringValue(), args[1]))
                },
                {
                    "get", 
                    new FunctionDefinition("get", new List<ArgumentType> {StringArgument()}, args =>
                    {
                        try
                        {
                            return internalDictionary[args[0].GetStringValue()];
                        }
                        catch (KeyNotFoundException)
                        {
                            return Value.NullValue();
                        }
                    })
                },
                {
                    "length", 
                    new FunctionDefinition("length", new List<ArgumentType>(), _ => Value.NumericalValue(internalDictionary.Count))
                },
                {
                    "clear", 
                    new FunctionDefinition("clear", new List<ArgumentType>(), _ => internalDictionary.Clear())
                },
                {
                    "to_string", 
                    new FunctionDefinition(ToStringFunctionName, new List<ArgumentType>(), toString)
                },
            };
        }

        internal override Value GetAtIndex(int index)
        {
            return Value.StringValue(_internalDictionary.Keys.Cast<string>().ElementAt(index));
        }

        internal override int Length()
        {
            return _internalDictionary.Count;
        }
    }
}