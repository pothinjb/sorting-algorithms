namespace SortAlgorithms.Core
{
    /// <summary>
    /// Counting sort (CLRS): sorts integers in a known range [0, k) by counting
    /// the occurrences of each value.
    /// </summary>
    /// <remarks>
    /// <para>Time: Θ(n + k). Space: Θ(n + k) extra. Stable. Not in place: the sorted values
    /// are returned in a new array and the input array is left unchanged.</para>
    /// </remarks>
    public static class CountingSort
    {
        /// <summary>
        /// Returns a sorted copy of <paramref name="A"/>.
        /// </summary>
        /// <param name="A">The array to sort. Every value must be in [0, <paramref name="k"/>).</param>
        /// <param name="k">Exclusive upper bound of the values (size of the count array).</param>
        /// <returns>A new array containing the elements of <paramref name="A"/> in ascending order
        /// (an empty array if <paramref name="A"/> is empty).</returns>
        /// <remarks>
        /// Values outside [0, <paramref name="k"/>) (including a value equal to <paramref name="k"/>)
        /// throw an <see cref="System.IndexOutOfRangeException"/>.
        /// This version keeps a running cumulative sum while building the prefix sums;
        /// see <see cref="_Sort1"/> for the textbook version.
        /// </remarks>
        public static int[] Sort(int[] A, int k)
        {
            int n = A.Length;
            if (n <= 0) return new int[0];

            int[] B = new int[n];

            int[] h = new int[k]; // initialized to 0

            var local = A;
            for (int i = 0; i < local.Length; ++i)
            {
                h[A[i]] += 1;
            }

            int cumsum = h[0];
            for (int j = 1; j < k; ++j)
            {
                cumsum += h[j];
                h[j] = cumsum; // = h[j] + h[j-1]
            }

            for (int i = n - 1; i >= 0; --i)
            {
                int v = A[i];
                int pos = h[v] - 1;
                B[pos] = v;
                h[v] = pos;
            }

            return B;
        }

        /// <summary>
        /// Textbook (CLRS) version of counting sort. Returns a sorted copy of <paramref name="A"/>.
        /// </summary>
        /// <param name="A">The array to sort. Every value must be in [0, <paramref name="k"/>).</param>
        /// <param name="k">Exclusive upper bound of the values (size of the count array).</param>
        /// <returns>A new array containing the elements of <paramref name="A"/> in ascending order
        /// (an empty array if <paramref name="A"/> is empty).</returns>
        public static int[] _Sort1(int[] A, int k)
        {
            int n = A.Length;
            if (n <= 0) return new int[0];

            int[] B = new int[n];

            int[] h = new int[k]; // initialized to 0

            var local = A;
            for (int i = 0; i < local.Length; ++i)
            {
                h[A[i]] += 1;
            }

            for (int j = 1; j < k; ++j)
            {
                h[j] = h[j] + h[j - 1];
            }

            for (int i = n - 1; i >= 0; --i)
            {
                int v = A[i];
                h[v] -= 1;
                B[h[v]] = v;
            }

            return B;
        }
    }
}
