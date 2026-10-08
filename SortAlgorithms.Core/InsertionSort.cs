using System.Collections.Generic;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Insertion sort and several variants (binary search, <see cref="List{T}"/>, <see cref="LinkedList{T}"/>).
    /// </summary>
    /// <remarks>
    /// O(n²) comparisons and moves in the worst and average cases, O(n) on already sorted input.
    /// The in-place variants are stable and use O(1) extra space.
    /// </remarks>
    public static class InsertionSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with the textbook insertion sort.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            for (int i = 1, n = A.Length; i < n; ++i)
            {
                int selected = A[i];
                int j = i - 1;

                while ((j >= 0) && (A[j] > selected))
                {
                    A[j + 1] = A[j];
                    j--;
                }

                A[j + 1] = selected;
            }
        }

        /// <summary>
        /// Sorts the sub-array <c>A[istart...iend]</c> (inclusive bounds) in ascending order with insertion sort.
        /// Elements outside the range are left untouched.
        /// </summary>
        /// <param name="A">Array containing the range to sort.</param>
        /// <param name="istart">Index of the first element of the range.</param>
        /// <param name="iend">Index of the last element of the range (inclusive).</param>
        public static void SortRange(int[] A, int istart, int iend)
        {
            for (int i = istart + 1; i <= iend; ++i)
            {
                int selected = A[i];
                int j = i - 1;

                while ((j >= istart) && (A[j] > selected))
                {
                    A[j + 1] = A[j];
                    j--;
                }

                A[j + 1] = selected;
            }
        }

        /// <summary>
        /// Insertion sort that finds the insertion point with a binary search instead of a sequential search.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        /// <remarks>
        /// O(n log n) comparisons but still O(n²) moves. When an equal key is found, the element is inserted
        /// right after it; with several equal keys this is not necessarily after the last one, so the variant
        /// is not guaranteed to be stable.
        /// </remarks>
        public static void _Sort1(int[] A)
        {
            for (int i = 0, n = A.Length; i < n; ++i)
            {
                int selected = A[i];
                int j = i - 1;

                // find location where selected should be inserted
                int low = 0;
                int high = j;
                while (low <= high)
                {
                    int mid = low + (high - low) / 2;
                    int v = A[mid];
                    if (selected == v)
                    {
                        low = mid + 1;
                        break;
                    }
                    else if (selected > v)
                        low = mid + 1;
                    else
                        high = mid - 1;
                }

                // Move all elements after location to create space
                while (j >= low)
                {
                    A[j + 1] = A[j];
                    j--;
                }

                A[j + 1] = selected;
            }
        }

        /// <summary>
        /// Insertion sort that builds the result in a <see cref="List{T}"/> using <see cref="List{T}.Insert"/>.
        /// </summary>
        /// <param name="A">Array to sort; it is not modified.</param>
        /// <returns>A new sorted array.</returns>
        /// <remarks>O(n) extra space. The position is searched sequentially from the end of the list.</remarks>
        public static int[] _Sort2(int[] A)
        {
            int n = A.Length;
            List<int> sorted = new List<int>(n);
            for (int i = 0; i < n; ++i)
            {
                int selected = A[i];
                int j = i - 1;
                while ((j >= 0) && (sorted[j] > selected))
                {
                    j--;
                }
                sorted.Insert(j + 1, selected);
            }
            return sorted.ToArray();
        }

        /// <summary>
        /// Insertion sort that builds the result in a <see cref="LinkedList{T}"/> using
        /// <see cref="LinkedList{T}.AddBefore(LinkedListNode{T}, T)"/> and <see cref="LinkedList{T}.AddLast(T)"/>.
        /// </summary>
        /// <param name="A">Array to sort; it is not modified.</param>
        /// <returns>A new sorted array.</returns>
        /// <remarks>
        /// O(n) extra space. The position is searched sequentially from the head of the list, so already sorted
        /// input is the worst case (O(n²)) and descending input the best case.
        /// </remarks>
        public static int[] _Sort3(int[] A)
        {
            int n = A.Length;
            LinkedList<int> sorted = new LinkedList<int>();
            for (int i = 0; i < n; ++i)
            {
                int selected = A[i];
                LinkedListNode<int> current = sorted.First;

                // Find the right position
                while (current != null && current.Value < selected)
                {
                    current = current.Next;
                }

                // Insert before the position found (or at the end if current == null)
                if (current != null)
                {
                    sorted.AddBefore(current, selected);
                }
                else
                {
                    sorted.AddLast(selected);
                }
            }

            // Convert to an array
            int[] result = new int[n];
            sorted.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Insertion sort using the same optimization as <see cref="ShellSort.ImprovedSort"/> (case h = 1):
        /// the shift loop and the write-back are skipped when <c>A[i]</c> is already in place.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void _Sort4(int[] A)
        {
            int n = A.Length;
            for (int i = 1; i < n; ++i)
            {
                int j = i - 1;
                int v = A[i];
                if (A[j] > v)
                {
                    do
                    {
                        A[j + 1] = A[j];
                        --j;
                    } while ((j >= 0) && (A[j] > v));
                    A[j + 1] = v;
                }
            }
        }
    }
}
