"""Run greedy scheduler on the full `test_facts.pl` and export results.

Produces an Excel file (or CSV fallback) with: number of vessels, processing
time (seconds), total_delay (as reported by the Prolog predicate), status,
stdout and stderr for debugging, and a timestamp.

Usage:
  python run_greedy_full.py --output greedy_results.xlsx

Options:
  --facts    Path to facts file (default: test_facts.pl)
  --greedy   Path to greedy Prolog file (default: algorithms/greedy_scheduling.pl)
  --timeout  Timeout in seconds for the Prolog run (default: 60)
  --output   Output path (xlsx or csv). Default: greedy_full_results.xlsx
"""

import argparse
import subprocess
import time
import datetime
import os
from typing import Optional, Tuple


def count_vessels(facts_path: str) -> int:
    cnt = 0
    with open(facts_path, 'r', encoding='utf-8') as f:
        for line in f:
            if line.strip().startswith('vessel('):
                cnt += 1
    return cnt


def write_temp_facts(n: int, source_facts: str, temp_path: str) -> None:
    """Write first n vessel facts from source_facts into temp_path."""
    written = 0
    with open(source_facts, 'r', encoding='utf-8') as src, open(temp_path, 'w', encoding='utf-8') as dst:
        for line in src:
            if line.strip().startswith('vessel('):
                dst.write(line)
                written += 1
                if written >= n:
                    break


def run_greedy_temp(temp_facts_path: str, greedy_file: str, timeout: int) -> Tuple[Optional[float], Optional[float], str, str]:
    """Run greedy using the given temporary facts file and return (elapsed, delay, stdout, stderr)."""
    goal = "once((obtain_seq_greedy(_, Delay), format('DELAY:~w\\n',[Delay])))."
    cmd = ['swipl', '-q', '-l', temp_facts_path, '-l', greedy_file, '-g', goal, '-t', 'halt']
    start = time.perf_counter()
    try:
        proc = subprocess.run(cmd, timeout=timeout, capture_output=True)
        end = time.perf_counter()
        elapsed = end - start
        stdout = proc.stdout.decode().strip()
        stderr = proc.stderr.decode().strip()
        # parse DELAY:
        delay_val = None
        for line in stdout.splitlines():
            if line.startswith('DELAY:'):
                raw = line.split('DELAY:', 1)[1].strip()
                try:
                    delay_val = int(raw)
                except Exception:
                    try:
                        delay_val = float(raw)
                    except Exception:
                        delay_val = None
                break
        return elapsed, delay_val, stdout, stderr
    except subprocess.TimeoutExpired:
        return None, None, '', 'TIMEOUT'
    except Exception as e:
        return None, None, '', str(e)


def save_results(out_path: str, num_vessels: int, elapsed: Optional[float], delay: Optional[float], stdout: str, stderr: str):
    row = {
        'timestamp': datetime.datetime.utcnow().isoformat(),
        'num_vessels': num_vessels,
        'processing_time_s': None if elapsed is None else float(elapsed),
        'total_delay': None if delay is None else delay,
        'stdout': stdout,
        'stderr': stderr,
    }
    try:
        import pandas as pd

        df = pd.DataFrame([row])
        lower = out_path.lower()
        if lower.endswith('.xlsx'):
            df.to_excel(out_path, index=False)
        else:
            df.to_csv(out_path, index=False)
        print(f'Results written to {out_path}')
    except Exception:
        # fallback to csv using builtin csv
        import csv
        try:
            csv_path = out_path if out_path.lower().endswith('.csv') else os.path.splitext(out_path)[0] + '.csv'
            with open(csv_path, 'w', newline='', encoding='utf-8') as f:
                writer = csv.writer(f)
                writer.writerow(list(row.keys()))
                writer.writerow([row[k] if row[k] is not None else '' for k in row.keys()])
            print(f'Results written to {csv_path} (CSV fallback)')
        except Exception as e:
            print('Failed to write results:', e)


def main():
    parser = argparse.ArgumentParser(description='Run greedy scheduler on full facts and export delay/time to XLSX')
    parser.add_argument('--facts', default='test_facts.pl', help='Path to facts file')
    parser.add_argument('--greedy', default='algorithms/greedy_scheduling.pl', help='Path to greedy Prolog file')
    parser.add_argument('--timeout', type=int, default=60, help='Timeout seconds for Prolog')
    parser.add_argument('--output', '-o', default='greedy_full_results.xlsx', help='Output XLSX/CSV path')
    args = parser.parse_args()

    if not os.path.exists(args.facts):
        print('Facts file not found:', args.facts)
        return
    if not os.path.exists(args.greedy):
        print('Greedy Prolog file not found:', args.greedy)
        return

    num = count_vessels(args.facts)
    print(f'Found {num} vessel facts in {args.facts}. Running greedy incrementally 1..{num}...')

    rows = []
    for size in range(1, num + 1):
        temp_path = f'temp_facts_{size}.pl'
        write_temp_facts(size, args.facts, temp_path)
        print(f'Running size={size} (temp file: {temp_path})')
        elapsed, delay, stdout, stderr = run_greedy_temp(temp_path, args.greedy, args.timeout)
        print(f'  time={elapsed}, delay={delay}, stderr={stderr or None}')
        rows.append({
            'timestamp': datetime.datetime.utcnow().isoformat(),
            'num_vessels': size,
            'processing_time_s': None if elapsed is None else float(elapsed),
            'total_delay': None if delay is None else delay,
            'stdout': stdout,
            'stderr': stderr,
        })
        try:
            os.remove(temp_path)
        except Exception:
            pass

    # write all rows at once
    try:
        import pandas as pd

        df = pd.DataFrame(rows)
        lower = args.output.lower()
        if lower.endswith('.xlsx'):
            df.to_excel(args.output, index=False)
        else:
            df.to_csv(args.output, index=False)
        print(f'Results written to {args.output}')
    except Exception:
        import csv
        try:
            csv_path = args.output if args.output.lower().endswith('.csv') else os.path.splitext(args.output)[0] + '.csv'
            with open(csv_path, 'w', newline='', encoding='utf-8') as f:
                writer = csv.writer(f)
                writer.writerow(list(rows[0].keys()))
                for r in rows:
                    writer.writerow([r[k] if r[k] is not None else '' for k in r.keys()])
            print(f'Results written to {csv_path} (CSV fallback)')
        except Exception as e:
            print('Failed to write results:', e)


if __name__ == '__main__':
    main()
