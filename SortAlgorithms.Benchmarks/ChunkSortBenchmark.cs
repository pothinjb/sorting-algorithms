using BenchmarkDotNet.Attributes;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the <see cref="Core.ChunkSort"/> variants (pair, triplet, quartet) with insertion sort (baseline).
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class ChunkSortBenchmark : BenchmarkSort
    {
        /// <summary>Classic insertion sort (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void InsertionSort()
        {
            int[] A = (int[])_A.Clone();
            Core.InsertionSort.Sort(A);
        }

        /// <summary><see cref="Core.ChunkSort.PairSort"/>.</summary>
        [Benchmark]
        public void PairSort()
        {
            int[] A = (int[])_A.Clone();
            Core.ChunkSort.PairSort(A);
        }

        /// <summary><see cref="Core.ChunkSort.TripletSort"/>.</summary>
        [Benchmark]
        public void TripletSort()
        {
            int[] A = (int[])_A.Clone();
            Core.ChunkSort.TripletSort(A);
        }

        /// <summary><see cref="Core.ChunkSort.QuartetSort"/>.</summary>
        [Benchmark]
        public void QuartetSort()
        {
            int[] A = (int[])_A.Clone();
            Core.ChunkSort.QuartetSort(A);
        }
    }
}
