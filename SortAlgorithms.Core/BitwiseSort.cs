namespace SortAlgorithms.Core
{
    /// <summary>
    /// Binary LSD radix sort: sorts integers by distributing them into a '0' group
    /// and a '1' group for each bit, from the least significant bit to the most significant one.
    /// </summary>
    /// <remarks>
    /// <para>Time: Θ(t·n) with t = 32 (all bits of an <see cref="int"/> are always processed).</para>
    /// <para>Space: Θ(n) extra (two auxiliary arrays of size n). Stable. Sorts in place
    /// (the result is copied back into the input array).</para>
    /// <para>See <see cref="OptimizedBitwiseSort"/> for variants that stop at the most significant bit actually used.</para>
    /// </remarks>
    public static class BitwiseSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">The array to sort. All values must be non-negative.</param>
        /// <remarks>
        /// Values must be non-negative: the sign bit is treated like any other bit,
        /// so negative values end up after all non-negative values.
        /// </remarks>
        public static void Sort(int[] A)
        {
            int n = A.Length;
            if (n <= 0) return;

            int[] A0 = new int[n];
            int[] A1 = new int[n];

            int t = sizeof(int) * 8; // number of bits in the int data type

            int n0; // size of group '0'
            int n1; // size of group '1'

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
                // concatenate the two arrays A0 and A1 back into A
                for (int i = 0; i < n0; i++)
                    A[i] = A0[i];
                for (int i = 0; i < n1; i++)
                    A[n0 + i] = A1[i];
            }
        }
    }
}
