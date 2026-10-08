using System.Threading.Tasks;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Merge sort that skips the merge step when the two sorted halves are already in order
    /// (that is, when A[q] &lt;= A[q+1]), with a parallel variant.
    /// </summary>
    /// <remarks>
    /// <para>Time: Θ(n log n) in the worst case, Θ(n) on already sorted input (every merge is skipped).
    /// Space: Θ(n) auxiliary per merge. Stable.</para>
    /// <para>The merge step uses <see cref="int.MaxValue"/> as a sentinel (as in CLRS); an explicit bound check on the left run
    /// keeps the merge correct even when the input itself contains <see cref="int.MaxValue"/>.</para>
    /// </remarks>
    public static class SkipMergeSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        public static void Sort(int[] A)
        {
            mergeSort(A, 0, A.Length - 1);
        }

        /// <summary>
        /// Recursively sorts the subarray A[p...r] (inclusive bounds), merging only when needed.
        /// </summary>
        private static void mergeSort(int[] A, int p, int r)
        {
            if (p < r)
            {
                int q = p + (r - p) / 2; // midpoint
                mergeSort(A, p, q); // sort the first half
                mergeSort(A, q + 1, r); // sort the second half
                // merge only if necessary
                if (A[q] > A[q + 1])
                    merge(A, p, q, r); // merge
            }
        }

        /// <summary>
        /// Merges two adjacent sorted subarrays of A: A[p...q] and A[q+1...r].
        /// </summary>
        private static void merge(int[] A, int p, int q, int r)
        {
            int n1 = q - p + 1; // number of elements in the left part A[p...q]
            int n2 = r - q; // number of elements in the right part A[q+1...r]
            int[] L = new int[n1 + 1]; // size +1 to hold a sentinel
            int[] R = new int[n2 + 1]; // size +1 to hold a sentinel
            int i, j;
            for (i = 0; i < n1; ++i)
            {
                L[i] = A[p + i];
            }
            for (j = 0; j < n2; ++j)
            {
                R[j] = A[q + 1 + j];
            }
            L[n1] = int.MaxValue; // sentinel
            R[n2] = int.MaxValue; // sentinel
            i = 0;
            j = 0;
            for (int k = p; k <= r; ++k)
            {
                if (i < n1 && L[i] <= R[j]) // i < n1: a real int.MaxValue in R must not pull the sentinel L[n1]
                {
                    A[k] = L[i];
                    i++;
                }
                else
                {
                    A[k] = R[j];
                    j++;
                }
            }
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, sorting the two halves in parallel
        /// and merging them sequentially (only when needed).
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <remarks>Subarrays with fewer than 128 elements are sorted sequentially.</remarks>
        public static void ParallelSort(int[] A)
        {
            ParallelMergeSort(A, 0, A.Length - 1);
        }

        /// <summary>
        /// Recursively sorts A[p...r], running both recursive calls in parallel
        /// as long as the subarray has at least <paramref name="thresholdClassicMerge"/> elements.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <param name="p">First index (inclusive).</param>
        /// <param name="r">Last index (inclusive).</param>
        /// <param name="thresholdClassicMerge">Below this number of elements, falls back to the sequential sort.</param>
        private static void ParallelMergeSort(int[] A, int p, int r, int thresholdClassicMerge = 128)
        {
            int delta = r - p;
            if (delta > 0)
            {
                int n = delta + 1; // number of elements to sort
                if (n < thresholdClassicMerge)
                {
                    // Small arrays: sequential processing
                    mergeSort(A, p, r);
                }
                else
                {
                    int q = p + delta / 2;
                    // Run both sorts in parallel
                    Parallel.Invoke(
                        () => ParallelMergeSort(A, p, q, thresholdClassicMerge),
                        () => ParallelMergeSort(A, q + 1, r, thresholdClassicMerge)
                    );
                    // Sequential merge (avoids memory conflicts)
                    if (A[q] > A[q + 1])
                        merge(A, p, q, r);
                }
            }
        }
    }
}
