using BenchmarkDotNet.Attributes;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the three <see cref="QuickSort"/> partition schemes (naive, Lomuto, Hoare) with merge sort
    /// (baseline) and optimized merge sort.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class QuickSortBenchmark : BenchmarkSort
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
            yield return 10;
            yield return 100;
            yield return 1000;
            yield return 10000;
            yield return 100000;
            yield return 1000000;
        }

        /// <summary>Classic merge sort (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void MergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MergeSort.Sort(A);
        }

        /// <summary>Optimized merge sort (<see cref="Core.OptimizedMergeSort.Sort"/>).</summary>
        [Benchmark]
        public void OptimizedMergeSort()
        {
            int[] A = (int[])_A.Clone();
            var optMerge = new OptimizedMergeSort();
            optMerge.Sort(A);
        }

        /// <summary>Quicksort with the naive partition.</summary>
        [Benchmark]
        public void QuicksortNaive()
        {
            int[] A = (int[])_A.Clone();
            var qsort = new QuickSort();
            qsort.Sort(A, QuickSortMethod.Naive);
        }

        /// <summary>Quicksort with Lomuto's partition.</summary>
        [Benchmark]
        public void QuicksortLomuto()
        {
            int[] A = (int[])_A.Clone();
            var qsort = new QuickSort();
            qsort.Sort(A, QuickSortMethod.Lomuto);
        }

        /// <summary>Quicksort with Hoare's partition.</summary>
        [Benchmark]
        public void QuicksortHoare()
        {
            int[] A = (int[])_A.Clone();
            var qsort = new QuickSort();
            qsort.Sort(A, QuickSortMethod.Hoare);
        }
    }
}
