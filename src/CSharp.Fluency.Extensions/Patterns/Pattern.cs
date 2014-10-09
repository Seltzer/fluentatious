using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Fluency.Extensions.Patterns
{

    /// <summary>
    /// Implementation of <see cref="IPattern{TSubject, TResult}"/>
    /// </summary>
    /// <remarks>
    ///     - Has a fluent interface, hence almost all public methods return 'this'. 
    ///     - In order to make things concise and minimal, even private methods do the same.
    /// </remarks>
    public class Pattern<TSubject, TResult> : IPattern<TSubject, TResult>
    {
        /// <summary>
        /// Subject of this pattern which is fed into predicates added by the user
        /// </summary>
        readonly TSubject subject;

        readonly List<Case<TSubject, TResult>> cases = new List<Case<TSubject, TResult>>();

        /// <summary>
        /// This is assigned whenever a user adds an 'open' case, one which has a predicate but no result. An open case
        /// is a precursor either to a <see cref="Then"/> call or to a series of subcases.
        ///  
        /// <remarks>
        /// An open case and its ancestors implicitly represent a stack which we're deliberately not using for performance reasons.
        /// </remarks> 
        /// </summary>
        Case<TSubject, TResult> openCase;

        /// <summary>
        /// Is true if the caller is using explicit case syntax at the moment (i.e. SubCase/SubSubCase etc.)
        /// </summary>
        bool inExplicitCaseMode;


        /// <summary>
        /// Assigned by <see cref="WithPredicate"/>
        /// </summary>
        Func<object, bool> predicate;


        Pattern(TSubject subject)
        {
            this.subject = subject;
        }


        /// <summary>
        /// Entry point for using a Pattern
        /// </summary>
        public static IPattern<TSubject, TResult> Match(TSubject subject)
        {
            return new Pattern<TSubject, TResult>(subject);
        }


        public IPattern<TSubject, TResult> Case(Func<TSubject, bool> predicate, TResult result)
        {
            return Case(intendedLevel => AddClosedCase(predicate, result, intendedLevel));
        }
        

        public IPattern<TSubject, TResult> Case(Func<TSubject, bool> predicate)
        {
            return Case(intendedLevel => AddOpenCase(predicate, intendedLevel: intendedLevel));
        }


        public IPattern<TSubject, TResult> Case(Func<TSubject, bool> predicate, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return Case(intendedLevel => AddOpenCase(predicate, subCases, intendedLevel));
        }


        public IPattern<TSubject, TResult> Case(bool condition, TResult result)
        {
            return Case(_ => condition, result);
        }


        public IPattern<TSubject, TResult> Case(bool condition)
        {
            return Case(_ => condition);
        }


        public IPattern<TSubject, TResult> Case(bool condition, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return Case(_ => condition, subCases);
        }


        public IPattern<TSubject, TResult> Case(object predicateArg, TResult result)
        {
            return Case(_ => predicate(predicateArg), result);
        }


        public IPattern<TSubject, TResult> Case(object predicateArg)
        {
            return Case((Func<TSubject, bool>) (_ => predicate(predicateArg)));
        }


        public IPattern<TSubject, TResult> Case(object predicateArg, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return Case(_ => predicate(predicateArg), subCases);
        }

          
        public IPattern<TSubject, TResult> Default(TResult result)
        {
            return Case(true, result);
        }



        #region Explicit syntax - Sub level

        public IPattern<TSubject, TResult> SubCase(Func<TSubject, bool> predicate, TResult result)
        {
            return AddExplicitCase(() => AddClosedCase(predicate, result, 2));
        }


        public IPattern<TSubject, TResult> SubCase(Func<TSubject, bool> predicate)
        {
            return AddExplicitCase(() => AddOpenCase(predicate, intendedLevel: 2));
        }


        public IPattern<TSubject, TResult> SubCase(Func<TSubject, bool> predicate, 
            Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return AddExplicitCase(() => AddOpenCase(predicate, subCases, 2));
        }


        public IPattern<TSubject, TResult> SubCase(bool matches, TResult result)
        {
            return SubCase(_ => matches, result);
        }


        public IPattern<TSubject, TResult> SubCase(bool condition)
        {
            return SubCase(_ => condition);
        }


        public IPattern<TSubject, TResult> SubCase(bool condition, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return SubCase(_ => condition, subCases);
        }


        public IPattern<TSubject, TResult> SubCase(object predicateArg, TResult result)
        {
            return SubCase(s => predicate(predicateArg), result);
        }


        public IPattern<TSubject, TResult> SubCase(object predicateArg)
        {
            return SubCase(s => predicate(predicateArg));
        }


        public IPattern<TSubject, TResult> SubCase(object predicateArg, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            if (predicate == null)
                throw new InvalidOperationException("No predicate is set");

            return SubCase(_ => predicate(predicateArg), subCases);
        }


        public IPattern<TSubject, TResult> SubDefault(TResult result)
        {
            return SubCase(true, result);
        }

        #endregion


        #region Explicit syntax - SubSub level


        public IPattern<TSubject, TResult> SubSubCase(Func<TSubject, bool> predicate, TResult result)
        {
            return AddExplicitCase(() => AddClosedCase(predicate, result, 3));
        }
        

        public IPattern<TSubject, TResult> SubSubCase(Func<TSubject, bool> predicate)
        {
            return AddExplicitCase(() => AddOpenCase(predicate, intendedLevel: 3));
        }


        public IPattern<TSubject, TResult> SubSubCase(Func<TSubject, bool> predicate, 
            Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return AddExplicitCase(() => AddOpenCase(predicate, subCases, 3));
        }

        
        public IPattern<TSubject, TResult> SubSubCase(bool matches, TResult result)
        {
            return SubSubCase(_ => matches, result);
        }


        public IPattern<TSubject, TResult> SubSubCase(bool condition)
        {
            return SubSubCase(_ => condition);
        }
        

        public IPattern<TSubject, TResult> SubSubCase(bool condition, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return SubSubCase(_ => condition, subCases);
        }


        public IPattern<TSubject, TResult> SubSubCase(object predicateArg, TResult result)
        {
            return SubSubCase(_ => predicate(predicateArg), result);
        }


        public IPattern<TSubject, TResult> SubSubCase(object predicateArg)
        {
            return SubSubCase(_ => predicate(predicateArg));
        }


        public IPattern<TSubject, TResult> SubSubCase(object predicateArg, Func<IPattern<TSubject, TResult>, IPattern<TSubject, TResult>> subCases)
        {
            return SubSubCase(_ => predicate(predicateArg), subCases);
        }

        public IPattern<TSubject, TResult> SubSubDefault(TResult result)
        {
            return SubSubCase(true, result);
        }

        #endregion


        public IPattern<TSubject, TResult> WithPredicate(Func<object, bool> predicate)
        {
            this.predicate = predicate;

            return this;
        }


        public IPattern<TSubject, TResult> Then(TResult result)
        {
            if (openCase == null)
                throw new InvalidOperationException("Syntax should be .Case(condition).Then(result)");

            openCase.Result = result;

            // Call Break in order to close the case for which we've just provided a result
            return Break();
        }
        

        public IPattern<TSubject, TResult> Break()
        {
            if (openCase != null)
                openCase = openCase.Parent;
            
            return this;
        }



        public TResult ResolveFirstOrDefault()
        {
            var matchingCase = cases.FirstOrDefault(c => c.AttemptToMatch(subject) != null);

            return matchingCase != null ? matchingCase.Result : default(TResult);
        }


        public TResult ResolveFirst()
        {
            var matchingCase = cases.Select(c => c.AttemptToMatch(subject)).FirstOrDefault(c => c != null);
            if (matchingCase == null)
                throw new InvalidOperationException("No case matches.");

            return matchingCase.Result;
        }


        /// <summary>
        /// Helper used by public Case methods
        /// </summary>
        Pattern<TSubject, TResult> Case(Action<int?> action)
        {
            int? intendedLevel = null;
            if (inExplicitCaseMode)
            {
                // We've just exited an explicit subcase (e.g. SubCase, SubDefault, SubSubCase, SubSubDefault etc.)
                inExplicitCaseMode = false;
                intendedLevel = 1;
            }

            action(intendedLevel);

            return this;
        }
        

        /// <summary>
        /// Helper for adding explicit sub-cases
        /// </summary>
        Pattern<TSubject, TResult> AddExplicitCase(Action action)
        {
            inExplicitCaseMode = true;

            action();

            return this;
        }

        
        /// <summary>
        /// Helper for adding an open case (one which has a predicate but no result)
        /// </summary>
        /// <param name="intendedLevel">
        /// If this isn't specified, the case will be added as a subcase of the currently open case, or at the top level if no case is open.
        /// </param>
        Pattern<TSubject, TResult> AddOpenCase(Func<TSubject, bool> predicate, Func<IPattern<TSubject, TResult>, 
            IPattern<TSubject, TResult>> subCases = null, int? intendedLevel = null)
        {
            var newCase = new Case<TSubject, TResult>(predicate);

            AddCase(newCase, intendedLevel);

            openCase = newCase;

            if (subCases != null)
            {
                // Apply subCases action to a new pattern, then absorb its generated subcases
                new Pattern<TSubject, TResult>(subject)
                    // FIXME: Hackiness follows
                    .Do(p => p.WithPredicate(this.predicate))
                    .Pipe(subCases)
                    .CastTo<Pattern<TSubject, TResult>>()
                    .cases.ForEach(openCase.AddSubCase);
                
                Break();
            }

            return this;
        }


        /// <summary>
        /// Helper for adding a closed case
        /// </summary>
        /// <param name="intendedLevel">
        /// If this isn't specified, the case will be added as a subcase of the currently open case, or at the top level if no case is open.
        /// </param>
        Pattern<TSubject, TResult> AddClosedCase(Func<TSubject, bool> predicate, TResult result, int? intendedLevel = null)
        {
            var newCase = new Case<TSubject, TResult>(predicate, result);

            return AddCase(newCase, intendedLevel);
        }
        
  
        /// <summary>
        /// Adds a case at the appropriate level, determined by <see cref="intendedLevel"/> and the currently open case.
        /// Used by <see cref="AddOpenCase"/> and <see cref="AddClosedCase"/>
        /// </summary>
        /// <param name="intendedLevel">
        /// If this isn't specified, the case will be added as a subcase of the currently open case, or at the top level if no case is open.
        /// </param>
        Pattern<TSubject, TResult> AddCase(Case<TSubject, TResult> newCase, int? intendedLevel = null)
        {
            // Insert breaks which are implied when transitioning between levels of explicit cases
            if (intendedLevel.HasValue)
            {
                while (openCase != null && openCase.Depth != (intendedLevel.Value - 1))
                    Break();   
            }
            
            if (openCase != null)
                openCase.AddSubCase(newCase);
            else
                cases.Add(newCase);

            return this;
        }
    }

}
