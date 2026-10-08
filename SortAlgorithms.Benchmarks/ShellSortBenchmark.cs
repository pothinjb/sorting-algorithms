using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Compares the gap sequences of <see cref="Core.ShellSort"/>. Shell's original sequence is the baseline.
    /// </summary>
    /// <remarks>
    /// Does not derive from <see cref="BenchmarkSort"/>: its own <see cref="Setup"/> also precomputes
    /// the size-dependent gap sequences (Shell, Pratt), so their cost is not measured.
    /// </remarks>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class ShellSortBenchmark
    {
        /// <summary>Input array, built by <see cref="Setup"/>. Each benchmark sorts a copy.</summary>
        private int[] _A;

        /// <summary>Array size, taken from <see cref="GetSizes"/>.</summary>
        [ParamsSource(nameof(GetSizes))]
        public int Size { get; set; }

        /// <summary>Input order (random, ascending or descending), taken from <see cref="GetCases"/>.</summary>
        [ParamsSource(nameof(GetCases))]
        public BenchmarkCase Case { get; set; }

        /// <summary>
        /// Array sizes to measure: the single size in <c>BENCH_SIZE</c> if set, otherwise 1,000 to 1,000,000.
        /// </summary>
        public static IEnumerable<int> GetSizes()
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
            yield return 2000;
            yield return 3000;
            yield return 4000;
            yield return 5000;
            yield return 6000;
            yield return 7000;
            yield return 8000;
            yield return 9000;
            yield return 10000;
            yield return 20000;
            yield return 30000;
            yield return 40000;
            yield return 50000;
            yield return 60000;
            yield return 70000;
            yield return 80000;
            yield return 90000;
            yield return 100000;
            yield return 200000;
            yield return 300000;
            yield return 400000;
            yield return 500000;
            yield return 600000;
            yield return 700000;
            yield return 800000;
            yield return 900000;
            yield return 1000000;
        }

        /// <summary>
        /// Cases to measure, chosen by <c>BENCH_CASE</c> (see <see cref="BenchmarkSort.GetCases"/>).
        /// </summary>
        public static IEnumerable<BenchmarkCase> GetCases()
        {
            // Read the environment variable (set by Program.cs)
            var env = Environment.GetEnvironmentVariable("BENCH_CASE");
            if (int.TryParse(env, out int benchCase) && benchCase > 0)
            {
                if (benchCase == 1)
                {
                    yield return BenchmarkCase.BEST;
                    yield break;
                }
                else if (benchCase == 2)
                {
                    yield return BenchmarkCase.WORST;
                    yield break;
                }
                else if (benchCase == 3)
                {
                    yield return BenchmarkCase.BEST;
                    yield return BenchmarkCase.AVERAGE;
                    yield return BenchmarkCase.WORST;
                    yield break;
                }
            }

            // fallback: default case when none is given
            yield return BenchmarkCase.AVERAGE;
        }

        /// <summary>
        /// Builds the input like <see cref="BenchmarkSort.Setup"/>, then precomputes the Shell and Pratt
        /// gap sequences for <see cref="Size"/>.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _A = new int[Size];
            if (Case == BenchmarkCase.AVERAGE)
            {
                Random rnd = new(Size); // fixed seed: every benchmark gets the same data
                for (int i = 0; i < Size; ++i)
                {
                    _A[i] = rnd.Next(Size);
                }
            }
            else if (Case == BenchmarkCase.BEST)
            {
                for (int i = 0; i < Size; ++i)
                {
                    _A[i] = i;
                }
            }
            else if (Case == BenchmarkCase.WORST)
            {
                for (int i = 0; i < Size; ++i)
                {
                    _A[i] = Size - i;
                }
            }
            Core.ShellSort._PrecomputeForBenchmark(Size); // !!!! keeps sequence generation out of the measurement
        }

        /// <summary>Shell sort with the Incerpi-Sedgewick gaps.</summary>
        [Benchmark]
        public void IncerpiSedgewick()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.IncerpiSedgewick);
        }

        /// <summary>Shell sort with Hibbard's gaps.</summary>
        [Benchmark]
        public void Hibbard()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Hibbard);
        }

        /// <summary>Shell sort with Knuth's gaps.</summary>
        [Benchmark]
        public void Knuth()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Knuth);
        }

        /// <summary>Shell sort with the Papernov-Stasevich gaps.</summary>
        [Benchmark]
        public void PapernovStasevich()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.PapernovStasevich);
        }
        
        /// <summary>Shell sort with Sedgewick's gaps.</summary>
        [Benchmark]
        public void Sedgewick()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Sedgewick);
        }

        /// <summary>Shell sort with Shell's original gaps (n/2, n/4, ...) (baseline).</summary>
        [Benchmark(Baseline = true)]
        public void Shell()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Shell);
        }

        /// <summary>Shell sort with Lee's gaps.</summary>
        [Benchmark]
        public void Lee()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Lee);
        }

        /// <summary>Shell sort with Pratt's gaps (3-smooth numbers).</summary>
        [Benchmark]
        public void Pratt()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.Sort(A, Core.ShellSequence.Pratt);
        }

        /// <summary><see cref="Core.ShellSort.ImprovedSort"/> with Knuth's gaps.</summary>
        [Benchmark]
        public void ImprovedKnuth()
        {
            int[] A = (int[])_A.Clone();
            Core.ShellSort.ImprovedSort(A, Core.ShellSequence.Knuth);
        }
    }
}
