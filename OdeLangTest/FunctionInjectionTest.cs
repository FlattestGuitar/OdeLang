using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OdeLang;

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

        private OdeFunction GetRangeFunction()
        {
            return new(
                "range",
                new Func<Robot, Robot, int>((robot1, robot2) => Math.Abs(robot1.Location - robot2.Location))
            );
        }

        //you can override OdeObject if you want to add fields or methods used in functions you define yourself
        private class Robot : OdeObject
        {
            internal int Location { get; }

            public Robot(int location, string objectName, List<OdeFunction> functions, Func<string> toStringFunc)
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
                new List<OdeFunction>(),
                () => "General Kenobi!"
            );
        }
    }
}