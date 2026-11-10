using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SortAlgorithms.Benchmarks
{
    /// <summary>
    /// Interactive entry point of the benchmarks.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// A sorting algorithm that can be picked in the comparison mode: one <c>[Benchmark]</c> method
        /// of one of the benchmark classes.
        /// </summary>
        /// <param name="Type">Benchmark class that declares the method (its <c>[GlobalSetup]</c> builds the input).</param>
        /// <param name="Method">The <c>[Benchmark]</c> method that runs the algorithm.</param>
        private sealed record Algorithm(Type Type, MethodInfo Method);

        /// <summary>Array size used by the comparison mode when the user leaves the size empty.</summary>
        private const int DefaultComparisonSize = 1000;

        /// <summary>
        /// Benchmark class under which the comparison mode lists a method declared by several classes,
        /// instead of the first class that declares it. <c>ArraySort</c> goes with the hybrid sorts:
        /// <see cref="Array.Sort(Array)"/> is an introsort.
        /// </summary>
        private static readonly Dictionary<string, Type> ComparisonHomeTypes = new()
        {
            ["ArraySort"] = typeof(HybridSortBenchmark),
        };

        /// <summary>
        /// Asks on the console which benchmark to run, the array size and the case, passes the size and
        /// case to the benchmark classes through the <c>BENCH_SIZE</c> and <c>BENCH_CASE</c> environment
        /// variables, then runs the chosen benchmark with BenchmarkDotNet (rank column, memory diagnoser,
        /// results ordered from fastest to slowest). Run it in Release.
        /// The last menu entry runs a custom selection of algorithms taken from any benchmark class,
        /// and joins their results in a single table (see <see cref="RunComparison"/>).
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
                typeof(HybridSortBenchmark),
            };

            // --- non-interactive mode: forward the arguments to BenchmarkDotNet (--filter, --job, --exporters...) ---
            if (args.Length > 0)
            {
                BenchmarkSwitcher.FromTypes(benchmarkTypes).Run(args, CreateConfig());
                return;
            }

            int comparisonChoice = benchmarkTypes.Length + 1;

            Console.WriteLine("=== Choose a benchmark to run ===");
            for (int i = 0; i < benchmarkTypes.Length; i++)
                Console.WriteLine($"{i + 1}. {benchmarkTypes[i].Name}");
            Console.WriteLine($"{comparisonChoice}. Compare algorithms (custom selection)");

            Console.Write("Your choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > comparisonChoice)
            {
                Console.WriteLine("Invalid choice!");
                return;
            }

            if (choice == comparisonChoice)
            {
                RunComparison(benchmarkTypes);
                return;
            }

            Type selectedBenchmark = benchmarkTypes[choice - 1];

            AskSize(null);
            AskCase();

            Console.WriteLine($"\n Starting benchmark {selectedBenchmark.Name} ...\n");

            BenchmarkRunner.Run(selectedBenchmark, CreateConfig());
        }

        /// <summary>
        /// Comparison mode: lists every sorting algorithm of the benchmark classes, lets the user pick one
        /// or more of them (e.g. <c>1,5,12</c> or <c>3-6,9</c>), asks the size and case, then runs only the
        /// selected methods and joins the results in a single summary, ranked per size and case.
        /// </summary>
        /// <remarks>
        /// Each method still runs in its own benchmark class, so it keeps that class's <c>[GlobalSetup]</c>;
        /// all classes build the same input for a given size and case, so the results are comparable.
        /// The <c>Ratio</c> columns are hidden: the baselines of the different classes mean nothing here.
        /// </remarks>
        /// <param name="benchmarkTypes">Benchmark classes in which to look for the algorithms.</param>
        private static void RunComparison(Type[] benchmarkTypes)
        {
            List<Algorithm> catalog = BuildAlgorithmCatalog(benchmarkTypes);

            Console.WriteLine("\n=== Choose the algorithms to compare ===");
            for (int i = 0; i < catalog.Count; i++)
                Console.WriteLine($"{i + 1,3}. {catalog[i].Method.Name,-32} ({catalog[i].Type.Name})");

            Console.Write("Your choice (e.g. 1,5,12 or 3-6,9): ");
            List<int> indexes = ParseSelection(Console.ReadLine(), catalog.Count);
            if (indexes == null)
            {
                Console.WriteLine("Invalid choice!");
                return;
            }

            List<Algorithm> selected = indexes.Select(i => catalog[i - 1]).ToList();

            AskSize(DefaultComparisonSize);
            AskCase();

            IConfig config = CreateConfig()
                .WithOptions(ConfigOptions.JoinSummary)                    // a single table for all the classes
                .AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByParams)  // rank the algorithms together, per Size and Case
                .HideColumns(Column.Ratio, Column.RatioSD, Column.AllocRatio); // baselines of different classes are meaningless here

            BenchmarkRunInfo[] runs = selected
                .GroupBy(a => a.Type)
                .Select(g => BenchmarkConverter.MethodsToBenchmarks(g.Key, g.Select(a => a.Method).ToArray(), config))
                .ToArray();

            // The [Orderer] attribute of each class overrides the config's orderer, so the joined table would
            // not be sorted: give every benchmark the same comparison config (fastest to slowest).
            ImmutableConfig sharedConfig = ImmutableConfigBuilder.Create(config);
            runs = runs
                .Select(r => new BenchmarkRunInfo(
                    r.BenchmarksCases
                        .Select(c => BenchmarkDotNet.Running.BenchmarkCase.Create(c.Descriptor, c.Job, c.Parameters, sharedConfig))
                        .ToArray(),
                    r.Type,
                    sharedConfig))
                .ToArray();

            Console.WriteLine($"\n Comparing {string.Join(", ", selected.Select(a => a.Method.Name))} ...\n");

            BenchmarkRunner.Run(runs);
        }

        /// <summary>
        /// Collects the <c>[Benchmark]</c> methods of the benchmark classes, in the order of
        /// <paramref name="benchmarkTypes"/> then in declaration order. A method name already seen in a
        /// previous class is skipped (<c>MergeSort</c>, <c>InsertionSort</c>, <c>ArraySort</c>... run the
        /// same code everywhere), except for the methods listed in <see cref="ComparisonHomeTypes"/>, which
        /// are only taken from their home class. <see cref="HybridMergeSortBenchmark"/> is left out: it is a
        /// threshold tuning benchmark, and its extra <c>ThresholdInsert</c> parameter would split the
        /// comparison table.
        /// </summary>
        /// <param name="benchmarkTypes">Benchmark classes to scan.</param>
        /// <returns>The algorithms that can be compared, without duplicate names.</returns>
        private static List<Algorithm> BuildAlgorithmCatalog(Type[] benchmarkTypes)
        {
            var catalog = new List<Algorithm>();
            var seenNames = new HashSet<string>();

            foreach (Type type in benchmarkTypes)
            {
                if (type == typeof(HybridMergeSortBenchmark))
                    continue;

                IEnumerable<MethodInfo> methods = type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.GetCustomAttribute<BenchmarkAttribute>() != null)
                    .OrderBy(m => m.MetadataToken); // declaration order

                foreach (MethodInfo method in methods)
                {
                    if (ComparisonHomeTypes.TryGetValue(method.Name, out Type homeType) && homeType != type)
                        continue; // listed with its home class

                    if (seenNames.Add(method.Name))
                        catalog.Add(new Algorithm(type, method));
                }
            }

            return catalog;
        }

        /// <summary>
        /// Parses a selection such as <c>1,5,12</c> or <c>3-6, 9</c> (numbers and ascending ranges
        /// separated by commas or spaces).
        /// </summary>
        /// <param name="line">The line typed by the user.</param>
        /// <param name="count">Number of entries in the menu: every number must be in [1, count].</param>
        /// <returns>The distinct chosen numbers (1-based) in ascending order, or <c>null</c> if the line is empty or invalid.</returns>
        private static List<int> ParseSelection(string line, int count)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var indexes = new SortedSet<int>();
            string[] tokens = line.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                string[] bounds = token.Split('-');
                if (bounds.Length == 1 && int.TryParse(bounds[0], out int single))
                {
                    if (single < 1 || single > count)
                        return null;
                    indexes.Add(single);
                }
                else if (bounds.Length == 2 &&
                         int.TryParse(bounds[0], out int first) &&
                         int.TryParse(bounds[1], out int last))
                {
                    if (first < 1 || last > count || first > last)
                        return null;
                    for (int i = first; i <= last; i++)
                        indexes.Add(i);
                }
                else
                {
                    return null;
                }
            }

            return indexes.ToList();
        }

        /// <summary>
        /// Asks the array size and stores it in the <c>BENCH_SIZE</c> environment variable, read by
        /// <see cref="BenchmarkSort.GetSizes"/>.
        /// </summary>
        /// <param name="defaultSize">
        /// Size used when the answer is empty or invalid; <c>null</c> unsets the variable so that each
        /// benchmark class uses its own default sizes.
        /// </param>
        private static void AskSize(int? defaultSize)
        {
            string defaultText = defaultSize.HasValue ? $"{defaultSize.Value}" : "the default sizes";
            Console.Write($"\nEnter the array size (Size), or leave empty for {defaultText}:");
            var line = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(line) && int.TryParse(line, out int size) && size > 0)
            {
                // Set an environment variable for the ParamsSource method to read
                Environment.SetEnvironmentVariable("BENCH_SIZE", size.ToString());
            }
            else
            {
                // Use the given default, or unset the variable so the default sizes are used
                Environment.SetEnvironmentVariable("BENCH_SIZE", defaultSize?.ToString());
            }
        }

        /// <summary>
        /// Asks the test case and stores it in the <c>BENCH_CASE</c> environment variable, read by
        /// <see cref="BenchmarkSort.GetCases"/>. An empty answer gives AVERAGE.
        /// </summary>
        private static void AskCase()
        {
            Console.WriteLine("\nTest case:");
            Console.WriteLine($"0. {BenchmarkCase.AVERAGE} [default]");
            Console.WriteLine($"1. {BenchmarkCase.BEST}");
            Console.WriteLine($"2. {BenchmarkCase.WORST}");
            Console.WriteLine($"3. {BenchmarkCase.BEST}, {BenchmarkCase.AVERAGE}, {BenchmarkCase.WORST}");
            Console.Write("Your choice: ");
            var line = Console.ReadLine();
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
