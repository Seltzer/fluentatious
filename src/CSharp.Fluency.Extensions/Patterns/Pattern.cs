using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp.Fluency.Extensions.Patterns
{

    /// <summary>
    /// TODO: Add switch/case groups
    /// TODO: Compare with if/else branches
    /// TODO: Compare with Dictionary syntax
    /// </summary>
    /// <typeparam name="TMatch"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <example>
    ///  string result = Pattern{SPWeb, string}
    ///         .Match(guidanceElementWeb)
    ///         .Case(AdmHelper.IsInsideDesignThinking, "sdfsdf")
    ///         .Case(AdmHelper.IsInsideProjectType, "sdfsdf")
    ///         .Case(w => AdmHelper.IsInsideProjectType(w) && w.GetParentOrAncestor(3).Name == "neighbourhoods", "fff")
    ///         .Case(AdmHelper.IsInsideDesignThinking, "sdfsdf")
    ///         .Resolve();
    /// 
    /// </example>
    public class Pattern<TMatch, TResult>
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


        public static Pattern<TMatch, TResult> Match(TMatch subject)
        {
            return new Pattern<TMatch, TResult>(subject);
        }


        public Pattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, TResult result)
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


        public Pattern<TMatch, TResult> Case(bool matches, TResult result)
        {
            return Case(_ => matches, result);
        }


        public Pattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
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


        public Pattern<TMatch, TResult> Case(bool matches, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            return Case(_ => matches, subCases);
        }


        public Pattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, TResult result)
        {
            AddExplicitCase(2, "Syntax should be .Case(condition).SubCase(...)", "Can only subcase a top level case", 
                () => AddClosedCase(predicate, result));
            
            return this;
        }


        public Pattern<TMatch, TResult> SubCase(bool matches, TResult result)
        {
            return SubCase(_ => matches, result);
        }


        public Pattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, 
            Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            AddExplicitCase(2, "Syntax should be .Case(condition).SubCase(...)", "Can only subcase a top level case", 
                () => Case(predicate, subCases));

            return this;
        }


        public Pattern<TMatch, TResult> SubCase(bool condition, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            return SubCase(_ => condition, subCases);
        }



        public Pattern<TMatch, TResult> SubSubCase(Func<TMatch, bool> predicate, TResult result)
        {
            AddExplicitCase(2, "Syntax should be .Case(condition).SubCase(condition).SubSubCase(...)", "Can only subsubcase a subcase", 
                () => AddClosedCase(predicate, result));

            return this;
        }


        public Pattern<TMatch, TResult> SubSubCase(bool matches, TResult result)
        {
            return SubCase(_ => matches, result);
        }


        public Pattern<TMatch, TResult> SubSubCase(Func<TMatch, bool> predicate, 
            Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            AddExplicitCase(2, "Syntax should be .Case(condition).SubCase(condition).SubSubCase(...)", "Can only subsubcase a subcase", 
                () => Case(predicate, subCases));

            return this;
        }


        public Pattern<TMatch, TResult> SubSubCase(bool condition, 
            Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            return SubCase(_ => condition, subCases);
        }


        public Pattern<TMatch, TResult> Then(TResult result)
        {
            if (openCase == null)
                throw new InvalidOperationException("Syntax should be .Case(condition).Then(result)");

            openCase.Result = result;

            CloseCase();

            return this;
        }


        public Pattern<TMatch, TResult> Default(TResult result)
        {
            Case(_ => true, result);
            
            return this;
        }


        public Pattern<TMatch, TResult> Break()
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

        
        void AddExplicitCase(int depth, string noOpenCaseError, string openCaseIsWrongDepthError, Action action)
        {
            if (openCase == null)
                throw new InvalidOperationException(noOpenCaseError);

            if (openCase.Depth != depth - 1)
                throw new InvalidOperationException(openCaseIsWrongDepthError);

            inExplicitCaseMode = true;

            action();
        }

        
        void AddOpenCase(Func<TMatch, bool> predicate, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null,
            int? intendedLevel = null)
        {
            var newCase = new Case<TMatch, TResult>(predicate);

            AddCase(newCase, intendedLevel);

            openCase = newCase;

            if (subCases != null)
            {
                subCases(this);
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
            if (intendedLevel.HasValue && openCase != null && openCase.Depth == intendedLevel.Value)
                Break();
            
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
