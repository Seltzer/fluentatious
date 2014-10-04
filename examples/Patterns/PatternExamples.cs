namespace CSharp.Fluency.Extensions.Patterns
{

    class Ting
    {
        internal string Url { get; set; }
    }


    static class PatternExamples
    {
        internal static void Do()
        {
            PatternExample1("sdfggouramisdfg");
            RandomExample();
        }


        static int PatternExample1(string d)
        {
            string picture = null;

            if (d.Contains("Dogs"))
                picture = "dogs.png";
            else if (d.Contains("Cats"))
                picture = "cats.png";
            else if (d.Contains("fish"))
            {
                if (d.Contains("Gourami"))
                    picture = "gourami.jpg";
                else if (d.Contains("Tetra"))
                    picture = "tetra.jpg";
            }
            else if (d.Contains("Fish") && d.Contains("Gourami"))
                picture = "gourami.jpg";
            else if (d.Contains("Fish") && d.Contains("Tetra"))
                picture = "tetra.jpg";
            else
                picture = "fallback.gif";

            return Foo(picture).Length;
        }


        static void RandomExample()
        {
            var ting = new Ting { Url = "http://www.google.co.nz" };
            
            var picUrl = Pattern<Ting, string>
                .Match(ting)
                    .Case(t => t.Url.Contains("asdfasdf"), "asdfasdf.jpg")
                    .Case(_ => true)
                        .Case(_ => true, "sdkdkd.jpg")
                        .Case(_ => true, "ddddd.jpg")
                        .Break()
                    .Case(_ => true, c => c
                        .Case(_ => true)
                        .Case(_ => true)
                        .Case(_ => true, cc =>
                            cc.Case(_ => true, "sdfsdf"))
                    )
                    .Case(_ => true)
                        .Case(_ => true, "sdkdkd.jpg")
                        .Case(_ => true, "ddddd.jpg")
                        .Default("fish.jpg")
                    .Case(_ => true, "dkdkdk.jpg")
                    .Default("horatio")
                    .ResolveFirst()
                .Pipe(fileName => @"C:\Animals\" + fileName);
        }  
 

        static string Foo(string input)
        {
            return input.Substring(1);
        }
    }

}
