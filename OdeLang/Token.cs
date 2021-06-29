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
        
        public static Token MoreThan(int line, int column)
        {
            return new(TokenType.MoreThan, ">", line, column);
        }
        
        public static Token LessThan(int line, int column)
        {
            return new(TokenType.LessThan, "<", line, column);
        }
        
        public static Token NotEqual(int line, int column)
        {
            return new(TokenType.NotEqual, "!=", line, column);
        }
        
        public static Token Equal(int line, int column)
        {
            return new(TokenType.Equal, "==", line, column);
        }
        
        public static Token Or(int line, int column)
        {
            return new(TokenType.Or, "or", line, column);
        }
        
        public static Token And(int line, int column)
        {
            return new(TokenType.And, "and", line, column);
        }

        public static Token Slash(int line, int column)
        {
            return new(TokenType.Slash, "/", line, column);
        }
        
        public static Token Modulo(int line, int column)
        {
            return new(TokenType.Modulo, "%", line, column);
        }

        public static Token Number(float value, int line, int column)
        {
            return new(TokenType.Number, value, line, column);
        }

        public static Token OpenParenthesis(int line, int column)
        {
            return new(TokenType.OpenParenthesis, "(", line, column);
        }

        public static Token If(int line, int column)
        {
            return new(TokenType.If, "if", line, column);
        }

        public static Token CloseParenthesis(int line, int column)
        {
            return new(TokenType.CloseParenthesis, ")", line, column);
        }

        public static Token Identifier(string id, int line, int column)
        {
            return new(TokenType.Identifier, id, line, column);
        }

        public static Token Assignment(int line, int column)
        {
            return new(TokenType.Assignment, "=", line, column);
        }

        public static Token String(string value, int line, int column)
        {
            return new(TokenType.String, value, line, column);
        }

        public static Token Boolean(bool value, int line, int column)
        {
            return value
                ? new Token(TokenType.Boolean, Language.BooleanTrue, line, column)
                : new Token(TokenType.Boolean, Language.BooleanFalse, line, column);
        }

        public static Token Eof(int line, int column)
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
                if (obj == null || !GetType().Equals(obj.GetType()))
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
        Modulo,
        Number,
        Identifier,
        String,
        Boolean,
        Assignment,
        MoreThan,
        LessThan,
        NotEqual,
        Equal,
        Or,
        And,
        CloseParenthesis,
        OpenParenthesis,
        Newline,
        Whitespace,
        EndOfFile,
        If
    }
}