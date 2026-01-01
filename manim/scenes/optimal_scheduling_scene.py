from manim import *
import math
import itertools
import numpy as np


class GanttUtils:
    """Utility methods to build a Gantt chart using Manim's Axes.

    This is a compact, self-contained implementation inspired by the reference
    provided by the user. The method `gantt_chart_without_ticks` returns a
    dictionary with components you can animate separately: `axes`, `ticks`,
    `tasks` (list of Rectangles), and `name_labels`.
    """

    def gantt_chart_without_ticks(self, width: float, height: float, data: list[dict], x_range: float = None,
                                  y_range=None, color_map: dict = None, resource_naming="Machine",
                                  n_machines: int = None, n_jobs: int = None, axis_config_kwargs=None) -> dict:

        if axis_config_kwargs is None:
            axis_config_kwargs = {}

        # calc y_range if not given
        unique_tasks = list(dict.fromkeys([d["Task"] for d in data])) if data else []
        jobs = len(unique_tasks)
        if n_jobs is not None:
            jobs = max(jobs, n_jobs)

        if y_range is None:
            y_range = jobs + 2

        # calc x_range if not given
        if x_range is None:
            if len(data):
                x_range = max(d["Finish"] for d in data) + 1
            else:
                x_range = 1

        # color map: if not provided, produce simple mapping for resources
        if color_map is None:
            palette = [BLUE, GREEN, YELLOW, RED, ORANGE, PURPLE]
            resources = list(dict.fromkeys([d["Resource"] for d in data])) if data else []
            color_map = {r: palette[i % len(palette)] for i, r in enumerate(resources)}

        # Build axes
        # Do not rely on the Axes number rendering (DecimalNumber/TeX). Instead
        # disable built-in numbers and draw tick labels with simple `Text` objects
        # which do not require a TeX toolchain.
        axes = Axes(
            x_range=[0, x_range, 1],
            y_range=[0, y_range, 1],
            x_length=width,
            y_length=height,
            axis_config={"include_numbers": False, "tip_width": 0.0, "tip_height": 0.0, **axis_config_kwargs},
        )

        # Prepare per-task y positions: top-down
        task_to_y = {}
        for idx, task in enumerate(unique_tasks):
            # place tasks starting below top (padding)
            task_to_y[task] = jobs - idx  # integer y coordinates

        tasks_group = []
        name_labels = []

        # unit sizes
        x_unit = axes.x_axis.unit_size
        y_unit = axes.y_axis.unit_size

        for d in data:
            task = d["Task"]
            start = d["Start"]
            finish = d["Finish"]
            resource = d.get("Resource")
            color = color_map.get(resource, BLUE)

            width_units = finish - start
            rect = Rectangle(width=width_units * x_unit, height=0.8 * y_unit, fill_color=color, fill_opacity=1, stroke_color=BLACK)
            center_x = start + width_units / 2
            center_pos = axes.c2p(center_x, task_to_y[task])
            rect.move_to(center_pos)
            tasks_group.append((d, rect))

        # Create name labels aligned to axis start (x=0)
        for task in unique_tasks:
            y = task_to_y[task]
            label = Text(task).scale(0.35).next_to(axes.c2p(0, y), LEFT, buff=0.4)
            name_labels.append(label)

        # Build ticks group (numeric labels on axis) using Text (no TeX)
        ticks = VGroup()
        for t in range(0, int(x_range) + 1):
            x = axes.c2p(t, 0)[0]
            # slightly lower tick and longer downward label offset for readability
            tick = Line(np.array([x, axes.c2p(0, 0)[1] + 0.05, 0]), np.array([x, axes.c2p(0, 0)[1] - 0.15, 0]), stroke_color=WHITE)
            label = Text(str(t)).scale(0.34).next_to(tick, DOWN, buff=0.08)
            ticks.add(tick, label)

        return {
            "axes": axes,
            "ticks": ticks,
            "tasks": tasks_group,
            "name_labels": name_labels,
            "x_range": x_range,
            "y_range": y_range,
        }


def get_optimal_schedule(vessels):
    """Compute optimal schedule by testing all permutations and finding minimum delay."""
    # Normalize vessel format
    norm = []
    for v in vessels:
        if isinstance(v, dict):
            norm.append({
                "name": v.get("name"),
                "arrival": v.get("arrival"),
                "due": v.get("due"),
                "unload": v.get("unload", 0),
                "load": v.get("load", 0),
                "cranes": v.get("cranes", [])
            })
        else:
            if len(v) >= 6:
                name, arrival, due, unload_cnt, load_cnt, cranes = v[:6]
                norm.append({
                    "name": name,
                    "arrival": arrival,
                    "due": due,
                    "unload": unload_cnt,
                    "load": load_cnt,
                    "cranes": cranes,
                })
            else:
                name, arrival, due = v[0], v[1], v[2]
                norm.append({"name": name, "arrival": arrival, "due": due, "unload": 1, "load": 0, "cranes": [1]})

    def crane_speed_sum(cranes_field):
        if cranes_field is None:
            return 0
        total = 0
        for c in cranes_field:
            if isinstance(c, (int, float)):
                total += c
            elif isinstance(c, (list, tuple)) and len(c) >= 2:
                try:
                    total += float(c[1])
                except Exception:
                    pass
        return total

    def schedule_sequence(vessel_order):
        """Schedule a specific permutation and calculate its delay."""
        schedule = []
        end_prev = 0
        
        for vessel in vessel_order:
            name = vessel["name"]
            arrival = vessel["arrival"]
            due = vessel["due"]
            unload_cnt = vessel.get("unload", 0)
            load_cnt = vessel.get("load", 0)
            cranes = vessel.get("cranes", [])
            
            crane_sum = crane_speed_sum(cranes)
            if crane_sum <= 0:
                crane_sum = 1
            
            t_unload = (unload_cnt / crane_sum) if unload_cnt and crane_sum > 0 else 0
            t_load = (load_cnt / crane_sum) if load_cnt and crane_sum > 0 else 0
            
            if arrival > end_prev:
                t_in_unload = arrival
            else:
                t_in_unload = end_prev + 1
            
            t_end_load = t_in_unload + t_unload + t_load - 1
            schedule.append((name, t_in_unload, t_end_load))
            end_prev = t_end_load
        
        # Calculate delay
        total_delay = 0
        for name, start, finish in schedule:
            original = next(v for v in vessel_order if v["name"] == name)
            due = original.get("due", 0)
            delay = max(0, (finish + 1) - due)
            total_delay += delay
        
        return schedule, total_delay

    # Generate all permutations and find the best
    best_schedule = None
    best_delay = float('inf')
    all_results = []
    
    for perm in itertools.permutations(norm):
        schedule, delay = schedule_sequence(list(perm))
        all_results.append((schedule, delay, [v["name"] for v in perm]))
        if delay < best_delay:
            best_delay = delay
            best_schedule = schedule
    
    return best_schedule, best_delay, all_results


class OptimalAlgorithm(GanttUtils, Scene):
    def construct(self):
        # Title and introduction
        title = Text("Optimal Scheduling Algorithm").scale(1)
        title.move_to(UP * 3)
        self.play(Write(title))
        self.wait(0.5)
        
        description = Text(
            "The optimal algorithm tests ALL possible orderings\n\n"
            "to find the sequence with minimum delay."
        ).scale(0.7)
        description.move_to(UP * 1.5)
        self.play(Write(description))
        self.wait(2)
        
        # Example vessels (smaller set for visualization)
        vessels = [
            ("A", 1, 8, 3, 1, [("Crane1", 2.0)]),
            ("B", 2, 6, 2, 1, [("Crane1", 1.0)]),
            ("C", 3, 10, 4, 0, [("Crane1", 2.0)]),
        ]
        
        self.play(FadeOut(description))
        
        # Show all vessels with their arrival and due times
        vessel_info_title = Text("Vessels to Schedule").scale(0.75).move_to(UP * 2)
        self.play(Write(vessel_info_title))
        self.wait(0.5)
        
        vessel_colors = {
            "A": BLUE,
            "B": GREEN,
            "C": ORANGE,
        }
        
        vessel_info_texts = VGroup()
        for v in vessels:
            name, arrival, due = v[0], v[1], v[2]
            color = vessel_colors.get(name, WHITE)
            
            # Create colored square indicator
            square = Square(side_length=0.3, fill_color=color, fill_opacity=0.8, stroke_color=WHITE, stroke_width=2)
            letter = Text(name, color=WHITE).scale(0.4)
            letter.move_to(square.get_center())
            vessel_icon = VGroup(square, letter)
            
            # Create info text
            info_text = Text(f"  Arrival: {arrival}  |  Due: {due}", color=WHITE).scale(0.45)
            
            # Combine icon and text
            vessel_line = VGroup(vessel_icon, info_text)
            vessel_line.arrange(RIGHT, buff=0.2)
            vessel_info_texts.add(vessel_line)
        
        vessel_info_texts.arrange(DOWN, buff=0.4, aligned_edge=LEFT)
        vessel_info_texts.move_to(ORIGIN)
        
        self.play(LaggedStart(*[FadeIn(vit) for vit in vessel_info_texts], lag_ratio=0.2))
        self.wait(2)
        
        self.play(FadeOut(vessel_info_texts), FadeOut(vessel_info_title))
        
        permutation_title = Text("Step 1: Generate All Permutations").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, permutation_title))
        self.wait(0.5)
        
        # Create colored vessel squares
        vessel_names = [v[0] for v in vessels]
        vessel_colors = {
            "A": BLUE,
            "B": GREEN,
            "C": ORANGE,
        }
        
        def create_vessel_square(name, color):
            """Create a colored square with vessel letter inside."""
            square = Square(side_length=0.6, fill_color=color, fill_opacity=0.8, stroke_color=WHITE, stroke_width=2)
            letter = Text(name, color=WHITE).scale(0.7)
            letter.move_to(square.get_center())
            return VGroup(square, letter)
        
        # Create initial vessel squares
        initial_vessels = VGroup(*[create_vessel_square(name, vessel_colors[name]) for name in vessel_names]).scale(1.5)
        initial_vessels.arrange(RIGHT, buff=0.5)
        initial_vessels.move_to(ORIGIN)
        
        self.play(LaggedStart(*[FadeIn(v) for v in initial_vessels], lag_ratio=0.2))
        self.wait(1)
        
        # Animate a few permutation transitions to show the process
        sample_perms = list(itertools.permutations(vessel_names))
        for perm in sample_perms[1:]:
            new_vessels = VGroup(*[create_vessel_square(name, vessel_colors[name]) for name in perm]).scale(1.5)
            new_vessels.arrange(RIGHT, buff=0.5)
            new_vessels.move_to(ORIGIN)
            self.play(Transform(initial_vessels, new_vessels), run_time=0.6)
            self.wait(0.3)
        
        self.wait(0.5)
        
        # Pan out and show all permutations
        self.wait(0.5)
        self.play(FadeOut(initial_vessels))
        
        # Create all permutation grids
        all_perm_groups = []
        for perm in sample_perms:
            perm_group = VGroup(*[create_vessel_square(name, vessel_colors[name]) for name in perm])
            perm_group.arrange(RIGHT, buff=0.2)
            perm_group.scale(1.0)
            all_perm_groups.append(perm_group)
        
        perm_grid = VGroup(*all_perm_groups)
        perm_grid.arrange_in_grid(rows=2, cols=3, buff=(1.2, 0.8))
        perm_grid.move_to(UP * 0.5)
        
        # Animate all permutations appearing
        self.play(LaggedStart(*[FadeIn(pg) for pg in all_perm_groups], lag_ratio=0.1), run_time=2)
        self.wait(1)
        
        # Compute delays for each permutation
        eval_title = Text("Step 2: Evaluate Each Sequence").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, eval_title))
        self.wait(0.5)
        
        schedule, delay, all_results = get_optimal_schedule(vessels)
        
        # Create delay labels below each permutation
        delay_labels = []
        best_idx = -1
        min_delay = float('inf')
        
        for i, (sched, del_val, perm_names) in enumerate(all_results):
            delay_label = Text(f"Delay: {del_val:.1f}", color=YELLOW).scale(0.5)
            delay_label.next_to(all_perm_groups[i], DOWN, buff=0.15)
            delay_labels.append(delay_label)
            
            if del_val < min_delay:
                min_delay = del_val
                best_idx = i
        
        # Animate delays appearing
        self.play(LaggedStart(*[Write(dl) for dl in delay_labels], lag_ratio=0.08), run_time=1.5)
        self.wait(1)
        
        # Highlight the best solution
        best_title = Text("Step 3: Select Minimum Delay").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, best_title))
        self.wait(0.5)
        
        if best_idx >= 0:
            best_perm = all_perm_groups[best_idx]
            best_delay = delay_labels[best_idx]
            
            # Create highlight box around best solution
            highlight_box = SurroundingRectangle(
                VGroup(best_perm, best_delay),
                color=GREEN,
                buff=0.2,
                stroke_width=4
            )
            
            # Change best delay color to green
            self.play(
                Create(highlight_box),
                best_delay.animate.set_color(GREEN),
                run_time=1
            )
            self.wait(1.5)
            
            # Fade out everything except we'll keep the screen clear
            self.play(
                FadeOut(perm_grid),
                FadeOut(VGroup(*delay_labels)),
                FadeOut(highlight_box)
            )
        
        # Show summary
        summary_title = Text("Algorithm Characteristics").scale(0.8).move_to(UP * 3)
        self.play(Transform(title, summary_title))
        self.wait(0.5)
        
        summary = VGroup(
            Text("✓ Tests ALL possible orderings", color=GREEN).scale(0.55),
            Text("✓ Guarantees optimal solution", color=GREEN).scale(0.55),
            Text("✗ Exponential time: O(n! * n)", color=RED).scale(0.55),
            Text("✗ Impractical for large n", color=RED).scale(0.55),
        )
        summary.arrange(DOWN, buff=0.4, aligned_edge=LEFT)
        summary.move_to(ORIGIN)
        
        self.play(LaggedStart(*[Write(line) for line in summary], lag_ratio=0.3))
        self.wait(3)


class OptimalVsGreedy(GanttUtils, Scene):
    """Comparison scene showing optimal vs greedy side by side."""
    
    def construct(self):
        title = Text("Optimal vs Greedy Comparison").scale(0.9)
        title.move_to(UP * 3.5)
        self.play(Write(title))
        self.wait(1)
        
        # Use same vessels for both
        vessels = [
            ("A", 1, 8, 3, 1, [("Crane1", 2.0)]),
            ("B", 2, 6, 2, 1, [("Crane1", 1.0)]),
            ("C", 3, 10, 4, 0, [("Crane1", 2.0)]),
        ]
        
        comparison = VGroup(
            Text("Greedy: Sort by due date", color=YELLOW).scale(0.55),
            Text("Optimal: Test all orderings", color=BLUE).scale(0.55),
        )
        comparison.arrange(DOWN, buff=0.3)
        comparison.move_to(UP * 2)
        self.play(Write(comparison))
        self.wait(2)
        
        self.play(FadeOut(comparison))
        
        # Show final message
        conclusion = Text(
            "Optimal guarantees best solution\n"
            "but at exponential computational cost"
        ).scale(0.65)
        conclusion.move_to(ORIGIN)
        self.play(Write(conclusion))
        self.wait(3)
