using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the exchange ("propagation") sorts: <see cref="BubbleSort"/>, <see cref="ImprovedBubbleSort"/> and <see cref="CocktailSort"/>.
    /// </summary>
    [TestClass]
    public class PropagationSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "BubbleSort",
            "ImprovedBubbleSort",
            "CocktailSort",
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "BubbleSort")
                BubbleSort.Sort(array);
            else if (methodName == "ImprovedBubbleSort")
                ImprovedBubbleSort.Sort(array);
            else if (methodName == "CocktailSort")
                CocktailSort.Sort(array);
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
                var instance = new PropagationSortTests();
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