using static OdeLang.TokenType;

namespace OdeLang
{

    public static class Tokens
    {
        public static Token Plus()
        {
            return new(TokenType.Plus, "");
        }
        
        public static Token Minus()
        {
            return new(TokenType.Minus, "");
        }

        public static Token Number(float value)
        {
            return new(TokenType.Number, value);
        }

        public static Token OpenParenthesis()
        {
            return new(TokenType.OpenParenthesis, "");
        }
        
        public static Token CloseParenthesis()
        {
            return new(TokenType.CloseParenthesis, "");
        }

        public static Token EOF()
        {
            return new(TokenType.EndOfFile, "");
        }

        public static Token Newline()
        {
            return new(TokenType.Newline, "");
        }

        public static Token Whitespace()
        {
            return new(TokenType.Whitespace, "");
        }
        
        public class Token
        {
            private TokenType TokenType;
            private object Value;

            //todo how do I hide this constructor from things outside of the Tokens class
            public Token(TokenType type, object value)
            {
                TokenType = type;
                Value = value;
            }
            public TokenType GetTokenType()
            {
                return TokenType; //todo how the fuck do getters and setters work what the fuck
            }

            public object GetValue()
            {
                return Value;
            }

            public override bool Equals(object? obj)
            {
                if (obj == null || ! this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }

                Token token = (Token) obj;
                return (GetTokenType().Equals(token.GetTokenType())) && (GetValue().Equals(token.GetValue()));
            }

            public override int GetHashCode()
            {
                return (TokenType, Value).GetHashCode();
            }
        }
    }
    
    public enum TokenType
    {
        Plus,
        Minus,
        Number,
        CloseParenthesis,
        OpenParenthesis,
        Newline,
        Whitespace,
        EndOfFile,
    }
}