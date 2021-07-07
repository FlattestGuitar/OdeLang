using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;
using static OdeLang.FunctionDefinition;
using static OdeLang.Value;

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

        private FunctionDefinition GetNumbersFunction()
        {
            return new(
                "numbers",
                new List<ArgumentType>(),
                _ => new OdeArray(new List<Value>{NumericalValue(1), NumericalValue(2)})
            );
        }

        private FunctionDefinition GetNegativeLengthFunction()
        {
            return new(
                "negative_length",
                new List<ArgumentType> {ObjectArgument(typeof(OdeArray))},
                args => -((OdeArray) args[0].GetObjectValue()).GetValues().Count
            );
        }
    }
}