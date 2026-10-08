using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the <see cref="InsertionSort"/> variants (plain, binary search, list, linked list, ...).
    /// </summary>
    [TestClass]
    public class InsertionSortTests: GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "InsertionSort",
            "InsertionSort1",
            "InsertionSort2",
            "InsertionSort3",
            "InsertionSort4"
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "InsertionSort")
                InsertionSort.Sort(array);
            else if (methodName == "InsertionSort1")
                InsertionSort._Sort1(array);
            else if (methodName == "InsertionSort2")
                return InsertionSort._Sort2(array);
            else if (methodName == "InsertionSort3")
                return InsertionSort._Sort3(array);
            else if (methodName == "InsertionSort4")
                InsertionSort._Sort4(array);
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
                var instance = new InsertionSortTests();
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
        public void Run_Generic_Sort_Tests(string methodName)
        {
            RunAllCommonTests(methodName);
        }
    }
}
