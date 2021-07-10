using System;

namespace OdeLang.ErrorExceptions
{
    public class ValueTypeException : Exception
    {
        public ValueTypeException(string expectedType, string actualType)
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        public string ExpectedType { get; }
        public string ActualType { get; }
        
    }
}