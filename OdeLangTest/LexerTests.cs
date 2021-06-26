using System.Collections;
using NUnit.Framework;
using OdeLang;
using static OdeLang.Tokens;

namespace OdeLangTest
{
    public class LexerTests
    {
        [Test]
        public void BasicArithmetic()
        {
            const string code = "16.2+52.+51\n61";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                Number(16.2f, 0, 0),
                Plus(0, 0),
                Number(52, 0, 0),
                Plus(0, 0),
                Number(51, 0, 0),
                Newline(0, 0),
                Number(61, 0, 0),
                Newline(0, 0),
                Eof(0, 0)
            };

            CollectionAssert.AreEqual(expected, result);
        }


        [Test]
        public void IdentifierTest()
        {
            const string code = "x = 15";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                Identifier("x", 0, 0),
                Assignment(0, 0),
                Number(15, 0, 0),
                Newline(0, 0),
                Eof(0, 0)
            };

            CollectionAssert.AreEqual(expected, result);
        }
        
        [Test]
        public void StringAssignmentTest()
        {
            const string code = "x = \"15\"";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                Identifier("x", 0, 0),
                Assignment(0, 0),
                String("15", 0, 0),
                Newline(0, 0),
                Eof(0, 0)
            };

            CollectionAssert.AreEqual(expected, result);
        }
        
        [Test]
        public void BooleanAssignmentTest()
        {
            const string code = "x = true";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                Identifier("x", 0, 0),
                Assignment(0, 0),
                Boolean(true, 0, 0),
                Newline(0, 0),
                Eof(0, 0)
            };

            CollectionAssert.AreEqual(expected, result);
        }
        
        [Test]
        public void ConditionalStatementTest()
        {
            const string code = 
@"if(true)
  print(""nice"")";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                If(0, 0),
                OpenParenthesis(0, 0),
                Boolean(true, 0, 0),
                CloseParenthesis(0, 0),
                Newline(0, 0),
                Whitespace(0, 0),
                Identifier("print", 0, 0),
                OpenParenthesis(0, 0),
                String("nice", 0, 0),
                CloseParenthesis(0, 0),
                Newline(0, 0),
                Eof(0, 0)
            };

            CollectionAssert.AreEqual(expected, result);
        }
    }
}