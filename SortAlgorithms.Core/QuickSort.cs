namespace SortAlgorithms.Core
{
    /// <summary>
    /// Partition scheme used by <see cref="QuickSort.Sort"/>.
    /// </summary>
    public enum QuickSortMethod
    {
        /// <summary>
        /// Counts the elements less than or equal to the first element to find its final position,
        /// moves it there, then swaps the misplaced elements across it (<see cref="QuickSort.PartitionNaive"/>).
        /// </summary>
        Naive,

        /// <summary>
        /// Lomuto partition, using the last element as the pivot (<see cref="QuickSort.PartitionLomuto"/>).
        /// </summary>
        Lomuto,

        /// <summary>
        /// Hoare partition, using the first element as the pivot (<see cref="QuickSort.PartitionHoare"/>).
        /// </summary>
        Hoare,
    }

    /// <summary>
    /// Recursive quicksort with a choice of partition scheme (see <see cref="QuickSortMethod"/>).
    /// </summary>
    /// <remarks>
    /// <para>Time: Θ(n log n) on average, Θ(n²) in the worst case. In place, not stable.</para>
    /// <para>The pivot is always the first or last element, as in the textbook versions, so already sorted or reversed
    /// input hits the Θ(n²) worst case. The recursion is only made on the smaller part, so the stack depth stays
    /// O(log n) even then.</para>
    /// <para>The selected partition function is stored in instance fields, so an instance must not be used by several threads at the same time.</para>
    /// </remarks>
    public class QuickSort
    {
        /// <summary>
        /// Partitions A[left...right] and returns the split index.
        /// </summary>
        private delegate int Partition(int[] A, int left, int right);

        /// <summary>Partition function selected by <see cref="Sort"/>.</summary>
        private Partition partitionFunc;

        /// <summary>
        /// 1 if <see cref="partitionFunc"/> puts the pivot in its final position (Lomuto, Naive), 0 otherwise (Hoare).
        /// The left recursive call ends at <c>pivotIndex - delta</c>.
        /// </summary>
        private int delta;

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <param name="partitionMethod">The partition scheme to use.</param>
        public void Sort(int[] A, QuickSortMethod partitionMethod = QuickSortMethod.Hoare)
        {
            if (partitionMethod == QuickSortMethod.Hoare)
            {
                partitionFunc = PartitionHoare;
                delta = 0;
            }
            else if (partitionMethod == QuickSortMethod.Lomuto)
            {
                partitionFunc = PartitionLomuto;
                delta = 1;
            }
            else if (partitionMethod == QuickSortMethod.Naive)
            {
                partitionFunc = PartitionNaive;
                delta = 1;
            }
            quicksort(A, 0, A.Length - 1);
        }

        /// <summary>
        /// Sorts the subarray A[left...right] (inclusive bounds).
        /// </summary>
        /// <remarks>
        /// Recurses on the smaller part and loops on the larger one (tail-call elimination),
        /// so the recursion depth stays below log2(n) even when the partitions are unbalanced.
        /// </remarks>
        private void quicksort(int[] A, int left, int right)
        {
            while (left < right) // stop when the subarray length is 0 or 1
            {
                int pivotIndex = partitionFunc(A, left, right);

                // left part: A[left...pivotIndex - delta], right part: A[pivotIndex + 1...right]
                // delta = 1 if partitionFunc has placed the pivot in its final position, 0 otherwise
                int leftEnd = pivotIndex - delta;
                if (leftEnd - left < right - pivotIndex)
                {
                    quicksort(A, left, leftEnd); // recurse on the smaller left part
                    left = pivotIndex + 1;
                }
                else
                {
                    quicksort(A, pivotIndex + 1, right); // recurse on the smaller right part
                    right = leftEnd;
                }
            }
        }

        /// <summary>
        /// Hoare partition of A[left...right], using A[left] as the pivot.
        /// </summary>
        /// <param name="A">The array to partition.</param>
        /// <param name="left">First index (inclusive).</param>
        /// <param name="right">Last index (inclusive).</param>
        /// <returns>
        /// An index j such that every element of A[left...j] is less than or equal to every element of A[j+1...right].
        /// The pivot is not necessarily at index j, so both A[left...j] and A[j+1...right] must still be sorted.
        /// </returns>
        public static int PartitionHoare(int[] A, int left, int right)
        {
            int i = left - 1;
            int j = right + 1;

            var pivot = A[left]; // Choose the first element as the pivot (acts as a sentinel for both i and j)

            while (true)
            {
                // Move the left index to the right at least once and while the element at
                // the left index is less than the pivot
                do
                {
                    i++;
                } while (A[i] < pivot);

                // Move the right index to the left at least once and while the element at
                // the right index is greater than the pivot
                do
                {
                    j--;
                } while (A[j] > pivot);

                // If the indices crossed, return
                if (i >= j)
                    return j; // j = last index of the left partition

                // Swap the elements at the left and right indices
                int temp = A[i];
                A[i] = A[j];
                A[j] = temp;
            }
        }

        /// <summary>
        /// Lomuto partition of A[left...right], using A[right] as the pivot.
        /// </summary>
        /// <param name="A">The array to partition.</param>
        /// <param name="left">First index (inclusive).</param>
        /// <param name="right">Last index (inclusive).</param>
        /// <returns>The final index of the pivot: smaller or equal elements are on its left, greater elements on its right.</returns>
        public static int PartitionLomuto(int[] A, int left, int right)
        {
            var x = A[right]; // use A[right] as the pivot

            int i = left - 1;

            for (int j = left; j < right; ++j) // note: last index j = right - 1
            {
                if (A[j] <= x)
                {
                    i++;
                    int temp = A[i];
                    A[i] = A[j];
                    A[j] = temp;
                }
            }

            // <- i + 1 = index of pivot

            // swap the pivot to the boundary between the two subarrays
            A[right] = A[i + 1];
            A[i + 1] = x;

            return i + 1;
        }

        /// <summary>
        /// "Naive" partition of A[left...right], using A[left] as the pivot: counts the elements less than
        /// or equal to the pivot to compute its final position, moves it there, then swaps the misplaced
        /// elements from both sides.
        /// </summary>
        /// <param name="A">The array to partition.</param>
        /// <param name="left">First index (inclusive).</param>
        /// <param name="right">Last index (inclusive).</param>
        /// <returns>The final index of the pivot: smaller or equal elements are on its left, greater elements on its right.</returns>
        public static int PartitionNaive(int[] A, int left, int right)
        {
            var x = A[left]; // use A[left] as the pivot (acts as sentinel)

            int count = 0;
            for (int k = left + 1; k <= right; ++k)
            {
                if (A[k] <= x)
                    count++;
            }

            // <- count = (number of elements less than or equal to the pivot) - 1

            int pivotIndex = left + count; // final position of the pivot

            A[left] = A[pivotIndex];
            A[pivotIndex] = x;

            int i = left, j = right;
            while (i < pivotIndex && j > pivotIndex)
            {
                if (A[i] <= x)
                    i++;
                else if (A[j] > x)
                    j--;
                else
                {
                    int tmp = A[j];
                    A[j] = A[i];
                    A[i] = tmp;
                    i++;
                    j--;
                }
            }

            return pivotIndex;
        }
    }
}
