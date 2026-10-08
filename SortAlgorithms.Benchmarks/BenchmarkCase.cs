namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Order of the input array of a benchmark.
    /// </summary>
    public enum BenchmarkCase
    {
        /// <summary>Already sorted, ascending (0, 1, ..., n - 1).</summary>
        BEST,
        /// <summary>Random values in [0, n), with a fixed seed.</summary>
        AVERAGE,
        /// <summary>Sorted in descending order (n, n - 1, ..., 1).</summary>
        WORST
    }
}
