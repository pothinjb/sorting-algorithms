namespace SortAlgorithms.Core
{
    /// <summary>
    /// Library sort (gapped insertion sort, Bender, Farach-Colton &amp; Mosteiro, 2004): an insertion sort
    /// that leaves gaps in an auxiliary array so that most insertions need few shifts, and spreads the
    /// elements out again (rebalancing) each time the gaps are used up.
    /// </summary>
    /// <remarks>
    /// O(n log n) time with high probability on randomly ordered input, O(n²) worst case (this
    /// implementation does not shuffle the input first). Uses an auxiliary array of
    /// <c>(1 + epsilon) * n</c> nullable cells. The result is copied back into the input array.
    /// </remarks>
    public static partial class LibrarySort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order.
        /// </summary>
        /// <param name="A">Array to sort in place. <c>null</c> or arrays of length ≤ 1 are left unchanged.</param>
        /// <param name="epsilon">
        /// Gap factor: the auxiliary array has <c>(1 + epsilon) * n</c> cells. Must be strictly positive.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="epsilon"/> is less than 1.</exception>
        public static void Sort(int[] A, int epsilon)
        {
            // with epsilon <= 0 there are no gaps, so no rebalancing would ever take place
            if (epsilon < 1)
                throw new System.ArgumentOutOfRangeException(nameof(epsilon), epsilon, "epsilon must be strictly positive.");
            if (A == null || A.Length <= 1) return;

            int n = A.Length;

            int h = 1 + epsilon;
            int sLen = h * n; // extended size of the auxiliary array

            int?[] S = new int?[sLen];

            int span = h; // size of the working area
            S[h >> 1] = A[0]; // insert the first element (base case)
            int remaining_holes = span - 1;

            for (int i = 1; i < n; ++i)
            {
                int x = A[i];
                int insPos = BinsearchGapAware(x, S, span);
                InsertGapAware(x, S, span, insPos);
                remaining_holes--;
                if (remaining_holes == 0)
                {
                    int prevSpan = span;
                    span = h * (i + 1);
                    Rebalance(S, prevSpan, span);
                    remaining_holes = span - (i + 1);
                }
            }

            // compaction
            for (int iread = 0, iwrite = 0; iwrite < n; iread++)
            {
                if (S[iread].HasValue)
                    A[iwrite++] = S[iread].Value;
            }
        }

        /// <summary>
        /// Binary search for the insertion position of <paramref name="x"/> in <c>S[0...span-1]</c>,
        /// skipping empty cells (<c>null</c>).
        /// </summary>
        /// <param name="x">Value to insert.</param>
        /// <param name="S">Gapped auxiliary array.</param>
        /// <param name="span">Size of the working area of <paramref name="S"/>.</param>
        /// <returns>The index where <paramref name="x"/> should be inserted (after any equal values).</returns>
        private static int BinsearchGapAware(int x, int?[] S, int span)
        {
            int first = 0;
            int last = span - 1;
            while (first < last && S[first] == null)
            {
                first++;
            }
            if (S[first] != null && S[first] > x)
                return first >> 1;
            while (last > first && S[last] == null)
            {
                last--;
            }
            if (S[last] != null && S[last] <= x)
                return span - 1;
            while (first <= last)
            {
                int middle = first + (last - first) / 2;
                int i = middle;
                while (i >= first && S[i] == null)
                    i--;
                if (i < first)
                {
                    i = middle + 1;
                    while (i <= last && S[i] == null)
                        i++;
                    if (i > last)
                        return first;
                }
                if (x < S[i])
                    last = i - 1;
                else
                    first = i + 1;
            }

            return first;
        }

        /// <summary>
        /// Inserts <paramref name="x"/> at <paramref name="pos"/>. If the cell is occupied, the elements are
        /// shifted towards the nearest empty cell (right or left) to make room.
        /// </summary>
        /// <param name="x">Value to insert.</param>
        /// <param name="S">Gapped auxiliary array.</param>
        /// <param name="span">Size of the working area of <paramref name="S"/>.</param>
        /// <param name="pos">Insertion position returned by <see cref="BinsearchGapAware"/>.</param>
        private static void InsertGapAware(int x, int?[] S, int span, int pos)
        {
            if (S[pos] == null)
                S[pos] = x; // direct insertion into a free cell, no shift needed
            else
            {
                // look for the first free cell on the right
                int right = pos + 1;
                while (right < span && S[right] != null)
                    right++;

                // look for the first free cell on the left
                int left = pos - 1;
                while (left >= 0 && S[left] != null)
                    left--;

                // shift elements to make room for the new one
                if (right < span && ((right - pos <= pos - left) || (left < 0)))
                {
                    // shift right
                    for (int i = right; i > pos; --i)
                        S[i] = S[i - 1];
                }
                else
                {
                    // shift left
                    if (S[pos] > x)
                        pos--;
                    for (int i = left; i < pos; ++i)
                        S[i] = S[i + 1];
                }
                S[pos] = x;
            }
        }

        /// <summary>
        /// Spreads the elements of <c>S[0...previousLen-1]</c> evenly over <c>S[0...expandedLen-1]</c>,
        /// starting from the end, so that gaps appear between them again.
        /// </summary>
        /// <param name="S">Gapped auxiliary array.</param>
        /// <param name="previousLen">Size of the working area before rebalancing.</param>
        /// <param name="expandedLen">Size of the working area after rebalancing.</param>
        private static void Rebalance(int?[] S, int previousLen, int expandedLen)
        {
            int w = expandedLen - 1; // write index (new end, after spreading)
            int step = expandedLen / previousLen;
            int r = previousLen - 1;
            while (r >= 0)
            {
                S[w] = S[r]; // copy the element to its new position
                S[r] = null;
                w -= step;
                r--;
            }
        }
    }
}
