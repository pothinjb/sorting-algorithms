using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the <see cref="SelectionSort"/> variants.
    /// </summary>
    [TestClass]
    public class SelectionSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "BasicSort",
            "SelectSort",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "BasicSort")
                return SelectionSort.BasicSort(array);
            else if (methodName == "SelectSort")
                SelectionSort.Sort(array);
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
                var instance = new SelectionSortTests();
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

        /// <summary>Regression test: <see cref="SelectionSort.BasicSort"/> returns a sorted copy and leaves its input unchanged.</summary>
        [TestMethod]
        public void BasicSort_DoesNotModifyInput()
        {
            int[] input = { 3, 1, 2 };
            int[] result = SelectionSort.BasicSort(input);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
            CollectionAssert.AreEqual(new[] { 3, 1, 2 }, input);
        }
    }
}