using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    
    //the string literals in this file are kinda weird,
    //but because indentation matters they need to be completely left-aligned
    public class InterpreterTest
    {
        [Test]
        public void BasicArithmeticOperation()
        {
            string code = "print(1+2)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "3");
        }

        [Test]
        public void ComplexArithmeticOperation()
        {
            string code = "print((56+12)*(12/(72+1.2))+2+4)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "17.147541");
        }

        [Test]
        public void ComplexMultilineArithmeticOperation()
        {
            string code = 
@"println((56+12)*(12/(72+1.2))+2+4)
println(12+5+82*6)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "17.147541\n509\n");
        }


        [Test]
        public void AssignmentTest()
        {
            string code = 
@"x = 5
println(x)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "5\n");
        }

        [Test]
        public void ComplexAssignmentTest()
        {
            string code = 
@"x = 5+13
y = 1/2
println(x - y)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "17.5\n");
        }
        
        [Test]
        public void StringAssignmentTest()
        {
            string code = 
@"x = ""asd""
print(x)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "asd");
        }
        
        [Test]
        public void StringArithmeticTest()
        {
            string code = 
@"x = ""asd"" + ""qwe""
print(x + 2)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "asdqwe2");
        }
        
        [Test]
        public void BooleanAssignmentTest()
        {
            string code = 
@"x = true
print(x)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "true");
        }

        [Test]
        public void BasicConditionalTest()
        {
            string code = 
@"if(true)
  print(""x"")";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "x");
        }
        
        
        [Test]
        public void NestedConditionalTest()
        {
            string code = 
@"if(true)
  if(true)
    print(""x"")
  if(false)
    print(""y"")
  if(true)
    print(""z"")
if(true)
  print(""o"")
if(false)
  print(""u"")
print(""t"")";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "xzot");
        }
    }
}