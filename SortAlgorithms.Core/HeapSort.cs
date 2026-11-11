namespace SortAlgorithms.Core
{
    /// <summary>
    /// Heap sort: builds a max-heap in the array, then repeatedly moves the root (the maximum)
    /// to the end of the array and restores the heap on the remaining elements.
    /// </summary>
    /// <remarks>O(n log n) in all cases, in place (O(1) extra space), not stable.</remarks>
    public static class HeapSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, using a max-heapify that moves the root value
        /// down by shifting children up instead of swapping.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void Sort(int[] A)
        {
            MaxHeapBuilding(A);
            for (int i = A.Length - 1; i > 0; --i)
            {
                int tmp = A[i];
                A[i] = A[0];
                A[0] = tmp;
                MaxHeapify(A, 0, i - 1);
            }
        }

        /// <summary>
        /// Turns <paramref name="A"/> into a max-heap (bottom-up construction, O(n)).
        /// </summary>
        /// <param name="A">Array to rearrange.</param>
        private static void MaxHeapBuilding(int[] A)
        {
            int n = A.Length;
            int last = n - 1;
            for (int i = (n - 2) / 2; i >= 0; --i)
            {
                MaxHeapify(A, i, last);
            }
        }

        /// <summary>
        /// Sifts the value at index <paramref name="i"/> down the heap <c>A[0...last]</c> until the max-heap
        /// property holds. Larger children are moved up and the value is written once, at its final position.
        /// </summary>
        /// <param name="A">Array holding the heap.</param>
        /// <param name="i">Index of the node to sift down.</param>
        /// <param name="last">Index of the last element of the heap (inclusive).</param>
        private static void MaxHeapify(int[] A, int i, int last)
        {
            int val = A[i];
            while (true)
            {
                int left = (i << 1) + 1;
                if (left > last) break;

                int right = left + 1;

                int largest = left;

                if (right <= last && A[right] > A[left])
                    largest = right;

                if (A[largest] <= val) break;

                A[i] = A[largest];
                i = largest;
            }
            A[i] = val;
        }

        /// <summary>
        /// Sorts the sub-array <c>A[p...r]</c> (inclusive bounds) in ascending order with heap sort. Elements
        /// outside the range are left untouched. Same algorithm as <see cref="Sort"/>, with every heap index
        /// shifted by <paramref name="p"/>: node <c>k</c> of the heap is stored in <c>A[p + k]</c>.
        /// Used as the fallback of <see cref="IntroSort"/> and <see cref="PdqSort"/>.
        /// </summary>
        /// <param name="A">Array containing the range to sort.</param>
        /// <param name="p">Index of the first element of the range.</param>
        /// <param name="r">Index of the last element of the range (inclusive).</param>
        public static void SortRange(int[] A, int p, int r)
        {
            // MaxHeapBuilding on the sub-array
            int n = r - p + 1;
            int last = n - 1;
            for (int i = (n - 2) / 2; i >= 0; --i)
            {
                MaxHeapify(A, p, i, last);
            }

            for (int i = last; i > 0; --i)
            {
                int tmp = A[p + i];
                A[p + i] = A[p];
                A[p] = tmp;
                MaxHeapify(A, p, 0, i - 1);
            }
        }

        /// <summary>
        /// Same as <see cref="MaxHeapify(int[], int, int)"/> for the heap stored in <c>A[p...p+last]</c>,
        /// node <c>k</c> being at index <c>p + k</c>.
        /// </summary>
        /// <param name="A">Array holding the heap.</param>
        /// <param name="p">Index of the root of the heap in <paramref name="A"/>.</param>
        /// <param name="i">Heap index of the node to sift down (stored in <c>A[p + i]</c>).</param>
        /// <param name="last">Heap index of the last element of the heap (inclusive).</param>
        private static void MaxHeapify(int[] A, int p, int i, int last)
        {
            int val = A[p + i];
            while (true)
            {
                int left = (i << 1) + 1;
                if (left > last) break;

                int right = left + 1;

                int largest = left;

                if (right <= last && A[p + right] > A[p + left])
                    largest = right;

                if (A[p + largest] <= val) break;

                A[p + i] = A[p + largest];
                i = largest;
            }
            A[p + i] = val;
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order, using the textbook (swap-based) max-heapify.
        /// </summary>
        /// <param name="A">Array to sort in place.</param>
        public static void SortUsingNaiveMaxHeapify(int[] A)
        {
            // MaxHeapBuilding inline
            int n = A.Length;
            int last = n - 1;
            for (int i = (n - 2) / 2; i >= 0; --i)
            {
                NaiveMaxHeapify(A, i, last);
            }

            for (int i = n - 1; i > 0; --i)
            {
                int tmp = A[i];
                A[i] = A[0];
                A[0] = tmp;
                NaiveMaxHeapify(A, 0, i - 1);
            }
        }

        /// <summary>
        /// Textbook max-heapify: swaps node <paramref name="i"/> with its largest child until the max-heap
        /// property holds in <c>A[0...last]</c>.
        /// </summary>
        /// <param name="A">Array holding the heap.</param>
        /// <param name="i">Index of the node to sift down.</param>
        /// <param name="last">Index of the last element of the heap (inclusive).</param>
        private static void NaiveMaxHeapify(int[] A, int i, int last)
        {
            while (true)
            {
                int left = 2 * i + 1; // index of the left child of node i
                int right = 2 * i + 2; // index of the right child

                int imax = i;

                if ((left <= last) && (A[left] > A[i]))
                    imax = left; // the left child is larger than the current node
                if ((right <= last) && (A[right] > A[imax]))
                    imax = right; // the right child is the largest

                if (imax == i)
                    return; // stop: node i satisfies the max-heap property

                int tmp = A[i];
                A[i] = A[imax];
                A[imax] = tmp;

                i = imax;
            }
        }
    }
}
