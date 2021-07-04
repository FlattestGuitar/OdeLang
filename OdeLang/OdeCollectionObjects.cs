using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

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
                {"append", new FunctionDefinition("append", -1, args => internalList.AddRange(args))},
                {"get", new FunctionDefinition("get", 1, args => internalList[(int) args[0].GetNumericalValue()])},
                {
                    "remove_at",
                    new FunctionDefinition("remove_at", 1,
                        args => internalList.RemoveAt((int) args[0].GetNumericalValue()))
                },
                {
                    "insert",
                    new FunctionDefinition("insert", 2,
                        args => internalList.Insert((int) args[0].GetNumericalValue(), args[1]))
                },
                {"clear", new FunctionDefinition("clear", 0, _ => internalList.Clear())},
                {"length", new FunctionDefinition("length", 0, _ => Value.NumericalValue(internalList.Count))},
                {"to_string", new FunctionDefinition("to_string", 0, toString)}
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
                    new FunctionDefinition("put", 2, args => internalDictionary.Add(args[0].GetStringValue(), args[1]))
                },
                {
                    "get", 
                    new FunctionDefinition("get", 1, args =>
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
                    new FunctionDefinition("length", 0, _ => Value.NumericalValue(internalDictionary.Count))
                },
                {
                    "clear", 
                    new FunctionDefinition("clear", 0, _ => internalDictionary.Clear())
                },
                {
                    "to_string", 
                    new FunctionDefinition("to_string", 0, toString)
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