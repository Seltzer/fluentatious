using System;
using System.Collections.Generic;
using System.Linq;
using CSharp.Fluency.Extensions.Extensions;

namespace CSharp.Fluency.Extensions.Examples.Extensions.ObjectExt
{
    
    static class DoExamples
    {
        internal static void Do()
        {
            Console.WriteLine("Do examples");

            var strings = new[] { "aaa", "bbb", "cccc" };

            Console.WriteLine("\nresult = " + WithoutDo(strings) + "\n");
            Console.WriteLine("\nresult = " + WithDo(strings));
        }


        static int WithoutDo(IEnumerable<string> input)
        {
            var sum = input
                .Select(i => i + i)
                .Sum(str => str.Length);

            Console.WriteLine(sum);
            Console.Write(sum);
            
            return Math.Max(sum, 7);
        }


        static int WithDo(IEnumerable<string> input)
        {
            return input
                .Select(i => i + i)
                .Sum(str => str.Length)
                .Do(Console.WriteLine, Console.Write)
                .Pipe(i => Math.Max(i, 7));
        }
    }

}
