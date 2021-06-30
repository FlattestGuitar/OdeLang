namespace OdeLang
{
    /// <summary>
    /// These tokens are output by the lexer. They are used by the parser to interpret the code.
    /// </summary>
    internal static class Tokens
    {
        internal static Token Plus(int line, int column)
        {
            return new(TokenType.Plus, "+", line, column);
        }

        internal static Token Minus(int line, int column)
        {
            return new(TokenType.Minus, "-", line, column);
        }

        internal static Token Asterisk(int line, int column)
        {
            return new(TokenType.Asterisk, "*", line, column);
        } 
        
        internal static Token MoreThan(int line, int column)
        {
            return new(TokenType.MoreThan, ">", line, column);
        }
        
        internal static Token LessThan(int line, int column)
        {
            return new(TokenType.LessThan, "<", line, column);
        }
        
        internal static Token NotEqual(int line, int column)
        {
            return new(TokenType.NotEqual, "!=", line, column);
        }
        
        internal static Token Equal(int line, int column)
        {
            return new(TokenType.Equal, "==", line, column);
        }
        
        internal static Token Not(int line, int column)
        {
            return new(TokenType.Not, "!", line, column);
        }

        internal static Token Or(int line, int column)
        {
            return new(TokenType.Or, "or", line, column);
        }
        
        internal static Token And(int line, int column)
        {
            return new(TokenType.And, "and", line, column);
        }

        internal static Token Slash(int line, int column)
        {
            return new(TokenType.Slash, "/", line, column);
        }
        
        internal static Token Modulo(int line, int column)
        {
            return new(TokenType.Modulo, "%", line, column);
        }

        internal static Token Number(float value, int line, int column)
        {
            return new(TokenType.Number, value, line, column);
        }

        internal static Token If(int line, int column)
        {
            return new(TokenType.If, "if", line, column);
        }

        internal static Token While(int line, int column)
        {
            return new(TokenType.While, "while", line, column);
        }

        internal static Token Break(int line, int column)
        {
            return new(TokenType.Break, "break", line, column);
        }

        internal static Token Def(int line, int column)
        {
            return new(TokenType.Def, "def", line, column);
        }

        internal static Token Continue(int line, int column)
        {
            return new(TokenType.Continue, "continue", line, column);
        }

        internal static Token OpenParenthesis(int line, int column)
        {
            return new(TokenType.OpenParenthesis, "(", line, column);
        }

        internal static Token CloseParenthesis(int line, int column)
        {
            return new(TokenType.CloseParenthesis, ")", line, column);
        }
        
        internal static Token OpenSquareBracket(int line, int column)
        {
            return new(TokenType.OpenSquareBracket, "[", line, column);
        }

        internal static Token ClosedSquareBracket(int line, int column)
        {
            return new(TokenType.ClosedSquareBracket, "]", line, column);
        }

        internal static Token OpenCurlyBracket(int line, int column)
        {
            return new(TokenType.OpenCurlyBracket, "{", line, column);
        }

        internal static Token ClosedCurlyBracket(int line, int column)
        {
            return new(TokenType.ClosedCurlyBracket, "}", line, column);
        }
        
        internal static Token Comma(int line, int column)
        {
            return new(TokenType.Comma, ",", line, column);
        }   
        
        internal static Token Period(int line, int column)
        {
            return new(TokenType.Period, ".", line, column);
        }   
        
        internal static Token Colon(int line, int column)
        {
            return new(TokenType.Colon, ":", line, column);
        }   
        
        internal static Token Return(int line, int column)
        {
            return new(TokenType.Return, "return", line, column);
        }

        internal static Token Identifier(string id, int line, int column)
        {
            return new(TokenType.Identifier, id, line, column);
        }

        internal static Token Assignment(int line, int column)
        {
            return new(TokenType.Assignment, "=", line, column);
        }

        internal static Token String(string value, int line, int column)
        {
            return new(TokenType.String, value, line, column);
        }

        internal static Token Boolean(bool value, int line, int column)
        {
            return value
                ? new Token(TokenType.Boolean, Language.BooleanTrue, line, column)
                : new Token(TokenType.Boolean, Language.BooleanFalse, line, column);
        }

        internal static Token Eof(int line, int column)
        {
            return new(TokenType.EndOfFile, "[eof]", line, column);
        }

        internal static Token Newline(int line, int column)
        {
            return new(TokenType.Newline, "[newline]", line, column);
        }

        internal static Token Whitespace(int line, int column)
        {
            return new(TokenType.Whitespace, "[whitespace]", line, column);
        }

        internal class Token
        {
            internal TokenType TokenType { get; }
            internal object Value { get; }
            internal int Line { get; }
            internal int Column { get; }

            //todo how do I hide this constructor from things outside of the Tokens class
            internal Token(TokenType type, object value, int line, int column)
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

    internal enum TokenType
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
        Not,
        NotEqual,
        Equal,
        Or,
        And,
        Break,
        Continue,
        CloseParenthesis,
        OpenParenthesis,
        OpenSquareBracket,
        ClosedSquareBracket,
        OpenCurlyBracket,
        ClosedCurlyBracket,
        Newline,
        Whitespace,
        EndOfFile,
        If,
        While,
        Comma,
        Period,
        Colon,
        Def,
        Return
    }
}