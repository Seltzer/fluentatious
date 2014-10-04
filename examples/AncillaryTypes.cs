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

}
