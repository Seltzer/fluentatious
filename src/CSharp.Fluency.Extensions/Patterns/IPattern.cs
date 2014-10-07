using System;

namespace CSharp.Fluency.Extensions.Patterns
{
    
    /// <summary>
    /// WARNING: Experimental toy!
    /// 
    /// An IPattern is created by calling <see cref="Pattern{TSubject, TResult}.Match"/> and passing in a subject. An IPattern is essentially
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
    ///         - By using Case/Default/Break (normal subcase syntax). Break is required to eliminate ambiguity.
    ///         - By using Case/Default/SubCase/SubDefault/SubSubCase/SubSubDefault (explicit subcase syntax)
    ///         - As a supplement to the above two, by passing in a subCases lambda to Case/SubCase/SubSubCase        
    ///     - There are two different syntaxes for adding a predicate with an associated result
    ///         - .Case(condition).Then(result)
    ///         - .Case(condition, result)
    ///     - SetPredicate/Case syntax allows you to store a parameterised predicate via the SetPredicate call and repeatedly probe it with varying 
    ///       input via Case(). This is useful when you have a bunch of repetitive Case predicates which are practically identical aside from a 
    ///       changing input
    /// </summary>
    /// <example>
    /// return Pattern{string, string}
    ///         .Match(input)
    ///         .Case(input.Contains("cat"))
    ///             .Case(input.Contains("manx"), "manx.png")
    ///             .Case(input.Contains("bengal"), "bengal.png")
    ///             .Default("generic-cat.png")
    ///             .Break()
    ///         .Case(input.Contains("dog"), "dog.png")
    ///         .Case(input.Contains("fish"))
    ///             .Case(input.Contains("tetra"), "tetra.png")
    ///             .Case(input.Contains("gourami"), "gourami.png")
    ///             .Break()
    ///         .Default("generic-animal.png)
    ///         .ResolveFirst();
    /// </example>
    /// <typeparam name="TSubject">Type of the subject of this pattern</typeparam>
    /// <typeparam name="TResult">Type of the result which is returned when resolving this pattern</typeparam>
    public interface IPattern<TSubject, TResult>
    {
        /// <summary>
        /// Adds a case defined by the specified subject predicate and result.
        /// </summary>
        IPattern<TSubject, TResult> Case(Func<TSubject, bool> predicate, TResult result);
        
        /// <summary>
        /// Adds an open case defined by the specified predicate. Can be followed either by a <see cref="Then"/> call or subcases
        /// </summary>
        IPattern<TSubject, TResult> Case(Func<TSubject, bool> predicate);
        
        /// <summary>
        /// Convenience method for adding a case and subcases under it. 
        /// </summary>
        IPattern<TSubject, TResult> Case(Func<TSubject, bool> predicate, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases);
        
        // bool versions of the above three methods
        IPattern<TSubject, TResult> Case(bool condition, TResult result);
        IPattern<TSubject, TResult> Case(bool condition);
        IPattern<TSubject, TResult> Case(bool condition, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases);
      
        // Companion methods to Using. predicateArg will be fed into a predicate you've already stored via <see cref="Using"/>
        IPattern<TSubject, TResult> Case(object predicateArg, TResult result);
        IPattern<TSubject, TResult> Case(object predicateArg);
        IPattern<TSubject, TResult> Case(object predicateArg, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases);
     
        IPattern<TSubject, TResult> Default(TResult result);

        // Explicit subcase syntax - analogous to above Case methods
        IPattern<TSubject, TResult> SubCase(Func<TSubject, bool> predicate, TResult result);
        IPattern<TSubject, TResult> SubCase(Func<TSubject, bool> predicate);
        IPattern<TSubject, TResult> SubCase(Func<TSubject, bool> predicate, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases);
        IPattern<TSubject, TResult> SubCase(bool matches, TResult result);
        IPattern<TSubject, TResult> SubCase(bool condition, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases = null);
        IPattern<TSubject, TResult> SubCase(object predicateArg, TResult result);
        IPattern<TSubject, TResult> SubCase(object predicateArg);
        IPattern<TSubject, TResult> SubCase(object predicateArg, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases);
        IPattern<TSubject, TResult> SubDefault(TResult result);
        
        // Explicit subcase syntax
        IPattern<TSubject, TResult> SubSubCase(Func<TSubject, bool> predicate, TResult result);
        IPattern<TSubject, TResult> SubSubCase(bool matches, TResult result);
        IPattern<TSubject, TResult> SubSubCase(Func<TSubject, bool> predicate, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases = null);
        IPattern<TSubject, TResult> SubSubCase(bool condition, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases = null);
        IPattern<TSubject, TResult> SubSubCase(object predicateArg, TResult result);
        IPattern<TSubject, TResult> SubSubCase(object predicateArg);
        IPattern<TSubject, TResult> SubSubCase(object predicateArg, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases);
        IPattern<TSubject, TResult> SubSubDefault(TResult result);

        /// <summary>
        /// SetPredicate/Case syntax allows you to store a parameterised predicate via the Using call and repeatedly probe it with varying input via Case().
        /// This is useful when you have a bunch of repetitive Case predicates which are practically identical aside from a changing input
        /// </summary>
        IPattern<TSubject, TResult> SetPredicate(Func<object, bool> predicate);

        /// <summary>
        /// Follows an open case
        /// </summary>
        /// <example>xxx.Case(condition.Then(result)</example>
        IPattern<TSubject, TResult> Then(TResult result);
        
        /// <summary>
        /// Required when using normal subcase syntax. Is used to break out of a parent case tree. Is not required if its absence doesn't
        /// make the next call ambiguous.
        /// </summary>
        /// <returns></returns>
        IPattern<TSubject, TResult> Break();
        

        /// <summary>
        /// Resolves this pattern by running all of the predicates against the pattern subject. Returns the result associated with the first 
        /// case which matches.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If none of the cases match.</exception>
        TResult ResolveFirst();

        /// <summary>
        /// Similar to <see cref="ResolveFirst"/> but doesn't explode if no cases match.
        /// </summary>
        /// <returns>
        /// The result associated with the first matching case. Or default(TResult) if no cases match.
        /// </returns>
        TResult ResolveFirstOrDefault();
    }

}
