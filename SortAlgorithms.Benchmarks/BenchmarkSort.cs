using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Base class for the sorting benchmarks. It declares the <see cref="Size"/> and <see cref="Case"/>
    /// parameters and builds the input array in <see cref="Setup"/>. Each <c>[Benchmark]</c> method of a
    /// derived class must sort a <c>Clone()</c> of <see cref="_A"/>, never <see cref="_A"/> itself.
    /// </summary>
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public abstract class BenchmarkSort
    {
        /// <summary>Input array, built by <see cref="Setup"/>. Shared by all iterations: do not sort it in place.</summary>
        protected int[] _A;

        /// <summary>Array size, taken from <see cref="GetSizes"/>.</summary>
        [ParamsSource(nameof(GetSizes))]
        public int Size { get; set; }

        /// <summary>Input order (random, ascending or descending), taken from <see cref="GetCases"/>.</summary>
        [ParamsSource(nameof(GetCases))]
        public BenchmarkCase Case { get; set; }

        /// <summary>
        /// Array sizes to measure. If the <c>BENCH_SIZE</c> environment variable holds a positive integer,
        /// only that size is used; otherwise the defaults are 10, 100, 1,000, 10,000 and 100,000.
        /// Override it to change the default sizes.
        /// </summary>
        public virtual IEnumerable<int> GetSizes()
        {
            // Read the environment variable (set by Program.cs)
            var env = Environment.GetEnvironmentVariable("BENCH_SIZE");
            if (int.TryParse(env, out int singleSize) && singleSize > 0)
            {
                yield return singleSize;
                yield break;
            }

            // fallback: default sizes when none is given
            yield return 10;
            yield return 100;
            yield return 1000;
            yield return 10000;
            yield return 100000;
        }

        /// <summary>
        /// Cases to measure, chosen by the <c>BENCH_CASE</c> environment variable:
        /// 1 = <see cref="BenchmarkCase.BEST"/>, 2 = <see cref="BenchmarkCase.WORST"/>, 3 = all three cases.
        /// Any other value (or none) gives <see cref="BenchmarkCase.AVERAGE"/>.
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
        /// Builds <see cref="_A"/> for the current <see cref="Size"/> and <see cref="Case"/>:
        /// random values in [0, Size) with a fixed seed of <see cref="Size"/> for AVERAGE,
        /// 0 to Size - 1 for BEST, and Size down to 1 for WORST.
        /// </summary>
        [GlobalSetup]
        public virtual void Setup()
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
        }
    }
}
