using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace OdeLang
{
    internal static class Objects
    {
        
        internal static OdeObject Array(List<Value> values)
        {

            return new OdeArray(
                values
            );
        }

        internal static OdeObject Dictionary(List<Tuple<Value, Value>> values)
        {
            OrderedDictionary objectValues = new OrderedDictionary();
            values.ForEach(tuple => objectValues.Add(tuple.Item1.GetStringValue(), tuple.Item2));

            return new OdeDictionary(objectValues);
        }
    }
}