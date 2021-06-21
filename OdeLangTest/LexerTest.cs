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
            const string code = "16.2+52.+51\n61";
            var result = new Lexer(code).LexicalAnalysis();

            var expected = new ArrayList()
            {
                Number(16.2f),
                Plus(),
                Number(52),
                Plus(),
                Number(51),
                Newline(),
                Number(61),
                Newline(),
                EOF()
            };

            CollectionAssert.AreEqual(result, expected);
        }
    }
}