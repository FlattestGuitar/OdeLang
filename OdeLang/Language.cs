using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.XPath;
using OdeLang.ErrorExceptions;
using static OdeLang.Value;

namespace OdeLang
{
    public static class Language
    {
        internal static readonly string BooleanTrue = "true";
        internal static readonly string BooleanFalse = "false";
        internal static readonly string ToStringFunctionName = "to_string";
        internal static string CommentStart = "//";
        internal static string StringType = "string";
        internal static string NumberType = "number";
        internal static string BoolType = "bool";
        internal static string NullType = "null";

        private static readonly Dictionary<Type, Func<object, Value>> ToValue =
            new Dictionary<Type, Func<object, Value>>
            {
                {typeof(Value), obj => (Value) obj},
                {typeof(bool), obj => BoolValue((bool) obj)},
                {typeof(int), obj => NumericalValue((int) obj)},
                {typeof(float), obj => NumericalValue((float) obj)},
                {typeof(double), obj => NumericalValue(Convert.ToSingle(obj))},
                {typeof(string), obj => StringValue((string) obj)}
            };

        private static readonly Dictionary<Type, Func<Value, object>> FromValue =
            new Dictionary<Type, Func<Value, object>>
            {
                {typeof(Value), obj => obj},
                {typeof(bool), val => val.GetBoolValue()},
                {typeof(int), val => (int) val.GetNumericalValue()},
                {typeof(float), val => val.GetNumericalValue()},
                {typeof(double), val => (double) val.GetNumericalValue()},
                {typeof(string), val => val.GetStringValue()},
            };

        internal static Value AutoMapToOdeType(object result)
        {
            if (result == null)
            {
                return NullValue();
            }
                
            if (result is OdeObject)
            {
                return ObjectValue((OdeObject) result);
            }

            var type = result.GetType();

            if (ToValue.ContainsKey(type))
            {
                return ToValue[type].Invoke(result);                
            }
            
            if(type.GetGenericTypeDefinition() ==  typeof(List<>))
            {
                IList listObject = (IList) result;
                var res = new List<Value>();
                
                foreach (var o in listObject)
                {
                    var converted = AutoMapToOdeType(o);
                    res.Add(converted);
                }
                
                return ObjectValue(new OdeArray(res));
            }

            throw new ArgumentException($"Wrong object return type: {result.GetType()}"); //ok
        }


        public static object AutoTypeFromValue(Value val, Type type)
        {
            //if it's a 'primitive' just map it from the dictionary
            if (FromValue.ContainsKey(type))
            {
                try
                {
                    return FromValue[type].Invoke(val);
                }
                catch (Exception)
                {
                    throw WrongTypeException(val, type);
                }
            }

            //if it's not a generic that means it's a direct object reference, ezpz
            if (!type.IsGenericType)
            {
                try
                {
                    if (type == val.GetObjectValue().GetType())
                    {
                        return val.GetObjectValue();
                    }

                    throw new ArgumentException(); //ok
                }
                catch (ArgumentException)
                {
                    throw WrongTypeException(val, type);
                }
            }
            
            if (type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var array = (OdeArray) val.GetObjectValue();

                var listParam = type.GetGenericArguments()[0];
                var instance = Activator.CreateInstance(type);
                
                MethodInfo addMethod = type.GetMethod("Add");

                array.GetValues().Select(value => AutoTypeFromValue(value, listParam))
                    .ToList()
                    .ForEach(item => addMethod.Invoke(instance, new[] {item}));

                return instance;
            }

            throw WrongTypeException(val, type);
        }

        private static ArgumentException WrongTypeException(Value val, Type type)
        {
            return new ArgumentException($"Could not use {val.GetStringValue()} as type {type}. This is Krzysiek's fault.");
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