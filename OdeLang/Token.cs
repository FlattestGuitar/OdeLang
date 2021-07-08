namespace OdeLang
{
    /// <summary>
    /// These tokens are output by the lexer. They are used by the parser to interpret the code.
    /// </summary>
    internal static class Tokens
    {
        internal static Token Plus(int line, int column)
        {
            return new Token(TokenType.Plus, "+", line, column);
        }

        internal static Token Minus(int line, int column)
        {
            return new Token(TokenType.Minus, "-", line, column);
        }

        internal static Token Asterisk(int line, int column)
        {
            return new Token(TokenType.Asterisk, "*", line, column);
        } 
        
        internal static Token MoreThan(int line, int column)
        {
            return new Token(TokenType.MoreThan, ">", line, column);
        }
        
        internal static Token LessThan(int line, int column)
        {
            return new Token(TokenType.LessThan, "<", line, column);
        }
        
        internal static Token NotEqual(int line, int column)
        {
            return new Token(TokenType.NotEqual, "!=", line, column);
        }
        
        internal static Token Equal(int line, int column)
        {
            return new Token(TokenType.Equal, "==", line, column);
        }
        
        internal static Token Not(int line, int column)
        {
            return new Token(TokenType.Not, "!", line, column);
        }

        internal static Token Or(int line, int column)
        {
            return new Token(TokenType.Or, "or", line, column);
        }
        
        internal static Token And(int line, int column)
        {
            return new Token(TokenType.And, "and", line, column);
        }

        internal static Token Slash(int line, int column)
        {
            return new Token(TokenType.Slash, "/", line, column);
        }
        
        internal static Token Modulo(int line, int column)
        {
            return new Token(TokenType.Modulo, "%", line, column);
        }
        
        internal static Token PlusAssignment(int line, int column)
        {
            return new Token(TokenType.PlusAssignment, "+=", line, column);
        }
        internal static Token MinusAssignment(int line, int column)
        {
            return new Token(TokenType.MinusAssignment, "-=", line, column);
        }
        internal static Token AsteriskAssignment(int line, int column)
        {
            return new Token(TokenType.AsteriskAssignment, "*=", line, column);
        }
        internal static Token SlashAssignment(int line, int column)
        {
            return new Token(TokenType.SlashAssignment, "/=", line, column);
        }
        internal static Token ModuloAssignment(int line, int column)
        {
            return new Token(TokenType.ModuloAssignment, "%=", line, column);
        }
        internal static Token Increment(int line, int column)
        {
            return new Token(TokenType.Increment, "++", line, column);
        }
        internal static Token Decrement(int line, int column)
        {
            return new Token(TokenType.Decrement, "--", line, column);
        }

        internal static Token Number(float value, int line, int column)
        {
            return new Token(TokenType.Number, value, line, column);
        }
        internal static Token If(int line, int column)
        {
            return new Token(TokenType.If, "if", line, column);
        }
        
        internal static Token Elif(int line, int column)
        {
            return new Token(TokenType.Elif, "elif", line, column);
        }
        
        internal static Token Else(int line, int column)
        {
            return new Token(TokenType.Else, "else", line, column);
        }

        internal static Token While(int line, int column)
        {
            return new Token(TokenType.While, "while", line, column);
        }
        
        internal static Token For(int line, int column)
        {
            return new Token(TokenType.For, "for", line, column);
        }
        
        internal static Token In(int line, int column)
        {
            return new Token(TokenType.In, "in", line, column);
        }

        internal static Token Break(int line, int column)
        {
            return new Token(TokenType.Break, "break", line, column);
        }

        internal static Token Fn(int line, int column)
        {
            return new Token(TokenType.Fn, "fn", line, column);
        }

        internal static Token Continue(int line, int column)
        {
            return new Token(TokenType.Continue, "continue", line, column);
        }

        internal static Token OpenParenthesis(int line, int column)
        {
            return new Token(TokenType.OpenParenthesis, "(", line, column);
        }

        internal static Token CloseParenthesis(int line, int column)
        {
            return new Token(TokenType.CloseParenthesis, ")", line, column);
        }
        
        internal static Token OpenSquareBracket(int line, int column)
        {
            return new Token(TokenType.OpenSquareBracket, "[", line, column);
        }

        internal static Token ClosedSquareBracket(int line, int column)
        {
            return new Token(TokenType.ClosedSquareBracket, "]", line, column);
        }

        internal static Token OpenCurlyBracket(int line, int column)
        {
            return new Token(TokenType.OpenCurlyBracket, "{", line, column);
        }

        internal static Token ClosedCurlyBracket(int line, int column)
        {
            return new Token(TokenType.ClosedCurlyBracket, "}", line, column);
        }
        
        internal static Token Comma(int line, int column)
        {
            return new Token(TokenType.Comma, ",", line, column);
        }   
        
        internal static Token Period(int line, int column)
        {
            return new Token(TokenType.Period, ".", line, column);
        }   
        
        internal static Token Colon(int line, int column)
        {
            return new Token(TokenType.Colon, ":", line, column);
        }   
        
        internal static Token Return(int line, int column)
        {
            return new Token(TokenType.Return, "return", line, column);
        }

        internal static Token Identifier(string id, int line, int column)
        {
            return new Token(TokenType.Identifier, id, line, column);
        }

        internal static Token Assignment(int line, int column)
        {
            return new Token(TokenType.Assignment, "=", line, column);
        }

        internal static Token String(string value, int line, int column)
        {
            return new Token(TokenType.String, value, line, column);
        }

        internal static Token Boolean(bool value, int line, int column)
        {
            return value
                ? new Token(TokenType.Boolean, Language.BooleanTrue, line, column)
                : new Token(TokenType.Boolean, Language.BooleanFalse, line, column);
        }

        internal static Token Eof(int line, int column)
        {
            return new Token(TokenType.EndOfFile, "[eof]", line, column);
        }

        internal static Token Newline(int line, int column)
        {
            return new Token(TokenType.Newline, "[newline]", line, column);
        }

        internal static Token Whitespace(int line, int column)
        {
            return new Token(TokenType.Whitespace, "[whitespace]", line, column);
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

            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }

                Token token = (Token) obj;
                return TokenType.Equals(token.TokenType) && Value.Equals(token.Value);
            }

            public override int GetHashCode()
            {
                return (TokenType, Value).GetHashCode();
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
        PlusAssignment,
        MinusAssignment,
        AsteriskAssignment,
        SlashAssignment,
        ModuloAssignment,
        Increment,
        Decrement,
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
        Elif,
        Else,
        While,
        For,
        In,
        Comma,
        Period,
        Colon,
        Fn,
        Return
    }
}