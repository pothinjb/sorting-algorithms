using BenchmarkDotNet.Attributes;
using SortAlgorithms.Core;
using System.Collections.Generic;
using System.Linq;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Measures the hybrid merge sorts (<see cref="MergeInsertionSort"/>, <see cref="MergeQuartetSort"/>)
    /// for each threshold from 1 to 64, to find the best size at which to switch to the simple sort.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class HybridMergeSortBenchmark : BenchmarkSort
    {
        /// <summary>Size below which the hybrid sorts switch to their simple sort.</summary>
        [ParamsSource(nameof(ValuesForTH))]
        public int ThresholdInsert { get; set; }

        /// <summary>Thresholds to measure: 1 to 64.</summary>
        public static IEnumerable<int> ValuesForTH() => Enumerable.Range(1, 64);

        /// <summary>Merge sort that switches to insertion sort below <see cref="ThresholdInsert"/> elements.</summary>
        [Benchmark]
        public void MergeInsertionSort()
        {
            int[] A = (int[])_A.Clone();
            var hybridMerge = new MergeInsertionSort();
            hybridMerge.Sort(A, ThresholdInsert);
        }

        /// <summary>Merge sort that switches to quartet sort below <see cref="ThresholdInsert"/> elements.</summary>
        [Benchmark]
        public void MergeQuartetSort()
        {
            int[] A = (int[])_A.Clone();
            var hybridMerge = new MergeQuartetSort();
            hybridMerge.Sort(A, ThresholdInsert);
        }
    }
}
