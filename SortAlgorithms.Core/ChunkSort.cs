using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SortAlgorithms.Core
{
    /// <summary>
    /// Insertion sort variants that insert elements by chunks (pairs, triplets, quartets) instead of one at a time.
    /// </summary>
    /// <remarks>
    /// <para>Each new chunk is first sorted locally, then its elements are inserted into the sorted prefix
    /// from the largest to the smallest: the scan for a smaller element resumes where the scan for the
    /// previous (larger) element stopped, so each chunk costs a single backward pass over the prefix.</para>
    /// <para>The first n mod c elements (c = chunk size) are sorted first, so that the remaining
    /// elements form whole chunks.</para>
    /// <para>Time: O(n²) in the worst case (descending input), O(n) for already sorted input.
    /// Space: O(1) extra. In place.</para>
    /// </remarks>
    public static class ChunkSort
    {
        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with an insertion sort that inserts two elements at a time.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        public static void PairSort(int[] A)
        {
            int n = A.Length;
            if (n == 0) return;
            int i0;
            if (n % 2 == 0)
            {
                int v = A[0];
                int w = A[1];
                if (v > w)
                {
                    A[0] = w;
                    A[1] = v;
                }
                i0 = 3;
            }
            else
                i0 = 2;
            for (int i = i0; i < n; i += 2)
            {
                int j = i - 2;
                int v = A[i - 1];
                int w = A[i];
                if (v > w)
                {
                    v = w;
                    w = A[i - 1];
                }
                // <- v <= w
                // <- j >= 0
                // insert w first
                while ((j >= 0) && (A[j] > w))
                {
                    A[j + 2] = A[j];
                    j--;
                }
                A[j + 2] = w;
                // continue by inserting v
                while ((j >= 0) && (A[j] > v))
                {
                    A[j + 1] = A[j];
                    j--;
                }
                A[j + 1] = v;
            }
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with an insertion sort that inserts three elements at a time.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        public static void TripletSort(int[] A)
        {
            int n = A.Length;
            if (n == 0) return;
            int i0;
            int r = n % 3;
            if (r == 1)
            {
                // <- A[0] is sorted
                i0 = 3;
            }
            else if (r == 2)
            {
                int v = A[1];
                int u = A[0];
                if (u > v)
                {
                    A[0] = v;
                    A[1] = u;
                }
                // : A[0] <= A[1]
                i0 = 4;
            }
            else // r == 0
            {
                int w = A[2];
                int v = A[1];
                int u = A[0];
                if (v > w)
                {
                    v = w;
                    w = A[1];
                }
                // <- v <= w
                if (u > v)
                {
                    int tmp = u;
                    u = v;
                    if (tmp > w)
                    {
                        v = w;
                        w = tmp;
                    }
                    else
                        v = tmp;
                }
                A[0] = u;
                A[1] = v;
                A[2] = w;
                // : A[0] <= A[1] <= A[2]
                i0 = 5;
            }

            for (int i = i0; i < n; i += 3)
            {
                int w = A[i];
                int v = A[i - 1];
                int u = A[i - 2];
                int j = i - 3;
                if (v > w)
                {
                    v = w;
                    w = A[i - 1];
                }
                // <- v <= w
                if (u > v)
                {
                    int tmp = u;
                    u = v;
                    if (tmp > w)
                    {
                        v = w;
                        w = tmp;
                    }
                    else
                        v = tmp;
                }
                // <- u <= v <= w
                // <- j >= 0
                // insert w first
                while ((j >= 0) && (A[j] > w))
                {
                    A[j + 3] = A[j];
                    j--;
                }
                A[j + 3] = w;
                // continue by inserting v
                while ((j >= 0) && (A[j] > v))
                {
                    A[j + 2] = A[j];
                    j--;
                }
                A[j + 2] = v;
                // finish with u
                while ((j >= 0) && (A[j] > u))
                {
                    A[j + 1] = A[j];
                    j--;
                }
                A[j + 1] = u;
            }
        }

        /// <summary>
        /// Sorts <paramref name="A"/> in ascending order with an insertion sort that inserts four elements at a time.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <remarks>
        /// Each quartet is sorted locally by sorting its two pairs, then merging them
        /// (with a shortcut for already ordered and fully reversed pairs).
        /// </remarks>
        public static void QuartetSort(int[] A)
        {
            int n = A.Length;
            if (n == 0) return;
            int i0;
            int r = n % 4;
            if (r == 1)
            {
                // <- A[0] is sorted
                i0 = 4;
            }
            else if (r == 2)
            {
                int v = A[1];
                int u = A[0];
                if (u > v)
                {
                    A[0] = v;
                    A[1] = u;
                }
                // : A[0] <= A[1]
                i0 = 5;
            }
            else if (r == 3)
            {
                int w = A[2];
                int v = A[1];
                int u = A[0];
                if (v > w)
                {
                    v = w;
                    w = A[1];
                }
                // <- v <= w
                if (u > v)
                {
                    int tmp = u;
                    u = v;
                    if (tmp > w)
                    {
                        v = w;
                        w = tmp;
                    }
                    else
                        v = tmp;
                }
                A[0] = u;
                A[1] = v;
                A[2] = w;
                // : A[0] <= A[1] <= A[2]
                i0 = 6;
            }
            else // r == 0
            {
                int u = A[0];
                int v = A[1];
                int w = A[2];
                int x = A[3];

                // Local sort of 4 elements (conditional comparisons)
                if (u > v) { int tmp = u; u = v; v = tmp; }
                if (w > x) { int tmp = w; w = x; x = tmp; }
                if (u > w) { int tmp = u; u = w; w = tmp; }
                if (v > x) { int tmp = v; v = x; x = tmp; }
                if (v > w) { int tmp = v; v = w; w = tmp; }

                // Write back the 4 sorted values
                A[0] = u;
                A[1] = v;
                A[2] = w;
                A[3] = x;

                i0 = 7;
            }

            int value;

            for (int i = i0; i < n; i += 4)
            {
                int j = i - 4;

                int u = A[i - 3];
                int v = A[i - 2];
                int w = A[i - 1];
                int x = A[i];

                // Local sort of 4 elements (conditional comparisons)

                // Step 1: sort the two pairs
                if (u > v) { int tmp = u; u = v; v = tmp; }
                if (w > x) { int tmp = w; w = x; x = tmp; }

                // Step 2: adaptive merge
                if (v > w)
                {
                    if (u > x)
                    {
                        // Fully reversed block: [w, x, u, v]
                        int tmpU = u, tmpV = v;
                        u = w; v = x; w = tmpU; x = tmpV;
                    }
                    else
                    {
                        // Intermediate cases: merge
                        int a, b, c, d;

                        if (u <= w)
                        {
                            a = u;
                            if (v <= w)
                            {
                                b = v;
                                c = w;
                                d = x;
                            }
                            else if (v <= x)
                            {
                                b = w;
                                c = v;
                                d = x;
                            }
                            else
                            {
                                b = w;
                                c = x;
                                d = v;
                            }
                        }
                        else
                        {
                            a = w;
                            if (x < u)
                            {
                                b = x;
                                c = u;
                                d = v;
                            }
                            else if (x < v)
                            {
                                b = u;
                                c = x;
                                d = v;
                            }
                            else
                            {
                                b = u;
                                c = v;
                                d = x;
                            }
                        }

                        u = a; v = b; w = c; x = d;
                    }
                }
                // else:
                // The two pairs are already in the right order
                // (u ≤ v ≤ w ≤ x)

                // <- u <= v <= w <= x
                // <- j >= 0

                // insert the largest element (x) first
                while ((j >= 0) && ((value = A[j]) > x))
                {
                    A[j-- + 4] = value;
                }
                A[j + 4] = x;

                // continue with w
                while ((j >= 0) && ((value = A[j]) > w))
                {
                    A[j-- + 3] = value;
                }
                A[j + 3] = w;

                // continue with v
                while ((j >= 0) && ((value = A[j]) > v))
                {
                    A[j-- + 2] = value;
                }
                A[j + 2] = v;

                // finish with the smallest element (u)
                while ((j >= 0) && ((value = A[j]) > u))
                {
                    A[j-- + 1] = value;
                }
                A[j + 1] = u;
            }
        }

        /// <summary>
        /// Sorts the subarray A[imin...imax] in ascending order with an insertion sort that inserts
        /// four elements at a time. Used as the base case of hybrid merge sorts.
        /// </summary>
        /// <param name="A">The array that contains the subarray to sort.</param>
        /// <param name="imin">Index of the first element of the subarray (inclusive).</param>
        /// <param name="imax">Index of the last element of the subarray (inclusive).</param>
        /// <remarks>Does nothing if <paramref name="imax"/> &lt; <paramref name="imin"/>.</remarks>
        public static void QuartetSort(int[] A, int imin, int imax)
        {
            int n = imax - imin + 1;
            if (n <= 0) return;
            int i0;
            int r = n % 4;
            if (r == 1)
            {
                // <- A[imin] is sorted
                i0 = 4;
            }
            else if (r == 2)
            {
                int v = A[imin + 1];
                int u = A[imin];
                if (u > v)
                {
                    A[imin] = v;
                    A[imin + 1] = u;
                }
                // : A[imin] <= A[imin+1]
                i0 = 5;
            }
            else if (r == 3)
            {
                int w = A[imin + 2];
                int v = A[imin + 1];
                int u = A[imin];
                if (v > w)
                {
                    v = w;
                    w = A[imin + 1];
                }
                // <- v <= w
                if (u > v)
                {
                    int tmp = u;
                    u = v;
                    if (tmp > w)
                    {
                        v = w;
                        w = tmp;
                    }
                    else
                        v = tmp;
                }
                A[imin] = u;
                A[imin + 1] = v;
                A[imin + 2] = w;
                // : A[imin] <= A[imin + 1] <= A[imin + 2]
                i0 = 6;
            }
            else // r == 0
            {
                int u = A[imin];
                int v = A[imin + 1];
                int w = A[imin + 2];
                int x = A[imin + 3];

                // Local sort of 4 elements (conditional comparisons)
                if (u > v) { int tmp = u; u = v; v = tmp; }
                if (w > x) { int tmp = w; w = x; x = tmp; }
                if (u > w) { int tmp = u; u = w; w = tmp; }
                if (v > x) { int tmp = v; v = x; x = tmp; }
                if (v > w) { int tmp = v; v = w; w = tmp; }

                // Write back the 4 sorted values
                A[imin] = u;
                A[imin + 1] = v;
                A[imin + 2] = w;
                A[imin + 3] = x;

                i0 = 7;
            }

            int value;

            for (int i = imin + i0; i <= imax; i += 4)
            {
                int j = i - 4;

                int u = A[i - 3];
                int v = A[i - 2];
                int w = A[i - 1];
                int x = A[i];

                // Local sort of 4 elements (conditional comparisons)

                // Step 1: sort the two pairs
                if (u > v) { int tmp = u; u = v; v = tmp; }
                if (w > x) { int tmp = w; w = x; x = tmp; }

                // Step 2: adaptive merge
                if (v > w)
                {
                    if (u > x)
                    {
                        // Fully reversed block: [w, x, u, v]
                        int tmpU = u, tmpV = v;
                        u = w; v = x; w = tmpU; x = tmpV;
                    }
                    else
                    {
                        // Intermediate cases: merge
                        int a, b, c, d;

                        if (u <= w)
                        {
                            a = u;
                            if (v <= w)
                            {
                                b = v;
                                c = w;
                                d = x;
                            }
                            else if (v <= x)
                            {
                                b = w;
                                c = v;
                                d = x;
                            }
                            else
                            {
                                b = w;
                                c = x;
                                d = v;
                            }
                        }
                        else
                        {
                            a = w;
                            if (x < u)
                            {
                                b = x;
                                c = u;
                                d = v;
                            }
                            else if (x < v)
                            {
                                b = u;
                                c = x;
                                d = v;
                            }
                            else
                            {
                                b = u;
                                c = v;
                                d = x;
                            }
                        }

                        u = a; v = b; w = c; x = d;
                    }
                } // otherwise the two pairs are already in the right order

                // <- u <= v <= w <= x
                // <- j >= imin

                // insert the largest element (x) first
                while ((j >= imin) && ((value = A[j]) > x))
                {
                    A[j-- + 4] = value;
                }
                A[j + 4] = x;

                // continue with w
                while ((j >= imin) && ((value = A[j]) > w))
                {
                    A[j-- + 3] = value;
                }
                A[j + 3] = w;

                // continue with v
                while ((j >= imin) && ((value = A[j]) > v))
                {
                    A[j-- + 2] = value;
                }
                A[j + 2] = v;

                // finish with the smallest element (u)
                while ((j >= imin) && ((value = A[j]) > u))
                {
                    A[j-- + 1] = value;
                }
                A[j + 1] = u;
            }
        }

        /// <summary>
        /// Parallel variant of <see cref="TripletSort"/>: all triplets are first sorted locally in parallel,
        /// then they are inserted sequentially into the sorted prefix.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <remarks>
        /// Only the local sort of the triplets runs in parallel (in ranges of 90000 elements, a multiple of 3
        /// so that no triplet straddles two ranges). The insertion phase is sequential.
        /// </remarks>
        public static void ParallelTripletSort(int[] A)
        {
            int n = A.Length;
            if (n == 0) return;
            int i0;
            int r = n % 3;
            if (r == 1)
            {
                // <- A[0] is sorted
                i0 = 3;
            }
            else if (r == 2)
            {
                int v = A[1];
                int u = A[0];
                if (u > v)
                {
                    A[0] = v;
                    A[1] = u;
                }
                // : A[0] <= A[1]
                i0 = 4;
            }
            else // r == 0
            {
                int w = A[2];
                int v = A[1];
                int u = A[0];
                if (v > w)
                {
                    v = w;
                    w = A[1];
                }
                // <- v <= w
                if (u > v)
                {
                    int tmp = u;
                    u = v;
                    if (tmp > w)
                    {
                        v = w;
                        w = tmp;
                    }
                    else
                        v = tmp;
                }
                A[0] = u;
                A[1] = v;
                A[2] = w;
                // : A[0] <= A[1] <= A[2]
                i0 = 5;
            }

            if (i0 >= n) return;

            int chunkSize = 90000; // tune for performance (must be a multiple of the group size)

            Parallel.ForEach(
                Partitioner.Create(i0, n, chunkSize),
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i += 3)
                    {
                        int w = A[i];
                        int v = A[i - 1];
                        int u = A[i - 2];
                        if (v > w)
                        {
                            v = w;
                            w = A[i - 1];
                        }
                        if (u > v)
                        {
                            int tmp = u;
                            A[i - 2] = v;
                            if (tmp > w)
                            {
                                v = w;
                                w = tmp;
                            }
                            else
                                v = tmp;
                        }
                        // else A[i-2] = u (<=v)
                        A[i - 1] = v;
                        A[i] = w;
                        // <- A[i-2] <= A[i-1] <= A[i]
                    }
                }
            );

            for (int i = i0; i < n; i += 3)
            {
                int j = i - 3;
                int w = A[i];
                int v = A[i - 1];
                int u = A[i - 2];
                // <- u <= v <= w
                // insert w first
                if (A[j] > w)
                {
                    do
                    {
                        A[j + 3] = A[j];
                        j--;
                    } while ((j >= 0) && (A[j] > w));
                }
                A[j + 3] = w;
                // continue by inserting v
                while ((j >= 0) && (A[j] > v))
                {
                    A[j + 2] = A[j];
                    j--;
                }
                A[j + 2] = v;
                // finish with u
                while ((j >= 0) && (A[j] > u))
                {
                    A[j + 1] = A[j];
                    j--;
                }
                A[j + 1] = u;
            }
        }

        /// <summary>
        /// Parallel variant of <see cref="QuartetSort(int[])"/>: all quartets are first sorted locally in parallel,
        /// then they are inserted sequentially into the sorted prefix.
        /// </summary>
        /// <param name="A">The array to sort.</param>
        /// <remarks>
        /// Only the local sort of the quartets runs in parallel (in ranges of 10000 elements, a multiple of 4
        /// so that no quartet straddles two ranges). The insertion phase is sequential.
        /// </remarks>
        public static void ParallelQuartetSort(int[] A)
        {
            int n = A.Length;
            if (n == 0) return;
            int i0;
            int r = n % 4;
            if (r == 1)
            {
                // <- A[0] is sorted
                i0 = 4;
            }
            else if (r == 2)
            {
                int v = A[1];
                int u = A[0];
                if (u > v)
                {
                    A[0] = v;
                    A[1] = u;
                }
                // : A[0] <= A[1]
                i0 = 5;
            }
            else if (r == 3)
            {
                int w = A[2];
                int v = A[1];
                int u = A[0];
                if (v > w)
                {
                    v = w;
                    w = A[1];
                }
                // <- v <= w
                if (u > v)
                {
                    int tmp = u;
                    u = v;
                    if (tmp > w)
                    {
                        v = w;
                        w = tmp;
                    }
                    else
                        v = tmp;
                }
                A[0] = u;
                A[1] = v;
                A[2] = w;
                // : A[0] <= A[1] <= A[2]
                i0 = 6;
            }
            else // r == 0
            {
                int u = A[0];
                int v = A[1];
                int w = A[2];
                int x = A[3];

                // Local sort of 4 elements (conditional comparisons)
                if (u > v) { int tmp = u; u = v; v = tmp; }
                if (w > x) { int tmp = w; w = x; x = tmp; }
                if (u > w) { int tmp = u; u = w; w = tmp; }
                if (v > x) { int tmp = v; v = x; x = tmp; }
                if (v > w) { int tmp = v; v = w; w = tmp; }

                // Write back the 4 sorted values
                A[0] = u;
                A[1] = v;
                A[2] = w;
                A[3] = x;

                i0 = 7;
            }

            if (i0 >= n) return;

            int chunkSize = 10000; // tune for performance (must be a multiple of the group size)

            Parallel.ForEach(
                Partitioner.Create(i0, n, chunkSize),
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i += 4)
                    {
                        int u = A[i - 3];
                        int v = A[i - 2];
                        int w = A[i - 1];
                        int x = A[i];

                        // Step 1: sort the two pairs
                        if (u > v) { int tmp = u; u = v; v = tmp; }
                        if (w > x) { int tmp = w; w = x; x = tmp; }

                        // Step 2: adaptive merge
                        if (v <= w)
                        {
                            // The two pairs are already in the right order
                            // (u ≤ v ≤ w ≤ x)
                        }
                        else if (u > x)
                        {
                            // Fully reversed block: [w, x, u, v]
                            int tmpU = u, tmpV = v;
                            u = w; v = x; w = tmpU; x = tmpV;
                        }
                        else
                        {
                            // Intermediate cases: merge
                            int a, b, c, d;

                            if (u <= w)
                            {
                                a = u;
                                if (v <= w)
                                {
                                    b = v;
                                    c = w;
                                    d = x;
                                }
                                else if (v <= x)
                                {
                                    b = w;
                                    c = v;
                                    d = x;
                                }
                                else
                                {
                                    b = w;
                                    c = x;
                                    d = v;
                                }
                            }
                            else
                            {
                                a = w;
                                if (x < u)
                                {
                                    b = x;
                                    c = u;
                                    d = v;
                                }
                                else if (x < v)
                                {
                                    b = u;
                                    c = x;
                                    d = v;
                                }
                                else
                                {
                                    b = u;
                                    c = v;
                                    d = x;
                                }
                            }

                            u = a; v = b; w = c; x = d;
                        }

                        A[i - 3] = u;
                        A[i - 2] = v;
                        A[i - 1] = w;
                        A[i] = x;

                        // <- A[i-3] <= A[i-2] <= A[i-1] <= A[i]
                    }
                }
            );

            for (int i = i0; i < n; i += 4)
            {
                int j = i - 4;

                int u = A[i - 3];
                int v = A[i - 2];
                int w = A[i - 1];
                int x = A[i];

                // <- u <= v <= w <= x
                // <- j >= 0

                // insert the largest element (x) first
                while ((j >= 0) && (A[j] > x))
                {
                    A[j + 4] = A[j];
                    j--;
                }
                A[j + 4] = x;

                // continue with w
                while ((j >= 0) && (A[j] > w))
                {
                    A[j + 3] = A[j];
                    j--;
                }
                A[j + 3] = w;

                // continue with v
                while ((j >= 0) && (A[j] > v))
                {
                    A[j + 2] = A[j];
                    j--;
                }
                A[j + 2] = v;

                // finish with the smallest element (u)
                while ((j >= 0) && (A[j] > u))
                {
                    A[j + 1] = A[j];
                    j--;
                }
                A[j + 1] = u;
            }
        }
    }
}
