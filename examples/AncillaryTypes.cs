namespace CSharp.Fluency.Extensions
{

    internal enum SortingCriteria
    {
        Alphabetical,
        LengthBased
    }


    internal enum SortingDirection
    {
        Ascending,
        Descending
    }


    internal class Foo
    {
        internal readonly string Bar;
        internal readonly int Length;
        

        internal Foo(string str)
        {
            Bar = str;
            Length = str.Length;
        }
    }


    internal class A
    {
        internal B Broccoli { get; set; }
    }


    internal class B
    {
        internal C Carrot { get; set; }
    }


    internal class C
    {
        internal string Value { get; set; }
    }

}
