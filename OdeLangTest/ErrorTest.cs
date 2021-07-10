using System;
using NUnit.Framework;
using OdeLang;
using OdeLang.ErrorExceptions;

namespace OdeLangTest
{
    public class ErrorTest
    {
        private static InterpretingContext Run(string code)
        {
            var context = new InterpretingContext();
            new Interpreter(code).Run(context);
            return context;
        }

        [Test]
        public void BadCodeTest()
        {
            string code =
@"
x = 1
fn main()
  robot.getLocation()x
";

            Assert.Throws<OdeException>(
                delegate { Run(code); });
        }
    }
}