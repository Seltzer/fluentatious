csharp-fluency-extensions
=========================

Fluent extensions to C# which help you to achieve a higher level of fluency, generally favouring 
functional style over imperative. 

## Motivation
Most of the C# I write is rather LINQ heavy. Readability / maintainability benefits aside, LINQ is a pleasure to use 
and I find myself missing its functional and fluent nature whenever I'm writing non-LINQ code in C#. And I find it 
especially jarring when I'm force to depart from functional coding and intersperse bits of imperative code. So here
are some of the fluent extensions / constructs which I've written in the past couple of years. 

## Aims
* Achieving fluency - to have nice code which flows from left to right like a pipeline in point-free style. No visual
jumping around necessary.
* Having fewer unnecessary local variables
* Mutating variables less often

## TODO
* Patterns:
    * Wire up usage examples to main
    * Error handling ain't great
* Do / Tap / Pipe:
    * Add Tap alias
    * Add 'expressiveness' tests
    * Change Do / Pipe to take params of actions/funcs
* Add IfElse
* README

## Docs
All of the features below have usage examples and tests present in the code. Most are implemented as extensions methods 
on Object,
aside from Pattern.


---
### If (object extension)
#### Motivation (code without If)

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

There are many things I don't like about this piece of code. I don't like that my nice functional LINQ expression has 
been cleaved in two by some imperative logic. And I don't like that I have to declare an unnecessary foo variable and 
mutate it at various points.

#### If Solution / Usage
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

Note that If can take either a bool or a predicate (i.e. Func\*<bool>\*).



---
### Pipe (object extension)
The non-pipe version isn't very point-free - it has a bunch of nested function calls whose parentheses are slightly painful to follow. You could make it more readable by assigning intermediate results to local variables, but that's something I'd preferably avoid too. Or you can use Pipe!
    
#### Motivation (code without Pipe)

#### Pipe Solution / Usage

Pipe is pretty simple really... you can interpret it as 'take my input, pipe it into this function, return the result. 
It's very easy to follow the flow of data and function calls. If you wanted, you could write a version of Pipe 
which takes multiple transformations in serial, rather than having to repeatedly call it.


---
### Do/Tap (object extension)
Do / Tap is inspired by the Underscore JS Tap function, of which Ruby also has an equivalent. It's very similar to Pipe with one important difference. Instead of piping the input into a Func and returning the result, it executes an Action against it and then returns the input value. It's not particularly functional in nature, but it's useful for intercepting a method chain and performing some action like so:
    
#### Motivation (code without Do/Tap)

#### Do/Tap Solution / Usage

Much easier than breaking up a method chain...



---
### IfNotNull (object extension)
    
I hate having to perform tedious null checking like this so I defined an object extension called IfNotNull which I use 
as below. It's a pretty simple example of continuation-style passing really. If the input to IfNotNull isn't null, 
its expression will be evaluated to produce something new... otherwise the entire expression will return null. 
Basically, we're executing a series of expressions until a failure occurs at any point. It makes the code considerably 
cleaner and allows us to achieve a nice fluent flow in point-free fashion.

    var value = input != null && input.Broccoli != null && input.Broccoli.Carrot != null 
        ? (input.Broccoli.Carrot.Value + "asdfasdf")
        : null;

    input.IfNotNull(i => i.Broccoli).IfNotNull(b => b.Carrot).IfNotNull(c => c.Value).IfNotNull(v => v + "asdfasdf").Do(Console.WriteLine);

Microsoft is actually planning to release a new language feature in C# 6 to address this very issue. You'll be able to use it like this: input?.Broccoli?.Carrot?.Value. People refer to it as monadic null checking, because it's similar in behaviour to the monadic bind operation in functional languages like Haskell.


---
### Patterns
Patterns...
I suspect this one could occupy an entire post on its own - it's basically a fluent equivalent of switch, vaguely similar to pattern matching in the functional world. I've seen a few attempts on the net at writing one of these... this is mine. I was worried about whether I was going overboard here, so as a litmus test for readability, I asked a few colleagues whether they could understand this piece of code.

A few notes:

Supports predicates in the form of bools and Func<bool>s,
Supports cases, sub-cases and default cases
Final method is named ResolveFirst and not Resolve to make the precedence of ordering clear.
Could split ...Case(predicate, result) into ...Case(predicate).Yields(result)
 

 