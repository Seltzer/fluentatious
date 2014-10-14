using System;
using Fluentatious.Extensions;
using Fluentatious.Patterns;
using NUnit.Framework;

namespace Fluentatious.Tests.Patterns
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
        
        
        [TestCase("terrier dog", Result = "terrier.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("cat", Result = "generic-cat.png")]
        [TestCase("pearl gourami fish", Result = "pearl-gourami.png")]
        [TestCase("tetra fish", Result = "tetra.png")]
        [TestCase("tetra", Result = "generic-animal.png")]
        [TestCase("poodle dog", Result = "generic-animal.png")]
        [TestCase("octopus", Result = "generic-animal.png")]
        public string WithPredicateTests(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .WithPredicate(word => input.Contains((string)word))
                .Case("cat")
                    .Case("manx", "manx.png")
                    .Case("siamese", "siamese.png")
                    .Case("bengal", "bengal.png")
                    .Default("generic-cat.png")
                    .Break()
                .Case(input.Contains("dog"))
                    .Case(input.Contains("terrier"), "terrier.png")
                    .Case(input.Contains("rottweiler"), "rottweiler.png")
                    .Break()
                .WithPredicate(word => input.Contains(((string)word).ToLowerInvariant()))
                .Case("FISH")
                    .Case("TETRA", "tetra.png")
                    .Case("GOURAMI", p => p
                        .Case("PEARL", "pearl-gourami.png"))
                    .Default("generic-fish.png")
                    .Break()
                .Default("generic-animal.png")
                .ResolveFirst();
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
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("tetra fish", Result = "tetra.png")]
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


        [TestCase("terrier dog", Result = "terrier.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("cat", Result = "generic-cat.png")]
        [TestCase("tetra fish", Result = "tetra.png")]
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
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("tetra fish", Result = "tetra.png")]
        [TestCase("tetra", ExpectedException = typeof(InvalidOperationException))]
        [TestCase("siamese cat", Result = "siamese.png")]
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
        
        
        [TestCase("bengal cat", Result = "bengal.png")]
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
        [TestCase("bengal cat", Result = "bengal.png")]
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
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("playful manx cat", Result = "playful-manx.png")]
        [TestCase("angry manx cat", Result = "angry-manx.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        public string Subcases_ExplicitOpenCases(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"))
                        .SubSubCase(input.Contains("playful"), "playful-manx.png")
                        .SubSubCase(input.Contains("angry"), "angry-manx.png")
                        .SubSubCase(input.Contains("morose"), "morose-manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                .Case(input.Contains("dog"), "dog.png")
                .ResolveFirst();
        }


        [TestCase("dog", Result = "dog.png")]
        [TestCase("cat", Result = "generic-cat.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("manx cat", Result = "generic-manx.png")]
        [TestCase("playful manx cat", Result = "playful-manx.png")]
        [TestCase("angry manx cat", Result = "angry-manx.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("gourami fish", Result = "gourami.png")]
        [TestCase("fish", Result = "generic-fish.png")]
        [TestCase("octopus", Result = "generic-animal.png")]
        public string Subcases_ExplicitDefaults(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"))
                        .SubSubCase(input.Contains("playful"), "playful-manx.png")
                        .SubSubCase(input.Contains("angry"), "angry-manx.png")
                        .SubSubCase(input.Contains("morose"), "morose-manx.png")
                        .SubSubDefault("generic-manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                    .SubDefault("generic-cat.png")
                .Case(input.Contains("dog"), "dog.png")
                .Case(input.Contains("fish"))
                    .SubCase(input.Contains("tetra"), "tetra.png")
                    .SubCase(input.Contains("gourami"), "gourami.png")
                    .SubDefault("generic-fish.png")
                .Default("generic-animal.png")
                .ResolveFirst();
        }


        [TestCase("dog", Result = "dog.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("playful manx cat", Result = "playful-manx.png")]
        [TestCase("angry manx cat", Result = "angry-manx.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        public string Subcases_ExplicitCaseThenSyntax(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx"))
                        .SubSubCase(input.Contains("playful"), "playful-manx.png")
                        .SubSubCase(input.Contains("angry")).Then("angry-manx.png")
                        .SubSubCase(input.Contains("morose"), "morose-manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal")).Then("bengal.png")
                .Case(input.Contains("dog"), "dog.png")
                .ResolveFirst();
        }


          
        [TestCase("dog", Result = "dog.png")]
        [TestCase("bengal cat", Result = "bengal.png")]
        [TestCase("tetra fish", Result = "tetra.png")]
        [TestCase("tetra", ExpectedException = typeof(InvalidOperationException))]
        [TestCase("yellow bear", Result = "generic-bear.png")]
        [TestCase("irate brown bear", Result = "generic-irate-brown-bear.png")]
        [TestCase("fast irate brown bear", Result = "fast-irate-brown-bear.png")]
        [TestCase("deadly irate brown bear", Result = "deadly-irate-brown-bear.png")]
        [TestCase("super beta smart fast calm brown bear", Result = "super-beta-smart-fast-calm-brown-bear.png")]
        public string Subcases_MixedSyntaxes(string input)
        {
            return Pattern<string, string>
                .Match(input)
                .Case(input.Contains("cat"))
                    .SubCase(input.Contains("manx")).Then("manx.png")
                    .SubCase(input.Contains("siamese"), "siamese.png")
                    .SubCase(input.Contains("bengal"), "bengal.png")
                .Case(input.Contains("dog"), "dog.png")
                .Case(input.Contains("fish"))
                    .Case(input.Contains("tetra"), "tetra.png")
                    .Case(input.Contains("gourami"), "gourami.png")
                    .Break()
                .Case(input.Contains("bear"))
                    .SubCase(input.Contains("brown"))
                        .SubSubCase(input.Contains("irate"), p => p
                            .Case(input.Contains("fast"), "fast-irate-brown-bear.png")
                            .Case(input.Contains("deadly"), "deadly-irate-brown-bear.png")
                            .Default("generic-irate-brown-bear.png")
                        )
                        .SubSubCase(input.Contains("mild"), "mild-brown-bear.png")
                        .SubSubCase(input.Contains("calm"), p => p
                            .Case(input.Contains("fast"))
                                .SubCase(input.Contains("smart"))
                                    .SubSubCase(input.Contains("alpha"), "alpha-smart-fast-calm-brown-bear.png")
                                    .SubSubCase(input.Contains("beta"), p2 => p2
                                        .Case(input.Contains("super"), "super-beta-smart-fast-calm-brown-bear.png")
                                        .Case(input.Contains("duper"), "duper-beta-smart-fast-calm-brown-bear.png"))
                                    .SubSubDefault("generic-smart-fast-calm-brown-bear.png")
                                .SubCase(input.Contains("dumb"), "dumb-fast-calm-brown-bear.png")
                                .SubDefault("generic-fast-calm-brown-bear.png")
                            .Case(input.Contains("deadly"), "deadly-calm-brown-bear.png")
                            .Default("generic-calm-brown-bear.png"))
                    .SubCase(input.Contains("black"), "black-bear.png")
                    .SubDefault("generic-bear.png")
                .ResolveFirst();
        }


        /// <summary>
        /// Similar to usage example. Tests a wide range of mixed syntaxes
        /// </summary>
        [TestCase("neon tetra fish", Result = "neon-tetra.png")]
        [TestCase("blind cave tetra fish", Result = "blind-cave-tetra.png")]
        [TestCase("glowlight tetra fish", Result = "glowlight-tetra.png")]
        [TestCase("colombian tetra fish", Result = "colombian-tetra.png")]
        [TestCase("tetra fish", Result = "unidentified-swimming-tetra.png")]
        [TestCase("dwarf gourami fish", Result = "dwarf-gourami.png")]
        [TestCase("pearl gourami fish", Result = "pearl-gourami.png")]
        [TestCase("manx cat", Result = "manx.png")]
        [TestCase("lynx siamese cat", Result = "lynx.png")]
        [TestCase("lilac siamese cat", Result = "lilac.png")]
        [TestCase("unknown siamese cat", Result = "generic-siamese-cat.png")]
        [TestCase("angry bengal cat", Result = "angry-bengal-cat.png")]
        [TestCase("happy bengal cat", Result = "happy-bengal-cat.png")]
        [TestCase("dopey bengal cat", ExpectedException = typeof(InvalidOperationException))]
        [TestCase("dog", Result = "dog.png")]
        [TestCase("super beta smart fast calm brown bear", Result = "super-beta-smart-fast-calm-brown-bear.png")]
        [TestCase("gamma smart fast calm brown bear", Result = "generic-smart-fast-calm-brown-bear.png")]
        [TestCase("gamma morose fast calm brown bear", Result = "generic-fast-calm-brown-bear.png")]
        [TestCase("gamma morose fast calm yellow bear", Result = "generic-bear.png")]
        [TestCase("super beta smart fast calm brown beaver", ExpectedException = typeof(InvalidOperationException))]
        public string AllTogetherNow(string input)
        {
            return Pattern<string, string>
                .Match(input)
                // Normal subcase case syntax using Case/Default/Break
                .Case(input.Contains("fish"))
                .Case(input.Contains("tetra"))
                .Case(input.Contains("neon"), "neon-tetra.png")
                .Case(input.Contains("blind cave"), "blind-cave-tetra.png")
                // Results can also be specified using Case/Then syntax
                .Case(input.Contains("glowlight")).Then("glowlight-tetra.png")
                .Case(input.Contains("colombian"), "colombian-tetra.png")
                // Default case
                .Default("unidentified-swimming-tetra.png")
                .Break()
                // Subcases can also be specified via a lambda
                .Case(input.Contains("gourami"), p => p
                    .Case(input.Contains("dwarf"), "dwarf-gourami.png")
                    .Case(input.Contains("pearl"), "pearl-gourami.png")
                )
                .Break()
                // I'm sick of repeating 'input.Contains()' everywhere! So let's store a predicate so that we need only specify the keywords
                .WithPredicate(substring => input.Contains((string) substring))
                // Explicit subcase syntax
                .Case("cat")
                    .SubCase("manx").Then("manx.png")
                    .SubCase("siamese")
                        .SubSubCase("lynx", "lynx.png")
                        // Explicit subcase methods also support Case/Then syntax
                        .SubSubCase("lilac").Then("lilac.png")
                        .SubSubDefault("generic-siamese-cat.png")
                    // Lambda syntax for subcases can also supplemenent explicit subcases
                    .SubCase("bengal", p => p
                        .Case("angry", "angry-bengal-cat.png")
                        .Case("happy", "happy-bengal-cat.png")
                    )
                .Case("dog", "dog.png")
                .Case("bear")
                    .SubCase("brown")
                        .SubSubCase("irate", p => p
                            .Case("fast", "fast-irate-brown-bear.png")
                            .Case("deadly", "deadly-irate-brown-bear.png")
                            .Default("generic-irate-brown-bear.png")
                        )
                        .SubSubCase("mild", "mild-brown-bear.png")
                        .SubSubCase("calm", p => p
                            .Case("fast")
                                .SubCase("smart")
                                    .SubSubCase("alpha", "alpha-smart-fast-calm-brown-bear.png")
                                    .SubSubCase("beta", p2 => p2
                                        .Case("super", "super-beta-smart-fast-calm-brown-bear.png")
                                        .Case("duper", "duper-beta-smart-fast-calm-brown-bear.png"))
                                    .SubSubDefault("generic-smart-fast-calm-brown-bear.png")
                                .SubCase("dumb", "dumb-fast-calm-brown-bear.png")
                                .SubDefault("generic-fast-calm-brown-bear.png")
                            .Case("deadly", "deadly-calm-brown-bear.png")
                            .Default("generic-calm-brown-bear.png"))
                    .SubCase("black", "black-bear.png")
                    .SubDefault("generic-bear.png")
                .ResolveFirst();
        }  
 
    }

}
