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

        public InterpretingContext Run()
        {
            var tokens = LexicalAnalysis();

            var rootStatement = Parsing(tokens);

            var context = Interpretation(rootStatement);

            return context;
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

        private static InterpretingContext Interpretation(Statement rootStatement)
        {
            var context = new InterpretingContext();
            rootStatement.Eval(context);
            return context;
        }
    }
}