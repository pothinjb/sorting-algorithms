using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the bitwise sorts (<see cref="Core.BitwiseSort"/>, <see cref="Core.OptimizedBitwiseSort"/> variants)
    /// with merge sort, parallel merge sort and <see cref="Array.Sort(Array)"/>, on sizes up to 10 million.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class BitwiseSortBenchmark : BenchmarkSort
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
            yield return 1000;
            yield return 10000;
            yield return 100000;
            yield return 500000;
            yield return 1000000;
            yield return 2000000;
            yield return 3000000;
            yield return 4000000;
            yield return 5000000;
            yield return 6000000;
            yield return 7000000;
            yield return 8000000;
            yield return 9000000;
            yield return 10000000;
        }

        /// <summary>Classic merge sort (<see cref="Core.MergeSort.Sort"/>).</summary>
        [Benchmark]
        public void MergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MergeSort.Sort(A);
        }

        /// <summary><see cref="Core.BitwiseSort.Sort"/>.</summary>
        [Benchmark]
        public void BitwiseSort()
        {
            int[] A = (int[])_A.Clone();
            Core.BitwiseSort.Sort(A);
        }

        /// <summary><see cref="Core.OptimizedBitwiseSort.Sort"/>.</summary>
        [Benchmark]
        public void OptimizedBitwiseSort()
        {
            int[] A = (int[])_A.Clone();
            Core.OptimizedBitwiseSort.Sort(A);
        }

        /// <summary><see cref="Core.OptimizedBitwiseSort._Sort1"/>.</summary>
        [Benchmark]
        public void OptimizedBitwiseSort1()
        {
            int[] A = (int[])_A.Clone();
            Core.OptimizedBitwiseSort._Sort1(A);
        }

        /// <summary><see cref="Core.OptimizedBitwiseSort._Sort2"/>.</summary>
        [Benchmark]
        public void OptimizedBitwiseSort2()
        {
            int[] A = (int[])_A.Clone();
            Core.OptimizedBitwiseSort._Sort2(A);
        }

        /// <summary><see cref="Core.OptimizedBitwiseSort._Sort3"/>.</summary>
        [Benchmark]
        public void OptimizedBitwiseSort3()
        {
            int[] A = (int[])_A.Clone();
            Core.OptimizedBitwiseSort._Sort3(A);
        }

        /// <summary>Parallel merge sort (<see cref="Core.MergeSort.ParallelSort"/>).</summary>
        [Benchmark]
        public void ParallelMergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MergeSort.ParallelSort(A);
        }

        /// <summary>.NET built-in sort (<see cref="Array.Sort(Array)"/>), for reference.</summary>
        [Benchmark]
        public void ArraySort()
        {
            int[] A = (int[])_A.Clone();
            Array.Sort(A);
        }

        /*[Benchmark]
        public void OptimizedBitwiseSort1()
        {
            int[] A = (int[])_A.Clone();
            Core.OptimizedBitwiseSort._Sort1(A);
        }*/
    }
}
