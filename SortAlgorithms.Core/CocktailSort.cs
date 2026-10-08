namespace SortAlgorithms.Core
{
    /// <summary>
    /// Cocktail shaker sort: a bidirectional bubble sort that alternates a left-to-right pass (moving the
    /// largest element to the end) and a right-to-left pass (moving the smallest element to the front).
    /// </summary>
    /// <remarks>
    /// O(n²) in the worst and average cases, O(n) on already sorted input. Stops as soon as a pass makes
    /// no swap. In place, stable.
    /// </remarks>
    public static class CocktailSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            int i = 0;
            int j = A.Length - 1;
            bool swapped = true;
            while (i < j && swapped)
            {
                swapped = false;
                for (int k = i; k < j; ++k)
                {
                    if (A[k] > A[k + 1])
                    {
                        Utils.Swap(A, k, k + 1);
                        swapped = true;
                    }
                }
                if (swapped)
                {
                    j--;
                    swapped = false;
                    for (int k = j; k >= i + 1; --k)
                    {
                        if (A[k] < A[k - 1])
                        {
                            Utils.Swap(A, k, k - 1);
                            swapped = true;
                        }
                    }
                    i++;
                }
            }
        }
    }
}
