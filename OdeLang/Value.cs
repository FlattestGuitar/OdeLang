using System;
using System.Collections.Generic;
using static OdeLang.Language;

namespace OdeLang
{
    /// <summary>
    /// This class is responsible for holding values both in variables and just ones being passed around during interpretation
    /// Each type of value can be interpreted as other, different types depending on the context
    /// For example, a boolean will be treated as a string if being joined with another string using the + operator
    /// </summary>
    public class Value
    {
        private readonly float? _number;
        private readonly string _string;
        private readonly bool? _boolean;
        private readonly OdeObject _odeObject;

        private Value(float? number, string stringy, bool? boolean, OdeObject odeObject)
        {
            _number = number;
            _string = stringy;
            _boolean = boolean;
            _odeObject = odeObject;
        }

        public static Value NumericalValue(float val)
        {
            return new Value(val, null, null, null);
        }

        public static Value StringValue(string val)
        {
            return new Value(null, val, null, null);
        }

        public static Value BoolValue(bool val)
        {
            return new Value(null, null, val, null);
        }

        public static Value ObjectValue(OdeObject val)
        {
            return new Value(null, null, null, val);
        }

        public static Value NullValue()
        {
            return new Value(null, null, null, null);
        }

        public OdeObject GetObjectValue()
        {
            if (_odeObject != null)
            {
                return _odeObject;
            }

            throw new ArgumentException("Not an object!");
        }

        public float GetNumericalValue()
        {
            if (_number != null)
            {
                return _number.Value;
            }

            if (_boolean != null)
            {
                return _boolean.Value
                    ? 1
                    : 0;
            }

            throw new ArgumentException("Not a numerical value!");
        }

        public string GetStringValue()
        {
            if (_string != null)
            {
                return _string;
            }

            if (_number != null)
            {
                return _number.Value.ToString();
            }

            if (_boolean != null)
            {
                return _boolean.Value
                    ? BooleanTrue
                    : BooleanFalse;
            }

            if (_odeObject != null)
            {
                return _odeObject.CallToStringFunc();
            }

            return "null";
        }

        public bool GetBoolValue()
        {
            if (_boolean != null)
            {
                return _boolean.Value;
            }

            if (_number != null)
            {
                return _number != 0;
            }

            if (_string != null)
            {
                return _string.Length > 0;
            }

            if (_odeObject != null)
            {
                return true;
            }

            return false;
        }

        //how we check equality - this has some consequences
        //if the types of two values is the same we just compare their raw value, nothing interesting
        //if the types differ we will compare their numerical representations:
        //true == 1
        //false == 0
        //true != 2
        //"any string" != false | true | any #
        internal bool LangEquals(Value other)
        {
            if (_string != null && other._string != null)
            {
                return _string == other._string;
            }

            if (_number != null && other._number != null)
            {
                return _number == other._number;
            }

            if (_boolean != null && other._boolean != null)
            {
                return _boolean == other._boolean;
            }

            //is this really smart or really dumb?
            if (_odeObject != null && other._odeObject != null)
            {
                return _odeObject.CallToStringFunc() == other._odeObject.CallToStringFunc();
            }

            return GetNumericalValue() == other.GetNumericalValue();
        }
    }
}