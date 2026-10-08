using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the merge sort family: <see cref="MergeSort"/>, <see cref="SkipMergeSort"/>, <see cref="MergeInsertionSort"/>, <see cref="MergeQuartetSort"/> and <see cref="OptimizedMergeSort"/>, sequential and parallel.
    /// </summary>
    [TestClass]
    public class MergeSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "MergeSort",            
            "ParallelMergeSort",
            "SkipMergeSort",
            "MergeInsertionSort",
            "MergeQuartetSort",
            "OptimizedMergeSort",
            "ParallelOptimizedMergeSort",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "MergeSort")
                MergeSort.Sort(array);
            else if (methodName == "SkipMergeSort")
                SkipMergeSort.Sort(array);
            else if (methodName == "MergeInsertionSort")
            {
                var sort = new MergeInsertionSort();
                sort.Sort(array, 32);
            }
            else if (methodName == "MergeQuartetSort")
            {
                var sort = new MergeQuartetSort();
                sort.Sort(array, 32);
            }
            else if (methodName == "OptimizedMergeSort")
            {
                var sort = new OptimizedMergeSort();
                sort.Sort(array);
            }
            else if (methodName == "ParallelMergeSort")
                MergeSort.ParallelSort(array);
            else if (methodName == "ParallelOptimizedMergeSort")
            {
                var sort = new OptimizedMergeSort();
                sort.ParallelSort(array);
            }
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
                var instance = new MergeSortTests();
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
        /// Regression test: the merge uses <see cref="int.MaxValue"/> as a sentinel, so real
        /// <see cref="int.MaxValue"/> values in the input must neither crash the sort nor corrupt the result.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_ArrayWithIntMaxValue(string methodName)
        {
            int[] small = { 5, 1, 2, int.MaxValue, int.MaxValue, 3, int.MaxValue, 0 };
            AssertSorted(methodName, small, ApplySort(methodName, (int[])small.Clone()));

            Random rnd = new(42);
            for (int n = 1; n <= 2000; n *= 3)
            {
                // about a quarter of the values are int.MaxValue
                int[] original = new int[n];
                for (int i = 0; i < n; ++i)
                    original[i] = rnd.Next(4) == 0 ? int.MaxValue : rnd.Next(100);
                AssertSorted($"{methodName} (n = {n})", original, ApplySort(methodName, (int[])original.Clone()));
            }
        }
    }
}