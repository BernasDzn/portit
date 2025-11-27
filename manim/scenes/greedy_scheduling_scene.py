from manim import *
import math
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

def get_greedy_schedule(vessels):
    # Accept either list of dicts or list of tuples for backward compatibility.
    # Normalize into list of dicts with keys: name, arrival, due, unload, load, cranes
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
            # tuple form
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

    # Sort by due date
    sorted_vessels = sorted(norm, key=lambda x: x["due"])

    schedule = []
    end_prev = 0  # corresponds to EndPrevSeq in Prolog

    def crane_speed_from_field(cranes_field):
        # Accept list of numbers or list of (name, speed) tuples
        if cranes_field is None:
            return 0
        total = 0
        for c in cranes_field:
            if isinstance(c, (int, float)):
                total += c
            elif isinstance(c, (list, tuple)) and len(c) >= 2:
                # (name, speed) pair
                try:
                    total += float(c[1])
                except Exception:
                    pass
        return total

    for vessel in sorted_vessels:
        name = vessel["name"]
        arrival = vessel["arrival"]
        due = vessel["due"]
        unload_cnt = vessel.get("unload", 0)
        load_cnt = vessel.get("load", 0)
        cranes = vessel.get("cranes", [])

        # Compute crane speed sum
        crane_sum = crane_speed_from_field(cranes)
        if crane_sum <= 0:
            # avoid division by zero: treat as 1 (serial slow crane)
            crane_sum = 1

        # Compute times (may be fractional)
        t_unload = (unload_cnt / crane_sum) if unload_cnt and crane_sum > 0 else 0
        t_load = (load_cnt / crane_sum) if load_cnt and crane_sum > 0 else 0

        # Decide start time following Prolog logic
        if arrival > end_prev:
            t_in_unload = arrival
        else:
            t_in_unload = end_prev + 1

        # Finish time uses Prolog formula: TEndLoad = TInUnload + TUnload + TLoad - 1
        t_end_load = t_in_unload + t_unload + t_load - 1

        # Append schedule entry
        schedule.append((name, t_in_unload, t_end_load))

        # Update end_prev for next vessel
        end_prev = t_end_load

    # Calculate total delay as sum of waiting times before each vessel started
    total_delay = 0
    for name, start, finish in schedule:
        # find normalized original to get arrival time
        original = next(v for v in norm if v["name"] == name)
        arrival = original.get("arrival", 0)
        wait = max(0, start - arrival)
        total_delay += wait

    return schedule, total_delay


class GreedyAlgorithm(GanttUtils, Scene):
    def construct(self):
        title = Text("Greedy Scheduling Algorithm").scale(1)
        title.move_to(UP * 3)
        self.play(Write(title))
        self.wait(0.5)
        
        description = Text("The greedy algorithm we implemented has\n"
                   "the earliest due date as its priority.").scale(0.75)
        description.move_to(UP * 1.5)
        self.play(Write(description))
        self.wait(1)
        # Example vessels with (name, arrival, due, unload_cnt, load_cnt, cranes)
        # `cranes` is a list of (name, speed) pairs or numeric speeds
        vessels = [
            ("Vessel A", 1, 10, 5, 2, [("Crane1", 2.0)]),
            ("Vessel B", 4, 8, 3, 1, [("Crane1", 1.0)]),
            ("Vessel C", 3, 6, 4, 0, [("Crane1", 2.0)]),
            ("Vessel D", 2, 4, 2, 0, [("Crane1", 1.0)]),
            ("Vessel E", 3, 12, 6, 2, [("Crane1", 2.0)]),
        ]
        
        self.play(FadeOut(description))
        
        vessel_texts = VGroup()
        for v in vessels:
            # support both short tuples (name, arrival, due)
            # and full tuples (name, arrival, due, unload_cnt, load_cnt, cranes)
            name = v[0]
            arrival = v[1]
            due = v[2]
            if len(v) >= 6:
                unload_cnt = v[3]
                load_cnt = v[4]
                cranes = v[5]
                # represent cranes as speeds for display
                try:
                    crane_speeds = ",".join(str(c[1]) if isinstance(c, (list, tuple)) else str(c) for c in cranes)
                except Exception:
                    crane_speeds = str(cranes)
                vessel_text = Text(f"{name}: Arrival={arrival}, Due={due}, Unl={unload_cnt}, Load={load_cnt}, Cranes={crane_speeds}").scale(0.5)
            else:
                vessel_text = Text(f"{name}: Arrival = {arrival}, Due = {due}").scale(9.5)
            vessel_texts.add(vessel_text)
        vessel_texts.arrange(DOWN)
        self.play(Write(vessel_texts))
        self.wait(3)
        
        self.play(FadeOut(vessel_texts))
        
        schedule, delay = get_greedy_schedule(vessels)

        # Convert schedule to data for Gantt utils: split waiting and service segments per vessel
        data = []
        name_to_arrival = {v[0]: v[1] for v in vessels}
        for (name, start, finish) in schedule:
            arrival = name_to_arrival.get(name, start)
            wait = max(0, start - arrival)
            if wait > 0:
                data.append({"Task": name, "Resource": "wait", "Start": arrival, "Finish": start})
            # service segment finishes at finish+1
            data.append({"Task": name, "Resource": "service", "Start": start, "Finish": finish + 1})

        color_map = {"wait": RED, "service": GREEN}

        # Build chart components
        comps = self.gantt_chart_without_ticks(width=11, height=4, data=data, x_range=None, color_map=color_map)
        axes = comps["axes"]
        ticks = comps.get("ticks")
        tasks_group = comps["tasks"]  # list of (d, rect)
        name_labels = comps["name_labels"]

        # Auto-scale entire chart group if necessary
        max_chart_width = 12
        chart_width = comps.get("x_range", 0) * axes.x_axis.unit_size
        if chart_width > max_chart_width:
            scale_factor = max_chart_width / chart_width
            axes.scale(scale_factor)
            if ticks is not None:
                ticks.scale(scale_factor)
            for _, rect in tasks_group:
                rect.scale(scale_factor)
            for lbl in name_labels:
                lbl.scale(scale_factor)

        # Animate axis and ticks first, then names, then events in time order
        self.play(Create(axes))
        if ticks is not None:
            self.play(Create(ticks))
        self.play(LaggedStart(*[Write(lbl) for lbl in name_labels], lag_ratio=0.12))

        # Map data entry to rect object and animate by Start time
        rect_map = {id(d): rect for (d, rect) in tasks_group}
        events_sorted = sorted([d for d, _ in tasks_group], key=lambda x: x["Start"])

        # Add all rectangles and static dashed lines instantly (no per-rect transition)
        all_rects = []
        for d in events_sorted:
            rect = rect_map[id(d)]
            # add only the rectangles instantly; defer dashed lines until after reveal
            self.add(rect)
            all_rects.append(rect)

        # Build a black cover rectangle sitting on top of all bars, then slide it right to reveal
        if all_rects:
            left_x = axes.c2p(0, 0)[0] - 0.02
            # prefer x_range from comps when available
            right_bound = comps.get("x_range", None)
            if right_bound is None:
                right_bound = max(d["Finish"] for d in events_sorted)
            right_x = axes.c2p(right_bound, 0)[0] + 0.02

            cover_w = right_x - left_x
            min_y = min(r.get_bottom()[1] for r in all_rects) - 0.06
            max_y = max(r.get_top()[1] for r in all_rects) + 0.06
            cover_h = max_y - min_y
            center_x = left_x + cover_w / 2.0
            center_y = (min_y + max_y) / 2.0

            black_cover = Rectangle(width=cover_w, height=cover_h, fill_color=BLACK, fill_opacity=1, stroke_opacity=0)
            black_cover.move_to(np.array([center_x, center_y, 0]))
            # ensure it's on top of the data, but axes/labels should remain visible above it
            self.add(black_cover)
            # re-add axes, ticks and name labels to bring them above the cover
            if ticks is not None:
                self.add(axes, ticks, *name_labels)
            else:
                self.add(axes, *name_labels)

            # animate the cover sliding right to reveal everything underneath (slower)
            slide_dist = cover_w + 0.5
            slide_run_time = max(1.2, min(3.5, 0.14 * cover_w))
            self.play(black_cover.animate.shift(RIGHT * slide_dist), run_time=slide_run_time)
            # remove the cover
            self.play(FadeOut(black_cover))

            # Now show the dashed lines (after the reveal) with a small stagger
            dash_objs = []
            for d in events_sorted:
                rect = rect_map[id(d)]
                start_x = axes.c2p(d["Start"], 0)[0]
                finish_x = axes.c2p(d["Finish"], 0)[0]
                axis_y = axes.c2p(0, 0)[1]
                top_y = rect.get_top()[1]
                dash_start = DashedLine(start=np.array([start_x, top_y, 0]), end=np.array([start_x, axis_y, 0]), dash_length=0.03, color=GREY, stroke_width=1, stroke_opacity=0.6)
                dash_end = DashedLine(start=np.array([finish_x, top_y, 0]), end=np.array([finish_x, axis_y, 0]), dash_length=0.03, color=GREY, stroke_width=1, stroke_opacity=0.6)
                dash_objs.extend([dash_start, dash_end])

            if dash_objs:
                # create them with a short lag so they appear as dotted-line annotations
                dash_run_time = max(0.6, min(2.0, 0.05 * cover_w))
                self.play(LaggedStart(*[Create(d) for d in dash_objs], lag_ratio=0.06), run_time=dash_run_time)

        delay_text = Text(f"Total Delay: {delay}").scale(0.45).next_to(axes, DOWN, buff=0.5)
        self.play(Write(delay_text))
        self.wait(1)

    def build_gantt_chart(self, schedule, vessels=None, unit_width=0.7, bar_height=0.45, top=1.5, left_padding=1.5):
        """Build Gantt components and return a dict with axis, separator, name_labels and events.

        The returned dict has keys: 'axis', 'separator', 'name_labels', 'events', 'axis_y'.
        Each event is a dict with keys: name, arrival, start, finish, wait_rect (or None),
        wait_dash_start, serv_rect, serv_dash_start, serv_dash_end.
        """

        if not schedule:
            return {"axis": VGroup()}

        # compute time range (we force axis to start at 0 per request)
        starts = [s for (_, s, _) in schedule]
        finishes = [f for (_, _, f) in schedule]
        time_min = 0
        time_max = int(math.ceil(max(f + 1 for f in finishes)))
        time_span = max(1, time_max - time_min)

        # layout parameters
        chart_width = time_span * unit_width
        left_anchor = LEFT * (chart_width / 2)  # this is the axis start coordinate

        # Gather arrival map if vessels provided
        arrival_map = {}
        if vessels is not None:
            for v in vessels:
                if isinstance(v, dict):
                    name = v.get("name")
                    arrival_map[name] = v.get("arrival")
                else:
                    name = v[0]
                    arrival_map[name] = v[1]

        # Axis and ticks: place axis below bars to avoid overlap
        n = len(schedule)
        v_gap = max(0.25, bar_height * 0.8)
        total_height = n * (bar_height + v_gap)
        axis_y = top - (total_height + 0.6)
        axis_start = left_anchor + UP * axis_y
        axis_end = left_anchor + RIGHT * chart_width + UP * axis_y
        axis = Line(axis_start, axis_end, stroke_color=WHITE)

        ticks = VGroup()
        for t in range(time_min, time_max + 1):
            x = left_anchor + RIGHT * ((t - time_min) * unit_width)
            tick = Line(x + UP * axis_y, x + UP * (axis_y - 0.12), stroke_color=WHITE)
            label = Text(str(t)).scale(0.28).next_to(tick, DOWN, buff=0.03)
            ticks.add(tick, label)

        # Bars and event objects (do NOT add rects to scene here; return them for animation)
        start_y = top
        name_labels = []
        events = []
        for idx, (name, start, finish) in enumerate(schedule):
            y = start_y - idx * (bar_height + v_gap)
            duration = (finish - start) + 1

            # positions
            arrival = arrival_map.get(name, start)
            arrival_x = left_anchor + RIGHT * ((arrival - time_min) * unit_width)

            # left label aligned to axis start (names aligned to axis' start)
            name_label = Text(name).scale(0.35).next_to(left_anchor + UP * y, LEFT, buff=0.4)
            name_labels.append(name_label)

            # waiting and service rects (constructed but not added to chart)
            wait_duration = max(0, start - arrival)
            service_duration = duration

            wait_rect = None
            wait_dash_start = None
            if wait_duration > 0:
                wait_w = wait_duration * unit_width
                wait_rect = Rectangle(width=wait_w, height=bar_height, fill_color=RED, fill_opacity=0.85, stroke_color=BLACK)
                wait_rect.move_to(arrival_x + RIGHT * (wait_w / 2) + UP * y)
                start_x = left_anchor + RIGHT * ((start - time_min) * unit_width)
                wait_dash_start = DashedLine(start=UP * (y + bar_height / 2) + start_x, end=UP * axis_y + start_x, dash_length=0.03, color=GREY, stroke_width=1, stroke_opacity=0.6)

            serv_w = service_duration * unit_width
            serv_left_x = left_anchor + RIGHT * ((start - time_min) * unit_width)
            serv_rect = Rectangle(width=serv_w, height=bar_height, fill_color=GREEN, fill_opacity=0.75, stroke_color=BLACK)
            serv_rect.move_to(serv_left_x + RIGHT * (serv_w / 2) + UP * y)
            serv_dash_start = DashedLine(start=UP * (y + bar_height / 2) + serv_left_x, end=UP * axis_y + serv_left_x, dash_length=0.03, color=GREY, stroke_width=1, stroke_opacity=0.6)
            serv_right_x = serv_left_x + RIGHT * serv_w
            serv_dash_end = DashedLine(start=UP * (y + bar_height / 2) + serv_right_x, end=UP * axis_y + serv_right_x, dash_length=0.03, color=GREY, stroke_width=1, stroke_opacity=0.6)

            events.append({
                "name": name,
                "arrival": arrival,
                "start": start,
                "finish": finish,
                "wait_rect": wait_rect,
                "wait_dash_start": wait_dash_start,
                "serv_rect": serv_rect,
                "serv_dash_start": serv_dash_start,
                "serv_dash_end": serv_dash_end,
            })

        # separator vertical line between names and chart (slightly left of axis start)
        separator = Line(left_anchor + LEFT * 0.4 + UP * (top + 0.3), left_anchor + LEFT * 0.4 + DOWN * (total_height + 0.6), stroke_color=WHITE)

        # axis group includes axis and ticks
        axis_group = VGroup(axis, ticks)

        return {
            "axis": axis_group,
            "separator": separator,
            "name_labels": name_labels,
            "events": events,
            "axis_y": axis_y,
            "chart_width": chart_width,
        }