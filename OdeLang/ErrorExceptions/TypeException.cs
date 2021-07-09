using System;

namespace OdeLang.ErrorExceptions
{
    public class TypeException : Exception
    {
        public TypeException(string requiredType, string actualType)
        {
            RequiredType = requiredType;
            ActualType = actualType;
        }

        public string RequiredType { get; }
        public string ActualType { get; }
    }
}