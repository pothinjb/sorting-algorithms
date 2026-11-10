using System;
using System.Numerics;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Pattern-defeating quicksort, or pdqsort (Peters, 2021): a quicksort that, like introsort, falls back to
    /// heap sort to guarantee O(n log n), and that also recognizes common patterns (already sorted input, many
    /// duplicates) to sort them in linear time.
    /// </summary>
    /// <remarks>
    /// <para>Mirrors the pseudo-code of the report (<c>PdqLoop</c>, <c>PartitionRight</c>, <c>PartitionLeft</c>),
    /// without the block partitioning (BlockQuicksort) of the reference implementation:</para>
    /// <list type="bullet">
    /// <item>sub-arrays of fewer than <see cref="InsertionSortThreshold"/> elements are sorted by insertion sort;</item>
    /// <item>the pivot is the median of three elements or, above <see cref="NintherThreshold"/> elements,
    /// Tukey's ninther (the median of the medians of three triples);</item>
    /// <item>when the partition needed no swap, both parts are finished by an insertion sort that gives up after
    /// <see cref="PartialInsertionSortLimit"/> moves;</item>
    /// <item>when the pivot equals the element just before the sub-array (a previous pivot, hence a lower bound),
    /// the elements equal to it are grouped at the front, where they are already in their final position;</item>
    /// <item>after an unbalanced partition (smaller part under 1/8 of the elements), a few elements are swapped to
    /// break the pattern; after floor(log2 n) of them on the same branch, the sub-array is sorted by heap sort.</item>
    /// </list>
    /// <para>Time: O(n log n) in the worst case, Θ(n) on already sorted input, O(nk) when the input holds k
    /// distinct values. In place (O(log n) stack), not stable.</para>
    /// </remarks>
    public static class PdqSort
    {
        /// <summary>Sub-arrays with fewer elements than this are sorted by insertion sort.</summary>
        public const int InsertionSortThreshold = 24;

        /// <summary>Above this size, the pivot is Tukey's ninther instead of the median of three.</summary>
        public const int NintherThreshold = 128;

        /// <summary>Maximum number of element moves allowed in <see cref="PartialInsertionSort"/>.</summary>
        public const int PartialInsertionSortLimit = 8;

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, tolerating floor(log2 n) unbalanced partitions on a
        /// branch before switching to heap sort.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            if (A.Length < 2) return;
            Sort(A, BitOperations.Log2((uint)A.Length));
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with an explicit budget of unbalanced partitions
        /// (exposed so that tests can force the heap sort fallback).
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <param name="bad">
        /// Number of unbalanced partitions tolerated on a branch; the sub-array is sorted by heap sort when the
        /// budget runs out. A budget of 0 or 1 switches to heap sort at the first unbalanced partition.
        /// </param>
        public static void Sort(int[] A, int bad)
        {
            if (A.Length < 2) return;
            PdqLoop(A, 0, A.Length - 1, bad, true);
        }

        /// <summary>
        /// Sorts <c>A[p...r]</c> (inclusive bounds): partitions and recurses on the left part, then loops on the
        /// right part.
        /// </summary>
        /// <param name="A">Array containing the sub-array.</param>
        /// <param name="p">Index of the first element of the sub-array.</param>
        /// <param name="r">Index of the last element of the sub-array (inclusive).</param>
        /// <param name="bad">Number of unbalanced partitions still tolerated on this branch.</param>
        /// <param name="leftmost">
        /// True if the sub-array starts the array. Otherwise <c>A[p-1]</c> is a previous pivot, not greater than
        /// any element of <c>A[p...r]</c>.
        /// </param>
        private static void PdqLoop(int[] A, int p, int r, int bad, bool leftmost)
        {
            while (true)
            {
                int size = r - p + 1;
                if (size < InsertionSortThreshold)
                {
                    InsertionSort.SortRange(A, p, r);
                    return;
                }

                ChoosePivot(A, p, r); // median moved to A[p]

                if (!leftmost && A[p - 1] == A[p]) // pivot = minimum of the sub-array
                {
                    p = PartitionLeft(A, p, r) + 1;
                    continue;
                }

                int q = PartitionRight(A, p, r, out bool sorted);
                if (Math.Min(q - p, r - q) < size / 8) // unbalanced partition
                {
                    bad--;
                    if (bad <= 0)
                    {
                        HeapSort.SortRange(A, p, r);
                        return;
                    }
                    BreakPatterns(A, p, q - 1);
                    BreakPatterns(A, q + 1, r);
                }
                else if (sorted && PartialInsertionSort(A, p, q - 1) && PartialInsertionSort(A, q + 1, r))
                {
                    return; // both parts are sorted
                }

                PdqLoop(A, p, q - 1, bad, leftmost); // left part
                p = q + 1;                           // loop on the right part
                leftmost = false;
            }
        }

        /// <summary>
        /// Moves the chosen pivot to <c>A[p]</c>: the median of <c>A[p]</c>, <c>A[m]</c> and <c>A[r]</c> (m being the
        /// middle index), or, above <see cref="NintherThreshold"/> elements, Tukey's ninther: the median of the
        /// medians of three triples spread over the sub-array.
        /// </summary>
        /// <param name="A">Array containing the sub-array.</param>
        /// <param name="p">Index of the first element of the sub-array.</param>
        /// <param name="r">Index of the last element of the sub-array (inclusive).</param>
        private static void ChoosePivot(int[] A, int p, int r)
        {
            int size = r - p + 1;
            int m = p + size / 2;
            int pivot;
            if (size > NintherThreshold)
            {
                int s = size / 8;
                int m1 = MedianOfThree(A, p, p + s, p + 2 * s);
                int m2 = MedianOfThree(A, m - s, m, m + s);
                int m3 = MedianOfThree(A, r - 2 * s, r - s, r);
                pivot = MedianOfThree(A, m1, m2, m3);
            }
            else
            {
                pivot = MedianOfThree(A, p, m, r);
            }
            Utils.Swap(A, p, pivot);
        }

        /// <summary>
        /// Returns the index (among <paramref name="i"/>, <paramref name="j"/> and <paramref name="k"/>) of the
        /// median of <c>A[i]</c>, <c>A[j]</c> and <c>A[k]</c>.
        /// </summary>
        /// <param name="A">Array holding the three values.</param>
        /// <param name="i">Index of the first value.</param>
        /// <param name="j">Index of the second value.</param>
        /// <param name="k">Index of the third value.</param>
        private static int MedianOfThree(int[] A, int i, int j, int k)
        {
            if (A[i] < A[j])
            {
                if (A[j] < A[k]) return j;      // A[i] < A[j] < A[k]
                return (A[i] < A[k]) ? k : i;   // A[j] is the maximum
            }
            if (A[i] < A[k]) return i;          // A[j] <= A[i] < A[k]
            return (A[j] < A[k]) ? k : j;       // A[i] is the maximum
        }

        /// <summary>
        /// Variant of Hoare's partition that puts the pivot <c>x = A[p]</c> in its final position and the elements
        /// equal to it on its right. The first two loops look for the first misplaced pair; if they cross without
        /// finding one, the sub-array was already partitioned. The <c>do...while</c> loops need no bound check: the
        /// element swapped on each side acts as a sentinel.
        /// </summary>
        /// <param name="A">Array containing the sub-array.</param>
        /// <param name="p">Index of the first element of the sub-array, which holds the pivot.</param>
        /// <param name="r">Index of the last element of the sub-array (inclusive).</param>
        /// <param name="sorted">True if no swap was needed (no misplaced element).</param>
        /// <returns>
        /// The final index q of the pivot: <c>A[p...q-1] &lt; x</c>, <c>A[q] = x</c>, <c>A[q+1...r] &gt;= x</c>.
        /// </returns>
        private static int PartitionRight(int[] A, int p, int r, out bool sorted)
        {
            int x = A[p];
            int i = p + 1;
            while (i <= r && A[i] < x)
            {
                i++;
            }
            int j = r;
            while (j > p && A[j] >= x)
            {
                j--;
            }
            sorted = i > j; // no misplaced element
            while (i < j)
            {
                Utils.Swap(A, i, j);
                do { i++; } while (A[i] < x);
                do { j--; } while (A[j] >= x);
            }
            Utils.Swap(A, p, j); // pivot in its final position
            return j;
        }

        /// <summary>
        /// Groups at the front of <c>A[p...r]</c> the elements equal to the pivot <c>x = A[p]</c>, with a
        /// Lomuto-style scan. Only called when no element is smaller than x, so the test <c>A[j] &lt;= x</c>
        /// amounts to <c>A[j] = x</c>.
        /// </summary>
        /// <param name="A">Array containing the sub-array.</param>
        /// <param name="p">Index of the first element of the sub-array, which holds the pivot.</param>
        /// <param name="r">Index of the last element of the sub-array (inclusive).</param>
        /// <returns>The index q such that <c>A[p...q] = x</c> and <c>A[q+1...r] &gt; x</c>.</returns>
        private static int PartitionLeft(int[] A, int p, int r)
        {
            int x = A[p];
            int i = p;
            for (int j = p + 1; j <= r; j++)
            {
                if (A[j] <= x)
                {
                    i++;
                    Utils.Swap(A, i, j);
                }
            }
            return i;
        }

        /// <summary>
        /// Insertion sort of <c>A[start...end]</c> (inclusive bounds) that gives up as soon as more than
        /// <see cref="PartialInsertionSortLimit"/> elements have been moved. When it gives up, the array is still a
        /// permutation of the input, and the sort goes on normally.
        /// </summary>
        /// <param name="A">Array containing the range to sort.</param>
        /// <param name="start">Index of the first element of the range.</param>
        /// <param name="end">Index of the last element of the range (inclusive).</param>
        /// <returns>True if the range is now sorted, false if the sort gave up.</returns>
        private static bool PartialInsertionSort(int[] A, int start, int end)
        {
            int moves = 0;
            for (int i = start + 1; i <= end; i++)
            {
                int selected = A[i];
                int j = i - 1;
                while (j >= start && A[j] > selected)
                {
                    A[j + 1] = A[j];
                    j--;
                }
                A[j + 1] = selected;
                moves += i - 1 - j;
                if (moves > PartialInsertionSortLimit) return false;
            }
            return true;
        }

        /// <summary>
        /// Swaps a few elements at fixed positions of <c>A[lo...hi]</c> (with those a quarter of the way from
        /// each end) to break the patterns that fooled the pivot choice and caused an unbalanced partition.
        /// </summary>
        /// <param name="A">Array containing the part.</param>
        /// <param name="lo">Index of the first element of the part.</param>
        /// <param name="hi">Index of the last element of the part (inclusive).</param>
        private static void BreakPatterns(int[] A, int lo, int hi)
        {
            int size = hi - lo + 1;
            if (size < InsertionSortThreshold) return;
            int quarter = size / 4;
            Utils.Swap(A, lo, lo + quarter);
            Utils.Swap(A, hi, hi - quarter);
            if (size > NintherThreshold)
            {
                Utils.Swap(A, lo + 1, lo + quarter + 1);
                Utils.Swap(A, lo + 2, lo + quarter + 2);
                Utils.Swap(A, hi - 1, hi - quarter - 1);
                Utils.Swap(A, hi - 2, hi - quarter - 2);
            }
        }
    }
}
