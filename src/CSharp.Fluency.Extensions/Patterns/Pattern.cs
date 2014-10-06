using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Fluency.Extensions.Patterns
{

    public class Pattern<TMatch, TResult> : IPattern<TMatch, TResult>
    {
        /// <summary>
        /// Subject of this pattern which is fed into predicates added by the user
        /// </summary>
        readonly TMatch subject;

        readonly List<Case<TMatch, TResult>> cases = new List<Case<TMatch, TResult>>();
        
        /// <summary>
        /// This is assigned whenever a user adds an 'open' case, one which has a predicate but no result. An open case
        /// is a precursor either to a <see cref="Then"/> call or to a series of subcases.
        ///  
        /// <remarks>
        /// An open case and its ancestors implicitly represent a stack which we're deliberately not using for performance reasons.
        /// </remarks> 
        /// </summary>
        Case<TMatch, TResult> openCase;

        /// <summary>
        /// Is true if the caller is using explicit case syntax at the moment (i.e. SubCase/SubSubCase etc.)
        /// </summary>
        bool inExplicitCaseMode;
        
        
        Pattern(TMatch subject)
        {
            this.subject = subject;
        }
        

        /// <summary>
        /// Entry point for using a Pattern
        /// </summary>
        public static IPattern<TMatch, TResult> Match(TMatch subject)
        {
            return new Pattern<TMatch, TResult>(subject);
        }


        public IPattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, TResult result)
        {
            int? intendedLevel = null;
            if (inExplicitCaseMode)
            {
                intendedLevel = 1;
                inExplicitCaseMode = false;
            }

            AddClosedCase(predicate, result, intendedLevel);
            
            return this;
        }


        public IPattern<TMatch, TResult> Case(bool matches, TResult result)
        {
            return Case(_ => matches, result);
        }


        public IPattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null)
        {
            int? intendedLevel = null;
            if (inExplicitCaseMode)
            {
                intendedLevel = 1;
                inExplicitCaseMode = false;
            }

            AddOpenCase(predicate, subCases, intendedLevel);
            
            return this;
        }


        public IPattern<TMatch, TResult> Case(bool matches, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null)
        {
            return Case(_ => matches, subCases);
        }


        public IPattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, TResult result)
        {
            AddExplicitCase(() => AddClosedCase(predicate, result, 2));
            
            return this;
        }
        

        public IPattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, 
            Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null)
        {
            AddExplicitCase(() => AddOpenCase(predicate, subCases, 2));

            return this;
        }


        public IPattern<TMatch, TResult> SubCase(bool matches, TResult result)
        {
            return SubCase(_ => matches, result);
        }


        public IPattern<TMatch, TResult> SubCase(bool condition, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null)
        {
            return SubCase(_ => condition, subCases);
        }


        public IPattern<TMatch, TResult> SubDefault(TResult result)
        {
            return SubCase(true, result);
        }


        public IPattern<TMatch, TResult> SubSubCase(Func<TMatch, bool> predicate, TResult result)
        {
            AddExplicitCase(() => AddClosedCase(predicate, result, 3));

            return this;
        }


        public IPattern<TMatch, TResult> SubSubCase(bool matches, TResult result)
        {
            return SubSubCase(_ => matches, result);
        }


        public IPattern<TMatch, TResult> SubSubCase(Func<TMatch, bool> predicate, 
            Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null)
        {
            AddExplicitCase(() => AddOpenCase(predicate, subCases, 3));

            return this;
        }


        public IPattern<TMatch, TResult> SubSubCase(bool condition, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null)
        {
            return SubSubCase(_ => condition, subCases);
        }


        public IPattern<TMatch, TResult> SubSubDefault(TResult result)
        {
            return SubSubCase(true, result);
        }



        public IPattern<TMatch, TResult> Then(TResult result)
        {
            if (openCase == null)
                throw new InvalidOperationException("Syntax should be .Case(condition).Then(result)");

            openCase.Result = result;

            CloseCase();

            return this;
        }


        public IPattern<TMatch, TResult> Default(TResult result)
        {
            return Case(true, result);
        }


        public IPattern<TMatch, TResult> Break()
        {
            CloseCase();

            return this;
        }



        public TResult ResolveFirstOrDefault()
        {
            var matchingCase = cases.FirstOrDefault(c => c.AttemptToMatch(subject) != null);

            return matchingCase != null ? matchingCase.Result : default(TResult);
        }


        public TResult ResolveFirst()
        {
            return cases.Select(c => c.AttemptToMatch(subject)).First(c => c != null).Result;
        }

        
        void AddExplicitCase(Action action)
        {
            inExplicitCaseMode = true;

            action();
        }

        
        void AddOpenCase(Func<TMatch, bool> predicate, Func<IPattern<TMatch, TResult>, IPattern<TMatch, TResult>> subCases = null,
            int? intendedLevel = null)
        {
            var newCase = new Case<TMatch, TResult>(predicate);

            AddCase(newCase, intendedLevel);

            openCase = newCase;

            if (subCases != null)
            {
                new Pattern<TMatch, TResult>(subject).Pipe(subCases).CastTo<Pattern<TMatch, TResult>>().cases.ForEach(openCase.AddSubCase);
                
                Break();
            }
        }


        void AddClosedCase(Func<TMatch, bool> predicate, TResult result, int? intendedLevel = null)
        {
            var newCase = new Case<TMatch, TResult>(predicate, result);

            AddCase(newCase, intendedLevel);
        }
        
        
        void AddCase(Case<TMatch, TResult> newCase, int? intendedLevel = null)
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
        }


        void CloseCase()
        {
             if (openCase != null)
                openCase = openCase.Parent;
        }
    }

}
