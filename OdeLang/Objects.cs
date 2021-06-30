using System;
using System.Collections.Generic;
using System.Linq;

namespace OdeLang
{
    public static class Objects
    {
        
        public static OdeObject Array(List<Value> values)
        {
            //all lambdas refer to this object
            List<Value> objectValues = new(values);

            return new OdeObject(
                "array",
                new List<OdeObject.FunctionDefinition>
                {
                    new("append", -1, args => objectValues.AddRange(args)),
                    new("get", 1, args => objectValues[(int) args[0].GetNumericalValue()]),
                    new("remove_at", 1, args => objectValues.RemoveAt((int) args[0].GetNumericalValue())),
                    new("insert", 2, args => objectValues.Insert((int) args[0].GetNumericalValue(), args[1])),
                    new("clear", 0, _ => objectValues.Clear()),
                    new("length", 0, _ => Value.NumericalValue(objectValues.Count))
                },
                () => "[" + String.Join(",", objectValues.Select(val => val.GetStringValue())) + "]"
            );
        }
    }
}