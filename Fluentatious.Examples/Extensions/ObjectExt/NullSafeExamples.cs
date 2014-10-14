using System;
using CSharp.Fluency.Extensions.Extensions;

namespace CSharp.Fluency.Extensions.Examples.Extensions.ObjectExt
{
    
    static class NullSafeExamples
    {
        internal static void Do()
        {
            Console.WriteLine("NullSafe examples");

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
            
            WithoutNullSafe(nested);
            WithNullSafe1(nested);
            WithNullSafe2(nested);
            InFutureCSharp(nested);
        }


        static void WithoutNullSafe(A input)
        {
            var value = input != null && input.Broccoli != null && input.Broccoli.Carrot != null 
                ? (input.Broccoli.Carrot.Value + "asdfasdf")
                : null;
            Console.WriteLine(value);
        }


        static void WithNullSafe1(A input)
        {
            input.NullSafe(i => i.Broccoli).NullSafe(b => b.Carrot).NullSafe(c => c.Value).NullSafe(v => v + "asdfasdf").Do(Console.WriteLine);
        }


        static void WithNullSafe2(A input)
        {
            input.NullSafe(i => i.Broccoli, b => b.Carrot, c => c.Value, v => v + "asdfasdf").Do(Console.WriteLine);
        }


        static void InFutureCSharp(A input)
        {
            // input?.Broccoli?.Carrot?.Value?.(v => v + "asdfasdf").Do(Console.WriteLine);
        }
    }

}
