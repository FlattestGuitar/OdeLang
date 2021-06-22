using static OdeLang.TokenType;

namespace OdeLang
{

    public static class Tokens
    {
        public static Token Plus(int line, int column)
        {
            return new(TokenType.Plus, "+", line, column);
        }
        
        public static Token Minus(int line, int column)
        {
            return new(TokenType.Minus, "-", line, column);
        } 
        
        public static Token Asterisk(int line, int column)
        {
            return new(TokenType.Asterisk, "*", line, column);
        }
        
        public static Token Slash(int line, int column)
        {
            return new(TokenType.Slash, "/", line, column);
        }

        public static Token Number(float value, int line, int column)
        {
            return new(TokenType.Number, value, line, column);
        }

        public static Token OpenParenthesis(int line, int column)
        {
            return new(TokenType.OpenParenthesis, "(", line, column);
        }
        
        public static Token CloseParenthesis(int line, int column)
        {
            return new(TokenType.CloseParenthesis, ")", line, column);
        }

        public static Token EOF(int line, int column)
        {
            return new(TokenType.EndOfFile, "[eof]", line, column);
        }

        public static Token Newline(int line, int column)
        {
            return new(TokenType.Newline, "[newline]", line, column);
        }

        public static Token Whitespace(int line, int column)
        {
            return new(TokenType.Whitespace, "[whitespace]", line, column);
        }
        
        public class Token
        {
            public TokenType TokenType { get; }
            public object Value { get; }
            public int Line { get; }
            public int Column { get; }

            //todo how do I hide this constructor from things outside of the Tokens class
            public Token(TokenType type, object value, int line, int column)
            {
                TokenType = type;
                Value = value;
                Line = line;
                Column = column;
            }
            
            public override bool Equals(object? obj)
            {
                if (obj == null || ! this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }

                Token token = (Token) obj;
                return (TokenType.Equals(token.TokenType)) && (Value.Equals(token.Value));
            }

            public override int GetHashCode()
            {
                return (TokenType: TokenType, Value: Value).GetHashCode();
            }
        }
    }
    
    public enum TokenType
    {
        Plus,
        Minus,
        Asterisk,
        Slash,
        Number,
        CloseParenthesis,
        OpenParenthesis,
        Newline,
        Whitespace,
        EndOfFile,
    }
}