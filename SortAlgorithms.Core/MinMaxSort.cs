namespace SortAlgorithms.Core
{
    /// <summary>
    /// Min-max selection sort (double selection sort): each pass finds both the minimum and the maximum of
    /// the unsorted range <c>A[ileft...iright]</c> and puts them at its two ends.
    /// </summary>
    /// <remarks>
    /// O(n²) comparisons in all cases, but half as many passes as <see cref="SelectionSort"/>.
    /// In place, not stable. When n is odd, the minimum is first moved to <c>A[0]</c> so that the remaining
    /// range has an even length.
    /// </remarks>
    public static class MinMaxSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, finding the minimum and maximum with a simple scan
        /// (about 2 comparisons per element).
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            int n = A.Length;
            int ileft = 0;
            int iright = n - 1;
            if ((n & 1) != 0)
            {
                int imin = IndexOf.Min(A);
                Utils.Swap(A, 0, imin);
                ileft = 1;
            }
            while (ileft < iright)
            {
                var (imin, imax) = IndexOf.MinMax(A, ileft, iright);
                Utils.Swap(A, ileft, imin);
                if (imax == ileft)
                    Utils.Swap(A, iright, imin);
                else
                    Utils.Swap(A, iright, imax);
                ileft += 1;
                iright -= 1;
            }
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, finding the minimum and maximum with
        /// <see cref="IndexOf.MinMaxTournament"/> (about 3 comparisons per pair of elements).
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void OptimizedSort(int[] A)
        {
            int n = A.Length;
            int ileft = 0;
            int iright = n - 1;
            if ((n & 1) != 0)
            {
                int imin = IndexOf.Min(A);
                Utils.Swap(A, 0, imin);
                ileft = 1;
            }
            while (ileft < iright)
            {
                var (imin, imax) = IndexOf.MinMaxTournament(A, ileft, iright);
                Utils.Swap(A, ileft, imin);
                if (imax == ileft)
                    Utils.Swap(A, iright, imin);
                else
                    Utils.Swap(A, iright, imax);
                ileft += 1;
                iright -= 1;
            }
        }
    }
}
