using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Base class for the sorting tests. A derived class lists the variants to test in
    /// <see cref="SortMethods"/>, maps each name to a call in <see cref="ApplySort"/>, and runs
    /// <see cref="RunAllCommonTests"/> for each name.
    /// </summary>
    public abstract class GenericSortTests
    {
        /// <summary>
        /// Names of the sort variants to test (defined by the derived class).
        /// </summary>
        protected abstract string[] SortMethods { get; }

        /// <summary>
        /// Applies the named sort variant to the given array.
        /// </summary>
        /// <param name="methodName">Variant name, one of <see cref="SortMethods"/>.</param>
        /// <param name="array">Array to sort. In-place sorts modify it directly.</param>
        /// <returns>The sorted array: <paramref name="array"/> itself, or a new array for sorts that do not work in place.</returns>
        /// <exception cref="ArgumentException">The name is not a known variant.</exception>
        protected abstract int[] ApplySort(string methodName, int[] array);

        private static readonly int[] SampleArray = { 1, 9, 3, 7, 2, 6, 11, 14, 10, 6, 8, 19, 22 };
        private static readonly int[] SortedArray = { 1, 2, 3, 6, 6, 7, 8, 9, 10, 11, 14, 19, 22 };

        /// <summary>Maximum size of the random arrays.</summary>
        const int N = 1000;

        /// <summary>
        /// Asserts that <paramref name="result"/> is <paramref name="original"/> in ascending order.
        /// </summary>
        /// <param name="methodName">Variant name, used in the failure message.</param>
        /// <param name="original">Input array (not modified).</param>
        /// <param name="result">Array returned by the sort.</param>
        protected static void AssertSorted(string methodName, int[] original, int[] result)
        {
            int[] expected = (int[])original.Clone();
            Array.Sort(expected);
            AssertAscendingOrder(methodName, result);
            CollectionAssert.AreEqual(expected, result, $"{methodName} did not return the input elements in ascending order");
        }

        /// <summary>
        /// Asserts that <paramref name="sorted"/> is in non-decreasing order.
        /// </summary>
        /// <param name="methodName">Variant name, used in the failure message.</param>
        /// <param name="sorted">Array to check.</param>
        protected static void AssertAscendingOrder(string methodName, int[] sorted)
        {
            for (int i = 0; i < sorted.Length - 1; i++)
            {
                Assert.IsTrue(sorted[i] <= sorted[i + 1],
                    $"{methodName} failed at index {i} (values {sorted[i]} > {sorted[i + 1]})");
            }
        }

        /// <summary>
        /// Runs the whole common suite for one variant: empty array, 1 to 3 elements, a fixed sample,
        /// random arrays, random arrays with many duplicates, descending, ascending and all-equal arrays.
        /// </summary>
        /// <param name="methodName">Variant name, one of <see cref="SortMethods"/>.</param>
        /// <param name="maxRange">
        /// Exclusive upper bound of the random values. Lower it for sorts limited by the value range
        /// (such as counting sort).
        /// </param>
        protected void RunAllCommonTests(string methodName, int maxRange = 100 * N)
        {
            Sort_EmptyArray(methodName);
            Sort_SingleElementArray(methodName);
            Sort_TwoElementsArray(methodName);
            Sort_ThreeElementsArray(methodName);
            Sort_SampleArray(methodName);
            Sort_RandomArrays(methodName, maxRange);
            Sort_RandomArraysWithDuplicates(methodName);
            Sort_WorstCaseDescendingArray(methodName);
            Sort_BestCaseAscendingArray(methodName);
            Sort_EqualArray(methodName);
        }

        // --- Reusable individual tests ---------------------

        /// <summary>Sorting an empty array returns an empty, non-null array.</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_EmptyArray(string methodName)
        {
            int[] result = ApplySort(methodName, Array.Empty<int>());
            Assert.IsNotNull(result, $"The result must not be null for {methodName}");
            Assert.AreEqual(0, result.Length, $"The array must stay empty for {methodName}");
        }

        /// <summary>Sorting a one-element array leaves it unchanged.</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_SingleElementArray(string methodName)
        {
            int[] single = { 42 };
            int[] result = ApplySort(methodName, single);
            Assert.IsNotNull(result, $"The result must not be null for {methodName}");
            Assert.AreEqual(1, result.Length, $"The array must have exactly one element for {methodName}");
            Assert.AreEqual(42, result[0], $"The single element must stay unchanged for {methodName}");
        }

        /// <summary>Sorts both orderings of a two-element array.</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_TwoElementsArray(string methodName)
        {
            CollectionAssert.AreEqual(new[] { 1, 2 }, ApplySort(methodName, new[] { 1, 2 }), methodName);
            CollectionAssert.AreEqual(new[] { 1, 2 }, ApplySort(methodName, new[] { 2, 1 }), methodName);
        }

        /// <summary>Sorts all six orderings of a three-element array.</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_ThreeElementsArray(string methodName)
        {
            int[][] permutations =
            {
                new[] { 1, 2, 3 }, new[] { 1, 3, 2 }, new[] { 2, 1, 3 },
                new[] { 2, 3, 1 }, new[] { 3, 1, 2 }, new[] { 3, 2, 1 },
            };
            foreach (int[] arr in permutations)
            {
                string input = string.Join(", ", arr);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ApplySort(methodName, arr), $"{methodName} on {{{input}}}");
            }
        }

        /// <summary>Sorting a fixed sample (with a duplicate) gives the expected sorted array.</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_SampleArray(string methodName)
        {
            int[] result = ApplySort(methodName, (int[])SampleArray.Clone());
            CollectionAssert.AreEqual(SortedArray, result);
        }

        /// <summary>
        /// Sorts 100 random arrays (one of size <see cref="N"/>, the others of random sizes from 4 to <see cref="N"/>)
        /// and checks each result against <see cref="Array.Sort(Array)"/>.
        /// </summary>
        /// <param name="methodName">Variant name.</param>
        /// <param name="maxRange">Exclusive upper bound of the random values.</param>
        protected void Sort_RandomArrays(string methodName, int maxRange)
        {
            Random rnd = new();
            for (int i = 0; i < 100; i++)
            {
                // generate a random array
                int n = (i == 0) ? N : rnd.Next(4, N);
                int[] original = Enumerable.Range(0, n).Select(_ => rnd.Next(0, maxRange)).ToArray();
                int[] result = ApplySort(methodName, (int[])original.Clone());
                AssertSorted($"{methodName} (generation {i + 1}, n = {n})", original, result);
            }
        }

        /// <summary>
        /// Same as <see cref="Sort_RandomArrays"/>, but values are drawn below <c>N / 10</c>,
        /// so each array holds many duplicates.
        /// </summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_RandomArraysWithDuplicates(string methodName)
        {
            Random rnd = new();
            for (int i = 0; i < 100; i++)
            {
                // generate a random array
                int n = (i == 0) ? N : rnd.Next(4, N);
                int[] original = Enumerable.Range(0, n).Select(_ => rnd.Next(0, N / 10)).ToArray();
                int[] result = ApplySort(methodName, (int[])original.Clone());
                AssertSorted($"{methodName} (generation {i + 1}, n = {n})", original, result);
            }
        }

        /// <summary>Sorts a descending array (the worst case for many algorithms).</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_WorstCaseDescendingArray(string methodName)
        {
            int[] arr = Enumerable.Range(1, 100).Reverse().ToArray();
            int[] result = ApplySort(methodName, (int[])arr.Clone());
            AssertSorted(methodName, arr, result);
        }

        /// <summary>Sorts an already ascending array (the best case for many algorithms).</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_BestCaseAscendingArray(string methodName)
        {
            int[] arr = Enumerable.Range(1, 100).ToArray();
            int[] result = ApplySort(methodName, (int[])arr.Clone());
            AssertSorted(methodName, arr, result);
        }

        /// <summary>Sorts an array whose elements are all equal.</summary>
        /// <param name="methodName">Variant name.</param>
        protected void Sort_EqualArray(string methodName)
        {
            int[] arr = { 1, 1, 1, 1, 1, 1, 1 };
            int[] result = ApplySort(methodName, arr);
            CollectionAssert.AreEqual(new[] { 1, 1, 1, 1, 1, 1, 1 }, result, methodName);
        }
    }
}
