﻿using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
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
            string code = "println((56+12)*(12/(72+1.2))+2+4)\n" +
                          "println(12+5+82*6)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "17.147541\n509\n");
        }


        [Test]
        public void AssignmentTest()
        {
            string code = "x = 5\n" +
                          "println(x)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "5\n");
        }

        [Test]
        public void ComplexAssignmentTest()
        {
            string code = "x = 5+13\n" +
                          "y = 1/2\n" +
                          "println(x - y)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "17.5\n");
        }
        
        [Test]
        public void StringAssignmentTest()
        {
            string code = "x = \"asd\"\n" +
                          "print(x)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "asd");
        }
        
        [Test]
        public void StringArithmeticTest()
        {
            string code = "x = \"asd\" + \"qwe\"\n" +
                          "print(x + 2)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "asdqwe2");
        }
        
        [Test]
        public void BooleanAssignmentTest()
        {
            string code = "x = true\n" +
                          "print(x)";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "true");
        }

        [Test]
        public void BasicConditionalTest()
        {
            string code = "if(true)\n" +
                          "  print(\"x\")";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "x");
        }
        
        
        [Test]
        public void NestedConditionalTest()
        {
            string code = "if(true)\n" +
                          "  if(true)\n" +
                          "    print(\"x\")\n" +
                          "  if(false)\n" +
                          "    print(\"y\")\n" +
                          "  if(true)\n" +
                          "    print(\"z\")\n" +
                          "if(true)\n" +
                          "  print(\"o\")\n" +
                          "if(false)\n" +
                          "  print(\"u\")\n" +
                          "print(\"t\")";

            var context = new Interpreter(code).Run();

            Assert.AreEqual(context.GetOutput(), "xzot");
        }
    }
}