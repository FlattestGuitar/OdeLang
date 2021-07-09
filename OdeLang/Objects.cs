using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

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
            return new OdeDictionary(values.Select(tuple => new Tuple<string, Value>(tuple.Item1.GetStringValue(), tuple.Item2)).ToList());
        }
    }
}