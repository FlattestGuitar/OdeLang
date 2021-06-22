using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    public class InterpreterTest
    {
        [Test]
        public void arithmeticOperation()
        {
            string code = "56+12";

            var context = new Interpreter(code).run();
            
            //no output, but at least no exceptions

        }
    }
}