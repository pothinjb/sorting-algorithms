using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the <see cref="CountingSort"/> variants.
    /// </summary>
    [TestClass]
    public class CountingSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "CountingSort",
            "CountingSort1",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "CountingSort")
                return CountingSort.Sort(array, 10000);
            else if (methodName == "CountingSort1")
                return CountingSort._Sort1(array, 10000);
            else
                throw new ArgumentException($"Unknown sort method: {methodName}");
        }

        /// <summary>
        /// Supplies each name in <see cref="SortMethods"/> to MSTest as one test row.
        /// </summary>
        public static IEnumerable<object[]> SortMethodData
        {
            get
            {
                var instance = new CountingSortTests();
                foreach (var name in instance.SortMethods)
                    yield return new object[] { name };
            }
        }

        /// <summary>
        /// Runs the common test suite for one sort variant.
        /// </summary>
        /// <param name="methodName">Variant name, as listed in <see cref="SortMethods"/>.</param>
        /// <remarks>
        /// Random values are drawn below 5000 so they stay within the value range
        /// (k = 10000) passed to <see cref="CountingSort"/>.
        /// </remarks>
        [TestMethod]
        [DynamicData(nameof(SortMethodData))]
        public void Run_Generic_Tests(string methodName)
        {
            RunAllCommonTests(methodName, 5000);
        }
    }
}
