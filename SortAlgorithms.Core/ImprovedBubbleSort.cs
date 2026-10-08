namespace SortAlgorithms.Core
{
    /// <summary>
    /// Bubble sort with early exit: stops as soon as a pass makes no swap.
    /// </summary>
    /// <remarks>O(n²) in the worst and average cases, O(n) on already sorted input. In place, stable.</remarks>
    public static class ImprovedBubbleSort
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
                bool swapped = false;
                for (int j = 0; j < i; ++j)
                {
                    if (A[j] > A[j + 1])
                    {
                        Utils.Swap(A, j, j + 1);
                        swapped = true;
                    }
                }
                if (!swapped)
                    break;
            }
        }
    }
}
