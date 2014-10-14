using System;
using CSharp.Fluency.Extensions.Extensions;

namespace CSharp.Fluency.Extensions.Examples.Extensions.ObjectExt
{
    
    static class CastAndAsExamples
    {
        internal static void Do()
        {
            Console.WriteLine("Cast / As examples");

            WithoutCastOrAs();
            WithCastAndAs();
        }


        static void WithoutCastOrAs()
        {
            var result1 = Foo(Foo((string) Foo((string) Foo("horatio"))) as string);
            Console.WriteLine(result1);

            var result2 = ((string) ("horatio".Pipe(Foo))).Substring(1).ToLowerInvariant().Trim();
            Console.WriteLine(result2);
        }


        static void WithCastAndAs()
        {
            var result1 = Foo("horatio").CastTo<string>().Pipe(Foo).CastTo<string>().Pipe(Foo).As<string>().Pipe(Foo);
            Console.WriteLine(result1);
            
            var result2 = "horatio".Pipe(Foo).CastTo<string>().Substring(1).ToLowerInvariant().Trim();
            Console.WriteLine(result2);
        }


        static object Foo(string input)
        {
            return input.Substring(1);
        }
    }

}
