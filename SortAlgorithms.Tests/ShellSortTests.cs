using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Runs the common sorting test suite against <see cref="ShellSort"/> (plain, improved and optimized) with each gap sequence.
    /// </summary>
    [TestClass]
    public class ShellSortTests : GenericSortTests
    {
        /// <inheritdoc/>
        protected override string[] SortMethods => new[]
        {
            "ShellSort_Hibbard",
            "ShellSort_Knuth",
            "ShellSort_Lee",
            "ShellSort_Sedgewick",
            "ShellSort_Shell",
            "ShellSort_PapernovStasevich",
            "ShellSort_Pratt",
            "ImprovedShellSort_Hibbard",
            "ImprovedShellSort_Knuth",
            "ImprovedShellSort_Lee",
            "ImprovedShellSort_Sedgewick",
            "ImprovedShellSort_Shell",
            "ImprovedShellSort_PapernovStasevich",
            "ImprovedShellSort_Pratt",
            "OptimizedShellSort_Knuth"
        };

        /// <inheritdoc/>
        protected override int[] ApplySort(string methodName, int[] array)
        {
            switch (methodName)
            {
                case "ShellSort_Hibbard":
                    ShellSort.Sort(array, ShellSequence.Hibbard);
                    return array;
                case "ShellSort_Knuth":
                    ShellSort.Sort(array, ShellSequence.Knuth);
                    return array;
                case "ShellSort_Lee":
                    ShellSort.Sort(array, ShellSequence.Lee);
                    return array;
                case "ShellSort_Sedgewick":
                    ShellSort.Sort(array, ShellSequence.Sedgewick);
                    return array;
                case "ShellSort_Shell":
                    ShellSort.Sort(array, ShellSequence.Shell);
                    return array;
                case "ShellSort_PapernovStasevich":
                    ShellSort.Sort(array, ShellSequence.PapernovStasevich);
                    return array;
                case "ShellSort_Pratt":
                    ShellSort.Sort(array, ShellSequence.Pratt);
                    return array;
                case "ImprovedShellSort_Hibbard":
                    ShellSort.ImprovedSort(array, ShellSequence.Hibbard);
                    return array;
                case "ImprovedShellSort_Knuth":
                    ShellSort.ImprovedSort(array, ShellSequence.Knuth);
                    return array;
                case "ImprovedShellSort_Lee":
                    ShellSort.ImprovedSort(array, ShellSequence.Lee);
                    return array;
                case "ImprovedShellSort_Sedgewick":
                    ShellSort.ImprovedSort(array, ShellSequence.Sedgewick);
                    return array;
                case "ImprovedShellSort_Shell":
                    ShellSort.ImprovedSort(array, ShellSequence.Shell);
                    return array;
                case "ImprovedShellSort_PapernovStasevich":
                    ShellSort.ImprovedSort(array, ShellSequence.PapernovStasevich);
                    return array;
                case "ImprovedShellSort_Pratt":
                    ShellSort.ImprovedSort(array, ShellSequence.Pratt);
                    return array;
                case "OptimizedShellSort_Knuth":
                    ShellSort.OptimizedSort(array, ShellSequence.Knuth);
                    return array;
                default:
                    throw new ArgumentException($"Unknown sort method: {methodName}");
            }
        }

        /// <summary>
        /// Supplies each name in <see cref="SortMethods"/> to MSTest as one test row.
        /// </summary>
        public static IEnumerable<object[]> SortMethodData
        {
            get
            {
                var instance = new ShellSortTests();
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