using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against <see cref="LibrarySort"/> (gapped insertion sort) with epsilon = 1.
    /// </summary>
    [TestClass]
    public class LibrarySortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "LibrarySort",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "LibrarySort")
                LibrarySort.Sort(array, 1);
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
                var instance = new LibrarySortTests();
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

        /// <summary>Regression test: an <c>epsilon</c> below 1 is rejected instead of crashing during the sort.</summary>
        [TestMethod]
        public void Sort_NonPositiveEpsilon_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LibrarySort.Sort(new[] { 3, 1, 2 }, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LibrarySort.Sort(new[] { 3, 1, 2 }, -1));
        }

        /// <summary>Larger gap factors also sort correctly.</summary>
        [TestMethod]
        public void Sort_VariousEpsilons()
        {
            Random rnd = new(7);
            int[] original = Enumerable.Range(0, 500).Select(_ => rnd.Next(1000)).ToArray();
            for (int epsilon = 1; epsilon <= 4; ++epsilon)
            {
                int[] array = (int[])original.Clone();
                LibrarySort.Sort(array, epsilon);
                AssertSorted($"LibrarySort (epsilon = {epsilon})", original, array);
            }
        }
    }
}