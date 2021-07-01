using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OdeLang
{
    internal static class Objects
    {
        
        internal static OdeObject Array(List<Value> values)
        {
            //all lambdas refer to this object
            List<Value> objectValues = new List<Value>(values);

            return new OdeObject(
                "array",
                new List<OdeObject.FunctionDefinition>
                {
                    new OdeObject.FunctionDefinition("append", -1, args => objectValues.AddRange(args)),
                    new OdeObject.FunctionDefinition("get", 1, args => objectValues[(int) args[0].GetNumericalValue()]),
                    new OdeObject.FunctionDefinition("remove_at", 1, args => objectValues.RemoveAt((int) args[0].GetNumericalValue())),
                    new OdeObject.FunctionDefinition("insert", 2, args => objectValues.Insert((int) args[0].GetNumericalValue(), args[1])),
                    new OdeObject.FunctionDefinition("clear", 0, _ => objectValues.Clear()),
                    new OdeObject.FunctionDefinition("length", 0, _ => Value.NumericalValue(objectValues.Count))
                },
                () => "[" + String.Join(",", objectValues.Select(val => val.GetStringValue())) + "]"
            );
        }

        //todo keys are always saved as string values, is that bad?
        internal static OdeObject Dictionary(Dictionary<Value, Value> values)
        {
            Dictionary<string, Value> objectValues = new Dictionary<string, Value>(values.ToDictionary(pair => pair.Key.GetStringValue(), pair => pair.Value));

            return new OdeObject(
                "dictionary",
                new List<OdeObject.FunctionDefinition>
                {
                    new OdeObject.FunctionDefinition("put", 2, args => objectValues.Add(args[0].GetStringValue(), args[1])),
                    new OdeObject.FunctionDefinition("get", 1, args =>
                    {
                        try
                        {
                            return objectValues[args[0].GetStringValue()];
                        }
                        catch (KeyNotFoundException)
                        {
                            return Value.NullValue();
                        }
                    }),
                    new OdeObject.FunctionDefinition("length", 0, _ => Value.NumericalValue(objectValues.Count)),
                    new OdeObject.FunctionDefinition("clear", 0, _ => objectValues.Clear()),
                },
                () => "{" + String.Join(",",
                    objectValues.Select(pair => pair.Key + ":" + pair.Value.GetStringValue())) + "}"
            );
        }
    }
}