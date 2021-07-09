using System;
using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    public class ArrayInjectionTest
    {
        [Test]
        public void FunctionReturnsArray()
        {
            var code =
@"
x = numbers()

print(x)
";
            var context = new InterpretingContext();
            context.InjectGlobalFunction(GetNumbersFunction());

            var interpreter = new Interpreter(code);
            interpreter.Run(context);

            Assert.AreEqual("[1,2]", context.GetOutput());
        }


        [Test]
        public void FunctionTakesArrayAsArg()
        {
            
            var code =
@"
x = negative_length([1, 2, 3])

print(x)
";
            var context = new InterpretingContext();
            context.InjectGlobalFunction(GetNegativeLengthFunction());

            var interpreter = new Interpreter(code);
            interpreter.Run(context);

            Assert.AreEqual("-3", context.GetOutput());
        }

        private OdeFunction GetNumbersFunction()
        {
            return new(
                "numbers",
                new Func<List<int>>(() => new List<int>(){1, 2})
            );
        }

        private OdeFunction GetNegativeLengthFunction()
        {
            return new(
                "negative_length",
                new Func<List<Value>, int>(list => -list.Count)
            );
        }
    }
}