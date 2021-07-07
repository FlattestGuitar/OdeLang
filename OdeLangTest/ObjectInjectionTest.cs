using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;
using static OdeLang.FunctionDefinition;
using static OdeLang.OdeObject;

namespace OdeLangTest
{
    /// <summary>
    /// This test serves as an example of how to inject objects into the context.
    /// 
    /// You have to wrap all return values in OdeLang's types and unwrap incoming arguments too.
    /// Remember that OdeLang has only one numerical type, so you might have to cast numbers.
    /// Good error checking is going to be very helpful for players.
    ///
    /// In order to create variables inside of the objects created you simply have to reference any previously created var
    /// in the object's functions. Unlike a normal, usable language (like Java) C# will always pass these as references,
    /// so you can rely on them being modified properly by the lambdas.
    /// </summary>
    public class ObjectInjectionTest
    {
        [Test]
        public void SimpleObjectInjectionTest()
        {
            var code = 
@"
println(robot.get_name())
robot.set_name(""Agnes"")
println(robot.get_name())

println(robot.get_number_of_legs())
robot.set_number_of_legs(2)
println(robot.get_number_of_legs())
";
            var context = new InterpretingContext();
            context.InjectObject(CreateRobot());
            
            var interpreter = new Interpreter(code);
            interpreter.Run(context);

            Assert.AreEqual("Jim the Robot\nAgnes\n4\n2\n", context.GetOutput());
        }

        private OdeObject CreateRobot()
        {
            //The simplest way to handle variables is to just define them here.
            //Even though these local references will be out of scope once we leave this method,
            //our lambdas will still keep the references and be able to operate on them

            string name = "Jim the Robot";
            int legCount = 4;

            return new OdeObject(
                "robot",
                new List<FunctionDefinition>
                {
                    new("get_name", new List<ArgumentType>(), _ => name),
                    new("get_number_of_legs", new List<ArgumentType>(), _ => legCount),
                    new("is_pretty", new List<ArgumentType>(), _ => true),
                    new("set_name", new List<ArgumentType>{StringArgument()}, args => name = args[0].GetStringValue()),
                    new("set_number_of_legs", new List<ArgumentType>{NumericalArgument()}, args => legCount = (int) args[0].GetNumericalValue())
                },
                () => "This is a NECESSARY to_string implementation. You can't make an object without one. Don't even try."
            );
        }
    }
}