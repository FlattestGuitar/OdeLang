using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;
using static OdeLang.Tokens;

namespace OdeLangTest
{
    public class Tests
    {

        [Test]
        public void BasicArithmetic()
        {
            const string code = "16+52";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                Number(16),
                Plus(),
                Number(52),
                Newline(),
                EOF()
            };

            CollectionAssert.AreEqual(result, expected);
        }
    }
}