using System;
using System.Collections.Generic;
using System.Linq;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Gap sequences available for <see cref="ShellSort"/>.
    /// </summary>
    public enum ShellSequence
    {
        /// <summary>Hibbard (1963): 2^k - 1.</summary>
        Hibbard,            // 1, 3, 7, 15, 31, ...
        /// <summary>Incerpi &amp; Sedgewick (1985).</summary>
        IncerpiSedgewick,   // 1, 3, 7, 21, 48, 112, ...
        /// <summary>Knuth (1973): (3^k - 1) / 2. Default sequence.</summary>
        Knuth,              // 1, 4, 13, 40, 121, ...
        /// <summary>Lee (2021): ceil((γ^k - 1) / (γ - 1)) with γ ≈ 2.2436.</summary>
        Lee,                // 1, 4, 9, 20, 45, 102, 230, 516, ...
        /// <summary>Papernov &amp; Stasevich (1965): 2^k + 1, prefixed with 1.</summary>
        PapernovStasevich,  // 1, 3, 5, 9, 17, 33, 65, ...
        /// <summary>Pratt (1971): all numbers of the form 2^i * 3^j.</summary>
        Pratt,              // 1, 2, 3, 4, 6, 8, 9, 12, ...
        /// <summary>Sedgewick (1986): interleaves 9(2^k - 2^(k/2)) + 1 and 8*2^k - 6*2^((k+1)/2) + 1.</summary>
        Sedgewick,          // 1, 5, 19, 41, 109, ...
        /// <summary>Shell's original sequence (1959): n/2, n/4, ..., 1.</summary>
        Shell,              // Original sequence: N/2, N/4, ...
    }

    /// <summary>
    /// Shell sort: a series of insertion sorts on elements that are <c>h</c> positions apart,
    /// with <c>h</c> taken from a decreasing gap sequence that ends with 1.
    /// </summary>
    /// <remarks>
    /// In place, not stable. Time complexity depends on the gap sequence
    /// (for example O(n^(3/2)) for Knuth and Hibbard, O(n log² n) for Pratt, O(n²) in the worst case for Shell).
    /// </remarks>
    public static class ShellSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order using Knuth's gap sequence.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            Sort(A, ShellSequence.Knuth);
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order using the given gap sequence.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <param name="seq">Gap sequence to use.</param>
        public static void Sort(int[] A, ShellSequence seq)
        {
            int n = A.Length;
            if (n == 0) return;
            int[] gaps = GetSequence(seq, n);
            int k0 = FindFirstGapIndex(seq, n, gaps);
            for (int k = k0; k < gaps.Length; ++k)
            {
                int h = gaps[k];
                for (int i = h; i < n; ++i)
                {
                    int v = A[i];
                    int j = i - h;
                    while ((j >= 0) && (A[j] > v))
                    {
                        A[j + h] = A[j];
                        j -= h;
                    }
                    A[j + h] = v;
                }
            }
        }

        /// <summary>
        /// Variant of <see cref="Sort(int[], ShellSequence)"/> that skips the shift loop and the
        /// write-back when <c>A[i]</c> is already in place relative to <c>A[i - h]</c>.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <param name="seq">Gap sequence to use.</param>
        public static void ImprovedSort(int[] A, ShellSequence seq)
        {
            int n = A.Length;
            if (n == 0) return;
            int[] gaps = GetSequence(seq, n);
            int k0 = FindFirstGapIndex(seq, n, gaps);
            for (int k = k0; k < gaps.Length; ++k)
            {
                int h = gaps[k];
                for (int i = h; i < n; ++i)
                {
                    int v = A[i];
                    int j = i - h;
                    if (A[j] > v)
                    {
                        do
                        {
                            A[j + h] = A[j];
                            j -= h;
                        } while ((j >= 0) && (A[j] > v));
                        A[j + h] = v;
                    }
                }
            }
        }

        /// <summary>
        /// Variant of <see cref="ImprovedSort"/> that only runs the passes with a gap of at least 5
        /// (never the last gap of the sequence), then finishes with <see cref="ChunkSort.QuartetSort(int[])"/>
        /// instead of the final h = 1 insertion pass.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <param name="seq">Gap sequence to use.</param>
        public static void OptimizedSort(int[] A, ShellSequence seq)
        {
            int n = A.Length;
            if (n == 0) return;
            int[] gaps = GetSequence(seq, n);
            int k0 = FindFirstGapIndex(seq, n, gaps);
            for (int k = k0, kmax = gaps.Length - 1; k < kmax && gaps[k] >= 5; ++k)
            {
                int h = gaps[k];
                for (int i = h; i < n; ++i)
                {
                    int j = i - h;
                    int v = A[i];
                    if (A[j] > v)
                    {
                        do
                        {
                            A[j + h] = A[j];
                            j -= h;
                        } while ((j >= 0) && (A[j] > v));
                        A[j + h] = v;
                    }
                }
            }
            // final iteration
            ChunkSort.QuartetSort(A);
        }

        // <<<<
        /// <summary>Cached Shell sequence, set by <see cref="_PrecomputeForBenchmark"/>.</summary>
        private static int[] _precomputed_Shell = null;
        /// <summary>Cached Pratt sequence, set by <see cref="_PrecomputeForBenchmark"/>.</summary>
        private static int[] _precomputed_Pratt = null;
        /// <summary>Array size the cached sequences were computed for (-1 if the cache is empty).</summary>
        private static int _precomputed_n = -1;

        /// <summary>
        /// Benchmark helper: precomputes and caches the Shell and Pratt sequences for size <paramref name="n"/>,
        /// so that their generation is not measured.
        /// </summary>
        /// <param name="n">Array size the sequences are computed for.</param>
        /// <remarks>
        /// The cache is global and is only used for arrays of size <paramref name="n"/>; it is replaced by the
        /// next call to this method. Not thread-safe.
        /// </remarks>
        public static void _PrecomputeForBenchmark(int n)
        {
            _precomputed_n = -1;
            _precomputed_Shell = Gaps.ShellGaps(n).ToArray();
            _precomputed_Pratt = Gaps.PrattGaps(n).ToArray();
            _precomputed_n = n;
        }
        // <<<<

        /// <summary>
        /// Returns the index of the first gap that is not larger than the useful maximum for an array of size
        /// <paramref name="n"/> (<c>ceil(n / 3)</c> for Knuth, as recommended by Knuth; <c>n</c> otherwise).
        /// </summary>
        /// <param name="seq">Gap sequence.</param>
        /// <param name="n">Array size.</param>
        /// <param name="gaps">Gaps in decreasing order.</param>
        /// <returns>Index into <paramref name="gaps"/> where the passes should start.</returns>
        private static int FindFirstGapIndex(ShellSequence seq, int n, int[] gaps)
        {
            int k0 = 0;
            if (gaps.Length > 0)
            {
                int hmax = (seq == ShellSequence.Knuth) ? (int)Math.Ceiling((double)n / 3) : n;
                while (k0 < gaps.Length && gaps[k0] > hmax)
                    k0++;
            }
            return k0;
        }

        /// <summary>
        /// Returns the gaps of <paramref name="seq"/> in decreasing order. Fixed sequences come from the
        /// precomputed tables in <see cref="Gaps"/>; Shell and Pratt are generated for <paramref name="n"/>
        /// (or taken from the benchmark cache when it was computed for the same <paramref name="n"/>).
        /// </summary>
        /// <param name="seq">Gap sequence.</param>
        /// <param name="n">Array size, used to generate the Shell and Pratt sequences.</param>
        /// <returns>The gaps, largest first.</returns>
        private static int[] GetSequence(ShellSequence seq, int n)
        {
            if (n == _precomputed_n)
            {
                if (seq == ShellSequence.Shell)
                    return _precomputed_Shell;
                if (seq == ShellSequence.Pratt)
                    return _precomputed_Pratt;
            }
            if (seq == ShellSequence.IncerpiSedgewick)
                return Gaps.IncerpiSedgewick;
            if (seq == ShellSequence.Sedgewick)
                return Gaps.Sedgewick;
            if (seq == ShellSequence.Knuth)
                return Gaps.Knuth;
            if (seq == ShellSequence.PapernovStasevich)
                return Gaps.PapernovStasevich;
            if (seq == ShellSequence.Lee)
                return Gaps.Lee;
            if (seq == ShellSequence.Hibbard)
                return Gaps.Hibbard;
            if (seq == ShellSequence.Shell)
                return Gaps.ShellGaps(n).ToArray();
            if (seq == ShellSequence.Pratt)
                return Gaps.PrattGaps(n).ToArray();
            return Gaps.Knuth; // default sequence
        }
    }

    /// <summary>
    /// Gap sequences for <see cref="ShellSort"/>: precomputed tables (decreasing order, covering the whole
    /// <see cref="int"/> range) and generators for a given array size.
    /// </summary>
    public static class Gaps
    {
        /// <summary>Hibbard sequence 2^k - 1, in decreasing order.</summary>
        public static readonly int[] Hibbard = new int[]
        {
            2147483647, 1073741823, 536870911, 268435455, 134217727,
            67108863, 33554431, 16777215, 8388607, 4194303,
            2097151, 1048575, 524287, 262143, 131071,
            65535, 32767, 16383, 8191, 4095,
            2047, 1023, 511, 255, 127,
            63, 31, 15, 7, 3,
            1
        };

        /// <summary>Incerpi &amp; Sedgewick sequence, in decreasing order.</summary>
        public static readonly int[] IncerpiSedgewick = new int[]
        {
            2085837936, 852913488, 343669872, 114556624, 49095696,
            21479367, 8382192, 3402672, 1391376, 463792,
            198768, 86961, 33936, 13776, 4592,
            1968, 861, 336, 112, 48,
            21, 7, 3, 1
        };

        /// <summary>Knuth sequence (3^k - 1) / 2, in decreasing order.</summary>
        public static readonly int[] Knuth = new int[]
        {
            581130733, 193710244, 64570081, 21523360, 7174453,
            2391484, 797161, 265720, 88573, 29524,
            9841, 3280, 1093, 364, 121,
            40, 13, 4, 1
        };

        /// <summary>Lee sequence, in decreasing order.</summary>
        public static readonly int[] Lee = new int[]
        {
            1071378536, 477524607, 212837706, 94863989, 42281871,
            18845471, 8399623, 3743800, 1668650, 743735,
            331490, 147748, 65853, 29351, 13082,
            5831, 2599, 1158, 516, 230,
            102, 45, 20, 9, 4,
            1
        };

        /// <summary>Papernov &amp; Stasevich sequence 2^k + 1 (then 1), in decreasing order.</summary>
        public static readonly int[] PapernovStasevich = new int[]
        {
            1073741825, 536870913, 268435457, 134217729, 67108865,
            33554433, 16777217, 8388609, 4194305, 2097153,
            1048577, 524289, 262145, 131073, 65537,
            32769, 16385, 8193, 4097, 2049,
            1025, 513, 257, 129, 65,
            33, 17, 9, 5, 3,
            1
        };

        /// <summary>Sedgewick (1986) sequence, in decreasing order.</summary>
        public static readonly int[] Sedgewick = new int[]
        {
            1073643521, 603906049, 268386305, 150958081, 67084289,
            37730305, 16764929, 9427969, 4188161, 2354689,
            1045505, 587521, 260609, 146305, 64769,
            36289, 16001, 8929, 3905, 2161,
            929, 505, 209, 109, 41,
            19, 5, 1
        };

        /// <summary>
        /// Generates the gaps of <paramref name="seq"/> for an array of size <paramref name="n"/>, in decreasing order.
        /// </summary>
        /// <param name="n">Array size (upper bound for the gaps; see each generator for the exact bound).</param>
        /// <param name="seq">Gap sequence.</param>
        /// <returns>
        /// The generated gaps. <see cref="ShellSequence.IncerpiSedgewick"/> has no generator: its gaps
        /// are taken from the <see cref="IncerpiSedgewick"/> table (gaps not larger than <paramref name="n"/>).
        /// </returns>
        public static int[] GenerateSequence(int n, ShellSequence seq)
        {
            if (seq == ShellSequence.Sedgewick)
                return SedgewickGaps(n).ToArray();
            if (seq == ShellSequence.Knuth)
                return KnuthGaps(n).ToArray();
            if (seq == ShellSequence.PapernovStasevich)
                return PapernovStasevichGaps(n).ToArray();
            if (seq == ShellSequence.Shell)
                return ShellGaps(n).ToArray();
            if (seq == ShellSequence.Lee)
                return LeeGaps(n).ToArray();
            if (seq == ShellSequence.Pratt)
                return PrattGaps(n).ToArray();
            if (seq == ShellSequence.Hibbard)
                return HibbardGaps(n).ToArray();
            if (seq == ShellSequence.IncerpiSedgewick)
                return IncerpiSedgewick.Where(h => h <= n).ToArray();
            throw new ArgumentOutOfRangeException(nameof(seq), seq, "Unknown gap sequence.");
        }

        /// <summary>
        /// Generates Shell's sequence n/2, n/4, n/8, ..., 1.
        /// </summary>
        /// <param name="n">Array size.</param>
        /// <returns>The gaps in decreasing order (empty when <paramref name="n"/> &lt; 2).</returns>
        public static List<int> ShellGaps(int n)
        {
            List<int> gaps = new();
            int gap = n / 2;
            while (gap > 0)
            {
                gaps.Add(gap);
                gap /= 2;
            }
            return gaps;
        }

        /// <summary>
        /// Generates Hibbard's sequence 2^k - 1, for k ≥ 1, up to <paramref name="n"/> included.
        /// </summary>
        /// <param name="n">Largest allowed gap.</param>
        /// <returns>The gaps in decreasing order.</returns>
        public static List<int> HibbardGaps(int n)
        {
            List<int> gaps = new List<int>();
            for (int k = 1; ; k++)
            {
                // compute h = 2^k - 1
                long h = (1L << k) - 1; // use long to avoid a temporary overflow
                if (h > n) // (|| h > int.MaxValue)
                    break; // stop before exceeding the range of int
                gaps.Add((int)h);
            }
            gaps.Reverse();
            return gaps;
        }

        /// <summary>
        /// Generates the Papernov &amp; Stasevich sequence 1, then 2^k + 1 for k ≥ 1, strictly below <paramref name="n"/>.
        /// </summary>
        /// <param name="n">Strict upper bound for the gaps.</param>
        /// <returns>The gaps in decreasing order.</returns>
        public static List<int> PapernovStasevichGaps(int n)
        {
            List<int> gaps = new();
            long h = 1;
            int k = 1;
            while (h < n)
            {
                gaps.Add((int)h);
                h = (1L << k) + 1;
                k++;
            }
            gaps.Reverse(); // decreasing order for Shell sort
            return gaps;
        }

        /// <summary>
        /// Generates Knuth's sequence (3^k - 1)/2, for k ≥ 1, up to <c>ceil(n / 3)</c> included.
        /// </summary>
        /// <param name="n">Array size.</param>
        /// <returns>The gaps in decreasing order.</returns>
        public static List<int> KnuthGaps(int n)
        {
            List<int> gaps = new();
            int k = 1;
            int h;
            int hmax = (int)Math.Ceiling((double)n / 3);
            while (true)
            {
                h = (int)((Math.Pow(3, k) - 1) / 2);
                if (h > hmax)
                    break;
                gaps.Add(h);
                k++;
            }
            gaps.Reverse(); // decreasing order for Shell sort
            return gaps;
        }

        /// <summary>
        /// Generates Sedgewick's sequence [1986] up to <paramref name="n"/> included:
        /// 9(2^k - 2^(k/2)) + 1 for even k, and 8*2^k - 6*2^((k+1)/2) + 1 for odd k.
        /// </summary>
        /// <param name="n">Largest allowed gap.</param>
        /// <returns>The gaps in decreasing order.</returns>
        public static List<int> SedgewickGaps(int n)
        {
            List<int> gaps = new();
            int k = 0;
            long gap;
            while (true)
            {
                if (k % 2 == 0)
                    gap = 9 * ((long)Math.Pow(2, k) - (long)Math.Pow(2, k / 2)) + 1;
                else
                    gap = 8 * ((long)Math.Pow(2, k)) - 6 * ((long)Math.Pow(2, (k + 1) / 2)) + 1;
                if (gap > n)
                    break;
                gaps.Add((int)gap);
                k++;
            }
            gaps.Reverse(); // decreasing order for Shell sort
            return gaps;
        }

        /// <summary>
        /// Generates Lee's sequence ceil((γ^k - 1) / (γ - 1)), for k ≥ 1 and γ = 2.243609061420001,
        /// strictly below <paramref name="n"/>.
        /// </summary>
        /// <param name="n">Strict upper bound for the gaps.</param>
        /// <returns>The gaps in decreasing order.</returns>
        public static List<int> LeeGaps(int n)
        {
            List<int> gaps = new();
            double gamma = 2.243609061420001;
            int k = 1;
            long gap; // use long to avoid a temporary overflow
            do
            {
                gap = (long)Math.Ceiling((Math.Pow(gamma, k) - 1) / (gamma - 1));
                if (gap < n)
                {
                    gaps.Add((int)gap);
                }
                k++;
            } while (gap < n);
            gaps.Reverse(); // decreasing order for Shell sort
            return gaps;
        }

        /// <summary>
        /// Generates Pratt's sequence: every number 2^i * 3^j strictly below <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">Strict upper bound for the gaps.</param>
        /// <returns>The gaps in decreasing order, without duplicates.</returns>
        public static List<int> PrattGaps(int maxValue)
        {
            HashSet<int> gaps = new(); // avoid duplicates

            // Generate every combination 2^i * 3^j < maxValue
            for (long i = 1; i < maxValue; i *= 2)
            {
                for (long j = i; j < maxValue; j *= 3)
                {
                    gaps.Add((int)j);
                }
            }

            // Decreasing order for Shell sort
            return gaps.OrderByDescending(x => x).ToList();
        }
    }
}
