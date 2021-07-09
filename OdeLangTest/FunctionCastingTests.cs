using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OdeLang;

namespace OdeLangTest
{
    public class FunctionCastingTests
    {
        [Test]
        public void TypeConversion()
        {
            var list = new OdeArray(new List<Value>() {Value.NumericalValue(1), Value.NumericalValue(2)});

            var result = Language.AutoTypeFromValue(Value.ObjectValue(list), typeof(List<int>));

            Assert.AreEqual(new List<int> {1, 2}, result);
        }

        [Test]
        public void OdeFunctionDef()
        {
            var func = new OdeFunction(
                "test_func",
                new Func<List<int>, int, IEnumerable<int>>((ints, bumpBy) =>
                ints.Select(a => a + bumpBy).ToList()));

            var result = func.Eval(new List<Value> {Value.ObjectValue(new OdeArray(new List<Value> {Value.NumericalValue(1), Value.NumericalValue(2)})), Value.NumericalValue(1)});
            
            Assert.AreEqual("[2,3]",
                result.GetStringValue());
        }
    }
}