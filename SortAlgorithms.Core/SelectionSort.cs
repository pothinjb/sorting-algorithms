namespace SortAlgorithms.Core
{
    /// <summary>
    /// Selection sort: repeatedly selects the smallest remaining element.
    /// </summary>
    /// <remarks>O(n²) comparisons in all cases.</remarks>
    public static class SelectionSort
    {
        /// <summary>
        /// Naive selection sort: scans the whole array n times, copies the minimum into a new array and
        /// overwrites it in a working copy with <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="A">Array to sort. It is left unchanged.</param>
        /// <returns>A new array with the elements of <paramref name="A"/> in ascending order.</returns>
        /// <remarks>Always n² comparisons, O(n) extra space.</remarks>
        public static int[] BasicSort(int[] A)
        {
            int n = A.Length;
            int[] C = (int[])A.Clone(); // working copy, so that the caller's array is not destroyed
            int[] B = new int[n];
            for (int i = 0; i < n; ++i)
            {
                int j0 = 0;
                for (int j = 1; j < n; ++j)
                {
                    if (C[j] < C[j0])
                        j0 = j;
                }
                B[i] = C[j0];
                C[j0] = int.MaxValue; // overwrite the selected element
            }
            return B;
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with the classic in-place selection sort:
        /// at step i, the minimum of <c>A[i...n-1]</c> is swapped into <c>A[i]</c>.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <remarks>n(n-1)/2 comparisons, at most n-1 swaps, O(1) extra space, not stable.</remarks>
        public static void Sort(int[] A)
        {
            for (int i = 0, n = A.Length, m = n - 1; i < m; ++i)
            {
                int j0 = i;
                int vmin = A[j0];
                for (int j = i + 1; j < n; ++j)
                {
                    if (A[j] < vmin)
                    {
                        j0 = j;
                        vmin = A[j];
                    }
                }
                if (j0 != i)
                    Utils.Swap(A, j0, i);
            }
        }
    }
}
