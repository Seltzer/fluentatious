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
        readonly TMatch subject;
        readonly List<Case<TMatch, TResult>> cases = new List<Case<TMatch, TResult>>();
        
        // Stack implicitly represented by currentParentCase and its parent case etc.
        Case<TMatch, TResult> currentParentCase;
        

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
            AddCase(new Case<TMatch, TResult>(predicate, result));
            
            return this;
        }


        public Pattern<TMatch, TResult> Case(bool matches, TResult result)
        {
            return Case(_ => matches, result);
        }


        public Pattern<TMatch, TResult> Case(Func<TMatch, bool> predicate, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            var newCase = new Case<TMatch, TResult>(predicate);

            AddCase(newCase);

            currentParentCase = newCase;

            if (subCases != null)
            {
                subCases(this);
                Break();
            }

            return this;
        }


        public Pattern<TMatch, TResult> Case(bool matches, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases = null)
        {
            return Case(_ => matches, subCases);
        }


        public Pattern<TMatch, TResult> SubCase(Func<TMatch, bool> predicate, Func<Pattern<TMatch, TResult>, Pattern<TMatch, TResult>> subCases)
        {
            throw new NotImplementedException();
        }


        public Pattern<TMatch, TResult> Default(TResult result)
        {
            Case(_ => true, result);
            
            return this;
        }


        public Pattern<TMatch, TResult> Break()
        {
            if (currentParentCase != null)
                currentParentCase = currentParentCase.ParentCase;

            return this;
        }



        //public U ResolveFirstOrDefault()
        //{
        //    return dispatchMap.First(kvPair => kvPair.Key(subject)).Value;
        //}


        public TResult ResolveFirst()
        {
            return cases.First(c => c.AttemptToMatch(subject) != null).Result;
        }
        

        void AddCase(Case<TMatch, TResult> newCase)
        {
            if (currentParentCase != null)
                currentParentCase.AddSubCase(newCase);
            else
                cases.Add(newCase);
        }
    }

}
