using System;

namespace OdeLang
{
    public class Helpers
    {
        public static Func<Value, Value, Value> ArithmeticTokenToOperation(TokenType type)
        {
            if (type == TokenType.Plus)
            {
                return (a, b) =>
                {
                    try
                    {
                        return Value.NumericalValue(a.GetNumericalValue() + b.GetNumericalValue());
                    }
                    catch (ArgumentException e) //todo catching exceptions in normal logic is ugly
                    {
                        //noop
                    }

                    try
                    {
                        return Value.StringValue(a.GetStringValue() + b.GetStringValue());
                    }
                    catch (ArgumentException e)
                    {
                        //todo more verbose errors
                        throw new ArgumentException("Cannot apply '+' operator to types");
                    }
                };
            }


            switch (type)
            {
                case TokenType.Minus:
                    return (a, b) => Value.NumericalValue(a.GetNumericalValue() - b.GetNumericalValue());
                case TokenType.Asterisk:
                    return (a, b) => Value.NumericalValue(a.GetNumericalValue() * b.GetNumericalValue());
                case TokenType.Slash:
                    return (a, b) => Value.NumericalValue(a.GetNumericalValue() / b.GetNumericalValue());
                default:
                    throw new ArgumentException("Can't fetch operation type for non-arithmetic token");
            }
        }
    }
}