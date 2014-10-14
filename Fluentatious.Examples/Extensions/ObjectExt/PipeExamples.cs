using System;
using Fluentatious.Extensions;

namespace Fluentatious.Examples.Extensions.ObjectExt
{
    
    static class PipeExamples
    {
        internal static void Do()
        {
            Console.WriteLine("Pipe examples");

            WithoutPipe();
            WithPipe();
            WithPipe2();
        }


        static void WithoutPipe()
        {
            var blah = Compute(SomeMethod(SomeMethod("dhdhdh", Foo(Bar("horatio"))), "catatatat"));
            Console.WriteLine(blah);
        }


        static void WithPipe()
        {
            var blah = "horatio"
                .Pipe(Bar)
                .Pipe(Foo)
                .Pipe(str => SomeMethod("dhdhdh", str))
                .Pipe(str => SomeMethod(str, "catatatat"))
                .Pipe(Compute)
                .Do(Console.WriteLine);
        }


        static void WithPipe2()
        {
            var blah = "horatio"
                .Pipe(Bar, Foo, str => SomeMethod("dhdhdh", str), str => SomeMethod(str, "catatatat"), Compute)
                .Do(Console.WriteLine);
        }


        static string SomeMethod(string horatio, string blah)
        {
            return horatio + blah;
        }


        static string Foo(string input)
        {
            return input.Substring(1);
        }


        static string Bar(string input)
        {
            return input.Substring(1, 3);
        }


        static int Compute(string input)
        {
            return input.Length;
        }
    }

}
