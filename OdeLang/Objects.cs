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

        //todo keys are always saved as string values, is that bad?
        public static OdeObject Dictionary(Dictionary<Value, Value> values)
        {
            Dictionary<string, Value> objectValues = new(values.ToDictionary(pair => pair.Key.GetStringValue(), pair => pair.Value));

            return new OdeObject(
                "dictionary",
                new List<OdeObject.FunctionDefinition>
                {
                    new("put", 2, args => objectValues.Add(args[0].GetStringValue(), args[1])),
                    new("get", 1, args => objectValues.GetValueOrDefault(args[0].GetStringValue(), Value.NullValue())),
                    new("length", 0, _ => Value.NumericalValue(objectValues.Count)),
                    new("clear", 0, _ => objectValues.Clear()),
                },
                () => "{" + String.Join(",",
                    objectValues.Select(pair => pair.Key + ":" + pair.Value.GetStringValue())) + "}"
            );
        }
    }
}