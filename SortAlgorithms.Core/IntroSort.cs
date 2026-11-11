using System.Numerics;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Introsort, or introspective sort (Musser, 1997): a quicksort that monitors its own recursion depth and
    /// switches to heap sort when the depth exceeds <c>2 floor(log2 n)</c>, which bounds the worst case to
    /// O(n log n). Small sub-arrays are sorted by insertion sort.
    /// </summary>
    /// <remarks>
    /// <para>Mirrors the pseudo-code of the report (<c>IntroSort</c>, <c>IntroSortLoop</c>, <c>MedianOfThree</c>):
    /// the median of the first, middle and last elements is moved to the front of the sub-array, which is then
    /// split by Hoare's partition (<see cref="QuickSort.PartitionHoare"/>, whose pivot is the first element).
    /// As in Musser's implementation, only the right part is sorted by a recursive call; the left part is
    /// handled by the loop itself.</para>
    /// <para>Time: O(n log n) in the worst case. Not adaptive: an already sorted input still takes
    /// Θ(n log n) comparisons. In place (O(log n) stack, the recursion depth being bounded by the depth limit),
    /// not stable.</para>
    /// </remarks>
    public static class IntroSort
    {
        /// <summary>
        /// Sub-arrays with at most this many elements are sorted by insertion sort (threshold <c>th</c> of the
        /// report).
        /// </summary>
        public const int InsertionSortThreshold = 16;

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, allowing <c>2 floor(log2 n)</c> partitioning levels
        /// before switching to heap sort.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            int n = A.Length;
            if (n > 1)
            {
                IntroSortLoop(A, 0, n - 1, 2 * BitOperations.Log2((uint)n));
            }
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with an explicit depth limit (exposed so that tests can
        /// force the heap sort fallback).
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <param name="depthLimit">
        /// Number of partitioning levels allowed before the current sub-array is sorted by heap sort; must be
        /// non-negative. A limit of 0 sorts any array of more than <see cref="InsertionSortThreshold"/> elements
        /// with heap sort alone.
        /// </param>
        public static void Sort(int[] A, int depthLimit)
        {
            int n = A.Length;
            if (n > 1)
            {
                IntroSortLoop(A, 0, n - 1, depthLimit);
            }
        }

        /// <summary>
        /// Sorts <c>A[p...r]</c> (inclusive bounds): while the sub-array has more than
        /// <see cref="InsertionSortThreshold"/> elements, partitions it, recurses on the right part and loops on
        /// the left part; switches to heap sort when the depth budget runs out.
        /// </summary>
        /// <param name="A">Array containing the sub-array.</param>
        /// <param name="p">Index of the first element of the sub-array.</param>
        /// <param name="r">Index of the last element of the sub-array (inclusive).</param>
        /// <param name="depth">Number of partitioning levels still allowed on this branch.</param>
        private static void IntroSortLoop(int[] A, int p, int r, int depth)
        {
            while (r - p + 1 > InsertionSortThreshold)
            {
                if (depth == 0)
                {
                    HeapSort.SortRange(A, p, r); // switch to heap sort
                    return;
                }
                depth--;
                int m = MedianOfThree(A, p, p + (r - p) / 2, r);
                Utils.Swap(A, p, m);                   // the pivot is moved to the front
                int q = QuickSort.PartitionHoare(A, p, r);
                IntroSortLoop(A, q + 1, r, depth);     // right part
                r = q;                                 // loop on the left part
            }
            InsertionSort.SortRange(A, p, r);
        }

        /// <summary>
        /// Returns the index (among <paramref name="i"/>, <paramref name="j"/> and <paramref name="k"/>) of the
        /// median of <c>A[i]</c>, <c>A[j]</c> and <c>A[k]</c>. Also used by <see cref="PdqSort"/> to choose its pivot.
        /// </summary>
        /// <param name="A">Array holding the three values.</param>
        /// <param name="i">Index of the first value.</param>
        /// <param name="j">Index of the second value.</param>
        /// <param name="k">Index of the third value.</param>
        /// <returns>The index m in {i, j, k} such that <c>A[m]</c> is the median of the three values.</returns>
        internal static int MedianOfThree(int[] A, int i, int j, int k)
        {
            if (A[i] < A[j])
            {
                if (A[j] < A[k]) return j;      // A[i] < A[j] < A[k]
                return (A[i] < A[k]) ? k : i;   // A[j] is the maximum
            }
            if (A[i] < A[k]) return i;          // A[j] <= A[i] < A[k]
            return (A[j] < A[k]) ? k : j;       // A[i] is the maximum
        }
    }
}
