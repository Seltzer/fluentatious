namespace CSharp.Fluency.Extensions.ObjectExt
{
    
    static class PipeExamples
    {
        internal static void Do()
        {
            WithoutPipe();
            WithPipe();
        }


        static void WithoutPipe()
        {
            var blah = Compute(SomeMethod(SomeMethod("dhdhdh", Foo(Bar("horatio"))), "catatatatat"));
        }


        static void WithPipe()
        {
            var blah = "horatio"
                .Pipe(Bar)
                .Pipe(Foo)
                .Pipe(str => SomeMethod("dhdhdh", str))
                .Pipe(str => SomeMethod("catatatat", str))
                .Pipe(Compute);
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
