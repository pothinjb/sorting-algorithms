using BenchmarkDotNet.Attributes;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares <see cref="Core.MinMaxSort"/> and its optimized variant with insertion sort (baseline).
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class MinMaxSortBenchmark : BenchmarkSort
    {
        /// <summary>Classic insertion sort (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void InsertionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort.Sort(A);
        }

        /// <summary><see cref="Core.MinMaxSort.Sort"/>.</summary>
        [Benchmark]
        public void MinMaxSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MinMaxSort.Sort(A);
        }

        /// <summary><see cref="Core.MinMaxSort.OptimizedSort"/>.</summary>
        [Benchmark]
        public void OptimizedMinMaxSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MinMaxSort.OptimizedSort(A);
        }
    }
}
