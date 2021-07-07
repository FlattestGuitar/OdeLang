﻿using System;
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
        internal static string StringType = "string";
        internal static string NumberType = "number";
        internal static string BoolType = "bool";
        internal static string NullType = "null";

        private static readonly Dictionary<Type, Func<object, Value>> Types = new Dictionary<Type, Func<object, Value>>
        {
            {typeof(Value), obj => (Value) obj},
            {typeof(bool), obj => BoolValue((bool) obj)},
            {typeof(int), obj => NumericalValue((int) obj)},
            {typeof(float), obj => NumericalValue((float) obj)},
            {typeof(double), obj => NumericalValue(Convert.ToSingle(obj))},
            {typeof(string), obj => StringValue((string) obj)}
        };
        
        internal static Value AutoMapToOdeType(object result)
        {
            try
            {
                if (result is OdeObject)
                {
                    return ObjectValue((OdeObject) result);
                }

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