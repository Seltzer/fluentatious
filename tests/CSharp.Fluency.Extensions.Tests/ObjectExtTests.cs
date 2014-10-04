using System;
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


        [Test]
        public void IfNotNull()
        {
            Assert.AreEqual("dagnabbit", "dag".IfNotNull(str => str + "nabbit"));
            Assert.Null("dag".IfNotNull<string, string>(_ => null));
            Assert.Null(((string)null).IfNotNull<string, string>(_ => null));
            Assert.Null(((string)null).IfNotNull(_ => "dagnabbit"));
        }


        [Test]
        public void Pipe()
        {
            // ReSharper disable ConvertClosureToMethodGroup
            Assert.AreEqual(4, 3.Pipe(i => Increment(i)));
            // ReSharper restore ConvertClosureToMethodGroup
            
            // The below will not compile if the call is ambiguous (e.g. if we had a Pipe overload which took an Action)
            Assert.AreEqual(4, 3.Pipe(Increment));
        }


        [Test]
        public void Do_SingleAction()
        {
            int i = 5;
            Assert.AreEqual(3, 3.Do(blah => { i += blah; }));
            Assert.AreEqual(8, i);
        }


        [Test]
        public void Do_MultipleAction()
        {
            int i = 12;
            Assert.AreEqual(3, 3.Do(blah => { i += blah; }, blah => { i /= blah; }));
            // This proves that both actions are executed in the correct order.
            Assert.AreEqual(5, i);
        }


        [Test]
        public void Cast_OnNull()
        {
            ObjectExt.CastTo<EquatableTestClass>(null);
            Assert.Pass();
        }


        [Test]
        public void Cast_ValidCastsOnNonNullObjects()
        {
            var blah = new EquatableTestClass("sdfsdf");

            Assert.AreEqual(blah, blah.CastTo<EquatableTestClass>());
            blah.CastTo<object>();
            Assert.Pass();
        }


        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void Cast_InvalidCasts()
        {
            var blah = new EquatableTestClass("sdfsdf");

            blah.CastTo<int>();
            Assert.Pass();
        }


        [Test]
        public void As_OnNull()
        {
            ObjectExt.As<EquatableTestClass>(null);
            Assert.Pass();
        }


        [Test]
        public void As_ValidCastsOnNonNullObjects()
        {
            var blah = new EquatableTestClass("sdfsdf");

            Assert.AreEqual(blah, blah.As<EquatableTestClass>());
            blah.As<object>();
            Assert.Pass();
        }


        [Test]
        public void As_InvalidCasts()
        {
            var blah = new EquatableTestClass("sdfsdf");

            Assert.Null(blah.As<string>());
        }


        static int Increment(int i)
        {
            return i + 1;
        }
    }

}
