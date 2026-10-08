using BenchmarkDotNet.Attributes;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the insertion sort variants: in place (baseline), binary search, <c>List</c> and <c>LinkedList</c>.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class InsertionSortBenchmark : BenchmarkSort
    {
        /// <summary>Classic insertion sort (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void InsertionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort.Sort(A);
        }

        /// <summary>Insertion sort that finds the insertion point by binary search (<see cref="Core.InsertionSort._Sort1"/>).</summary>
        [Benchmark]
        public void InsertionSortUsingBinarySearch()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort._Sort1(A);
        }

        /// <summary>Insertion sort into a <c>List</c> (<see cref="Core.InsertionSort._Sort2"/>).</summary>
        [Benchmark]
        public void InsertionSortUsingList()
        {
            int[] A = (int[])_A.Clone();
            int[] sorted = Core.InsertionSort._Sort2(A);
        }

        /// <summary>Insertion sort into a <c>LinkedList</c> (<see cref="Core.InsertionSort._Sort3"/>).</summary>
        [Benchmark]
        public void InsertionSortUsingLinkedList()
        {
            int[] A = (int[])_A.Clone();
            int[] sorted = Core.InsertionSort._Sort3(A);
        }
    }
}
