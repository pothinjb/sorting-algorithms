# SortAlgorithms

[![CI](https://github.com/pothinjb/sorting-algorithms/actions/workflows/ci.yml/badge.svg)](https://github.com/pothinjb/sorting-algorithms/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![C#](https://img.shields.io/badge/language-C%23-239120.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)
![Tests: MSTest](https://img.shields.io/badge/tests-MSTest-blue.svg)
![Benchmarks: BenchmarkDotNet](https://img.shields.io/badge/benchmarks-BenchmarkDotNet-purple.svg)

> 📖 **Companion ebook (in French):** the algorithms implemented here are presented and analyzed in the companion ebook, [available on Amazon](https://www.amazon.fr/dp/B0HRHGB7D4).

A collection of classic and experimental **sorting algorithms in C#**, written to be read side by side with their textbook pseudo-code, together with a shared **unit test suite** and **BenchmarkDotNet** benchmarks to compare them.

This code is the companion of a (French) LaTeX report on algorithms. The implementations deliberately stay close to the CLRS-style pseudo-code (sentinels, inclusive `p..r` / `left..right` bounds, 0-based indices): **readability and faithfulness to the algorithm come first**, raw speed second. Several algorithms also come in "optimized" or parallel variants so you can measure what each trick actually buys you.

## Algorithms

All algorithms sort `int[]`. Most sort **in place**; a few return a new array (noted below).

| Family | Class | Entry points | Notes |
|---|---|---|---|
| Exchange | `BubbleSort`, `ImprovedBubbleSort`, `CocktailSort` | `Sort(A)` | Bubble sort, early-exit bubble sort, bidirectional (shaker) sort |
| Selection | `SelectionSort` | `Sort(A)`, `BasicSort(A)` | `BasicSort` returns a new array |
| Selection | `MinMaxSort` | `Sort(A)`, `OptimizedSort(A)` | Places both the minimum and the maximum on each pass |
| Insertion | `InsertionSort` | `Sort(A)`, `SortRange(A, i, j)` | Plus experimental variants `_Sort1`…`_Sort4` |
| Insertion | `ChunkSort` | `PairSort`, `TripletSort`, `QuartetSort`, `ParallelTripletSort`, `ParallelQuartetSort` | Insertion sort that inserts 2, 3 or 4 elements at a time |
| Insertion | `ShellSort` | `Sort(A)`, `Sort(A, seq)`, `ImprovedSort`, `OptimizedSort` | Gap sequences: Shell, Hibbard, Knuth, Papernov–Stasevich, Sedgewick, Incerpi–Sedgewick, Pratt, Lee (`ShellSequence`, `Gaps`) |
| Insertion | `LibrarySort` | `Sort(A, epsilon)` | Gapped insertion sort; the integer `epsilon` (≥ 1) controls the amount of free space |
| Divide & conquer | `MergeSort` | `Sort(A)`, `ParallelSort(A)` | CLRS merge sort with sentinels |
| Divide & conquer | `MergeInsertionSort`, `MergeQuartetSort`, `OptimizedMergeSort`, `SkipMergeSort` | `Sort(A, …)`, `ParallelSort(A, …)` | Hybrid merge sorts (insertion/quartet sort below a threshold, skipped merges on already ordered runs, parallel recursion) |
| Divide & conquer | `QuickSort` | `new QuickSort().Sort(A, QuickSortMethod.X)` | Hoare, Lomuto and a "naive" partition scheme; partitions are also exposed as static methods |
| Heap | `HeapSort` | `Sort(A)`, `SortUsingNaiveMaxHeapify(A)`, `SortRange(A, p, r)` | `MaxHeapify` that shifts a "hole" down vs. the naive swap-based version |
| Hybrid | `IntroSort` | `Sort(A)`, `Sort(A, depthLimit)` | Introspective sort: median-of-three quicksort with Hoare partition, insertion sort on sub-arrays of at most 16 elements, heap sort fallback beyond `depthLimit` (default 2⌊log₂ n⌋) partitioning levels |
| Hybrid | `PdqSort` | `Sort(A)`, `Sort(A, bad)` | Pattern-defeating quicksort: median-of-three or ninther pivot, linear time on sorted input and on few distinct values, heap sort fallback after `bad` (default ⌊log₂ n⌋) unbalanced partitions |
| Non-comparison | `CountingSort` | `Sort(A, k)` | Returns a new array; values must be in `[0, k)` |
| Non-comparison | `BitwiseSort`, `OptimizedBitwiseSort` | `Sort(A)` | Binary LSD radix sort; values must be non-negative |

Helpers such as `IndexOf.Min` / `IndexOf.MinMax*` (min/max search, including a tournament method) live in `SortAlgorithms.Core/Services`.

Every public type and method carries XML documentation (complexity, stability, preconditions), so IntelliSense is the quickest way to explore the API.

## Getting started

### Prerequisites

- The [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later.

### Editor

Everything works from the command line with the `dotnet` CLI. In an IDE:

| Editor | .NET 10 support |
|---|---|
| [Visual Studio Code](https://code.visualstudio.com/) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) | ✅ Open the folder; projects appear in *Solution Explorer* and the tests in the *Testing* view |
| [Visual Studio 2026](https://visualstudio.microsoft.com/) (18.0+) | ✅ Open `SortAlgorithms.sln` |
| Visual Studio 2022 | ❌ Cannot target `net10.0` — use VS Code, VS 2026 or the CLI |
| JetBrains Rider | ✅ With a release that supports .NET 10 |

### Build and test

```bash
git clone https://github.com/pothinjb/sorting-algorithms.git
cd sorting-algorithms

dotnet build SortAlgorithms.sln
dotnet test SortAlgorithms.Tests
```

Run a single test class:

```bash
dotnet test SortAlgorithms.Tests --filter "FullyQualifiedName~QuickSortTests"
```

### Usage

The APIs are not fully uniform: some algorithms are static classes, others are instances configured by a variant enum or by thresholds.

```csharp
using SortAlgorithms.Core;

int[] a = { 5, 2, 9, 1, 7 };

MergeSort.Sort(a);                                   // static, in place
new QuickSort().Sort(a, QuickSortMethod.Lomuto);     // instance + partition scheme
ShellSort.Sort(a, ShellSequence.Knuth);              // choose a gap sequence
new MergeInsertionSort().ParallelSort(a);            // hybrid, parallel
int[] sorted = CountingSort.Sort(a, k: 10);          // returns a new array, values in [0, 10)
```

## Benchmarks

Benchmarks use [BenchmarkDotNet](https://benchmarkdotnet.org/) and **must be run in Release, from the repository root** (BenchmarkDotNet looks for the project file from the current directory):

```bash
dotnet run -c Release --project SortAlgorithms.Benchmarks
```

The program is interactive. It asks for:

1. the benchmark class to run (one per algorithm family), or the last entry, *Compare algorithms*, to build your own comparison (see below);
2. an array size (leave empty for the defaults 10, 100, 1 000, 10 000, 100 000);
3. the input case: `0` AVERAGE (random data, fixed seed), `1` BEST (ascending), `2` WORST (descending), `3` all three.

These choices are passed to BenchmarkDotNet through the `BENCH_SIZE` and `BENCH_CASE` environment variables. To skip the prompts (in a script, for instance), pipe the three answers:

```bash
# benchmark #12 (QuickSortBenchmark), size 1000, AVERAGE case
printf '12\n1000\n0\n' | dotnet run -c Release --project SortAlgorithms.Benchmarks
```

Every method of a benchmark class sorts a copy of the same input, so the results are directly comparable; one method (usually `MergeSort`) is the baseline of the `Ratio` column. Results (including memory allocations and a rank column) are written to `BenchmarkDotNet.Artifacts/`.

> Each benchmark runs several sizes and can take a while, especially the quadratic sorts on large arrays. Start with a single size.

### Comparing algorithms of your choice

The last menu entry (`15. Compare algorithms (custom selection)`) lists every sorting algorithm of all the benchmark classes (each name once, `ArraySort` being listed with the hybrid sorts; the threshold-tuning `HybridMergeSortBenchmark` is left out) and lets you pick one or more of them, from any family: type numbers and ranges such as `1,5,12` or `3-6,9`. The size prompt then defaults to **1000** (the families have very different default sizes), and the case prompt is unchanged.

All the selected algorithms are reported in a **single table**, sorted from fastest to slowest and ranked together for each size and case. Each algorithm still runs in its own class, on the same input. The `Ratio` columns are hidden, because the baselines of the different classes mean nothing in a mixed comparison.

```bash
# HeapSortUsingFloydMaxHeapify, QuicksortHoare and Shell, size 1000, all three cases
printf '15\n23,38,44\n1000\n3\n' | dotnet run -c Release --project SortAlgorithms.Benchmarks
```

The algorithm numbers come from the menu and shift when benchmark methods are added: check the list before scripting a selection.

Any command-line argument skips the prompts and is passed to BenchmarkDotNet, so the standard options are available (`--filter`, `--job short`, `--exporters json`…). The size and case still come from `BENCH_SIZE` / `BENCH_CASE`:

```bash
BENCH_CASE=0 dotnet run -c Release --project SortAlgorithms.Benchmarks -- --job short --filter '*QuickSortBenchmark*'
```

### Example: binary radix sort vs. comparison sorts

![Mean time of OptimizedBitwiseSort, ParallelMergeSort, Array.Sort, BitwiseSort and MergeSort for 10 million elements](docs/images/benchmark_bitwise.png)

| Algorithm | n = 1 M | n = 5 M | n = 10 M | vs `MergeSort` (10 M) | vs `Array.Sort` (10 M) |
|---|---:|---:|---:|---:|---:|
| `MergeSort.Sort` | 82.8 ms | 456.5 ms | 885.8 ms | 1.00 | 1.68 |
| `BitwiseSort.Sort` | 69.2 ms | 403.5 ms | 817.0 ms | 0.92 | 1.55 |
| `Array.Sort` (native) | 44.8 ms | 246.4 ms | 528.0 ms | 0.60 | 1.00 |
| `MergeSort.ParallelSort` | 30.2 ms | 147.8 ms | 326.4 ms | 0.37 | 0.62 |
| `OptimizedBitwiseSort.Sort` | 12.4 ms | 71.5 ms | 148.0 ms | 0.17 | 0.28 |

The input here is random non-negative integers in `[0, n)`. Merge sort runs in Θ(n log n), while the binary LSD radix sorts grow linearly in Θ(k·n), where k is the number of bits of the largest value (24 for n = 10 M). Taken as is, the textbook `BitwiseSort` is only about 8 % faster than merge sort: it makes many passes, each of which is cheap but full of unpredictable branches. `OptimizedBitwiseSort.Sort` is the last step of a chain of optimizations (`_Sort1` → … → `_Sort6` → `Sort`, documented in the code): it skips every bit that is the same in all values, makes the inner loop **branchless** (each value is written to both groups and only the right counter advances), writes through pointers without bounds checks, unrolls the inner loop by 4, and uses a single auxiliary array in which group 0 grows from the front and group 1 from the back, so the concatenation needs no copy at all. At 10 M it is 5.5× faster than `BitwiseSort`, 6.0× faster than merge sort and **3.6× faster than the runtime's `Array.Sort`** (introsort). On a single thread, it is also 2.2× faster than `MergeSort.ParallelSort`, which runs on all cores but only gains 2.7× over the sequential merge sort. The gap opens from n = 10,000 (82 µs vs 274 µs for `Array.Sort`); at n = 1,000 the two are level (6.2 µs vs 6.0 µs). The price is generality and memory: the bitwise sorts only accept non-negative integers and need auxiliary arrays of size n. At n = 10 M, `OptimizedBitwiseSort.Sort` allocates 76 MB per call (the benchmark's 38 MB input copy plus a single auxiliary array) and `BitwiseSort` 114 MB (two auxiliary arrays), while `Array.Sort` allocates only the 38 MB of the input copy. The CLRS merge sort, which allocates its sentinel sub-arrays at every merge, allocates 1.5 GB per call, and this garbage-collector pressure probably explains why both merge sorts stay behind.

*Measured with BenchmarkDotNet 0.15.8 (default job), .NET 10.0.12, AMD Ryzen 5 8645HS (6 cores, 12 threads), Windows 11. To reproduce, run the command below, then `python scripts/plot_bitwise_benchmark_bars.py` (requires matplotlib), which reads the Markdown report from `BenchmarkDotNet.Artifacts/results/` and writes the figure:*

```bash
BENCH_CASE=0 dotnet run -c Release --project SortAlgorithms.Benchmarks -- \
  --filter '*BitwiseSortBenchmark.MergeSort' '*BitwiseSortBenchmark.BitwiseSort' \
           '*BitwiseSortBenchmark.OptimizedBitwiseSort' '*BitwiseSortBenchmark.ParallelMergeSort' \
           '*BitwiseSortBenchmark.ArraySort'
```

## Project structure

```
SortAlgorithms.sln
├── .github/workflows/ci.yml      # build + tests on GitHub Actions
├── SortAlgorithms.Core/          # the algorithms, one file per algorithm
│   └── Services/                 # shared helpers (Swap, min/max search)
├── SortAlgorithms.Tests/         # MSTest suite shared by all algorithms
└── SortAlgorithms.Benchmarks/    # BenchmarkDotNet benchmarks + interactive launcher
```

## Known limitations

These are documented in the code as well.

- **Value ranges** — `CountingSort` needs values in `[0, k)` and the bitwise sorts need non-negative values (these preconditions are not checked).
- **QuickSort worst case** — the pivot is always the first or last element, as in the textbook versions, so sorted or reversed input takes Θ(n²) time. The recursion only goes into the smaller partition, so the stack depth stays O(log n).
- **`LibrarySort`** requires `epsilon >= 1` and throws `ArgumentOutOfRangeException` otherwise.
- **Instance-based sorts** (`QuickSort`, `MergeInsertionSort`, `MergeQuartetSort`, `OptimizedMergeSort`) keep their settings in fields: don't share one instance between threads.

## Contributing

Issues and pull requests are welcome — new algorithms, new variants, bug fixes or better benchmarks.

### Adding an algorithm

1. **Core** — add a file in `SortAlgorithms.Core/` that sorts an `int[]`. Keep the code close to the pseudo-code it comes from and document it with XML comments (summary, complexity, preconditions).
2. **Tests** — create an `XxxSortTests` class deriving from `GenericSortTests`:
   - list the variant names in `SortMethods`;
   - map each name to a call in `ApplySort`;
   - copy the `SortMethodData` property and the `[TestMethod]` method that calls `RunAllCommonTests` from an existing test class (for example `QuickSortTests`), replacing the class name.

   The class automatically runs the common suite: empty array, 1–3 elements, random arrays, duplicates, ascending, descending, all-equal. For algorithms limited by value range, pass a `maxRange` to `RunAllCommonTests`.
3. **Benchmarks** — create an `XxxBenchmark` deriving from `BenchmarkSort`:
   - in each `[Benchmark]` method, `Clone()` the input `_A` before sorting;
   - mark one method `Baseline = true` (usually `MergeSort`);
   - add the new class to the `benchmarkTypes` array in `Program.cs`.

### Conventions

- Comments, documentation, test messages and console output are in English.
- Use the textbook naming (`A`, `p`, `q`, `r`, `left`, `right`…) when it helps compare the code with the pseudo-code.
- Run `dotnet test` before opening a pull request. The CI workflow (`.github/workflows/ci.yml`) builds the solution and runs the tests on every push to `main` and on every pull request.

## References

- T. H. Cormen, C. E. Leiserson, R. L. Rivest, C. Stein, *Introduction to Algorithms* (CLRS), MIT Press — the pseudo-code most implementations follow.
- D. E. Knuth, *The Art of Computer Programming, Vol. 3: Sorting and Searching*, Addison-Wesley.
- M. A. Bender, M. Farach-Colton, M. A. Mosteiro, "Insertion Sort is O(n log n)", *Theory of Computing Systems*, 2006 — library sort.
- [Shellsort gap sequences](https://en.wikipedia.org/wiki/Shellsort#Gap_sequences) — overview of the sequences implemented in `ShellSort`.
- D. R. Musser, "Introspective Sorting and Selection Algorithms", *Software: Practice and Experience* 27(8), 1997 — `IntroSort`.
- O. R. L. Peters, ["Pattern-defeating Quicksort"](https://arxiv.org/abs/2106.05123), arXiv:2106.05123, 2021 — `PdqSort`.

## License

Released under the [MIT License](LICENSE).
