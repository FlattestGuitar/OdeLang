using System;
using static OdeLang.Language;

#nullable enable
namespace OdeLang
{
    public class Value
    {
        private readonly float? _number;
        private readonly string? _stringy;
        private readonly bool? _boolean;


        private Value(float? number, string? stringy, bool? boolean)
        {
            _number = number;
            _stringy = stringy;
            _boolean = boolean;
        }

        public static Value NumericalValue(float val)
        {
            return new(val, null, null);
        }

        public static Value StringValue(string val)
        {
            return new(null, val, null);
        }

        public static Value BooleanValue(bool val)
        {
            return new(null, null, val);
        }

        public static Value NullValue()
        {
            return new(null, null, null);
        }

        public float GetNumericalValue()
        {
            if (_number == null)
            {
                throw new ArgumentException("Not a numerical value"); //todo better exceptions
            }

            return _number.Value;
        }

        public string GetStringValue()
        {
            if (_stringy != null)
            {
                return _stringy;
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

            throw new ArgumentException("Not a string"); //todo better exceptions
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

            throw new ArgumentException("Not a boolean"); //todo better exceptions
        }
    }
}