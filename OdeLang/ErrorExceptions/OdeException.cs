using System;
using static OdeLang.Tokens;

namespace OdeLang.ErrorExceptions
{
    public class OdeException : Exception
    {
        internal OdeException(string message, int line, int column) : base(message)
        {
            Line = line;
            Column = column;
        }
        
        internal OdeException(string message, Statement statement) : base(message)
        {
            Line = statement.Line;
            Column = statement.Column;
        }
        
        internal OdeException(string message, Token token) : base(message)
        {
            Line = token.Line;
            Column = token.Column;
        }

        public int Line { get; }

        public int Column { get; }

        public string displayError()
        {
            return $"{Line}:{Column} | {Message}";
        }
    }
}