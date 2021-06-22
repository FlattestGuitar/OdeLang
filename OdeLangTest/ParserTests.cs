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
                Number(61, 0, 0),
                Plus(0, 0),
                Number(21, 0, 0),
                Newline(0, 0),
                EOF(0, 0)
            };
            
            var result = new Parser(input).Parse();

            //todo this test is not actually going to verify the AST is correct
            //we have no sensible way of doing that, so we might as well remove it once we can properly print output
           Assert.AreEqual("OdeLang.CompoundStatement", result.ToString());
        }
    }
}