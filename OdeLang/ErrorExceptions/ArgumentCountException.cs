using System;

namespace OdeLang.ErrorExceptions
{
    public class ArgumentCountException : Exception
    {
        public ArgumentCountException(int required, int actual)
        {
            Required = required;
            Actual = actual;
        }

        public int Required { get; }
        public int Actual { get; }
    }
    
    
}