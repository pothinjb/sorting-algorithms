using BenchmarkDotNet.Attributes;
using SortAlgorithms.Core;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares <see cref="Core.CountingSort"/> with insertion sort and optimized merge sort on small arrays.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class CountingSortBenchmark : BenchmarkSort
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

        /// <summary>Classic insertion sort (<see cref="Core.InsertionSort.Sort"/>).</summary>
        [Benchmark]
        public void InsertionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort.Sort(A);
        }

        /// <summary>Optimized merge sort (<see cref="Core.OptimizedMergeSort.Sort"/>).</summary>
        [Benchmark]
        public void OptimizedMergeSort()
        {
            int[] A = (int[])_A.Clone();
            var s = new OptimizedMergeSort();
            s.Sort(A);
        }

        /// <summary>
        /// Counting sort with k = <see cref="BenchmarkSort.Size"/> + 1
        /// (the WORST case generates values from 1 to <see cref="BenchmarkSort.Size"/>).
        /// </summary>
        [Benchmark]
        public int[] CountingSort()
        {
            int[] A = (int[])_A.Clone();
            return Core.CountingSort.Sort(A, Size + 1);
        }

        /// <summary>Counting sort variant <see cref="Core.CountingSort._Sort1"/> with k = <see cref="BenchmarkSort.Size"/> + 1.</summary>
        [Benchmark]
        public int[] CountingSort1()
        {
            int[] A = (int[])_A.Clone();
            return Core.CountingSort._Sort1(A, Size + 1);
        }
    }
}
