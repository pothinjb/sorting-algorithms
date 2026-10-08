namespace SortAlgorithms.Core
{
    /// <summary>
    /// Small helpers shared by the sorting algorithms.
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// Swaps <c>A[i]</c> and <c>A[j]</c>.
        /// </summary>
        /// <param name="A">Array holding the two elements.</param>
        /// <param name="i">Index of the first element.</param>
        /// <param name="j">Index of the second element.</param>
        public static void Swap(int[] A, int i, int j)
        {
            int temp = A[j];
            A[j] = A[i];
            A[i] = temp;
        }
    }
}
