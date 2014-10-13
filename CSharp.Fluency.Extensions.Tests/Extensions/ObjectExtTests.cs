using System;
using System.Collections.Generic;
using System.Linq;
using CSharp.Fluency.Extensions.Extensions;
using NUnit.Framework;

namespace CSharp.Fluency.Extensions.Tests.Extensions
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
        public void Unless_BoolOverload()
        {
            IEnumerable<int> input = new[] { 0, 1, 2, 3, 4 };

            CollectionAssert.AreEqual(input, input.Unless(true, elements => elements.Reverse()));
            CollectionAssert.AreEqual(input.Reverse(), input.Unless(false, elements => elements.Reverse()));
        }


        [Test]
        public void Unless_SimpleFuncOverload()
        {
            IEnumerable<int> input = new[] { 0, 1, 2, 3, 4 };

            CollectionAssert.AreEqual(input, input.Unless(() => true, elements => elements.Reverse()));
            CollectionAssert.AreEqual(input.Reverse(), input.Unless(() => false, elements => elements.Reverse()));
        }


        [Test]
        public void Unless_ComplexFuncOverload()
        {
            IEnumerable<int> input = new[] { 0, 1, 2, 3, 4 };

            CollectionAssert.AreEqual(input, input.Unless(
                elements => elements.Count() != 4 && elements.Select( (index, e) => index == e).All(r => r), 
                elements => elements.Reverse()));

            CollectionAssert.AreEqual(input.Reverse(), input.Unless(
                elements => elements.Count() != 5 && elements.Select( (index, e) => index == e).All(r => r), 
                elements => elements.Reverse()));
        }

        
        [Test]
        public void IfElse()
        {
            CollectionAssert.AreEqual(
                new[] { "ax", "bx", "ay", "by", "bz", "az"},
                new[] { "ay", "bz", "ax", "bx", "by", "az" }
                    .AsEnumerable()
                    .IfElse(false, 
                        strings => strings.OrderBy(c => c.First()),
                        strings => strings.OrderBy(c => c.Last()))
                );
        }


        [Test]
        public void IfElseT_BoolConditionAndTResultOverload()
        {
            Assert.AreEqual("yay", new[] { 0, 1, 2, 3, 4 }.IfElseT(true, "yay", "nay"));
            Assert.AreEqual("nay", new[] { 0, 1, 2, 3, 4 }.IfElseT(false, "yay", "nay"));
        }

        [Test]
        public void IfElseT_SimplePredicateAndTResultOverload()
        {
            Assert.AreEqual("yay", new[] { 0, 1, 2, 3, 4 }.IfElseT(() => true, "yay", "nay"));
            Assert.AreEqual("nay", new[] { 0, 1, 2, 3, 4 }.IfElseT(() => false, "yay", "nay"));
        }


        [Test]
        public void IfElseT_ComplexPredicateAndTResultOverload()
        {
            Assert.AreEqual("yay", new[] { 0, 1, 2, 3, 4 }.IfElseT(n => n.Contains(3), "yay", "nay"));
            Assert.AreEqual("nay", new[] { 0, 1, 2, 3, 4 }.IfElseT(n => n.Contains(30), "yay", "nay"));
        }


        [Test]
        public void IfElseT_BoolConditionAndFuncOverload()
        {
            Assert.AreEqual(5, new[] { 0, 1, 2, 3, 4 }.IfElseT(true, numbers => numbers.Count(), numbers => numbers.Count() - 3));
            Assert.AreEqual(2, new[] { 0, 1, 2, 3, 4 }.IfElseT(false, numbers => numbers.Count(), numbers => numbers.Count() - 3));
        }
        

        [Test]
        public void IfElseT_SimplePredicateAndFuncOverload()
        {
            Assert.AreEqual(4, 
                new[] { 0, 1, 2, 3, 4 }.IfElseT(() => true, numbers => numbers.Take(4), numbers => numbers.Take(2)).Count());
            Assert.AreEqual(2, 
                new[] { 0, 1, 2, 3, 4 }.IfElseT(() => false, numbers => numbers.Take(4), numbers => numbers.Take(2)).Count());
        }


        [Test]
        public void IfElseT_ComplexPredicateAndFuncOverload()
        {
            Assert.AreEqual(4, 
                new[] { 0, 1, 2, 3, 4 }.IfElseT(n => n.Contains(3), numbers => numbers.Take(4), numbers => numbers.Take(2)).Count());
            Assert.AreEqual(2, 
                new[] { 0, 1, 2, 3, 4 }.IfElseT(n => n.Contains(30), numbers => numbers.Take(4), numbers => numbers.Take(2)).Count());

        }


        [Test]
        public void NullSafe()
        {
            Assert.AreEqual("dagnabbit", "dag".NullSafe(str => str + "nabbit"));
            Assert.Null("dag".NullSafe<string, string>(_ => null));
            Assert.Null(((string)null).NullSafe<string, string>(_ => null));
            Assert.Null(((string)null).NullSafe(_ => "dagnabbit"));
        }


        [Test]
        public void NullSafe_MultipleTransforms()
        {
            Assert.AreEqual("sdfasdf", "asdfasdf".NullSafe(s => s.Substring(1), s => new EquatableTestClass(s), e => e.Key));
            Assert.IsNull("asdfasdf".NullSafe(s => (string)null, s => new EquatableTestClass(s), e => e.Key));
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
        public void Pipe_MultipleTransforms()
        {
            var input = new DateTime(17);

            Func<DateTime, long> tr1 = dt => dt.Ticks % 10;
            Func<long, string> tr2 = l => l.ToString();
            Func<string, int> tr3 = int.Parse;
            Func<int, DateTime> tr4 = i => new DateTime(i);
            Func<DateTime, string> tr5 = dt => dt.ToShortDateString();
            
            Assert.AreEqual(new DateTime(7).ToShortDateString(), input.Pipe(tr1, tr2, tr3, tr4, tr5));
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
