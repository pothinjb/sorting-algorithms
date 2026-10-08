using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares <see cref="Core.LibrarySort"/> with epsilon = 1 to 4 against insertion sort and Shell sort.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class LibrarySortBenchmark : BenchmarkSort
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
            yield return 1250;
            yield return 1500;
            yield return 1750;
            yield return 2000;
            yield return 2250;
            yield return 2500;
            yield return 2750;
            yield return 3000;
            yield return 3250;
            yield return 3500;
            yield return 3750;
            yield return 4000;
            yield return 4250;
            yield return 4500;
            yield return 4750;
            yield return 5000;
            yield return 5250;
            yield return 5500;
        }

        /// <summary>Classic insertion sort (<see cref="Core.InsertionSort.Sort"/>).</summary>
        [Benchmark]
        public void InsertionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort.Sort(A);
        }

        /// <summary>Shell sort with Shell's original gaps.</summary>
        [Benchmark]
        public void ShellSort()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Shell);
        }

        /// <summary>Library sort with epsilon = 1.</summary>
        [Benchmark]
        public void LibrarySort_eps1()
        {
            int[] A = (int[])_A.Clone();
            Core.LibrarySort.Sort(A, 1);
        }

        /// <summary>Library sort with epsilon = 2.</summary>
        [Benchmark]
        public void LibrarySort_eps2()
        {
            int[] A = (int[])_A.Clone();
            Core.LibrarySort.Sort(A, 2);
        }

        /// <summary>Library sort with epsilon = 3.</summary>
        [Benchmark]
        public void LibrarySort_eps3()
        {
            int[] A = (int[])_A.Clone();
            Core.LibrarySort.Sort(A, 3);
        }

        /// <summary>Library sort with epsilon = 4.</summary>
        [Benchmark]
        public void LibrarySort_eps4()
        {
            int[] A = (int[])_A.Clone();
            Core.LibrarySort.Sort(A, 4);
        }
    }
}
