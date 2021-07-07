using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OdeLang;
using static OdeLang.FunctionDefinition;

namespace OdeLangTest
{
    public class FunctionInjectionTest
    {
        [Test]
        public void GlobalFunctionInjectionTest()
        {
            var code =
@"
range = range(robot, enemy)
print(range)
";
            var context = new InterpretingContext();
            context.InjectObject(CreateRobot(3, "robot"));
            context.InjectObject(CreateRobot(6, "enemy"));
            context.InjectGlobalFunction(GetRangeFunction());

            var interpreter = new Interpreter(code);
            interpreter.Run(context);

            Assert.AreEqual("3", context.GetOutput());
        }

        private FunctionDefinition GetRangeFunction()
        {
            return new(
                "range",
                new List<ArgumentType>
                {
                    ObjectArgument(typeof(Robot)), 
                    ObjectArgument(typeof(Robot))
                },
                args =>
                {
                    var a = (Robot) args[0].GetObjectValue();
                    var b = (Robot) args[1].GetObjectValue();
                    return Math.Abs(a.Location - b.Location);
                }
            );
        }

        //you can override OdeObject if you want to add fields or methods used in functions you define yourself
        private class Robot : OdeObject
        {
            internal int Location { get; }

            public Robot(int location, string objectName, List<FunctionDefinition> functions, Func<string> toStringFunc)
                : base(objectName, functions, toStringFunc)
            {
                Location = location;
            }
        }
        
        private Robot CreateRobot(int location, string name)
        {
            return new Robot(
                location,
                name,
                new List<FunctionDefinition>(),
                () => "General Kenobi!"
            );
        }
    }
}