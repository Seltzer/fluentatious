using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CSharp.Fluency.Extensions.Tests
{

    class ObjectExtTests
    {
        [Test]
        public void If_BoolOverload()
        {
            IEnumerable<int> input = new[] { 0, 1, 2, 3, 4 };

            CollectionAssert.AreEqual(input, input.If(false, elements => elements.Reverse()));
            CollectionAssert.AreEqual(input.Reverse(), input.If(true, elements => elements.Reverse()));
        }


        [Test]
        public void If_SimpleFuncOverload()
        {
            IEnumerable<int> input = new[] { 0, 1, 2, 3, 4 };

            CollectionAssert.AreEqual(input, input.If(() => false, elements => elements.Reverse()));
            CollectionAssert.AreEqual(input.Reverse(), input.If(() => true, elements => elements.Reverse()));
        }


        [Test]
        public void If_ComplexFuncOverload()
        {
            IEnumerable<int> input = new[] { 0, 1, 2, 3, 4 };

            CollectionAssert.AreEqual(input, input.If(
                elements => elements.Count() == 4 && elements.Select( (index, e) => { return index == e;  }).All(r => r), 
                elements => elements.Reverse()));

            CollectionAssert.AreEqual(input.Reverse(), input.If(
                elements => elements.Count() == 5 && elements.Select( (index, e) => { return index == e;  }).All(r => r), 
                elements => elements.Reverse()));
        }
    }

}
