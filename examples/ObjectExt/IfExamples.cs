﻿using System.Collections.Generic;
using System.Linq;

namespace CSharp.Fluency.Extensions.ObjectExt
{
    
    static class IfExamples
    {
        internal static void Do()
        {
            var input = new[] { "aaaa", "aaaaa", "adkdkdkdk", "sdfsdfsdf" };
            const SortingCriteria sortingCriterion = SortingCriteria.Alphabetical;
            const SortingDirection sortingDirection = SortingDirection.Ascending;

            WithoutIf(input, sortingCriterion, sortingDirection);
            WithIf(input, sortingCriterion, sortingDirection);
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
    }

}
