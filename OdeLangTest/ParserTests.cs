using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;
using static OdeLang.Tokens;

namespace OdeLangTest
{
    public class ParserTests
    {

        [Test]
        public void BasicArithmetic()
        {
            List<Token> input = new List<Token>()
            {
                Number(61),
                Plus(),
                Number(21)
            };
            
            var result = new Parser(input).Parse();

            Assert.AreEqual(result.Eval(new InterpretingContext()), 82);
        }
    }
}