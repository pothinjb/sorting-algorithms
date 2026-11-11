using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the hybrid sorts (<see cref="IntroSort"/>, <see cref="PdqSort"/>),
    /// plus patterned inputs large enough to exercise their pattern detection and their heap sort fallback.
    /// </summary>
    [TestClass]
    public class HybridSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "IntroSort",
            "IntroSortDepthLimit0",
            "IntroSortDepthLimit1",
            "PdqSort",
            "PdqSortBadAllowed1",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "IntroSort")
                IntroSort.Sort(array);
            else if (methodName == "IntroSortDepthLimit0") // heap sort alone above the insertion sort threshold
                IntroSort.Sort(array, 0);
            else if (methodName == "IntroSortDepthLimit1") // one partition, then heap sort on both parts
                IntroSort.Sort(array, 1);
            else if (methodName == "PdqSort")
                PdqSort.Sort(array);
            else if (methodName == "PdqSortBadAllowed1") // heap sort at the first unbalanced partition
                PdqSort.Sort(array, 1);
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
                var instance = new HybridSortTests();
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
        /// Sorts large patterned inputs (ascending, descending, organ pipe, sawtooth, few distinct values,
        /// random runs, random) with every variant.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Sort_PatternedArrays(string methodName)
        {
            Random rnd = new(12345);
            foreach (int n in new[] { 63, 64, 65, 1000, 4097, 100_000 })
            {
                var inputs = new Dictionary<string, int[]>
                {
                    ["ascending"] = Enumerable.Range(0, n).ToArray(),
                    ["descending"] = Enumerable.Range(0, n).Reverse().ToArray(),
                    ["organ pipe"] = Enumerable.Range(0, n).Select(i => Math.Min(i, n - i)).ToArray(),
                    ["sawtooth"] = Enumerable.Range(0, n).Select(i => i % 97).ToArray(),
                    ["few distinct"] = Enumerable.Range(0, n).Select(_ => rnd.Next(0, 4)).ToArray(),
                    ["random runs"] = RandomRuns(rnd, n),
                    ["random"] = Enumerable.Range(0, n).Select(_ => rnd.Next()).ToArray(),
                };
                foreach (var (name, original) in inputs)
                {
                    int[] result = ApplySort(methodName, (int[])original.Clone());
                    AssertSorted($"{methodName} ({name}, n = {n})", original, result);
                }
            }
        }

        /// <summary>
        /// Builds an array of <paramref name="n"/> elements made of consecutive ascending or descending runs
        /// of random lengths (1 to 199).
        /// </summary>
        /// <param name="rnd">Random generator.</param>
        /// <param name="n">Array size.</param>
        /// <returns>The new array.</returns>
        private static int[] RandomRuns(Random rnd, int n)
        {
            int[] A = new int[n];
            int i = 0;
            while (i < n)
            {
                int len = Math.Min(n - i, rnd.Next(1, 200));
                bool ascending = rnd.Next(2) == 0;
                int start = rnd.Next(0, 1000);
                for (int k = 0; k < len; k++)
                    A[i + k] = ascending ? start + k : start - k;
                i += len;
            }
            return A;
        }
    }
}
