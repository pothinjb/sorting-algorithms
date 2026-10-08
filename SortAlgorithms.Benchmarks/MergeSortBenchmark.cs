using BenchmarkDotNet.Attributes;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the merge sort variants with <see cref="Array.Sort(Array)"/>. Classic merge sort is the baseline.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class MergeSortBenchmark : BenchmarkSort
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

        /// <summary><see cref="Core.SkipMergeSort.Sort"/>.</summary>
        [Benchmark]
        public void SkipMergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.SkipMergeSort.Sort(A);
        }

        /// <summary>Optimized merge sort (<see cref="Core.OptimizedMergeSort.Sort"/>).</summary>
        [Benchmark]
        public void OptimizedMergeSort()
        {
            int[] A = (int[])_A.Clone();
            var optMerge = new OptimizedMergeSort();
            optMerge.Sort(A);
        }

        /// <summary>.NET built-in sort (<see cref="Array.Sort(Array)"/>), for reference.</summary>
        [Benchmark]
        public void ArraySort()
        {
            int[] A = (int[])_A.Clone();
            Array.Sort(A);
        }
    }
}
