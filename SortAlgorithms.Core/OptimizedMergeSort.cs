using System;
using System.Threading.Tasks;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Optimized merge sort: quartet sort (<see cref="ChunkSort.QuartetSort(int[], int, int)"/>) on small subarrays,
    /// a single auxiliary buffer allocated once, only the left half copied during a merge,
    /// and the merge skipped when the two halves are already in order. Includes a parallel variant.
    /// </summary>
    /// <remarks>
    /// <para>Time: Θ(n log n) in the worst case. Space: Θ(n) auxiliary (one buffer of n + 1 elements, allocated once per call).</para>
    /// <para>The buffer and cutoff are stored in instance fields, so an instance must not be used by several threads at the same time.</para>
    /// <para>The merge step uses <see cref="int.MaxValue"/> as a sentinel (as in CLRS); an explicit bound check on the left run
    /// keeps the merge correct even when the input itself contains <see cref="int.MaxValue"/>.</para>
    /// </remarks>
    public class OptimizedMergeSort
    {
        /// <summary>Auxiliary buffer that holds a copy of the left half during a merge.</summary>
        private int[] auxL;

        /// <summary>Cutoff on <c>r - p</c>: subarrays with fewer than <c>SizeCutOff + 1</c> elements are sorted by quartet sort.</summary>
        private int SizeCutOff = 45 - 1;

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <param name="MinSize">Subarrays with fewer elements than this are sorted by quartet sort.</param>
        public void Sort(int[] A, int MinSize = 45)
        {
            int n = A.Length;
            if (n <= 1) return;
            SizeCutOff = MinSize - 1;
            auxL = new int[n + 1];
            MergeSort(A, 0, n - 1);
        }

        /// <summary>
        /// Recursively sorts the subarray A[p...r] (inclusive bounds), merging only when needed.
        /// </summary>
        private void MergeSort(int[] A, int p, int r)
        {
            int delta = r - p; // delta + 1 = number of elements to sort
            if (delta <= 0) return;
            if (delta < SizeCutOff) // equivalent to (delta + 1 < MinSize)
                ChunkSort.QuartetSort(A, p, r);
            else
            {
                int q = p + delta / 2; // midpoint
                MergeSort(A, p, q); // sort the first half
                MergeSort(A, q + 1, r); // sort the second half
                if (A[q] > A[q + 1])
                    Merge(A, p, q, r); // merge
            }
        }

        /// <summary>
        /// Merges two adjacent sorted subarrays of A: A[p...q] and A[q+1...r].
        /// Only the left half is copied (into <see cref="auxL"/>); the right half is read in place.
        /// </summary>
        private void Merge(int[] A, int p, int q, int r)
        {
            // copy the left half
            Array.Copy(A, p, auxL, p, q - p + 1);

            auxL[q + 1] = int.MaxValue; // sentinel

            int i = p;
            int j = q + 1;
            int v = A[j];
            for (int k = p; k <= r; ++k)
            {
                if (i <= q && auxL[i] <= v) // i <= q: a real int.MaxValue in v must not pull the sentinel
                {
                    A[k] = auxL[i++];
                }
                else
                {
                    ++j;
                    A[k] = v;
                    if (j <= r)
                        v = A[j];
                    else
                    {
                        // <- right part is empty
                        while (++k <= r)
                        {
                            A[k] = auxL[i++];
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, sorting the two halves in parallel
        /// and merging them sequentially (only when needed).
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <remarks>
        /// Subarrays with fewer than 512 elements are sorted sequentially. The quartet sort cutoff is the one
        /// set by the last call to <see cref="Sort"/> (45 by default).
        /// </remarks>
        public void ParallelSort(int[] A)
        {
            int n = A.Length;
            if (n <= 1) return;
            auxL = new int[n + 1];
            ParallelMergeSort(A, 0, n - 1);
        }

        /// <summary>
        /// Recursively sorts A[p...r], running both recursive calls in parallel
        /// as long as the subarray has at least <paramref name="thresholdClassicMerge"/> elements.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <param name="p">First index (inclusive).</param>
        /// <param name="r">Last index (inclusive).</param>
        /// <param name="thresholdClassicMerge">Below this number of elements, falls back to the sequential sort.</param>
        private void ParallelMergeSort(int[] A, int p, int r, int thresholdClassicMerge = 512)
        {
            int delta = r - p;

            if (delta <= 0) return;

            int n = delta + 1; // number of elements to sort

            if (n < thresholdClassicMerge)
            {
                // Small arrays: sequential processing
                MergeSort(A, p, r);
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
                    Merge(A, p, q, r);
            }
        }
    }
}
