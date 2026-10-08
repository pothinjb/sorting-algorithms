using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the bitwise (binary radix) sorts: <see cref="BitwiseSort"/> and the <see cref="OptimizedBitwiseSort"/> variants.
    /// </summary>
    [TestClass]
    public class BitwiseSortTests: GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "BitwiseSort",
            "OptimizedBitwiseSort",
            "OptimizedBitwiseSort1",
            "OptimizedBitwiseSort2",
            "OptimizedBitwiseSort3",
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
    }
}
