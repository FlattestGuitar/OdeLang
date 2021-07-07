using System;
using System.Collections.Generic;
using static OdeLang.Value;

namespace OdeLang
{
    internal static class Language
    {
        internal static readonly string BooleanTrue = "true";
        internal static readonly string BooleanFalse = "false";
        internal static readonly string ToStringFunctionName = "to_string";
        internal static string CommentStart = "//";
        
        private static readonly Dictionary<Type, Func<object, Value>> Types = new Dictionary<Type, Func<object, Value>>
        {
            {typeof(Value), obj => (Value) obj},
            {typeof(bool), obj => BoolValue((bool) obj)},
            {typeof(int), obj => NumericalValue((int) obj)},
            {typeof(float), obj => NumericalValue((float) obj)},
            {typeof(double), obj => NumericalValue(Convert.ToSingle(obj))},
            {typeof(string), obj => StringValue((string) obj)},
            {typeof(OdeObject), obj => ObjectValue((OdeObject) obj)}
        };
        
        internal static Value AutoMapToOdeType(object result)
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

        internal static Func<List<Value>, Value> WrapFunctionWithTypeMapping(Func<List<Value>, object> function)
        {
            return args =>
            {
                var result = function.Invoke(args);
                return AutoMapToOdeType(result);
            };
        }
    }
}