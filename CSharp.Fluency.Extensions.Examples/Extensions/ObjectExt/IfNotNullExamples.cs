using System;
using CSharp.Fluency.Extensions.Extensions;

namespace CSharp.Fluency.Extensions.Examples.Extensions.ObjectExt
{
    
    static class IfNotNullExamples
    {
        internal static void Do()
        {
            Console.WriteLine("IfNotNull examples");

            var nested = new A
            {
                Broccoli = new B
                {
                    Carrot = new C
                    {
                        Value = "sdfsdf"    
                    }
                }
            };
            
            WithoutIfNotNull1(nested);
            WithIfNotNull2(nested);
            InFutureCSharp(nested);
        }


        static void WithoutIfNotNull1(A input)
        {
            var value = input != null && input.Broccoli != null && input.Broccoli.Carrot != null 
                ? (input.Broccoli.Carrot.Value + "asdfasdf")
                : null;
            Console.WriteLine(value);
        }


        static void WithIfNotNull2(A input)
        {
            input.IfNotNull(i => i.Broccoli).IfNotNull(b => b.Carrot).IfNotNull(c => c.Value).IfNotNull(v => v + "asdfasdf").Do(Console.WriteLine);
        }


        static void InFutureCSharp(A input)
        {
            // input?.Broccoli?.Carrot?.Value.IfNotNull(v => v + "asdfasdf").Do(Console.WriteLine);
        }
    }

}
