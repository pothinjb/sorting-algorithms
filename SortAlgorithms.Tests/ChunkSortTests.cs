using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against the <see cref="ChunkSort"/> variants (pair, triplet and quartet sorts, sequential and parallel).
    /// </summary>
    [TestClass]
    public class ChunkSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "PairSort",
            "TripletSort",
            "QuartetSort",
            "ParallelTripletSort",
            "ParallelQuartetSort"
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            if (methodName == "PairSort")
                ChunkSort.PairSort(array);
            else if (methodName == "TripletSort")
                ChunkSort.TripletSort(array);
            else if (methodName == "QuartetSort")
                ChunkSort.QuartetSort(array);
            else if (methodName == "ParallelTripletSort")
                ChunkSort.ParallelTripletSort(array);
            else if (methodName == "ParallelQuartetSort")
                ChunkSort.ParallelQuartetSort(array);
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
                var instance = new ChunkSortTests();
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