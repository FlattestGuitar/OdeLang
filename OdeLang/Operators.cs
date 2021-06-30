using System;

namespace OdeLang
{
    public class Operators
    {

        public static Func<Value, Value> ArithmeticTokenToUnaryOperation(TokenType type)
        {
            switch(type)
            {
                case TokenType.Plus:
                    return value => value;
                case TokenType.Minus:
                    return value => Value.NumericalValue(-value.GetNumericalValue());
                case TokenType.Not:
                    return value => Value.BooleanValue(!value.GetBoolValue());
                default:
                    throw new ArgumentException($"Cannot apply {type} operator to type");
            }
        }
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
                case TokenType.Modulo:
                    return (a, b) => Value.NumericalValue(a.GetNumericalValue() % b.GetNumericalValue());
                case TokenType.MoreThan:
                    return (a, b) => Value.BooleanValue(a.GetNumericalValue() > b.GetNumericalValue());
                case TokenType.LessThan:
                    return (a, b) => Value.BooleanValue(a.GetNumericalValue() < b.GetNumericalValue());
                case TokenType.Equal:
                    return (a, b) => Value.BooleanValue(a.LangEquals(b));
                case TokenType.NotEqual:
                    return (a, b) => Value.BooleanValue(!a.LangEquals(b));
                case TokenType.And:
                    return (a, b) => Value.BooleanValue(a.GetBoolValue() && b.GetBoolValue());
                case TokenType.Or:
                    return (a, b) => Value.BooleanValue(a.GetBoolValue() || b.GetBoolValue());
                default:
                    throw new ArgumentException("Can't fetch operation type for non-arithmetic token");
            }
        }
    }
}