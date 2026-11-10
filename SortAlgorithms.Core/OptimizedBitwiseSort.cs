using System;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Optimized variants of the binary LSD radix sort (<see cref="BitwiseSort"/>).
    /// </summary>
    /// <remarks>
    /// <para>All variants skip the bits that cannot change the order and swap the roles of the input
    /// and auxiliary arrays instead of copying group '0' back. <see cref="_Sort1"/> to <see cref="_Sort3"/>
    /// only process the bits up to the most significant bit set in any value (computed by OR-ing all
    /// values together) and skip the copy for passes where every value falls into the same group.
    /// From <see cref="_Sort4"/> on, every bit that has the same value in all elements is skipped
    /// (computed as the OR of all values XOR their AND).</para>
    /// <para>Time: Θ(kmax·n), where kmax is the number of bits needed to represent max(A[i]); from
    /// <see cref="_Sort4"/> on, kmax is the number of bits that are not the same in every element.
    /// Space: Θ(n) extra (two auxiliary arrays of size n, one for <see cref="Sort"/>).
    /// Stable. Sorts in place (the result ends up in the input array).</para>
    /// <para>Values must be non-negative: the sign bit is treated like any other bit,
    /// so negative values end up after all non-negative values.</para>
    /// <para><see cref="_Sort5"/>, <see cref="_Sort6"/> and <see cref="Sort"/> use pointers (unsafe code)
    /// to avoid the bounds checks on the writes into the auxiliary arrays.</para>
    /// <para>The variants are numbered in the order of the successive optimizations:
    /// <see cref="_Sort1"/> → <see cref="_Sort2"/> → <see cref="_Sort3"/> → <see cref="_Sort4"/>
    /// → <see cref="_Sort5"/> → <see cref="_Sort6"/> → <see cref="Sort"/>.</para>
    /// </remarks>
    public static class OptimizedBitwiseSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order using the most optimized variant.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        /// <remarks>
        /// <para>Cumulative improvements:</para>
        /// <list type="bullet">
        /// <item><description>over <see cref="_Sort1"/>: uses <see cref="Array.Copy(Array, int, Array, int, int)"/> instead of <c>for</c> loops for the concatenation;</description></item>
        /// <item><description>over <see cref="_Sort2"/>: replaces the mask (1 &lt;&lt; k) with a right shift and a test of the LSB, and removes branches from the inner loop;</description></item>
        /// <item><description>over <see cref="_Sort3"/>: computes (1 - bit) without a subtraction, as (1 ^ bit), and skips every bit that is the same in all elements;</description></item>
        /// <item><description>over <see cref="_Sort4"/>: writes through pointers, without bounds checks;</description></item>
        /// <item><description>over <see cref="_Sort5"/>: unrolls the inner loop by 4;</description></item>
        /// <item><description>over <see cref="_Sort6"/>: uses a single auxiliary array, in which group '0' grows from the front and
        /// group '1' grows backward from the back, so the concatenation needs no copy at all.</description></item>
        /// </list>
        /// <para>Group '1' ends up in reverse order. The next pass reads that part backward, which
        /// restores the original order and keeps the sort stable. After the last pass, group '1' is
        /// put back in order once.</para>
        /// <para>The unconditional writes stay valid: the slots between the two write positions are
        /// free, and on the last element of a pass both positions are the same slot and receive the
        /// same value.</para>
        /// </remarks>
        public static unsafe void Sort(int[] A)
        {
            int n = A.Length;
            if (n <= 1) return;

            uint varying = VaryingBits(A); // bits that are not the same in every element
            if (varying == 0) return;      // all elements are equal: nothing to do

            // single auxiliary array; every slot is written before it is read, so it is not zeroed
            int[] B = GC.AllocateUninitializedArray<int>(n);

            // size of group '0' of the previous pass: src[0..n0) is in order, src[n0..n) is in reverse order
            // (the input array is entirely in order)
            int n0 = n;

            fixed (int* pSource = A, pB = B)
            {
                int* src = pSource; // array read by the pass
                int* dst = pB;      // array written by the pass

                for (int k = 0; varying != 0; ++k, varying >>= 1)
                {
                    if ((varying & 1) == 0) continue; // bit k is the same in every element: the pass would not move anything

                    int* p0 = dst;         // next slot of group '0' (grows forward from the front)
                    int* p1 = dst + n - 1; // next slot of group '1' (grows backward from the back)
                    int* mid = src + n0;

                    // 1) group '0' of the previous pass, read forward
                    int* p = src;
                    for (; p + 3 < mid; p += 4)
                    {
                        int v0 = p[0], v1 = p[1], v2 = p[2], v3 = p[3];
                        int b0 = (v0 >> k) & 1, b1 = (v1 >> k) & 1, b2 = (v2 >> k) & 1, b3 = (v3 >> k) & 1;
                        *p0 = v0; *p1 = v0; p0 += 1 ^ b0; p1 -= b0;
                        *p0 = v1; *p1 = v1; p0 += 1 ^ b1; p1 -= b1;
                        *p0 = v2; *p1 = v2; p0 += 1 ^ b2; p1 -= b2;
                        *p0 = v3; *p1 = v3; p0 += 1 ^ b3; p1 -= b3;
                    }
                    for (; p < mid; p++)
                    {
                        int v = *p;
                        int bit = (v >> k) & 1;
                        *p0 = v; *p1 = v; p0 += 1 ^ bit; p1 -= bit;
                    }

                    // 2) group '1' of the previous pass, stored in reverse order: read backward
                    p = src + n - 1;
                    for (; p - 3 >= mid; p -= 4)
                    {
                        int v0 = p[0], v1 = p[-1], v2 = p[-2], v3 = p[-3];
                        int b0 = (v0 >> k) & 1, b1 = (v1 >> k) & 1, b2 = (v2 >> k) & 1, b3 = (v3 >> k) & 1;
                        *p0 = v0; *p1 = v0; p0 += 1 ^ b0; p1 -= b0;
                        *p0 = v1; *p1 = v1; p0 += 1 ^ b1; p1 -= b1;
                        *p0 = v2; *p1 = v2; p0 += 1 ^ b2; p1 -= b2;
                        *p0 = v3; *p1 = v3; p0 += 1 ^ b3; p1 -= b3;
                    }
                    for (; p >= mid; p--)
                    {
                        int v = *p;
                        int bit = (v >> k) & 1;
                        *p0 = v; *p1 = v; p0 += 1 ^ bit; p1 -= bit;
                    }

                    n0 = (int)(p0 - dst);
                    int* temp = src; src = dst; dst = temp; // the output becomes the input of the next pass
                }

                // put group '1' of the last pass back in order (and the result back into A if needed)
                if (src != pSource)
                {
                    Buffer.MemoryCopy(src, pSource, (long)n0 * sizeof(int), (long)n0 * sizeof(int));
                    for (int i = n0, j = n - 1; i < n; i++, j--)
                        pSource[i] = src[j];
                }
                else
                {
                    for (int i = n0, j = n - 1; i < j; i++, j--)
                    {
                        int t = pSource[i];
                        pSource[i] = pSource[j];
                        pSource[j] = t;
                    }
                }
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
        /// Returns the mask of the bits that are not the same in every element of <paramref name="A"/>:
        /// the bitwise OR of all elements XOR their bitwise AND.
        /// </summary>
        /// <param name="A">The array to inspect.</param>
        /// <returns>
        /// A mask whose bit k is 1 if and only if bit k is 0 in at least one element and 1 in at least
        /// one other; 0 if all elements are equal (or if there are fewer than two).
        /// </returns>
        /// <remarks>
        /// A pass on a bit that is the same in every element puts all the elements into one group
        /// without moving them, so it can be skipped. Conversely, on a bit of the mask, both groups are
        /// non-empty.
        /// </remarks>
        private static uint VaryingBits(int[] A)
        {
            uint or = 0;    // bits set in at least one element
            uint and = ~0u; // bits set in every element
            var local = A;
            for (int i = 0; i < local.Length; ++i)
            {
                uint x = (uint)local[i];
                or |= x;
                and &= x;
            }
            return or ^ and;
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

        /// <summary>
        /// Improves <see cref="_Sort3"/> by computing (1 - bit) without a subtraction, as (1 ^ bit),
        /// and by skipping every bit that is the same in all elements, not only the bits above the MSB.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        /// <remarks>
        /// The mask of the bits to process is the OR of all values XOR their AND (<see cref="VaryingBits"/>).
        /// On such a bit, both groups are non-empty, so the test <c>n0 != 0 &amp;&amp; n1 != 0</c> before
        /// the concatenation is no longer needed.
        /// </remarks>
        public static void _Sort4(int[] A)
        {
            int n = A.Length;
            if (n <= 1) return;

            uint varying = VaryingBits(A); // bits that are not the same in every element
            if (varying == 0) return;      // all elements are equal: nothing to do

            int[] A0 = new int[n];
            int[] A1 = new int[n];

            int[] source = A;

            for (int k = 0; varying != 0; ++k, varying >>= 1)
            {
                if ((varying & 1) == 0) continue; // bit k is the same in every element: the pass would not move anything

                int n0 = 0; // size of group '0'
                int n1 = 0; // size of group '1'
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

                // bit k varies, so both groups are non-empty: swap the roles of A0 and A
                int[] temp = A;
                A = A0;
                A0 = temp;

                // append array A1
                Array.Copy(A1, 0, A, n0, n1);
            }

            if (A != source)
            {
                Array.Copy(A, source, A.Length);
            }
        }

        /// <summary>
        /// Improves <see cref="_Sort4"/> by writing through pointers, which removes the bounds checks
        /// on <c>A0[n0]</c> and <c>A1[n1]</c>.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        /// <remarks>
        /// The three arrays are pinned once for the whole sort (not once per pass), and the roles of
        /// the arrays are swapped by swapping pointers. Every slot of the auxiliary arrays is written
        /// before it is read, so they are allocated without being zeroed.
        /// </remarks>
        public static unsafe void _Sort5(int[] A)
        {
            int n = A.Length;
            if (n <= 1) return;

            uint varying = VaryingBits(A); // bits that are not the same in every element
            if (varying == 0) return;      // all elements are equal: nothing to do

            int[] A0 = GC.AllocateUninitializedArray<int>(n);
            int[] A1 = GC.AllocateUninitializedArray<int>(n);

            fixed (int* pSource = A, pA0 = A0, pA1 = A1)
            {
                int* a = pSource; // current array A
                int* a0 = pA0;    // group '0' (becomes A at the end of the pass)
                int* a1 = pA1;    // group '1'

                for (int k = 0; varying != 0; ++k, varying >>= 1)
                {
                    if ((varying & 1) == 0) continue; // bit k is the same in every element: the pass would not move anything

                    int* p0 = a0; // next slot of group '0'
                    int* p1 = a1; // next slot of group '1'
                    for (int* p = a, end = a + n; p < end; p++)
                    {
                        int v = *p;
                        int bit = (v >> k) & 1;
                        *p0 = v; // unconditional write into A0 ...
                        *p1 = v; // ... and into A1
                        p0 += 1 ^ bit;
                        p1 += bit;
                    }
                    long n0 = p0 - a0; // size of group '0'
                    long n1 = p1 - a1; // size of group '1'

                    // swap the roles of A0 and A
                    int* temp = a;
                    a = a0;
                    a0 = temp;

                    // append array A1
                    Buffer.MemoryCopy(a1, a + n0, n1 * sizeof(int), n1 * sizeof(int));
                }

                if (a != pSource)
                {
                    Buffer.MemoryCopy(a, pSource, (long)n * sizeof(int), (long)n * sizeof(int));
                }
            }
        }

        /// <summary>
        /// Improves <see cref="_Sort5"/> by unrolling the inner loop by 4: the four values are read
        /// first, then their four bits are extracted, then they are written.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        /// <remarks>
        /// Measured on .NET 5, unrolling by 4 is faster than by 2 on large arrays, and unrolling by 8
        /// is slower than by 4.
        /// </remarks>
        public static unsafe void _Sort6(int[] A)
        {
            int n = A.Length;
            if (n <= 1) return;

            uint varying = VaryingBits(A); // bits that are not the same in every element
            if (varying == 0) return;      // all elements are equal: nothing to do

            int[] A0 = GC.AllocateUninitializedArray<int>(n);
            int[] A1 = GC.AllocateUninitializedArray<int>(n);

            fixed (int* pSource = A, pA0 = A0, pA1 = A1)
            {
                int* a = pSource; // current array A
                int* a0 = pA0;    // group '0' (becomes A at the end of the pass)
                int* a1 = pA1;    // group '1'

                for (int k = 0; varying != 0; ++k, varying >>= 1)
                {
                    if ((varying & 1) == 0) continue; // bit k is the same in every element: the pass would not move anything

                    int* p0 = a0; // next slot of group '0'
                    int* p1 = a1; // next slot of group '1'
                    int* p = a;
                    int* end = a + n;
                    for (; p + 3 < end; p += 4)
                    {
                        int v0 = p[0], v1 = p[1], v2 = p[2], v3 = p[3];
                        int b0 = (v0 >> k) & 1, b1 = (v1 >> k) & 1, b2 = (v2 >> k) & 1, b3 = (v3 >> k) & 1;
                        *p0 = v0; *p1 = v0; p0 += 1 ^ b0; p1 += b0;
                        *p0 = v1; *p1 = v1; p0 += 1 ^ b1; p1 += b1;
                        *p0 = v2; *p1 = v2; p0 += 1 ^ b2; p1 += b2;
                        *p0 = v3; *p1 = v3; p0 += 1 ^ b3; p1 += b3;
                    }
                    for (; p < end; p++) // remaining 0 to 3 elements
                    {
                        int v = *p;
                        int bit = (v >> k) & 1;
                        *p0 = v; *p1 = v; p0 += 1 ^ bit; p1 += bit;
                    }
                    long n0 = p0 - a0; // size of group '0'
                    long n1 = p1 - a1; // size of group '1'

                    // swap the roles of A0 and A
                    int* temp = a;
                    a = a0;
                    a0 = temp;

                    // append array A1
                    Buffer.MemoryCopy(a1, a + n0, n1 * sizeof(int), n1 * sizeof(int));
                }

                if (a != pSource)
                {
                    Buffer.MemoryCopy(a, pSource, (long)n * sizeof(int), (long)n * sizeof(int));
                }
            }
        }
    }
}
