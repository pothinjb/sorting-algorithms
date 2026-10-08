namespace SortAlgorithms.Core
{
    /// <summary>
    /// Helpers that return the index of the minimum and/or maximum of an array or a sub-array.
    /// <c>MinMax_1</c> to <c>MinMax_3</c> are earlier variants of <see cref="MinMax"/>, kept for comparison.
    /// </summary>
    internal class IndexOf
    {
        /// <summary>
        /// Returns the index of the first occurrence of the minimum of <paramref name="x"/>.
        /// </summary>
        /// <param name="x">Array to scan.</param>
        /// <returns>Index of the minimum, or -1 if <paramref name="x"/> is empty.</returns>
        public static int Min(int[] x)
        {
            int n = x.Length;
            if (n == 0) return -1;
            int imin = 0;
            for (int i = 1; i < n; ++i)
            {
                if (x[i] < x[imin])
                {
                    imin = i;
                }
            }
            return imin;
        }

        /// <summary>
        /// Returns the indices of the minimum and maximum of <c>x[ileft...iright]</c> (inclusive bounds),
        /// reading the current extremes from the array at each comparison and skipping the minimum test when
        /// a new maximum is found.
        /// </summary>
        /// <param name="x">Array to scan.</param>
        /// <param name="ileft">Index of the first element of the range.</param>
        /// <param name="iright">Index of the last element of the range (inclusive).</param>
        /// <returns>The pair <c>(imin, imax)</c>.</returns>
        public static (int, int) MinMax_1(int[] x, int ileft, int iright)
        {
            int imin = ileft, imax = ileft;
            for (int i = ileft + 1; i <= iright; ++i)
            {
                if (x[i] > x[imax])
                    imax = i;
                else if (x[i] < x[imin])
                    imin = i;
            }
            return (imin, imax);
        }

        /// <summary>
        /// Same as <see cref="MinMax_1"/>, but keeps the current minimum and maximum values in local variables.
        /// </summary>
        /// <param name="x">Array to scan.</param>
        /// <param name="ileft">Index of the first element of the range.</param>
        /// <param name="iright">Index of the last element of the range (inclusive).</param>
        /// <returns>The pair <c>(imin, imax)</c>.</returns>
        public static (int, int) MinMax_2(int[] x, int ileft, int iright)
        {
            int imin = ileft, imax = ileft;
            int xmin = x[ileft], xmax = x[ileft];
            for (int i = ileft + 1; i <= iright; ++i)
            {
                int v = x[i];
                if (v > xmax)
                {
                    xmax = v;
                    imax = i;
                }
                else if (v < xmin)
                {
                    xmin = v;
                    imin = i;
                }
            }
            return (imin, imax);
        }

        /// <summary>
        /// Same as <see cref="MinMax_2"/>, but always performs both tests (two independent <c>if</c>s
        /// instead of <c>else if</c>).
        /// </summary>
        /// <param name="x">Array to scan.</param>
        /// <param name="ileft">Index of the first element of the range.</param>
        /// <param name="iright">Index of the last element of the range (inclusive).</param>
        /// <returns>The pair <c>(imin, imax)</c>.</returns>
        public static (int, int) MinMax_3(int[] x, int ileft, int iright)
        {
            int imin = ileft, imax = ileft;
            int xmin = x[ileft], xmax = x[ileft];
            for (int i = ileft + 1; i <= iright; ++i)
            {
                int v = x[i];
                if (v > xmax)
                {
                    xmax = v;
                    imax = i;
                }
                if (v < xmin)
                {
                    xmin = v;
                    imin = i;
                }
            }
            return (imin, imax);
        }

        /// <summary>
        /// Returns the indices of the minimum and maximum of <c>x[ileft...iright]</c> (inclusive bounds)
        /// with a single scan (2 comparisons per element). Used by <see cref="MinMaxSort.Sort"/>.
        /// </summary>
        /// <param name="x">Array to scan.</param>
        /// <param name="ileft">Index of the first element of the range.</param>
        /// <param name="iright">Index of the last element of the range (inclusive).</param>
        /// <returns>The pair <c>(imin, imax)</c>; on ties, the first occurrence is returned.</returns>
        public static (int, int) MinMax(int[] x, int ileft, int iright)
        {
            int imin = ileft, imax = ileft;
            int xmin = x[ileft], xmax = x[ileft];
            for (int i = ileft + 1; i <= iright; ++i)
            {
                int v = x[i];
                if (v > xmax)
                {
                    xmax = v;
                    imax = i;
                }
                if (v < xmin)
                {
                    xmin = v;
                    imin = i;
                }
            }
            return (imin, imax);
        }

        /// <summary>
        /// Returns the indices of the minimum and maximum of <c>x[ileft...iright]</c> (inclusive bounds)
        /// by processing elements in pairs: the two elements of a pair are compared together, then the
        /// smaller one is compared with the current minimum and the larger one with the current maximum.
        /// Used by <see cref="MinMaxSort.OptimizedSort"/>.
        /// </summary>
        /// <param name="x">Array to scan.</param>
        /// <param name="ileft">Index of the first element of the range.</param>
        /// <param name="iright">Index of the last element of the range (inclusive).</param>
        /// <returns>The pair <c>(imin, imax)</c>.</returns>
        /// <remarks>About 3n/2 comparisons instead of 2n.</remarks>
        public static (int, int) MinMaxTournament(int[] x, int ileft, int iright)
        {
            int i, imin, imax;
            int n = iright - ileft + 1;
            if ((n & 1) == 0)
            {
                if (x[ileft] < x[ileft + 1])
                {
                    imin = ileft;
                    imax = ileft + 1;
                }
                else
                {
                    imin = ileft + 1;
                    imax = ileft;
                }
                i = ileft + 2;
            }
            else
            {
                imin = ileft;
                imax = ileft;
                i = ileft + 1;
            }
            int xmin = x[imin];
            int xmax = x[imax];
            while (i < iright)
            {
                int v = x[i];
                int w = x[i + 1];
                if (v < w)
                {
                    if (v < xmin)
                    {
                        xmin = v;
                        imin = i;
                    }
                    if (w > xmax)
                    {
                        xmax = w;
                        imax = i + 1;
                    }
                }
                else
                {
                    if (v > xmax)
                    {
                        xmax = v;
                        imax = i;
                    }
                    if (w < xmin)
                    {
                        xmin = w;
                        imin = i + 1;
                    }
                }
                i += 2; // move to the next pair
            }
            return (imin, imax);
        }
    }
}
