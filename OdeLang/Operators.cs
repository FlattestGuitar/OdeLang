using System;
using OdeLang.ErrorExceptions;

namespace OdeLang
{
    internal class Operators
    {
        
        internal static Func<Value, Value, Value> PlusOperation = (a, b) =>
        {
            try
            {
                return Value.NumericalValue(a.GetNumericalValue() + b.GetNumericalValue());
            }
            catch (ValueTypeException)
            {
                return Value.StringValue(a.GetStringValue() + b.GetStringValue());
            }
        };
        
        internal static Func<Value, Value, Value> MinusOperation = (a, b) => Value.NumericalValue(a.GetNumericalValue() - b.GetNumericalValue());
        internal static Func<Value, Value, Value> AsteriskOperation = (a, b) => Value.NumericalValue(a.GetNumericalValue() * b.GetNumericalValue());
        internal static Func<Value, Value, Value> SlashOperation = (a, b) => Value.NumericalValue(a.GetNumericalValue() / b.GetNumericalValue());
        internal static Func<Value, Value, Value> ModuloOperation = (a, b) => Value.NumericalValue(a.GetNumericalValue() % b.GetNumericalValue());
        
        

        internal static Func<Value, Value> ArithmeticTokenToUnaryOperation(Tokens.Token token)
        {
            switch(token.TokenType)
            {
                case TokenType.Plus:
                    return value => value;
                case TokenType.Minus:
                    return value => Value.NumericalValue(-value.GetNumericalValue());
                case TokenType.Not:
                    return value => Value.BoolValue(!value.GetBoolValue());
                default:
                    throw new OdeException($"{token.Value} is not a unary operator!", token);
            }
        }
        internal static Func<Value, Value, Value> ArithmeticTokenToOperation(Tokens.Token token)
        {
            if (token.TokenType == TokenType.Plus)
            {
                return PlusOperation;
            }


            switch (token.TokenType)
            {
                case TokenType.Minus:
                    return MinusOperation;
                case TokenType.Asterisk:
                    return AsteriskOperation;
                case TokenType.Slash:
                    return SlashOperation;
                case TokenType.Modulo:
                    return ModuloOperation;
                case TokenType.MoreThan:
                    return (a, b) => Value.BoolValue(a.GetNumericalValue() > b.GetNumericalValue());
                case TokenType.LessThan:
                    return (a, b) => Value.BoolValue(a.GetNumericalValue() < b.GetNumericalValue());
                case TokenType.Equal:
                    return (a, b) => Value.BoolValue(a.LangEquals(b));
                case TokenType.NotEqual:
                    return (a, b) => Value.BoolValue(!a.LangEquals(b));
                case TokenType.And:
                    return (a, b) => Value.BoolValue(a.GetBoolValue() && b.GetBoolValue());
                case TokenType.Or:
                    return (a, b) => Value.BoolValue(a.GetBoolValue() || b.GetBoolValue());
                default:
                    throw new OdeException($"{token.Value} is not a valid binary operator!", token);
            }
        }
    }
}