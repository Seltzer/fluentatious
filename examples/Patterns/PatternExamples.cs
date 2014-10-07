using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Fluency.Extensions.Patterns
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
                picture = "dogs.png";
            else if (d.Contains("Cats"))
                picture = "cats.png";
            else if (d.Contains("Fish"))
            {
                if (d.Contains("Gourami"))
                    picture = "gourami.png";
                else if (d.Contains("Tetra"))
                    picture = "tetra.png";
            }
            else
                picture = "fallback.png";

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
            Func<string, bool> contains = input.Contains;

            return Pattern<string, string>
                .Match(input)
                // Normal subcase case syntax using Case/Default/Break
                .Case(contains("fish"))
                    .Case(contains("tetra"), "tetra.png")
                        .Case(contains("neon"), "neon-tetra.png")
                        .Case(contains("blind cave"), "blind-cave-tetra.png")
                        // Results can also be specified using Case/Then syntax
                        .Case(contains("glowlight")).Then("glowlight-tetra.png")
                        .Case(contains("colombian"), "colombian-tetra.png")
                        // Default case
                        .Default("unidentified-swimming-tetra.png")
                        .Break()
                    // Subcases can also be specified via a lambda
                    .Case(contains("gourami"), p => p
                        .Case(contains("dwarf"), "dwarf-gourami.png")
                        .Case(contains("pearl"), "pearl-gourami.png")
                    )
                    .Break()
                // Explicit subcase syntax
                .Case(contains("cat"))
                    .SubCase(contains("manx")).Then("manx.png")
                    .SubCase(contains("siamese"), "siamese.png")
                        .SubSubCase(contains("lynx"), "lynx.png")
                        // Explicit subcase methods also support Case/Then syntax
                        .SubSubCase(contains("lilac")).Then("lilac.png")
                        .SubSubDefault("generic-siamese-cat.png")
                    // Lambda syntax for subcases can also supplemenent explicit subcases
                    .SubCase(contains("bengal"), p => p
                        .Case(contains("angry"), "angry-bengal-cat.png")
                        .Case(contains("happy"), "happy-bengal-cat.png")
                    )
                .Case(contains("dog"), "dog.png")
                .Case("ocelot", "ocelot.png")
                .Case(contains("bear"))
                    .SubCase(contains("brown"))
                        .SubSubCase(contains("irate"), p => p
                            .Case(contains("fast"), "fast-irate-brown-bear.png")
                            .Case(contains("deadly"), "deadly-irate-brown-bear.png")
                            .Default("generic-irate-brown-bear.png")
                        )
                        .SubSubCase(contains("mild"), "mild-brown-bear.png")
                        .SubSubCase(contains("calm"), p => p
                            .Case(contains("fast"))
                                .SubCase(contains("smart"))
                                    .SubSubCase(contains("alpha"), "alpha-smart-fast-calm-brown-bear.png")
                                    .SubSubCase(contains("beta"), p2 => p2
                                        .Case(contains("super"), "super-beta-smart-fast-calm-brown-bear.png")
                                        .Case(contains("duper"), "duper-beta-smart-fast-calm-brown-bear.png"))
                                    .SubSubDefault("generic-smart-fast-calm-brown-bear.png")
                                .SubCase(contains("dumb"), "dumb-fast-calm-brown-bear.png")
                                .SubDefault("generic-fast-calm-brown-bear.png")
                            .Case(contains("deadly"), "deadly-calm-brown-bear.png")
                            .Default("generic-calm-brown-bear.png"))
                    .SubCase(contains("black"), "black-bear.png")
                    .SubDefault("generic-bear.png")
                .ResolveFirst();
        }  
 

        static string Foo(string input)
        {
            return input.Substring(1);
        }
    }

}
