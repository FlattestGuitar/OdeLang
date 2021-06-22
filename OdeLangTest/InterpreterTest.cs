using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    public class InterpreterTest
    {
       
        [Test]
        public void ComplexArithmeticOperation()
        {
            string code = "(56+12)*(12/(72+1.2))+2";

            var context = new Interpreter(code).run();
            
            Assert.AreEqual(context.getOutput(), "13.147542");
        }
    }
}