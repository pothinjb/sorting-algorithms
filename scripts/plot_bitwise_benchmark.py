"""Plots the mean execution time of BitwiseSortBenchmark as a function of the array size.

Input: the full JSON report written by BenchmarkDotNet with `--exporters json`, e.g.

    BENCH_CASE=0 dotnet run -c Release --project SortAlgorithms.Benchmarks -- \
        --job short --exporters json --filter '*BitwiseSortBenchmark.*'

Usage:

    python scripts/plot_bitwise_benchmark.py [report-full.json] [output.png]

Requires matplotlib. Also prints a Markdown table of the times for a few sizes.
"""

import json
import sys
from collections import defaultdict

import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt  # noqa: E402

DEFAULT_INPUT = "BenchmarkDotNet.Artifacts/results/SortAlgorithms.Benchmarks.BitwiseSortBenchmark-report-full-compressed.json"
DEFAULT_OUTPUT = "docs/images/benchmark_bitwise.png"

# benchmark method -> (legend label, color, marker, filled marker)
SERIES = {
    "MergeSort": ("MergeSort", "red", "^", False),
    "BitwiseSort": ("BitwiseSort", "black", "o", False),
    "OptimizedBitwiseSort": ("BranchlessBitwiseSort", "blue", "s", False),  # OptimizedBitwiseSort.Sort
    "OptimizedBitwiseSort1": ("OptimizedBitwiseSort", "deepskyblue", "s", False),  # OptimizedBitwiseSort._Sort1
    "ParallelMergeSort": ("ParallelMergeSort", "magenta", "D", False),
    "ArraySort": ("Array.Sort", "gray", "+", True),
}

TABLE_SIZES = [1_000_000, 5_000_000, 10_000_000]


def load(path):
    """Returns ({method: {size: mean in µs}}, {method: {size: allocated bytes}}, host info)."""
    with open(path, encoding="utf-8") as f:
        report = json.load(f)
    means = defaultdict(dict)
    allocated = defaultdict(dict)
    for b in report["Benchmarks"]:
        params = dict(p.split("=", 1) for p in b["Parameters"].split("&"))
        size = int(params["Size"])
        stats = b.get("Statistics")
        if not stats:
            continue  # failed benchmark (NA)
        means[b["Method"]][size] = stats["Mean"] / 1000.0  # ns -> µs
        memory = b.get("Memory") or {}
        allocated[b["Method"]][size] = memory.get("BytesAllocatedPerOperation")
    return means, allocated, report.get("HostEnvironmentInfo", {})


def plot(means, output):
    fig, ax = plt.subplots(figsize=(7, 5.25))
    # legend from the slowest to the fastest, by the mean time at the largest size
    methods = sorted((m for m in SERIES if m in means), key=lambda m: means[m][max(means[m])], reverse=True)
    for method in methods:
        label, color, marker, filled = SERIES[method]
        sizes = sorted(means[method])
        ax.plot(
            sizes,
            [means[method][n] for n in sizes],
            color=color,
            marker=marker,
            markerfacecolor=color if filled else "none",
            markersize=6,
            linewidth=1.2,
            label=label,
        )
    ax.set_xlabel("Array size (n)")
    ax.set_ylabel("Mean time (µs)")
    ax.set_xlim(left=0)
    ax.set_ylim(bottom=0)
    ax.ticklabel_format(style="sci", axis="both", scilimits=(0, 0))
    ax.grid(True, color="0.85")
    ax.legend(loc="upper left")
    fig.tight_layout()
    fig.savefig(output, dpi=150)


def print_table(means, allocated):
    nmax = TABLE_SIZES[-1]
    present = sorted((m for m in SERIES if m in means), key=lambda m: means[m][nmax], reverse=True)
    header = "| Algorithm | " + " | ".join(f"n = {n:,}" for n in TABLE_SIZES) + " | vs MergeSort (10 M) | vs ArraySort (10 M) | Allocated (10 M) |"
    print(header)
    print("|" + "---|" * (len(TABLE_SIZES) + 4))
    for m in present:
        cells = [f"{means[m][n] / 1000:,.1f} ms" if n in means[m] else "NA" for n in TABLE_SIZES]
        ratio_merge = means[m][nmax] / means["MergeSort"][nmax]
        ratio_array = means[m][nmax] / means["ArraySort"][nmax]
        alloc = allocated[m].get(nmax)
        alloc_cell = f"{alloc / 2**20:,.1f} MB" if alloc is not None else "NA"
        print(f"| {SERIES[m][0]} | " + " | ".join(cells) + f" | {ratio_merge:.2f} | {ratio_array:.2f} | {alloc_cell} |")


def main():
    path = sys.argv[1] if len(sys.argv) > 1 else DEFAULT_INPUT
    output = sys.argv[2] if len(sys.argv) > 2 else DEFAULT_OUTPUT
    means, allocated, host = load(path)
    plot(means, output)
    print(f"Plot written to {output}")
    print(f"Host: {host.get('ProcessorName')} / {host.get('RuntimeVersion')} / "
          f"{host.get('PhysicalCoreCount')} physical cores, {host.get('LogicalCoreCount')} logical\n")
    print_table(means, allocated)


if __name__ == "__main__":
    main()
