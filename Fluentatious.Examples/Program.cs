using System;
using System.Collections.Generic;
using Fluentatious.Examples.Extensions.ObjectExt;
using Fluentatious.Examples.Patterns;

namespace Fluentatious.Examples
{

    class Program
    {
        static void Main(string[] args)
        {
            new List<Action>
            {
                DoExamples.Do,
                IfExamples.Do,
                NullSafeExamples.Do,
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
