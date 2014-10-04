using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp.Fluency.Extensions.Patterns
{

    class Case<T, U>
    {
        internal readonly U Result;
        internal readonly Case<T, U> ParentCase;
        
        readonly List<Case<T, U>> subCases = new List<Case<T, U>>();
        readonly Func<T, bool> predicate;
        

        internal Case(Func<T, bool> predicate, Case<T, U> parentCase = null)
        {
            this.predicate = predicate;
            ParentCase = parentCase;
        }


        internal Case(Func<T, bool> predicate, U result, Case<T, U> parentCase = null)
            : this(predicate, parentCase)
        {
            Result = result;
        }


        internal Case(bool predicate, Case<T, U> parentCase = null)
            : this(_ => predicate, parentCase)
        {
        }


        internal Case(bool predicate, U result, Case<T, U> parentCase = null)
            : this(_ => predicate, result, parentCase)
        {
        }


        internal void AddSubCase(Case<T, U> subCase)
        {
            subCases.Add(subCase);
        }


        internal Case<T, U> AttemptToMatch(T subject)
        {
            return predicate(subject)
                ? subCases.Any() ? subCases.Select(c => c.AttemptToMatch(subject)).FirstOrDefault(mr => mr != null) : this
                : null;
        }
    }

}
