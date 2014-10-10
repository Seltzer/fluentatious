csharp-fluency-extensions
=========================

## Motivation
Most of the C# I write is rather LINQ heavy. Readability / maintainability benefits aside, LINQ is a pleasure to use 
and I find myself missing its functional and fluent nature whenever I'm writing non-LINQ code in C#. And I find it 
especially jarring when I'm force to depart from functional coding and intersperse bits of imperative code. So here
are some of the fluent extensions / constructs which I've written in the past couple of years. 

## What C# Fluency Extensions is:
A library of object extensions and constructs which aim to make it easier to write fluent and functional C#, even when
you're not working with LINQ. It aims to help you:
* Achieve fluency - to have nice code which flows from left to right like a pipeline in point-free style. No visual
jumping around necessary.
* Have fewer unnecessary local variables
* Mutate variables less often

## What C# Fluency Extensions isn't:
* An attempt to turn C# into a functional language by emulating functional constructs such as option types,
monads etc.
* A framework to aid in writing fluent interfaces of your own.
 

## Overview
All of the features below have usage examples and tests present in the code. Most are implemented as extensions methods 
on Object, aside from Pattern.


---
### If / Unless (object extension)

There are many things I don't like about the below piece of code. I don't like that my nice functional LINQ expression has 
been cleaved in two by some imperative logic. And I don't like that I have to declare an unnecessary foos variable and 
mutate it at various points.

    // Without If
    var foos = input
        .Where(str => str.StartsWith("a"))
        .Select(str => new Foo(str));

    if (sortingCriterion == SortingCriteria.Alphabetical)
        foos = foos.OrderBy(f => f.Bar);
    else if (sortingCriterion == SortingCriteria.LengthBased)
        foos = foos.OrderBy(f => f.Length);

    if (sortingDirection == SortingDirection.Descending)
        foos = foos.Reverse();

    return foos
        .Skip(1)
        .Take(5)
        .ToList();

Now, if only LINQ offered some sort of If expression... well, I initially implemented an IEnumerable extension called 
'If', but later generalised it as an object extension. See the result below:

    return input
        .Where(str => str.StartsWith("a"))
        .Select(str => new Foo(str))
        .If(sortingCriterion == SortingCriteria.Alphabetical, foos => foos.OrderBy(f => f.Bar))
        .If(sortingCriterion == SortingCriteria.LengthBased, foos => foos.OrderBy(f => f.Length))
        .If(() => sortingDirection == SortingDirection.Descending, Enumerable.Reverse)
        .Skip(1)
        .Take(5)
        .ToList();

Note that If can take either a bool or a predicate (i.e. Func`<bool>`). Also, 'Unless' is provided as a convenience
method which operates identically to If, except that the condition/predicate is inverted.
    

---
### Pipe (object extension)

Pipe is pretty simple really... you can interpret it as 'take my input, pipe it into this function, return the result. 
It's very easy to follow the flow of data and function calls. Pipe is intended to be used in a functional manner; i.e.
the specified transform functions should not mutate their input.
    
See the example below and observe that the non-pipe version isn't very point-free. It has a bunch of nested 
function calls whose parentheses are painful to follow. We coould could make it more readable by assigning intermediate 
results to local variables, but that's something we're also trying to avoid. Or in the name of minimalism and 
readability, we could use Pipe!
    
    // Without Pipe
    var blah = Compute(SomeMethod(SomeMethod("dhdhdh", Foo(Bar("horatio"))), "catatatat"));

    // Using Pipe 
    var blah = "horatio"
        .Pipe(Bar)
        .Pipe(Foo)
        .Pipe(str => SomeMethod("dhdhdh", str))
        .Pipe(str => SomeMethod(str, "catatatat"))
        .Pipe(Compute)

    // Using Pipe in another way (you can combine Pipes as you see fit)
     var blah = "horatio".Pipe(Bar, Foo, str => SomeMethod("dhdhdh", str), str => SomeMethod(str, "catatatat"), Compute)
    
---
### Do / Tap (object extensions)
Do is inspired by the Underscore JS Tap function, of which Ruby also has an equivalent. It's actually quite 
similar to Pipe with one important difference. Instead of piping the input into a Func and returning the result, 
it performs any number of Actions on it in the order specified, and then returns the original object. Tap is 
provided as an alias for Do.

Do is intended to be fluent, but not necessarily functional. It's implied that any of the Actions can mutate 
the input, although this isn't necessarily the case. Case in point... imagine you have this code and you want to 
print the result of Sum for debugging purposes.
    
    // Original
    var length = new[] { "go", "tuk", "hodor" }
        .Select(i => i + i)
        .Sum(str => str.Length)
        .Pipe(i => Math.Max(i, 7);

Without Do/Tap, you'd split it like so:

    // Without Do/Tap
    var length = new[] { "go", "tuk", "hodor" }
        .Select(i => i + i)
        .Sum(str => str.Length);

    Console.WriteLine(length);

    return length.Pipe(i => Math.Max(i, 7);
    
Annoying. For the price of copious of keystrokes, you've managed to cleave your nice method chain into three and
introduce an unnecessary local var. All just to print the value of a variable. And then you have to clean up afterwards. 
However, if you have a construct like Do/Tap, you can jump in an intercept the method chain to cleanly inject your 
logging. Using Do / Tap:

    // With Do / Tap
    return new[] { "go", "tuk", "hodor" }
        .Select(i => i + i)
        .Sum(str => str.Length)
        .Tap(Console.WriteLine)
        .Pipe(i => Math.Max(i, 7));
    
    
---
### IfNotNull (object extension)
    
Performing tedious null checking is annoying, so I defined an object extension called IfNotNull which I use 
as below. It's a pretty simple example of continuation-style passing really. If the input to IfNotNull isn't null, 
its expression will be evaluated to produce something new... otherwise the entire expression will return null. 
Basically, we're executing a series of expressions until a failure occurs at any point. It makes the code considerably 
cleaner and allows us to achieve a nice fluent flow in point-free fashion.

    // Without IfNotNull
    var value = input != null && input.Broccoli != null && input.Broccoli.Carrot != null 
        ? (input.Broccoli.Carrot.Value + "asdfasdf")
        : null;

    // With IfNotNull
    var value = input.IfNotNull(i => i.Broccoli).IfNotNull(b => b.Carrot).IfNotNull(c => c.Value).IfNotNull(v => v + "asdfasdf").Do(Console.WriteLine);

Microsoft is actually planning to release a new language feature in C# 6 to address this very issue. You'll be able 
to use it as per below. People refer to it as monadic null checking, because it's similar in behaviour to the 
monadic bind operation in functional languages like Haskell. It's also similar to the way promises work in jQuery - 
promises (can) allow you to string together synchronous/asynchronous actions which execute as normal until failure 
occurs.

    // Future C# syntax
    var value = input?.Broccoli?.Carrot?.Value.IfNotNull(v => v + "asdfasdf").Do(Console.WriteLine);


---
### CastTo / As (object extensions)
CastTo and As are pretty self-explanatory. They're simply method equivalents of intrinsic C# operators    

// Without CastTo / As
var result1 = Foo(Foo((string) Foo((string) Foo("horatio"))) as string);
var result2 = ((string) ("horatio".Pipe(Foo))).Substring(1).ToLowerInvariant().Trim();
        
// With CastTo / As / Pipe
var result1 = Foo("horatio").CastTo`<string>`().Pipe(Foo).CastTo`<string>`().Pipe(Foo).As`<string>`().Pipe(Foo);
var result2 = "horatio".Pipe(Foo).CastTo`<string>`().Substring(1).ToLowerInvariant().Trim();

---
### Patterns
WARNING! Patterns are an elaborate feature I'm experimenting with at the moment. While I have used them in 
production code, they're more of a toy at this stage, and I think they only lead to more readable code in a few cases. 

A Pattern (as in pattern matching) is essentially a fluent substitute for a series of if/then statements or a switch
statement, whose branches all conditionally assign different values to a local var. It is essentially a series
of ordered cases, where each case is composed of:
    # A condition or a predicate which is to be run against the subject of the pattern
    # An associated result or a bunch of associated subcases.  

Patterns support:

* Two syntaxes for specifying predicates and associating them with results:
    * Case(condition, result)
    * Case(condition).Then(result)
* Default cases
* Subcases using three different syntaxes, which can be mixed:
    * Normal syntax:    Case / Default / Break (break is required to eliminate ambiguity)
    * Explicit syntax:  Case / Default / SubCase / SubDefault / SubSubCase / SubSubDefault
    * Lambda syntax:    As a supplement to the above two, subcases can by passing in a subcases lambda.
* A means of storing a parameterised predicate so that you need only pass in the thing which changes.
    Useful when you have a bunch of repetitive Case predicates which are practically identical aside from a 
    changing input.

Example without patterns:

    // Without patterns
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


Example which realises the above as a mapping from bool conditions to string results:

    // Without patterns #2
    return new Dictionary`<Func<string, bool>, string>`
    {
        { str => str.Contains("Dogs"), "dogs.png" },
        { str => str.Contains("Cats"), "cats.png" },
        { str => str.Contains("Fish") && d.Contains("Gourami"), "gourami.png" },
        { str => str.Contains("Fish") && d.Contains("Tetra"), "tetra.png" },
        { str => !str.Contains("Fish") && d.Contains("Tetra"), "tetra.png" },
        { str => !str.Contains("Fish"), "fallback.png" }
    }
        .FirstOrDefault(kvPair => kvPair.Key(d))
        .Value
        .Pipe(Foo)
        .Length;

Example which uses Pattern:

    // With patterns
    return Pattern`<string, string>`
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

Super extreme example which shows off the capabilities of patterns:

    return Pattern`<string, string>`
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