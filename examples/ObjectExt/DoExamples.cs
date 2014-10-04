using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Fluency.Extensions.ObjectExt
{
    
    static class DoExamples
    {
        internal static void Do()
        {
            var strings = new[] { "aaa", "bbb", "cccc" };

            WithoutDo(strings);
            WithDo(strings);
        }


        static int WithoutDo(IEnumerable<string> input)
        {
            var sum = input
                .Select(i => i + i)
                .Sum(str => str.Length);

            Console.WriteLine(sum);
            
            return Math.Max(sum, 7);
        }


        static int WithDo(IEnumerable<string> input)
        {
            return input
                .Select(i => i + i)
                .Sum(str => str.Length)
                .Do(Console.WriteLine)
                .Pipe(i => Math.Max(i, 7));
        }
    }

}
