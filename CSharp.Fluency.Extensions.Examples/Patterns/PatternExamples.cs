using System.Collections.Generic;
using System.Linq;
using CSharp.Fluency.Extensions.Extensions;
using CSharp.Fluency.Extensions.Patterns;

namespace CSharp.Fluency.Extensions.Examples.Patterns
{

    static class PatternExamples
    {
        internal static void Do()
        {
            const string input = "Gourami Fish";
            WithoutPatternsUsingIfElse(input);
            WithoutPatternsUsingDictionary(input);
            WithPatterns(input);
            
            AdvancedPatternsExample(input);
        }


        static int WithoutPatternsUsingIfElse(string d)
        {
            string picture = null;

            if (d.Contains("Dogs"))
            {
                picture = "dogs.png";
            }
            else if (d.Contains("Cats"))
            {
                picture = "cats.png";
            }
            else if (d.Contains("Fish"))
            {
                if (d.Contains("Gourami"))
                    picture = "gourami.png";
                else if (d.Contains("Tetra"))
                    picture = "tetra.png";
            }
            else
            {
                picture = "fallback.png";
            }

            return Foo(picture).Length;
        }


        static int WithoutPatternsUsingDictionary(string d)
        {
            return new Dictionary<bool, string>
            {
                { d.Contains("Dogs"), "dogs.png" },
                { d.Contains("Cats"), "cats.png" },
                { d.Contains("Fish") && d.Contains("Gourami"), "gourami.png" },
                { d.Contains("Fish") && d.Contains("Tetra"), "tetra.png" },
                { !d.Contains("Fish"), "fallback.png" }
            }
                .First(kvPair => kvPair.Key)
                .Value
                .Pipe(Foo)
                .Length;
        }
        

        static int WithPatterns(string s)
        {
            return Pattern<string, string>
                .Match(s)
                    .Case(s.Contains("Dogs"), "dogs.png")
                    .Case(s.Contains("Cats"), "cats.png")
                    .Case(s.Contains("Fish"))
                        .Case(s.Contains("Gourami"), "gourami.png")
                        .Case(s.Contains("Tetra"), "tetra.png")
                        .Break()
                    .Default("fallback.png")
                .ResolveFirst()
                .Pipe(Foo)
                .Length;
        }  


        static string AdvancedPatternsExample(string input)
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
                .WithPredicate(substring => input.Contains((string)substring))
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
 

        static string Foo(string input)
        {
            return input.Substring(1);
        }
    }

}
