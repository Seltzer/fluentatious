using System;

namespace CSharp.Fluency.Extensions.Patterns
{
    
    /// <summary>
    /// An IPattern is created by calling <see cref="Pattern{TMatch, TResult}.Match"/> and passing in a subject. An IPattern is essentially
    /// a fluent substitute for a series of if/then statements or a switch statement, whose branches all conditionally assign different values to
    /// a local var. See the below example, or the usage examples elsewhere.
    ///   
    /// An IPattern basically consists of a bunch of ordered cases, where each case is composed of:
    ///     - A condition or a predicate which is to be run against the subject of the pattern
    ///     - An associated result or a bunch of associated subcases.  
    /// 
    /// Notes:
    ///     - An IPattern is executed by calling <see cref="ResolveFirst"/> or <see cref="ResolveFirstOrDefault"/>. 
    ///       When this happens, all of the predicates will be run against the subject in order until one (with a result) matches.
    ///     - Nested/default cases are supported
    ///     - For convenience, there are three different syntaxes for adding subcases and they can be mixed:
    ///         - By using Case/Default/Break
    ///         - By using Case/Default/SubCase/SubDefault/SubSubCase/SubSubDefault (explicit subcase syntax)
    ///         - As a supplement to the above two, by passing in a subCases lambda to Case/SubCase/SubSubCase        
    ///     - There are two different syntaxes for adding a predicate with an associated result
    ///         - .Case(condition).Then(result)
    ///         - .Case(condition, result)
    /// </summary>
    /// <example>
    /// return Pattern{string, string}
    ///         .Match(input)
    ///         .Case(input.Contains("cat"))
    ///             .Case(input.Contains("manx"), "manx.png")
    ///             .Case(input.Contains("siamese"), "siamese.png")
    ///             .Case(input.Contains("bengal"), "bengal.png")
    ///             .Default("generic-cat.png")
    ///             .Break()
    ///         .Case(input.Contains("dog"), "dog.png")
    ///         .Case(input.Contains("fish"))
    ///             .Case(input.Contains("tetra"), "tetra.png")
    ///             .Case(input.Contains("gourami"), "gourami.png")
    ///             .Break()
    ///         .ResolveFirst();
    /// </example>
    /// <typeparam name="TMatch"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IPattern<TMatch, TResult>
    {
        IPattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, TResult result);
        IPattern<TMatch, TResult> Case(bool matches, TResult result);
        // TODO: Remove optional parameter
        IPattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null);
        IPattern<TMatch, TResult> Case(bool matches, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null);
     
        // Explicit subcase syntax
        IPattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, TResult result);
        IPattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null);
        IPattern<TMatch, TResult> SubCase(bool matches, TResult result);
        IPattern<TMatch, TResult> SubCase(bool condition, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null);
        IPattern<TMatch, TResult> SubDefault(TResult result);
        
        // Explicit subcase syntax
        IPattern<TMatch, TResult> SubSubCase(Func<TMatch, bool> predicate, TResult result);
        IPattern<TMatch, TResult> SubSubCase(bool matches, TResult result);
        IPattern<TMatch, TResult> SubSubCase(Func<TMatch, bool> predicate, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null);
        IPattern<TMatch, TResult> SubSubCase(bool condition, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null);
        IPattern<TMatch, TResult> SubSubDefault(TResult result);
        
        IPattern<TMatch, TResult> Then(TResult result);
        IPattern<TMatch, TResult> Default(TResult result);
        IPattern<TMatch, TResult> Break();
        
        TResult ResolveFirstOrDefault();
        TResult ResolveFirst();
    }

}
