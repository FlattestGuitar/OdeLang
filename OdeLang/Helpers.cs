using System;

namespace OdeLang
{
    public class Helpers
    {
        public static Func<float, float, float> arithmeticTokenToOperation(TokenType type)
        {
            switch (type)
            {
                case TokenType.Minus:
                    return (a, b) => a - b;
                case TokenType.Plus:
                    return (a, b) => a + b;
                case TokenType.Asterisk:
                    return (a, b) => a * b;
                case TokenType.Slash:
                    return (a, b) => a / b;
                
                default:
                    throw new ArgumentException("Can't fetch operation type for non-arithmetic token");
            }
        }
    }
}