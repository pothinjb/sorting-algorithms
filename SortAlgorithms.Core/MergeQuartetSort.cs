using System.Threading.Tasks;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Hybrid merge sort that switches to <see cref="ChunkSort.QuartetSort(int[], int, int)"/> on small subarrays,
    /// with a parallel variant.
    /// </summary>
    /// <remarks>
    /// <para>Time: Θ(n log n) in the worst case. Space: Θ(n) auxiliary per merge.</para>
    /// <para>The thresholds are stored in instance fields, so an instance must not be used by several threads at the same time.</para>
    /// <para>The merge step uses <see cref="int.MaxValue"/> as a sentinel (as in CLRS); an explicit bound check on the left run
    /// keeps the merge correct even when the input itself contains <see cref="int.MaxValue"/>.</para>
    /// </remarks>
    public class MergeQuartetSort
    {
        /// <summary>Subarrays with fewer elements than this are sorted by <see cref="ChunkSort.QuartetSort(int[], int, int)"/>.</summary>
        private int thresholdInsertionSort = 16;

        /// <summary>Subarrays with fewer elements than this are sorted sequentially in <see cref="ParallelSort"/>.</summary>
        private int thresholdParallelization = 128;

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <param name="thInsertionSort">Subarrays with fewer elements than this are sorted by <see cref="ChunkSort.QuartetSort(int[], int, int)"/>.</param>
        public void Sort(int[] A, int thInsertionSort = 16)
        {
            int n = A.Length;
            if (n < thInsertionSort)
                ChunkSort.QuartetSort(A);
            else
            {
                thresholdInsertionSort = thInsertionSort;
                MergeSort(A, 0, n - 1);
            }
        }

        /// <summary>
        /// Recursively sorts the subarray A[p...r] (inclusive bounds),
        /// using quartet sort below <see cref="thresholdInsertionSort"/> elements.
        /// </summary>
        private void MergeSort(int[] A, int p, int r)
        {
            int delta = r - p;
            if (delta <= 0) return;
            int n = delta + 1; // number of elements to sort
            if (n < thresholdInsertionSort)
                ChunkSort.QuartetSort(A, p, r);
            else
            {
                int q = p + delta / 2; // midpoint
                MergeSort(A, p, q); // sort the first half
                MergeSort(A, q + 1, r); // sort the second half
                Merge(A, p, q, r); // merge
            }
        }

        /// <summary>
        /// Merges two adjacent sorted subarrays of A: A[p...q] and A[q+1...r].
        /// </summary>
        private static void Merge(int[] A, int p, int q, int r)
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
        /// and merging them sequentially.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <param name="thParallelization">Subarrays with fewer elements than this are sorted sequentially.</param>
        /// <param name="thInsertionSort">Subarrays with fewer elements than this are sorted by <see cref="ChunkSort.QuartetSort(int[], int, int)"/>.</param>
        public void ParallelSort(int[] A, int thParallelization = 128, int thInsertionSort = 16)
        {
            thresholdParallelization = thParallelization;
            thresholdInsertionSort = thInsertionSort;
            ParallelMergeSort(A, 0, A.Length - 1);
        }

        /// <summary>
        /// Recursively sorts A[p...r], running both recursive calls in parallel
        /// as long as the subarray has at least <see cref="thresholdParallelization"/> elements.
        /// </summary>
        private void ParallelMergeSort(int[] A, int p, int r)
        {
            int delta = r - p;
            if (delta <= 0) return;
            int n = delta + 1; // number of elements to sort
            if (n < thresholdParallelization)
            {
                // Small arrays: sequential processing
                MergeSort(A, p, r);
            }
            else
            {
                int q = p + delta / 2;
                // Run both sorts in parallel
                Parallel.Invoke(
                    () => ParallelMergeSort(A, p, q),
                    () => ParallelMergeSort(A, q + 1, r)
                    );
                // Sequential merge (avoids memory conflicts)
                Merge(A, p, q, r);
            }
        }
    }
}
