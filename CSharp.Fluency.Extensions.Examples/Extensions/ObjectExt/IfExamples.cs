using System;
using System.Collections.Generic;
using System.Linq;
using CSharp.Fluency.Extensions.Extensions;

namespace CSharp.Fluency.Extensions.Examples.Extensions.ObjectExt
{
    
    static class IfExamples
    {
        internal static void Do()
        {
            Console.WriteLine("If examples");

            var input = new[] { "aaaa", "aaaaa", "adkdkdkdk", "sdfsdfsdf" };
            const SortingCriteria sortingCriterion = SortingCriteria.Alphabetical;
            const SortingDirection sortingDirection = SortingDirection.Ascending;
            
            WithoutIf(input, sortingCriterion, sortingDirection).Do(Print);
            WithIf(input, sortingCriterion, sortingDirection).Do(Print);
        }


        static IEnumerable<Foo> WithoutIf(IEnumerable<string> input, SortingCriteria sortingCriterion, SortingDirection sortingDirection)
        {
            var foos = input
                .Where(str => str.StartsWith("a"))
                .Select(str => new Foo(str));

            if (sortingCriterion == SortingCriteria.Alphabetical)
                foos = foos.OrderBy(f => f.Bar);
            else if (sortingCriterion == SortingCriteria.LengthBased)
                foos = foos.OrderBy(f => f.Length);

            if (sortingDirection == SortingDirection.Descending)
                foos = foos.Reverse();

            return foos
                .Skip(1)
                .Take(5)
                .ToList();
        }   


        static IEnumerable<Foo> WithIf(IEnumerable<string> input, SortingCriteria sortingCriterion, SortingDirection sortingDirection)
        {
            return input
                .Where(str => str.StartsWith("a"))
                .Select(str => new Foo(str))
                .If(sortingCriterion == SortingCriteria.Alphabetical, foos => foos.OrderBy(f => f.Bar))
                .If(sortingCriterion == SortingCriteria.LengthBased, foos => foos.OrderBy(f => f.Length))
                .If(() => sortingDirection == SortingDirection.Descending, Enumerable.Reverse)
                .Skip(1)
                .Take(5)
                .ToList();
        }


        static void Print(IEnumerable<Foo> foos)
        {
            foos.Select(f => f.Bar).Pipe(bars => String.Join(", ", bars)).Do(Console.WriteLine);
        }
    }

}
