using System.Collections.Generic;
using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    //the string literals in this file are kinda weird,
    //but because indentation matters they need to be completely left-aligned
    public class InterpreterTest
    {
        private static InterpretingContext Run(string code)
        {
            var context = new InterpretingContext();
            new Interpreter(code).Run(context);
            return context;
        }

        [Test]
        public void BasicArithmeticOperation()
        {
            string code = "print(1+2)";

            var context = Run(code);

            Assert.AreEqual("3", context.GetOutput());
        }

        [Test]
        public void ComplexArithmeticOperation()
        {
            string code = "print((56+12)*(12/(72+1.2))+2+4)";

            var context = Run(code);

            Assert.AreEqual("17.147541", context.GetOutput());
        }

        [Test]
        public void ModuloArithmeticOperation()
        {
            string code = "print(5 % 2 * 2)";

            var context = Run(code);

            Assert.AreEqual("2", context.GetOutput());
        }

        [Test]
        public void NegationOperation()
        {
            string code = "print(-5)";

            var context = Run(code);

            Assert.AreEqual("-5", context.GetOutput());
        }

        [Test]
        public void LogicalNoOperation()
        {
            string code = "print(!false and true)";

            var context = Run(code);

            Assert.AreEqual("true", context.GetOutput());
        }

        [Test]
        public void ComplexMultilineArithmeticOperation()
        {
            string code =
@"
println((56+12)*(12/(72+1.2))+2+4)
println(12+5+82*6)
";

            var context = Run(code);

            Assert.AreEqual("17.147541\n509\n", context.GetOutput());
        }

        [Test]
        public void AssignmentTest()
        {
            string code =
@"
x = 5
println(x)
";

            var context = Run(code);

            Assert.AreEqual("5\n", context.GetOutput());
        }

        [Test]
        public void ComplexAssignmentTest()
        {
            string code =
@"
x = 5+13
y = 1/2
println(x - y)
";

            var context = Run(code);

            Assert.AreEqual("17.5\n", context.GetOutput());
        }

        [Test]
        public void StringAssignmentTest()
        {
            string code =
@"
x = ""asd""
print(x)
";

            var context = Run(code);

            Assert.AreEqual("asd", context.GetOutput());
        }

        [Test]
        public void StringArithmeticTest()
        {
            string code =
@"
x = ""asd"" + ""qwe""
print(x + 2)
";

            var context = Run(code);

            Assert.AreEqual("asdqwe2", context.GetOutput());
        }

        [Test]
        public void BooleanAssignmentTest()
        {
            string code =
@"
x = true
print(x)
";

            var context = Run(code);

            Assert.AreEqual("true", context.GetOutput());
        }

        [Test]
        public void BasicConditionalTest()
        {
            string code =
@"
if(false)
  print(""x"")
else
  print(""y"")
";

            var context = Run(code);

            Assert.AreEqual("y", context.GetOutput());
        }

        [Test]
        public void CompoundConditionalTest()
        {
            string code =
@"
fn printtest(x)
  if(x < 2)
    print(""<2"")
  elif(x == 2)
    print(""=2"")
  elif(x > 2)
    print("">2"")

printtest(1)
printtest(2)
printtest(3)
";

            var context = Run(code);

            Assert.AreEqual("<2=2>2", context.GetOutput());
        }


        [Test]
        public void NestedConditionalTest()
        {
            string code =
@"
if(true)
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
print(""t"")
";

            var context = Run(code);

            Assert.AreEqual("xzot", context.GetOutput());
        }

        [Test]
        public void LogicalArithmeticTest()
        {
            string code =
@"
x = 6 < 5
println(x)
y = 7 > 1 == 5 < 1
println(y)
z = (7 > 1) == (5 < 1)
println(z)
a = false or (6 > 2 and true)
println(a)
";

            var context = Run(code);

            Assert.AreEqual("false\ntrue\nfalse\ntrue\n", context.GetOutput());
        }

        [Test]
        public void IfStatementAndLogicalArithmeticTest()
        {
            string code =
@"
if(6 < 5)
  print(""6"")
if(3 > 2 and true)
  if(3 > 1)
    print(""y"")

  if(1 < 3)
    if(false)
      print(""1"")
    if true == 1
      print(""2"")
";

            var context = Run(code);

            Assert.AreEqual("y2", context.GetOutput());
        }

        [Test]
        public void LoopTest()
        {
            string code =
@"
i = 0
while(i < 2)
  print(""a"")
  i = i + 1
";

            var context = Run(code);

            Assert.AreEqual("aa", context.GetOutput());
        }

        [Test]
        public void LoopControlTest()
        {
            string code =
@"
i = 0
while(true)
  print(""a"")
  i = i + 1
  if(i == 2)
    break
";

            var context = Run(code);

            Assert.AreEqual("aa", context.GetOutput());
        }

        [Test]
        public void ContinueLoopControlTest()
        {
            string code =
@"
i = 5
while(true)
  i = i - 1
  if(i > 3)
    continue
  if(i < 1)
    print(""a"")
  if i < 0
    break
";

            var context = Run(code);

            Assert.AreEqual("aa", context.GetOutput());
        }
        
        
        [Test]
        public void ChainedIfTest()
        {
            string code =
@"
while(true)
  if(true)
    print(""x"")
  if(true)
    print(""y"")
  break
";

            var context = Run(code);

            Assert.AreEqual("xy", context.GetOutput());
        }
        
        
        [Test]
        public void FunctionDefinitionTest()
        {
            string code =
@"
fn test(qwe)
  print(qwe + 2)

test(5)
";

            var context = Run(code);

            Assert.AreEqual("7", context.GetOutput());
        }

        [Test]
        public void ComplexFunctionDefinitionTest()
        {
            string code =
@"
fn test(number_of_iterations, should_stop_on_even)
  while(number_of_iterations)
    number_of_iterations = number_of_iterations - 1
    print(1)
    if(should_stop_on_even and number_of_iterations % 2 == 0)
      break

test(5, false)
";

            var context = Run(code);

            Assert.AreEqual("11111", context.GetOutput());
        }
    
        [Test]
        public void FunctionWithReturnValueTest()
        {
            string code =
@"
fn test()
  return 5

print(test())
";
            var context = Run(code);

            Assert.AreEqual("5", context.GetOutput());
        }

        [Test]
        public void FunctionWithNoReturnValueTest()
        {
            string code =
@"
fn test()
  return

print(test())
";
            var context = Run(code);

            Assert.AreEqual("null", context.GetOutput());
        }
        
        
        [Test]
        public void ArrayTest()
        {
            string code =
@"
x = [1, 2, 3]
x.append(4)
print(x)
print(x[1])
";
            var context = Run(code);

            Assert.AreEqual("[1,2,3,4]2", context.GetOutput());
        }  
        
        [Test]
        public void EmptyArrayTest()
        {
            string code =
@"
x = []
print(x.length())
";
            var context = Run(code);

            Assert.AreEqual("0", context.GetOutput());
        }
        
        [Test]
        public void ComplexArrayTest()
        {
            string code =
@"
x = [1, [6, 3], 3]
x.get(1).append(5)

x[0] = 5
print(x)
";
            var context = Run(code);

            Assert.AreEqual("[5,[6,3,5],3]", context.GetOutput());
        }
        
        [Test]
        public void DictionaryTest()
        {
            string code =
@"
x = {
""a"": 1,
""b"": 2,//this is a comment
""c"": [6, 4, ""test""] //this is another comment
}

x[""d""] = 4
print(x)
print(x.get(""a""))
print(x[""b""])
";
            var context = Run(code);

            Assert.AreEqual("{a:1,b:2,c:[6,4,test],d:4}12", context.GetOutput());
        }
        
        [Test]
        public void ShorthandAssignmentTest()
        {
            string code =
@"
x = 5
x += 1
print(x)
";
            var context = Run(code);

            Assert.AreEqual("6", context.GetOutput());
        }
        
        [Test]
        public void ShorthandIncrementTest()
        {
            string code =
@"
x = 5
x++
print(x)
print(++x)
print(x++)
print(x)
";
            var context = Run(code);

            Assert.AreEqual("6778", context.GetOutput());
        }

        [Test]
        public void ForLoopTest()
        {
            string code =
@"
for i in [1, 2, ""hello there"", 4]
  print(i)
";
            var context = Run(code);

            Assert.AreEqual("12hello there4", context.GetOutput());
        }
        
        [Test]
        public void AssignmentAndManipulationForLoop()
        {
            string code =
@"
x = [1, 2, 3]
x.append(3)
for i in x
  print(i)
";
            var context = Run(code);

            Assert.AreEqual("1233", context.GetOutput());
        }
        
        [Test]
        public void DictionaryForLoop()
        {
            string code =
@"
x = {
""jim"": 1,
""agnes"": 2,
""charlie"": 51
}

for i in x
  print(i)
  print("" has "")
  println(x.get(i))
";
            var context = Run(code);

            Assert.AreEqual("jim has 1\nagnes has 2\ncharlie has 51\n", context.GetOutput());
        }
    }
}