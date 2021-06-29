using System;

namespace OdeLang
{
    public class FunctionReturnException : Exception
    {
        public Value ReturnValue { get; }

        public FunctionReturnException(Value returnValue)
        {
            ReturnValue = returnValue;
        }
    }
}