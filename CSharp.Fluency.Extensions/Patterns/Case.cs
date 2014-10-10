using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Fluency.Extensions.Patterns
{

    class Case<T, U>
    {
        internal int Depth
        {
            get { return 1 + (Parent != null ? Parent.Depth : 0); }
        }

        internal U Result { get; set; }
        
        internal Case<T, U> Parent { get; set; }

        readonly List<Case<T, U>> subCases = new List<Case<T, U>>();

        readonly Func<T, bool> predicate;
        

        internal Case(Func<T, bool> predicate)
        {
            this.predicate = predicate;
        }


        internal Case(Func<T, bool> predicate, U result)
            : this(predicate)
        {
            Result = result;
        }


        internal Case(bool predicate)
            : this(_ => predicate)
        {
        }


        internal Case(bool predicate, U result)
            : this(_ => predicate, result)
        {
        }


        internal void AddSubCase(Case<T, U> subCase)
        {
            subCase.Parent = this;
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
