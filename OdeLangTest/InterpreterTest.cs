using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    public class InterpreterTest
    {
        [Test]
        public void BasicArithmeticOperation()
        {
            string code = "print(1+2)";

            var context = new Interpreter(code).run();
            
            Assert.AreEqual(context.getOutput(), "3");
        }

        [Test]
        public void ComplexArithmeticOperation()
        {
            string code = "print((56+12)*(12/(72+1.2))+2+4)";

            var context = new Interpreter(code).run();
            
            Assert.AreEqual(context.getOutput(), "17.147541");
        }
        
        [Test]
        public void ComplexMultilineArithmeticOperation()
        {
            string code = "println((56+12)*(12/(72+1.2))+2+4)\n" +
                          "println(12+5+82*6)";

            var context = new Interpreter(code).run();
            
            Assert.AreEqual(context.getOutput(), "17.147541\n509\n");
        }
    }
}