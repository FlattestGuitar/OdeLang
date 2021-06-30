using System;

namespace OdeLang
{
    internal class FunctionReturnException : Exception
    {
        internal Value ReturnValue { get; }

        internal FunctionReturnException(Value returnValue)
        {
            ReturnValue = returnValue;
        }
    }
}