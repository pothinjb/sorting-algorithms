using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against <see cref="QuickSort"/> with each partition scheme (Hoare, Lomuto, naive).
    /// </summary>
    [TestClass]
    public class QuickSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "QuickSort_Hoare",
            "QuickSort_Lomuto",
            "QuickSort_Naive",            
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "QuickSort_Hoare")
                new QuickSort().Sort(array, QuickSortMethod.Hoare);
            else if (methodName == "QuickSort_Lomuto")
                new QuickSort().Sort(array, QuickSortMethod.Lomuto);
            else if (methodName == "QuickSort_Naive")
                new QuickSort().Sort(array, QuickSortMethod.Naive);
            else
                throw new ArgumentException($"Unknown sort method: {methodName}");
            return array;
        }

        /// <summary>
        /// Supplies each name in <see cref="SortMethods"/> to MSTest as one test row.
        /// </summary>
        public static IEnumerable<object[]> SortMethodData
        {
            get
            {
                var instance = new QuickSortTests();
                foreach (var name in instance.SortMethods)
                    yield return new object[] { name };
            }
        }

        /// <summary>
        /// Runs the common test suite for one sort variant.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Run_Generic_Tests(string methodName)
        {
            RunAllCommonTests(methodName);
        }

        /// <summary>
        /// Regression test: sorted and reversed inputs produce maximally unbalanced partitions. The sort runs on a
        /// thread with a 256 KB stack, which a recursion depth proportional to n (about 10,000 frames) would overflow.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_UnbalancedPartitions_DoesNotOverflowTheStack(string methodName)
        {
            const int n = 10000;
            int[] ascending = Enumerable.Range(0, n).ToArray();
            int[] descending = Enumerable.Range(0, n).Reverse().ToArray();
            int[] resultAscending = null, resultDescending = null;

            var thread = new Thread(() =>
            {
                resultAscending = ApplySort(methodName, (int[])ascending.Clone());
                resultDescending = ApplySort(methodName, (int[])descending.Clone());
            }, 256 * 1024);
            thread.Start();
            thread.Join();

            AssertSorted($"{methodName} (ascending)", ascending, resultAscending);
            AssertSorted($"{methodName} (descending)", descending, resultDescending);
        }
    }
}