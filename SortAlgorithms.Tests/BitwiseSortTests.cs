using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the bitwise (binary radix) sorts: <see cref="BitwiseSort"/> and the <see cref="OptimizedBitwiseSort"/> variants,
    /// plus tests specific to binary radix sorts (bits that are the same in every element, large values, negative values).
    /// </summary>
    [TestClass]
    public class BitwiseSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "BitwiseSort",
            "OptimizedBitwiseSort",
            "OptimizedBitwiseSort1",
            "OptimizedBitwiseSort2",
            "OptimizedBitwiseSort3",
            "OptimizedBitwiseSort4",
            "OptimizedBitwiseSort5",
            "OptimizedBitwiseSort6",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "BitwiseSort")
                BitwiseSort.Sort(array);
            else if (methodName == "OptimizedBitwiseSort")
                OptimizedBitwiseSort.Sort(array);
            else if (methodName == "OptimizedBitwiseSort1")
                OptimizedBitwiseSort._Sort1(array);
            else if (methodName == "OptimizedBitwiseSort2")
                OptimizedBitwiseSort._Sort2(array);
            else if (methodName == "OptimizedBitwiseSort3")
                OptimizedBitwiseSort._Sort3(array);
            else if (methodName == "OptimizedBitwiseSort4")
                OptimizedBitwiseSort._Sort4(array);
            else if (methodName == "OptimizedBitwiseSort5")
                OptimizedBitwiseSort._Sort5(array);
            else if (methodName == "OptimizedBitwiseSort6")
                OptimizedBitwiseSort._Sort6(array);
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
                var instance = new BitwiseSortTests();
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
        /// Sorts random arrays of every size from 0 to 64, so that every remainder of the unrolled
        /// loops (n mod 4) is met, with group sizes of any value at each pass.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_EverySmallSize(string methodName)
        {
            Random rnd = new(1);
            for (int n = 0; n <= 64; n++)
            {
                for (int trial = 0; trial < 20; trial++)
                {
                    int[] original = Enumerable.Range(0, n).Select(_ => rnd.Next(0, 64)).ToArray();
                    int[] result = ApplySort(methodName, (int[])original.Clone());
                    AssertSorted($"{methodName} (n = {n}, trial {trial + 1})", original, result);
                }
            }
        }

        /// <summary>
        /// Sorts arrays in which some bits are the same in every element: low bits always set, a high
        /// bit always set, or a single bit that varies. These passes are skipped by the optimized variants.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_BitsSameInEveryElement(string methodName)
        {
            Random rnd = new(2);
            foreach (int n in new[] { 2, 3, 5, 100, 1001 })
            {
                // bits 0-2 always 101, bit 24 always 1, bits 3 and above vary
                int[] constantLowAndHigh = Enumerable.Range(0, n).Select(_ => (1 << 24) + 8 * rnd.Next(0, 1000) + 5).ToArray();
                AssertSorted($"{methodName} (constant low and high bits, n = {n})", constantLowAndHigh,
                    ApplySort(methodName, (int[])constantLowAndHigh.Clone()));

                // only bit 29 varies
                int[] singleVaryingBit = Enumerable.Range(0, n).Select(_ => 7 + (rnd.Next(0, 2) << 29)).ToArray();
                AssertSorted($"{methodName} (single varying bit, n = {n})", singleVaryingBit,
                    ApplySort(methodName, (int[])singleVaryingBit.Clone()));
            }
        }

        /// <summary>
        /// Sorts non-negative values spread over the whole <see cref="int"/> range, including 0 and
        /// <see cref="int.MaxValue"/>, so that all 31 value bits are processed.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_FullRangeValues(string methodName)
        {
            Random rnd = new(3);
            foreach (int n in new[] { 2, 7, 1000, 4099 })
            {
                int[] original = Enumerable.Range(0, n).Select(_ => rnd.Next(0, 4) switch
                {
                    0 => int.MaxValue,
                    1 => 0,
                    _ => rnd.Next(),
                }).ToArray();
                int[] result = ApplySort(methodName, (int[])original.Clone());
                AssertSorted($"{methodName} (full range, n = {n})", original, result);
            }
        }

        /// <summary>
        /// Checks the documented behavior on negative values: the sign bit is treated like any other
        /// bit, so the values are sorted as unsigned integers and the negative values end up after
        /// all non-negative values.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_NegativeValues_SortedAsUnsigned(string methodName)
        {
            Random rnd = new(4);
            foreach (int n in new[] { 2, 9, 1000 })
            {
                int[] original = Enumerable.Range(0, n).Select(_ => rnd.Next(int.MinValue, int.MaxValue)).ToArray();
                int[] expected = original.OrderBy(x => (uint)x).ToArray();
                int[] result = ApplySort(methodName, (int[])original.Clone());
                CollectionAssert.AreEqual(expected, result, $"{methodName} did not sort negative values as unsigned integers (n = {n})");
            }
        }
    }
}
