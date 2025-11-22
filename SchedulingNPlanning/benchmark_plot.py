"""
Benchmarking script for Greedy vs Optimal Prolog scheduling algorithms

This file was regenerated to repair corruption and to provide:
- precise timing using time.perf_counter()
- robust handling of Prolog timeouts
- plotting with faint grey grid, black/dimgray markers, integer x ticks,
  labeled points, and improved smoothing (LOWESS/Savitzky-Golay/spline fallback).

Usage: run from the `SchedulingNPlanning` directory where `test_facts.pl` and
the `algorithms/` folder live. Adjust `TIMEOUT`, `MAX_SIZE`, and paths below
if necessary.
"""

import os
import subprocess
import time
from typing import List, Optional, Tuple

import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import numpy as np
import argparse

# --- Config ---
FACTS_FILE = 'test_facts.pl'
PROLOG_PATH = 'swipl'
GREEDY_FILE = 'algorithms/greedy_scheduling.pl'
OPTIMAL_FILE = 'algorithms/optimal_scheduling.pl'
TIMEOUT = 60
START_SIZE = 2
MAX_SIZE = 20
STEP = 1


def get_vessel_facts(n: int) -> str:
    with open(FACTS_FILE, 'r', encoding='utf-8') as f:
        lines = [line for line in f if line.strip().startswith('vessel(')]
    return ''.join(lines[:n])


def write_temp_facts(n: int, temp_path: str) -> None:
    facts = get_vessel_facts(n)
    with open(temp_path, 'w', encoding='utf-8') as f:
        f.write(facts)


def run_prolog(file: str, predicate: str, size: int) -> Tuple[Optional[float], bool, Optional[float]]:
    temp_facts = f'temp_facts_{size}.pl'
    write_temp_facts(size, temp_facts)
    # Ask Prolog to run the predicate and print the Delay value using format/2
    # Use once/1 to avoid printing multiple solutions
    goal = f"once(({predicate}(_, Delay), format('DELAY:~w\\n',[Delay])))."
    cmd = [PROLOG_PATH, '-q', '-l', temp_facts, '-l', file, '-g', goal, '-t', 'halt']
    print(f"Running: {' '.join(cmd)}")
    try:
        start = time.perf_counter()
        proc = subprocess.run(cmd, timeout=TIMEOUT, capture_output=True)
        end = time.perf_counter()
        s_out = proc.stdout.decode().strip()
        s_err = proc.stderr.decode().strip()
        if s_out:
            print('STDOUT:')
            print(s_out)
        if s_err:
            print('STDERR:')
            print(s_err)
        # Try to extract Delay value printed as 'DELAY:<value>' from stdout
        delay_val: Optional[float] = None
        try:
            for line in s_out.splitlines():
                if line.startswith('DELAY:'):
                    raw = line.split('DELAY:', 1)[1].strip()
                    # try int then float
                    try:
                        delay_val = int(raw)
                    except Exception:
                        try:
                            delay_val = float(raw)
                        except Exception:
                            delay_val = None
                    break
        except Exception:
            delay_val = None
        try:
            os.remove(temp_facts)
        except Exception:
            pass
        return end - start, proc.returncode == 0, delay_val
    except subprocess.TimeoutExpired:
        print('TIMEOUT occurred!')
        try:
            os.remove(temp_facts)
        except Exception:
            pass
        return None, False, None
    except Exception as e:
        print('Error running Prolog:', e)
        try:
            os.remove(temp_facts)
        except Exception:
            pass
        return None, False, None


def smooth_curve(x_vals_src: List[float], y_vals_src: List[float]) -> Tuple[Optional[np.ndarray], Optional[np.ndarray], str]:
    pairs = list(zip(x_vals_src, y_vals_src))
    if len(pairs) < 2:
        return None, None, ''
    xf = np.array([p[0] for p in pairs])
    yf = np.array([p[1] for p in pairs])

    # Try LOWESS (statsmodels)
    try:
        from statsmodels.nonparametric.smoothers_lowess import lowess

        frac = 0.4 if len(xf) >= 6 else 0.8
        y_low = lowess(yf, xf, frac=frac, return_sorted=False)
        return xf, y_low, f'LOWESS(frac={frac})'
    except Exception:
        pass

    # Try Savitzky-Golay (scipy)
    try:
        from scipy.signal import savgol_filter

        win = len(xf) if len(xf) % 2 == 1 else len(xf) - 1
        win = max(3, min(win, 9))
        polyorder = 2 if win > 2 else 1
        if win >= polyorder + 1 and win <= len(xf):
            y_sg = savgol_filter(yf, window_length=win, polyorder=polyorder)
            x_new = np.linspace(xf.min(), xf.max(), 200)
            y_new = np.interp(x_new, xf, y_sg)
            return x_new, y_new, f'SavGol(w={win},p={polyorder})'
    except Exception:
        pass

    # Try cubic spline (scipy)
    try:
        from scipy.interpolate import make_interp_spline

        k = 3 if len(xf) >= 4 else 1
        x_new = np.linspace(xf.min(), xf.max(), 200)
        spline = make_interp_spline(xf, yf, k=k)
        y_new = spline(x_new)
        return x_new, y_new, f'Spline(k={k})'
    except Exception:
        pass

    # Fallback: moving average + interpolation
    win = max(1, min(3, len(xf)))
    kernel = np.ones(win) / win
    y_ma = np.convolve(yf, kernel, mode='same')
    x_new = np.linspace(xf.min(), xf.max(), 200)
    y_new = np.interp(x_new, xf, y_ma)
    return x_new, y_new, f'MovAvg(w={win})'


def safe_poly_eq(x_vals_src: List[float], y_vals_src: List[float]) -> str:
    pairs = list(zip(x_vals_src, y_vals_src))
    if len(pairs) < 2:
        return ''
    xf = [p[0] for p in pairs]
    yf = [p[1] for p in pairs]
    try:
        deg = 2 if len(xf) >= 3 else 1
        coeffs = np.polyfit(xf, yf, deg)
        if deg == 2:
            a, b, c = coeffs
            return f'({a:.3e}x^2 + {b:.3e}x + {c:.3e})'
        else:
            m, b = coeffs
            return f'({m:.3e}x + {b:.3e})'
    except Exception:
        return ''


def main(output_file: Optional[str] = None, data_output: Optional[str] = None):
    greedy_timed_out = False
    optimal_timed_out = False

    x_sizes: List[int] = []
    greedy_times: List[Optional[float]] = []
    optimal_times: List[Optional[float]] = []
    # capture total delays reported by Prolog (None for timeout/fail)
    greedy_delays: List[Optional[float]] = []
    optimal_delays: List[Optional[float]] = []

    for size in range(START_SIZE, MAX_SIZE + 1, STEP):
        if greedy_timed_out and optimal_timed_out:
            print(f"Early stop: both algorithms timed out at size {size - STEP}")
            break
        print(f"Testing input size: {size}")
        x_sizes.append(size)

        # Greedy
        if not greedy_timed_out:
            t_g, ok_g, d_g = run_prolog(GREEDY_FILE, 'obtain_seq_greedy', size)
            if ok_g and t_g is not None:
                greedy_times.append(t_g)
                greedy_delays.append(d_g)
                print(f"  Greedy:   {t_g:.2f}s (delay={d_g})")
            else:
                greedy_timed_out = True
                greedy_times.append(None)
                greedy_delays.append(None)
                print(f"  Greedy:   TIMEOUT/FAIL")
        else:
            greedy_times.append(None)
            greedy_delays.append(None)
            print(f"  Greedy:   SKIPPED (TIMEOUT)")

        # Optimal
        if not optimal_timed_out:
            t_o, ok_o, d_o = run_prolog(OPTIMAL_FILE, 'obtain_seq_shortest_delay', size)
            if ok_o and t_o is not None:
                optimal_times.append(t_o)
                optimal_delays.append(d_o)
                print(f"  Optimal:  {t_o:.2f}s (delay={d_o})")
            else:
                optimal_timed_out = True
                optimal_times.append(None)
                optimal_delays.append(None)
                print(f"  Optimal:  TIMEOUT/FAIL")
        else:
            optimal_times.append(None)
            optimal_delays.append(None)
            print(f"  Optimal:  SKIPPED (TIMEOUT)")

    # Prepare data for plotting
    x_g = [s for s, t in zip(x_sizes, greedy_times) if t is not None]
    y_g = [t for t in greedy_times if t is not None]
    x_o = [s for s, t in zip(x_sizes, optimal_times) if t is not None]
    y_o = [t for t in optimal_times if t is not None]

    # Export data if requested (preserve None for timeouts)
    if data_output:
        # build rows corresponding to x_sizes
        rows = {
            'size': x_sizes,
            'greedy_time': greedy_times,
            'greedy_delay': greedy_delays,
            'optimal_time': optimal_times,
            'optimal_delay': optimal_delays,
        }
        try:
            import pandas as pd

            df = pd.DataFrame(rows)
            # choose format by extension
            lower = data_output.lower()
            if lower.endswith('.xlsx'):
                try:
                    df.to_excel(data_output, index=False)
                    print(f'Data exported to {data_output}')
                except Exception as e:
                    print(f'Failed to write Excel: {e}')
            else:
                try:
                    df.to_csv(data_output, index=False)
                    print(f'Data exported to {data_output}')
                except Exception as e:
                    print(f'Failed to write CSV: {e}')
        except Exception:
            # pandas not available — fallback to CSV via builtin csv
            import csv

            try:
                with open(data_output, 'w', newline='', encoding='utf-8') as f:
                    writer = csv.writer(f)
                    writer.writerow(['size', 'greedy_time', 'greedy_delay', 'optimal_time', 'optimal_delay'])
                    for s, g, gd, o, od in zip(x_sizes, greedy_times, greedy_delays, optimal_times, optimal_delays):
                        writer.writerow([
                            s,
                            '' if g is None else f'{g:.6f}',
                            '' if gd is None else f'{gd}',
                            '' if o is None else f'{o:.6f}',
                            '' if od is None else f'{od}',
                        ])
                print(f'Data exported to {data_output} (CSV)')
            except Exception as e:
                print(f'Failed to write data file: {e}')

    plt.figure(figsize=(10, 6))
    plt.plot(x_g, y_g, marker='o', linestyle='-', color='black')
    plt.plot(x_o, y_o, marker='^', linestyle='--', color='dimgray')

    # labels near points
    for x, y in zip(x_g, y_g):
        plt.text(x, y, f'{y:.2f}', fontsize=8, ha='right', va='bottom', color='black')
    for x, y in zip(x_o, y_o):
        plt.text(x, y, f'{y:.2f}', fontsize=8, ha='left', va='top', color='dimgray')

    # smoothing
    xs_g_s, ys_g_s, gm = (None, None, '')
    xs_o_s, ys_o_s, om = (None, None, '')
    if len(x_g) >= 2:
        xs_g_s, ys_g_s, gm = smooth_curve(x_g, y_g)
        if xs_g_s is not None:
            plt.plot(xs_g_s, ys_g_s, linestyle='-', color='black', alpha=0.22)
    if len(x_o) >= 2:
        xs_o_s, ys_o_s, om = smooth_curve(x_o, y_o)
        if xs_o_s is not None:
            plt.plot(xs_o_s, ys_o_s, linestyle='--', color='dimgray', alpha=0.22)

    # polynomial eq in legend
    g_eq = safe_poly_eq(x_g, y_g)
    o_eq = safe_poly_eq(x_o, y_o)
    g_label = f"Greedy Algorithm {g_eq} [{gm}]" if (g_eq or gm) else 'Greedy Algorithm'
    o_label = f"Optimal Algorithm {o_eq} [{om}]" if (o_eq or om) else 'Optimal Algorithm'

    # replot points with labels to make legend show our constructed labels
    plt.plot(x_g, y_g, marker='o', linestyle='', color='black', label=g_label)
    plt.plot(x_o, y_o, marker='^', linestyle='', color='dimgray', label=o_label)

    plt.xlabel('Input Size (Number of Vessels)')
    plt.ylabel('Execution Time (seconds)')
    plt.title('Benchmarking Greedy vs Optimal Scheduling Algorithms')
    plt.legend()
    plt.grid(color='lightgrey', linestyle='-', linewidth=0.5, alpha=0.4)
    if x_sizes:
        plt.gca().xaxis.set_major_locator(ticker.MultipleLocator(1))
        plt.xticks(list(range(min(x_sizes), max(x_sizes) + 1, 1)))
    plt.tight_layout()
    if output_file:
        try:
            plt.savefig(output_file, dpi=150)
            print(f"Plot saved to {output_file}")
        except Exception as e:
            print(f"Failed to save plot: {e}")
    else:
        plt.show()


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Benchmark greedy vs optimal Prolog schedulers and plot results')
    parser.add_argument('--output', '-o', help='Path to save the plot (PNG). If omitted, the plot is shown interactively.')
    parser.add_argument('--data-output', '-d', help='Path to export benchmark data (xlsx or csv).')
    args = parser.parse_args()
    main(output_file=args.output, data_output=args.data_output)