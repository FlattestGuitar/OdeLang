using System.Collections.Generic;

namespace OdeLang
{
    public class Interpreter
    {
        private string _code;

        public Interpreter(string code)
        {
            _code = code;
        }

        public void Run(InterpretingContext context)
        {
            var tokens = LexicalAnalysis();

            var rootStatement = Parsing(tokens);

            Interpretation(rootStatement, context);
        }

        private List<Tokens.Token> LexicalAnalysis()
        {
            var lexer = new Lexer(_code);
            var tokens = lexer.LexicalAnalysis();
            return tokens;
        }

        private static Statement Parsing(List<Tokens.Token> tokens)
        {
            var parser = new Parser(tokens);
            var rootStatement = parser.Parse();
            return rootStatement;
        }

        private static void Interpretation(Statement rootStatement, InterpretingContext context)
        {
            rootStatement.Eval(context);
        }
    }
}