using BenchmarkDotNet.Attributes;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares merge sort (baseline), optimized merge sort and their parallel versions with
    /// <see cref="Array.Sort(Array)"/> on large arrays (100,000 elements and more).
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class OptimizedMergeSortBenchmark : BenchmarkSort
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
            yield return 100000;
            yield return 200000;
            yield return 300000;
            yield return 400000;
            yield return 500000;
            yield return 600000;
            yield return 700000;
            yield return 800000;
            yield return 900000;
            yield return 1000000;
            yield return 1100000;
            yield return 1200000;
            yield return 1300000;
            yield return 1400000;
            yield return 1500000;
            yield return 1600000;
            yield return 1700000;
            yield return 1800000;
            yield return 1900000;
            yield return 2000000;
        }

        /// <summary>.NET built-in sort (<see cref="Array.Sort(Array)"/>), for reference.</summary>
        [Benchmark]
        public void ArraySort()
        {
            int[] A = (int[])_A.Clone();
            Array.Sort(A);
        }

        /// <summary>Classic merge sort (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void MergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MergeSort.Sort(A);
        }

        /// <summary>Parallel merge sort (<see cref="Core.MergeSort.ParallelSort"/>).</summary>
        [Benchmark]
        public void ParallelMergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MergeSort.ParallelSort(A);
        }

        /// <summary>Optimized merge sort with a threshold of 45.</summary>
        [Benchmark]
        public void OptimizedMergeSort()
        {
            int[] A = (int[])_A.Clone();
            var hybridMerge = new OptimizedMergeSort();
            hybridMerge.Sort(A, 45);
        }

        /// <summary>Parallel optimized merge sort (<see cref="Core.OptimizedMergeSort.ParallelSort"/>).</summary>
        [Benchmark]
        public void ParallelOptimizedMergeSort()
        {
            int[] A = (int[])_A.Clone();
            var hybridMerge = new OptimizedMergeSort();
            hybridMerge.ParallelSort(A);
        }
    }
}
