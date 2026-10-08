namespace SortAlgorithms.Core
{
    /// <summary>
    /// Bubble sort: repeatedly swaps adjacent elements that are out of order, so that after each pass
    /// the largest remaining element reaches its final position.
    /// </summary>
    /// <remarks>
    /// Always n(n-1)/2 comparisons (no early exit, see <see cref="ImprovedBubbleSort"/>),
    /// in place, stable.
    /// </remarks>
    public static class BubbleSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            int n = A.Length;
            for (int i = n - 1; i > 0; --i)
            {
                for (int j = 0; j < i; ++j)
                {
                    if (A[j] > A[j + 1])
                        Utils.Swap(A, j, j + 1);
                }
            }
        }
    }
}
