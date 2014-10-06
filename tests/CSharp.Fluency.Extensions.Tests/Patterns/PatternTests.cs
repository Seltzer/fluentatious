using System;
using CSharp.Fluency.Extensions.Patterns;
using NUnit.Framework;

namespace CSharp.Fluency.Extensions.Tests.Patterns
{

    [TestFixture]
    class PatternTests
    {
        [TestCase("a", Result = 1)]
        [TestCase("b", Result = 2)]
        public int TestConditionModes(string input)
        {
            return Pattern<string, int>
                .Match(input)
                .Case(input == "a", 1)                  // Using bool
                .Case(str => str.StartsWith("b"), 2)    // Using predicate
                .ResolveFirst();
        }


        [TestCase("aaa", Result = 42)]
        [TestCase("b", Result = 27)]
        [TestCase("x", Result = 99)]
        [TestCase("sdfsdf", Result = default(int))]
        public int SimpleFlatDistinctCases(string input)
        {
            return Pattern<string, int>
                .Match(input)
                .Case(input == "aaa", 42)
                .Case(input.StartsWith("b"), 27)
                .Case(str => str.StartsWith("x"), 99)
                .ResolveFirstOrDefault();
        }

        
        [TestCase("aaa", Result = 42)]
        [TestCase("b", Result = 27)]
        [TestCase("x", Result = 99)]
        [TestCase("sdfsdf", Result = default(int))]
        public int TestCaseThenSyntax(string input)
        {
            return Pattern<string, int>
                .Match(input)
                .Case(input == "aaa").Then(42)
                .Case(input.StartsWith("b")).Then(27)
                .Case(str => str.StartsWith("x")).Then(99)
                .ResolveFirstOrDefault();
        }
        
        
        [Test]
        public void ResolveFirst_CorrectPrecedence()
        {
            Pattern<string, int>
                .Match("asdf")
                .Case(true, 27)
                .Case(true, 16)
                .ResolveFirst()
                .Do(result => Assert.AreEqual(27, result));
        }


        [TestCase("a", Result = 27)]
        [TestCase("b", Result = 16)]
        [TestCase("c", Result = 100)]
        public int Default(string input)
        {
            return Pattern<string, int>
                .Match(input)
                .Case(input == "a", 27)
                .Case(input == "b", 16)
                .Default(100)
                .ResolveFirst();
        }


        [TestCase("a", Result = 27)]
        [TestCase("b", Result = 16)]
        [TestCase("c", Result = 100)]
        public int IncorrectBreaksHaveNoEffect(string input)
        {
            return Pattern<string, int>
                .Match(input)
                .Case(input == "a", 27)
                .Break()
                .Case(input == "b", 16)
                .Break()
                .Break()
                .Default(100)
                .ResolveFirst();
        }


        [TestCase("dog", Result = "dog.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        [TestCase("fish tetra", Result = "tetra.png")]
        [TestCase("tetra", ExpectedException = typeof(InvalidOperationException))]
        public string Subcases_BreakSyntax_SimpleTest(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .Case(input.Contains("manx"), "manx.png")
                    .Case(input.Contains("siamese"), "siamese.png")
                    .Case(input.Contains("bengal"), "bengal.png")
                    .Break()
                .Case(input.Contains("dog"), "dog.png")
                .Case(input.Contains("fish"))
                    .Case(input.Contains("tetra"), "tetra.png")
                    .Case(input.Contains("gourami"), "gourami.png")
                    .Break()
                .ResolveFirst();
        }


        [TestCase("dog terrier", Result = "terrier.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        [TestCase("cat", Result = "generic-cat.png")]
        [TestCase("fish tetra", Result = "tetra.png")]
        [TestCase("tetra", Result = "generic-animal.png")]
        [TestCase("poodle dog", Result = "generic-animal.png")]
        [TestCase("octopus", Result = "generic-animal.png")]
        public string Subcases_BreakSyntax_WithDefaults(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .Case(input.Contains("manx"), "manx.png")
                    .Case(input.Contains("siamese"), "siamese.png")
                    .Case(input.Contains("bengal"), "bengal.png")
                    .Default("generic-cat.png")
                    .Break()
                .Case(input.Contains("dog"))
                    .Case(input.Contains("terrier"), "terrier.png")
                    .Case(input.Contains("rottweiler"), "rottweiler.png")
                    .Break()
                .Case(input.Contains("fish"))
                    .Case(input.Contains("tetra"), "tetra.png")
                    .Case(input.Contains("gourami"), "gourami.png")
                    .Default("generic-fish.png")
                    .Break()
                .Default("generic-animal.png")
                .ResolveFirst();
        }


        
        [TestCase("catfish", Result = "catfish.png")]
        [TestCase("doge cat", Result = "dogecat.png")]
        public string Subcases_BreakSyntax_NonDistinct(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("fish"))
                    .Case(input.Contains("tetra"), "tetra.png")
                    .Case(input.Contains("gourami"), "gourami.png")
                    .Case(input.Contains("catfish"), "catfish.png")    
                    .Break()
                .Case(input.Contains("dog"))
                    .Case(input.Contains("terrier"), "terrier.png")
                    .Break()
                .Case(input.Contains("cat"))
                    .Case(input.Contains("manx"), "manx.png")
                    .Case(input.Contains("siamese"), "siamese.png")
                    .Case(input.Contains("bengal"), "bengal.png")
                    .Case(input.Contains("doge"), "dogecat.png")
                    .Break()
                .Default("generic-animal.png")
                .ResolveFirst();
        }


        [TestCase("dog", Result = "dog.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        [TestCase("fish tetra", Result = "tetra.png")]
        [TestCase("tetra", ExpectedException = typeof(InvalidOperationException))]
        [TestCase("cat siamese", Result = "siamese.png")]
        public string Subcases_BreakSyntax_WithCaseThenSyntax(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .Case(input.Contains("manx"), "manx.png")
                    .Case(input.Contains("siamese")).Then("siamese.png")
                    .Case(input.Contains("bengal"), "bengal.png")
                    .Break()
                .Case(input.Contains("dog"), "dog.png")
                .Case(input.Contains("fish"))
                    .Case(input.Contains("tetra")).Then("tetra.png")
                    .Case(input.Contains("gourami"), "gourami.png")
                    .Break()
                .ResolveFirst();
        }
        
        
        [TestCase("cat bengal", Result = "bengal.png")]
        public string Subcases_ExplicitSyntax_SimpleTest(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"), "manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                .ResolveFirst();
        }
        

        [TestCase("dog", Result = "dog.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        public string Subcases_ExplicitSyntax_ComplexTest(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"), "manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                .Case(input.Contains("dog"), "dog.png")
                .ResolveFirst();
        }


        [TestCase("dog", Result = "dog.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        [TestCase("cat manx", Result = "generic-manx.png")]
        [TestCase("playful manx cat", Result = "playful-manx.png")]
        [TestCase("angry manx cat", Result = "angry-manx.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        public string Subcases_ExplicitOpenCases(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"))
                        .SubSubCase(input.Contains("playful"), "playful-manx.png")
                        .SubSubCase(input.Contains("angry"), "angry-manx.png")
                        .SubSubCase(input.Contains("morose"), "morose-manx.png")
                        .Default("generic-manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                .Case(input.Contains("dog"), "dog.png")
                .ResolveFirst();
        }


          
        [TestCase("dog", Result = "dog.png")]
        [TestCase("cat bengal", Result = "bengal.png")]
        [TestCase("fish tetra", Result = "tetra.png")]
        [TestCase("tetra", ExpectedException = typeof(InvalidOperationException))]
        public string Subcases_MixedSyntaxes(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"), "manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                .Case(input.Contains("dog"), "dog.png")
                .Case(input.Contains("fish"))
                    .Case(input.Contains("tetra"), "tetra.png")
                    .Case(input.Contains("gourami"), "gourami.png")
                    .Break()
                .ResolveFirst();
        }
    }

}
