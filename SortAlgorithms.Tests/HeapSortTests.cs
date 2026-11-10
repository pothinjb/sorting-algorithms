using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against <see cref="HeapSort"/>, with both the Floyd and the naive max-heapify,
    /// and against the sub-array version <see cref="HeapSort.SortRange"/>.
    /// </summary>
    [TestClass]
    public class HeapSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "HeapSort",
            "HeapSortUsingNaiveHeapify",
            "HeapSortRange",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "HeapSort")
                HeapSort.Sort(array);
            else if (methodName == "HeapSortUsingNaiveHeapify")
                HeapSort.SortUsingNaiveMaxHeapify(array);
            else if (methodName == "HeapSortRange")
                HeapSort.SortRange(array, 0, array.Length - 1);
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
                var instance = new HeapSortTests();
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
        /// <see cref="HeapSort.SortRange"/> sorts the sub-array only and leaves the elements around it untouched.
        /// </summary>
        [TestMethod]
        public void SortRange_SortsOnlyTheSubArray()
        {
            Random rnd = new(12345);
            int[] original = Enumerable.Range(0, 200).Select(_ => rnd.Next(0, 1000)).ToArray();
            const int p = 37, r = 151;

            int[] result = (int[])original.Clone();
            HeapSort.SortRange(result, p, r);

            int[] expected = (int[])original.Clone();
            Array.Sort(expected, p, r - p + 1);
            CollectionAssert.AreEqual(expected, result, "HeapSort.SortRange must sort A[p...r] and nothing else");
        }
    }
}