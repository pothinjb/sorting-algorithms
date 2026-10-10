using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Interactive entry point of the benchmarks.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Asks on the console which benchmark to run, the array size and the case, passes the size and
        /// case to the benchmark classes through the <c>BENCH_SIZE</c> and <c>BENCH_CASE</c> environment
        /// variables, then runs the chosen benchmark with BenchmarkDotNet (rank column, memory diagnoser,
        /// results ordered from fastest to slowest). Run it in Release.
        /// </summary>
        /// <param name="args">
        /// If not empty, the prompts are skipped and the arguments are passed to BenchmarkDotNet's
        /// <see cref="BenchmarkSwitcher"/> (for example <c>--filter</c>, <c>--job short</c>, <c>--exporters json</c>).
        /// </param>
        static void Main(string[] args)
        {
            // --- available benchmarks ---
            Type[] benchmarkTypes = new Type[]
            {
                typeof(BitwiseSortBenchmark),
                typeof(BubbleSortBenchmark),
                typeof(ChunkSortBenchmark),
                typeof(CountingSortBenchmark),
                typeof(HeapSortBenchmark),
                typeof(HybridMergeSortBenchmark),
                typeof(InsertionSortBenchmark),
                typeof(LibrarySortBenchmark),
                typeof(MergeSortBenchmark),
                typeof(MinMaxSortBenchmark),
                typeof(OptimizedMergeSortBenchmark),
                typeof(QuickSortBenchmark),
                typeof(ShellSortBenchmark),
            };

            // --- non-interactive mode: forward the arguments to BenchmarkDotNet (--filter, --job, --exporters...) ---
            if (args.Length > 0)
            {
                BenchmarkSwitcher.FromTypes(benchmarkTypes).Run(args, CreateConfig());
                return;
            }

            Console.WriteLine("=== Choose a benchmark to run ===");
            for (int i = 0; i < benchmarkTypes.Length; i++)
                Console.WriteLine($"{i + 1}. {benchmarkTypes[i].Name}");

            Console.Write("Your choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > benchmarkTypes.Length)
            {
                Console.WriteLine("Invalid choice!");
                return;
            }

            Type selectedBenchmark = benchmarkTypes[choice - 1];

            Console.Write("\nEnter the array size (Size), or leave empty for the default sizes:");
            var line = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(line) && int.TryParse(line, out int size) && size > 0)
            {
                // Set an environment variable for the ParamsSource method to read
                Environment.SetEnvironmentVariable("BENCH_SIZE", size.ToString());
            }
            else
            {
                // Make sure it is unset so the default sizes are used
                Environment.SetEnvironmentVariable("BENCH_SIZE", null);
            }

            Console.WriteLine("\nTest case:");
            Console.WriteLine($"0. {BenchmarkCase.AVERAGE} [default]");
            Console.WriteLine($"1. {BenchmarkCase.BEST}");
            Console.WriteLine($"2. {BenchmarkCase.WORST}");
            Console.WriteLine($"3. {BenchmarkCase.BEST}, {BenchmarkCase.AVERAGE}, {BenchmarkCase.WORST}");
            Console.Write("Your choice: ");
            line = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(line) && int.TryParse(line, out int benchCase) && benchCase >= 0)
            {
                // Set an environment variable for the ParamsSource method to read
                Environment.SetEnvironmentVariable("BENCH_CASE", benchCase.ToString());
            }
            else
            {
                // Fall back to the default case (AVERAGE)
                Environment.SetEnvironmentVariable("BENCH_CASE", "0");
            }

            Console.WriteLine($"\n Starting benchmark {selectedBenchmark.Name} ...\n");

            BenchmarkRunner.Run(selectedBenchmark, CreateConfig());
        }

        /// <summary>
        /// BenchmarkDotNet config shared by both modes: rank column, memory diagnoser,
        /// results ordered from fastest to slowest.
        /// </summary>
        private static IConfig CreateConfig()
        {
            return ManualConfig.Create(DefaultConfig.Instance)
                .AddColumn(RankColumn.Arabic)
                .AddDiagnoser(MemoryDiagnoser.Default)
                .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));
        }
    }
}
