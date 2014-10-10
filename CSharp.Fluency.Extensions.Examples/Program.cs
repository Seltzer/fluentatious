using System;
using System.Collections.Generic;
using CSharp.Fluency.Extensions.Examples.Extensions.ObjectExt;
using CSharp.Fluency.Extensions.Examples.Patterns;

namespace CSharp.Fluency.Extensions.Examples
{

    class Program
    {
        static void Main(string[] args)
        {
            new List<Action>
            {
                DoExamples.Do,
                IfExamples.Do,
                IfNotNullExamples.Do,
                PipeExamples.Do,
                CastAndAsExamples.Do,
                PatternExamples.Do
            }
                .ForEach(a =>
                {
                    Console.Write("*** ");
                    a();
                    Console.Write("\n\n");
                });

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(false);
        }
    }

}
