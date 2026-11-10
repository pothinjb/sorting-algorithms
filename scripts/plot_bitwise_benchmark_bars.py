"""Bar chart of the BitwiseSortBenchmark mean times at the largest measured size.

Input: a BenchmarkDotNet Markdown summary table (the GitHub report written to
BenchmarkDotNet.Artifacts/results/, or the table copied from the console).

Usage:

    python scripts/plot_bitwise_benchmark_bars.py [report.md] [output.png] [--exclude Method ...]

Requires matplotlib. Also prints the mean times at the largest size.
"""

import argparse
import re
from collections import defaultdict

import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt  # noqa: E402
from matplotlib.ticker import FuncFormatter  # noqa: E402

DEFAULT_INPUT = "BenchmarkDotNet.Artifacts/results/SortAlgorithms.Benchmarks.BitwiseSortBenchmark-report-github.md"
DEFAULT_OUTPUT = "docs/images/benchmark_bitwise.png"

UNIT_TO_MS = {"ns": 1e-6, "us": 1e-3, "μs": 1e-3, "µs": 1e-3, "ms": 1.0, "s": 1e3}

BASELINE = "ArraySort"
# benchmark method -> label on the y axis
LABELS = {"ArraySort": "Array.Sort (native)"}
# Fixed color per method, so that adding or removing a method never repaints the others
COLORS = {
    "ArraySort": "#2a78d6",
    "BitwiseSort": "#eb6834",
    "MergeSort": "#1baf7a",
    "OptimizedBitwiseSort": "#eda100",
    "ParallelMergeSort": "#008300",
}
FALLBACK_COLORS = ["#e87ba4", "#6250d6", "#e34948"]

SURFACE, TEXT, TEXT_2, GRID = "#fcfcfb", "#0b0b0b", "#52514e", "#e4e3df"


def load(path):
    """Returns ({method: {size: mean in ms}}, header lines above the table)."""
    means = defaultdict(dict)
    header, preamble = None, []
    with open(path, encoding="utf-8") as f:
        for line in f:
            if not line.startswith("|"):
                if header is None and line.strip() and not line.startswith("```"):
                    preamble.append(line.strip())
                continue
            if line.startswith("|--") or line.startswith("|:-"):
                continue
            cells = [c.strip() for c in line.strip().strip("|").split("|")]
            if header is None:
                header = cells
                continue
            row = dict(zip(header, cells))
            m = re.match(r"([\d,.]+)\s*(\S+)", row["Mean"])
            if not m:
                continue  # failed benchmark (NA)
            means[row["Method"]][int(row["Size"])] = float(m.group(1).replace(",", "")) * UNIT_TO_MS[m.group(2)]
    return means, preamble


def subtitle(preamble):
    """Builds 'BenchmarkDotNet vX · .NET Y · CPU' from the environment lines of the report."""
    parts = []
    for line in preamble:
        if line.startswith("BenchmarkDotNet"):
            parts.append(line.split(",")[0])
        elif line.startswith("[Host]"):
            m = re.search(r"\.NET [\d.]+", line)
            if m:
                parts.append(m.group(0))
        elif "CPU" in line or "Processor" in line or "GHz" in line:
            parts.append(line.split(" with ")[0].split(",")[0].split(" w/ ")[0])
    return " · ".join(["Random integer arrays (Case = AVERAGE)", *parts, "lower is better"])


def fmt_ms(v):
    return f"{v / 1000:.2f} s" if v >= 1000 else f"{v:,.0f} ms"


def plot(means, preamble, output):
    methods = list(means)
    extra = iter(FALLBACK_COLORS)
    colors = {m: COLORS.get(m) or next(extra) for m in methods}
    size = max(max(s) for s in means.values())
    order = sorted((m for m in methods if size in means[m]), key=lambda m: means[m][size], reverse=True)
    values = [means[m][size] for m in order]
    base = means[BASELINE][size]
    base_name = "Array.Sort"

    plt.rcParams.update({
        "font.family": ["Segoe UI", "DejaVu Sans"], "font.size": 11,
        "axes.edgecolor": GRID, "axes.labelcolor": TEXT_2,
        "xtick.color": TEXT_2, "ytick.color": TEXT_2, "text.color": TEXT,
        "axes.spines.top": False, "axes.spines.right": False,
        "figure.facecolor": SURFACE, "axes.facecolor": SURFACE,
    })
    fig, ax = plt.subplots(figsize=(13, 6.2), dpi=150)
    fig.subplots_adjust(left=0.19, right=0.97, top=0.80, bottom=0.12)
    fig.text(0.19, 0.93, f"Sorting algorithms — mean execution time for {size:,} elements",
             fontsize=16, fontweight="semibold")
    fig.text(0.19, 0.875, subtitle(preamble), fontsize=10.5, color=TEXT_2)

    bars = ax.barh([LABELS.get(m, m) for m in order], values, height=0.62,
                   color=[colors[m] for m in order], edgecolor=SURFACE, linewidth=2, zorder=3)
    for bar, m, v in zip(bars, order, values):
        ratio = v / base
        if m == BASELINE:
            note = "baseline"
        elif ratio < 1:
            note = f"{1 / ratio:.2f}× faster than {base_name}"
        else:
            note = f"{ratio:.2f}× slower than {base_name}"
        ax.text(v + max(values) * 0.008, bar.get_y() + bar.get_height() / 2,
                f"{fmt_ms(v)}  ·  {note}", va="center", fontsize=10.5, color=TEXT)

    ax.set_xlabel("Mean execution time (ms)")
    ax.xaxis.set_major_formatter(FuncFormatter(lambda v, _: f"{v:,.0f}"))
    ax.set_xlim(0, max(values) * 1.45)
    ax.grid(True, axis="x", color=GRID, lw=0.8)
    ax.set_axisbelow(True)
    ax.tick_params(axis="y", length=0, labelsize=11, labelcolor=TEXT)
    ax.spines["left"].set_visible(False)
    fig.savefig(output, dpi=150, facecolor=SURFACE)

    for m in order:
        print(f"{LABELS.get(m, m):<22} {fmt_ms(means[m][size]):>9}   x{means[m][size] / base:.2f} vs {base_name}")


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("input", nargs="?", default=DEFAULT_INPUT)
    parser.add_argument("output", nargs="?", default=DEFAULT_OUTPUT)
    parser.add_argument("--exclude", nargs="*", default=[], metavar="METHOD",
                        help="benchmark methods to leave out of the chart")
    args = parser.parse_args()

    means, preamble = load(args.input)
    for m in args.exclude:
        means.pop(m, None)
    plot(means, preamble, args.output)


if __name__ == "__main__":
    main()
