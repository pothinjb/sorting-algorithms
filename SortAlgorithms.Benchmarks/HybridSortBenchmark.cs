using BenchmarkDotNet.Attributes;
using System;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the hybrid sorts (pdqsort) with heap sort, their fallback, and with <see cref="Array.Sort(Array)"/>.
    /// Classic merge sort is the baseline.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class HybridSortBenchmark : BenchmarkSort
    {
        /// <summary>Classic merge sort (<see cref="Core.MergeSort.Sort"/>), the baseline.</summary>
        [Benchmark(Baseline = true)]
        public void MergeSort()
        {
            int[] A = (int[])_A.Clone();
            Core.MergeSort.Sort(A);
        }

        /// <summary>Heap sort with Floyd's max-heapify (<see cref="Core.HeapSort.Sort"/>).</summary>
        [Benchmark]
        public void HeapSortUsingFloydMaxHeapify()
        {
            int[] A = (int[])_A.Clone();
            Core.HeapSort.Sort(A);
        }

        /// <summary>Pattern-defeating quicksort (<see cref="Core.PdqSort.Sort(int[])"/>).</summary>
        [Benchmark]
        public void PdqSort()
        {
            int[] A = (int[])_A.Clone();
            Core.PdqSort.Sort(A);
        }

        /// <summary>The .NET library sort (<see cref="Array.Sort(Array)"/>), an introsort.</summary>
        [Benchmark]
        public void ArraySort()
        {
            int[] A = (int[])_A.Clone();
            Array.Sort(A);
        }
    }
}
