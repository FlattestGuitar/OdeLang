using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{

    public class OutsideFunctionCallTest
    {
        [Test]
        public void FunctionCallTest()
        {
            var code = 
@"
x = []
y = 12

fn main()
  y = y + 1
  x.append(y)
  print(x)
";
            var context = new InterpretingContext();
            
            var interpreter = new Interpreter(code);
            interpreter.Run(context);

            context.CallGlobalFunction("main", new List<Value>());
            context.CallGlobalFunction("main", new List<Value>());
            context.CallGlobalFunction("main", new List<Value>());
            context.CallGlobalFunction("main", new List<Value>());

            Assert.AreEqual("[13][13,14][13,14,15][13,14,15,16]", context.GetOutput());
        }
    }
}