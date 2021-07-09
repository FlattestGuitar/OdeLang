using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using static OdeLang.Language;

namespace OdeLang
{
    public abstract class OdeCollection : OdeObject
    {
        protected OdeCollection(string objectName, List<OdeFunction> functions, Func<string> toStringFunc) :
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
            Func<string> toString = () =>
                "[" + String.Join(",", internalList.Select(val => val.GetStringValue())) + "]";

            Functions = new Dictionary<string, OdeFunction>
            {
                {"append", new OdeFunction("append", new Action<Value>(internalList.Add))},
                {"get", new OdeFunction("get", new Func<int, Value>(i => internalList[i]))},
                {
                    "remove_at",
                    new OdeFunction("remove_at", new Action<int>(internalList.RemoveAt))
                },
                {
                    "insert",
                    new OdeFunction("insert", new Action<int, Value>(internalList.Insert))
                },
                {"clear", new OdeFunction("clear", new Action(internalList.Clear))},
                {"length", new OdeFunction("length", new Func<int>(() => internalList.Count))},
                {ToStringFunctionName, new OdeFunction("to_string", new Func<string>(toString))}
            };
        }

        public List<Value> GetValues()
        {
            return _internalList;
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
        private readonly List<Tuple<string, Value>> _internalList;

        public OdeDictionary(List<Tuple<string, Value>> internalList)
        {
            _internalList = internalList;
            Name = "array";
            Func<string> toString = () => "{" + String.Join(",",
                _internalList
                    .Select(entry => entry.Item1 + ":" + entry.Item2.GetStringValue())) + "}";

            Functions = new Dictionary<string, OdeFunction>
            {
                {
                    "put",
                    new OdeFunction("put",
                        new Action<Value, Value>((key, val) =>
                        {
                            var existing = internalList.Find(tuple => tuple.Item1 == val.GetStringValue());

                            if (existing != null)
                            {
                                internalList.Remove(existing);
                            }
                            
                            internalList.Add(new Tuple<string, Value>(key.GetStringValue(), val));
                        }))
                },
                {
                    "get",
                    new OdeFunction("get", new Func<Value, Value>(val =>
                    {
                        var res = internalList.Find(tuple => tuple.Item1 == val.GetStringValue()).Item2;

                        if (res == null)
                        {
                            return Value.NullValue();
                        }

                        return res;
                    }))
                },
                {
                    "length",
                    new OdeFunction("length", new Func<int>(() => internalList.Count))
                },
                {
                    "clear",
                    new OdeFunction("clear", new Action(() => internalList.Clear()))
                },
                {
                    ToStringFunctionName,
                    new OdeFunction(ToStringFunctionName, new Func<string>(toString))
                },
            };
        }

        public List<Tuple<string, Value>> GetValues()
        {
            return _internalList;
        }

        internal override Value GetAtIndex(int index)
        {
            return Value.StringValue(_internalList[index].Item1);
        }

        internal override int Length()
        {
            return _internalList.Count;
        }
    }
}