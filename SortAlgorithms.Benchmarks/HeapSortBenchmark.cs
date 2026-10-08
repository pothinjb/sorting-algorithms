using BenchmarkDotNet.Attributes;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares heap sort (naive and Floyd max-heapify) with merge sort and parallel merge sort.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class HeapSortBenchmark : BenchmarkSort
    {
        /// <summary>Classic merge sort (<see cref="Core.MergeSort.Sort"/>).</summary>
        [Benchmark]
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

        /// <summary>Heap sort with the naive max-heapify (<see cref="Core.HeapSort.SortUsingNaiveMaxHeapify"/>).</summary>
        [Benchmark]
        public void HeapSortUsingNaiveMaxHeapify()
        {
            int[] A = (int[])_A.Clone();
            Core.HeapSort.SortUsingNaiveMaxHeapify(A);
        }

        /// <summary>Heap sort with Floyd's max-heapify (<see cref="Core.HeapSort.Sort"/>).</summary>
        [Benchmark]
        public void HeapSortUsingFloydMaxHeapify()
        {
            int[] A = (int[])_A.Clone();
            Core.HeapSort.Sort(A);
        }
    }
}
