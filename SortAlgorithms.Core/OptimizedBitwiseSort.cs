using System;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Optimized variants of the binary LSD radix sort (<see cref="BitwiseSort"/>).
    /// </summary>
    /// <remarks>
    /// <para>All variants only process the bits up to the most significant bit set in any value
    /// (computed by OR-ing all values together), skip the copy for passes where every value falls
    /// into the same group, and swap the roles of the input and auxiliary arrays instead of copying
    /// group '0' back.</para>
    /// <para>Time: Θ(kmax·n), where kmax is the number of bits needed to represent max(A[i]).
    /// Space: Θ(n) extra. Stable. Sorts in place (the result ends up in the input array).</para>
    /// <para>Values must be non-negative: the sign bit is treated like any other bit,
    /// so negative values end up after all non-negative values.</para>
    /// <para>The variants are numbered in the order of the successive optimizations:
    /// <see cref="_Sort1"/> → <see cref="_Sort2"/> → <see cref="_Sort3"/> → <see cref="Sort"/>.</para>
    /// </remarks>
    public static class OptimizedBitwiseSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order using the most optimized variant.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        /// <remarks>
        /// Cumulative improvements:
        /// <list type="bullet">
        /// <item><description>over <see cref="_Sort1"/>: uses <see cref="Array.Copy(Array, int, Array, int, int)"/> instead of <c>for</c> loops for the concatenation;</description></item>
        /// <item><description>over <see cref="_Sort2"/>: replaces the mask (1 &lt;&lt; k) with a right shift and a test of the LSB, and removes branches from the inner loop;</description></item>
        /// <item><description>over <see cref="_Sort3"/>: computes (1 - bit) without a subtraction, as (1 ^ bit).</description></item>
        /// </list>
        /// </remarks>
        public static void Sort(int[] A)
        {
            int n = A.Length;
            if (n <= 0) return;

            int kmax = BitAggregationMSB(A) + 1; // number of bits needed to represent max(A[i])
            if (kmax == 0) return;

            int[] A0 = new int[n];
            int[] A1 = new int[n];

            int n0; // size of group '0'
            int n1; // size of group '1'

            int[] source = A;

            for (int k = 0; k < kmax; ++k)
            {
                n0 = 0; // size of group '0'
                n1 = 0; // size of group '1'
                var local = A;
                for (int i = 0; i < local.Length; i++)
                {
                    int v = local[i];
                    int bit = (v >> k) & 1;
                    A0[n0] = v; // unconditional write into A0 ...
                    A1[n1] = v; // ... and into A1
                    n0 += (1 ^ bit);
                    n1 += bit;
                    // the "losing" write is overwritten by the next insertion.
                }
                if (n0 != 0 && n1 != 0)
                {
                    // swap the roles of A0 and A
                    int[] temp = A;
                    A = A0;
                    A0 = temp;

                    // append array A1
                    Array.Copy(A1, 0, A, n0, n1);
                }
            }

            if (A != source)
            {
                Array.Copy(A, source, A.Length);
            }
        }

        /// <summary>
        /// Returns the index of the most significant set bit of <paramref name="n"/>.
        /// </summary>
        /// <param name="n">The value to inspect.</param>
        /// <returns>The 0-based index of the highest set bit, or -1 if <paramref name="n"/> is 0.</returns>
        private static int IndexOfMSB(uint n)
        {
            if (n == 0)
                return -1;
            int msb = 0;
            while (n > 1)
            {
                n >>= 1;
                msb++;
            }
            return msb;
        }

        /// <summary>
        /// Returns the index of the most significant bit set in any element of <paramref name="A"/>
        /// (that is, the MSB of the bitwise OR of all elements).
        /// </summary>
        /// <param name="A">The array to inspect.</param>
        /// <returns>The 0-based index of the highest set bit, or -1 if all elements are 0.</returns>
        private static int BitAggregationMSB(int[] A)
        {
            uint x = 0;
            var local = A;
            for (int i = 0; i < local.Length; ++i)
            {
                x |= (uint)local[i];
            }
            return IndexOfMSB(x);
        }

        /// <summary>
        /// Basic version: only processes the bits up to the MSB of max(A[i]) and swaps arrays
        /// instead of copying group '0' back.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        public static void _Sort1(int[] A)
        {
            int n = A.Length;
            if (n <= 0) return;

            int[] A0 = new int[n];
            int[] A1 = new int[n];

            uint x = 0;
            for (int i = 0; i < n; ++i)
            {
                x |= (uint)A[i];
            }

            int t = IndexOfMSB(x) + 1; // number of bits needed to represent max(A[i])

            int n0; // size of group '0'
            int n1; // size of group '1'

            int[] source = A;

            for (int k = 0; k < t; ++k)
            {
                n0 = 0; // size of group '0'
                n1 = 0; // size of group '1'
                int mask = 1 << k; // 2^k
                for (int i = 0; i < n; i++)
                {
                    int v = A[i];
                    if ((v & mask) != 0) // test the k-th bit of the i-th element
                    {
                        A1[n1] = v;
                        n1++;
                    }
                    else
                    {
                        A0[n0] = v;
                        n0++;
                    }
                }
                if (n0 != 0 && n1 != 0)
                {
                    // swap the roles of A0 and A
                    int[] temp = A;
                    A = A0;
                    A0 = temp;

                    // append array A1
                    for (int i = 0; i < n1; i++)
                        A[n0 + i] = A1[i];
                }
            }

            if (A != source)
            {
                for (int i = 0; i < n; i++)
                    source[i] = A[i];
            }
        }

        /// <summary>
        /// Improves <see cref="_Sort1"/> by using <see cref="Array.Copy(Array, int, Array, int, int)"/>
        /// instead of a <c>for</c> loop for the concatenation.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        public static void _Sort2(int[] A)
        {
            int n = A.Length;
            if (n <= 0) return;

            int kmax = BitAggregationMSB(A) + 1; // number of bits needed to represent max(A[i])
            if (kmax == 0) return;

            int[] A0 = new int[n];
            int[] A1 = new int[n];

            int n0; // size of group '0'
            int n1; // size of group '1'

            int[] source = A;

            for (int k = 0; k < kmax; ++k)
            {
                n0 = 0; // size of group '0'
                n1 = 0; // size of group '1'
                int mask = 1 << k; // 2^k
                var local = A;
                for (int i = 0; i < local.Length; i++)
                {
                    int v = local[i];
                    if ((v & mask) != 0) // test the k-th bit of the i-th element
                    {
                        A1[n1] = v;
                        n1++;
                    }
                    else
                    {
                        A0[n0] = v;
                        n0++;
                    }
                }
                if (n0 != 0 && n1 != 0)
                {
                    // swap the roles of A0 and A
                    int[] temp = A;
                    A = A0;
                    A0 = temp;

                    // append array A1
                    Array.Copy(A1, 0, A, n0, n1);
                }
            }

            if (A != source)
            {
                Array.Copy(A, source, A.Length);
            }
        }

        /// <summary>
        /// Improves <see cref="_Sort2"/> by avoiding branches in the inner loop: each value is
        /// written unconditionally into both groups and only the matching group counter advances.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        public static void _Sort3(int[] A)
        {
            int n = A.Length;
            if (n <= 0) return;

            int kmax = BitAggregationMSB(A) + 1; // number of bits needed to represent max(A[i])
            if (kmax == 0) return;

            int[] A0 = new int[n];
            int[] A1 = new int[n];

            int n0; // size of group '0'
            int n1; // size of group '1'

            int[] source = A;

            for (int k = 0; k < kmax; ++k)
            {
                n0 = 0; // size of group '0'
                n1 = 0; // size of group '1'
                var local = A;
                for (int i = 0; i < local.Length; i++)
                {
                    int v = local[i];
                    int bit = (v >> k) & 1;
                    A0[n0] = v; // unconditional write into A0 ...
                    A1[n1] = v; // ... and into A1
                    n0 += (1 - bit);
                    n1 += bit;
                    // the "losing" write is overwritten by the next insertion.
                }
                if (n0 != 0 && n1 != 0)
                {
                    // swap the roles of A0 and A
                    int[] temp = A;
                    A = A0;
                    A0 = temp;

                    // append array A1
                    Array.Copy(A1, 0, A, n0, n1);
                }
            }

            if (A != source)
            {
                Array.Copy(A, source, A.Length);
            }
        }
    }
}
