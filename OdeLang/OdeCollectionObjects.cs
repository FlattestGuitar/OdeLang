using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace OdeLang
{
    public abstract class OdeCollection : OdeObject
    {
        protected OdeCollection(string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc) : base(objectName, functions, toStringFunc)
        {
        }

        internal abstract Value GetAtIndex(int index);

        internal abstract int Length();
    }

    public class OdeArray : OdeCollection
    {
        private readonly List<Value> _internalList;

        public OdeArray(string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc, List<Value> internalList) : base(objectName, functions, toStringFunc)
        {
            this._internalList = internalList;
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

        public OdeDictionary(string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc, OrderedDictionary internalDictionary) : base(objectName, functions, toStringFunc)
        {
            this._internalDictionary = internalDictionary;
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