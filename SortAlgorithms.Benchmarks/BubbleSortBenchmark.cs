using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the quadratic sorts (selection, insertion, bubble, improved bubble, cocktail) on small arrays.
    /// Bubble sort is the baseline.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class BubbleSortBenchmark : BenchmarkSort
    {
        /// <inheritdoc/>
        public override IEnumerable<int> GetSizes()
        {
            // Read the environment variable (set by Program.cs)
            var env = Environment.GetEnvironmentVariable("BENCH_SIZE");
            if (int.TryParse(env, out int singleSize) && singleSize > 0)
            {
                yield return singleSize;
                yield break;
            }

            // fallback: default sizes when none is given
            yield return 100;
            yield return 200;
            yield return 300;
            yield return 400;
            yield return 500;
            yield return 600;
            yield return 700;
            yield return 800;
            yield return 900;
            yield return 1000;
        }

        /// <summary>Selection sort (<see cref="Core.SelectionSort.Sort"/>).</summary>
        [Benchmark]
        public void SelectionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.SelectionSort.Sort(A);
        }

        /// <summary>Classic insertion sort (<see cref="Core.InsertionSort.Sort"/>).</summary>
        [Benchmark]
        public void InsertionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort.Sort(A);
        }

        /// <summary>Bubble sort (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void BubbleSort()
        {
            int[] A = (int[])_A.Clone();
            Core.BubbleSort.Sort(A);
        }

        /// <summary>Bubble sort with early exit (<see cref="Core.ImprovedBubbleSort.Sort"/>).</summary>
        [Benchmark]
        public void ImprovedBubbleSort()
        {
            int[] A = (int[])_A.Clone();
            Core.ImprovedBubbleSort.Sort(A);
        }

        /// <summary>Cocktail shaker sort (<see cref="Core.CocktailSort.Sort"/>).</summary>
        [Benchmark]
        public void CocktailSort()
        {
            int[] A = (int[])_A.Clone();
            Core.CocktailSort.Sort(A);
        }
    }
}
