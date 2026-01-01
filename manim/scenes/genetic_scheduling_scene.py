from manim import *
import math
import random
import numpy as np


class GeneticAlgorithm(Scene):
    def construct(self):
        # Title and introduction
        title = Text("Genetic Scheduling Algorithm").scale(1)
        title.move_to(UP * 3)
        self.play(Write(title))
        self.wait(0.5)
        
        description = Text(
            "Inspired by natural evolution, this algorithm evolves\n\n"
            "a population of solutions over multiple generations."
        ).scale(0.65)
        description.move_to(UP * 1.5)
        self.play(Write(description))
        self.wait(2)
        
        # Example vessels
        vessels = [
            ("A", 1, 10),
            ("B", 4, 8),
            ("C", 3, 6),
            ("D", 2, 4),
            ("E", 3, 12),
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
            "D": RED,
            "E": PURPLE,
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
        
        # Step 1: Initial Population
        pop_title = Text("Step 1: Generate Initial Population").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, pop_title))
        self.wait(0.5)
        
        def create_vessel_square(name, color):
            """Create a colored square with vessel letter inside."""
            square = Square(side_length=0.5, fill_color=color, fill_opacity=0.8, stroke_color=WHITE, stroke_width=2)
            letter = Text(name, color=WHITE).scale(0.5)
            letter.move_to(square.get_center())
            return VGroup(square, letter)
        
        def create_sequence(vessel_order, delay=None):
            """Create a sequence of vessel squares with optional delay label."""
            seq = VGroup()
            for name in vessel_order:
                vessel_sq = create_vessel_square(name, vessel_colors[name])
                seq.add(vessel_sq)
            seq.arrange(RIGHT, buff=0.15)
            
            if delay is not None:
                delay_label = Text(f"Delay: {delay:.1f}", color=YELLOW).scale(0.35)
                delay_label.next_to(seq, RIGHT, buff=0.3)
                return VGroup(seq, delay_label)
            return seq
        
        # Create initial population (4 random permutations with 5 vessels)
        population = [
            (["D", "C", "B", "A", "E"], 4.2),
            (["A", "B", "E", "C", "D"], 5.8),
            (["C", "A", "D", "B", "E"], 6.5),
            (["B", "D", "C", "E", "A"], 3.9),
        ]
        
        pop_sequences = VGroup()
        for seq, delay in population:
            seq_group = create_sequence(seq, delay)
            pop_sequences.add(seq_group)
        
        pop_sequences.arrange(DOWN, buff=0.4)
        pop_sequences.move_to(ORIGIN)
        
        self.play(LaggedStart(*[FadeIn(seq) for seq in pop_sequences], lag_ratio=0.2))
        self.wait(1)
        
        pop_text = Text("Random permutations evaluated for fitness").scale(0.5).next_to(pop_sequences, DOWN, buff=0.5)
        self.play(Write(pop_text))
        self.wait(2)
        
        self.play(FadeOut(pop_sequences), FadeOut(pop_text))
        
        # Step 2: Crossover
        cross_title = Text("Step 2: Crossover (Genetic Recombination)").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, cross_title))
        self.wait(0.5)
        
        # Show two parents
        parent1_seq = ["A", "B", "E", "C", "D"]
        parent2_seq = ["D", "C", "B", "A", "E"]
        
        parent1 = create_sequence(parent1_seq)
        parent1_label = Text("Parent 1", color=BLUE).scale(0.4).next_to(parent1, LEFT, buff=0.3)
        parent1_group = VGroup(parent1_label, parent1)
        parent1_group.move_to(UP * 1.5)
        
        parent2 = create_sequence(parent2_seq)
        parent2_label = Text("Parent 2", color=GREEN).scale(0.4).next_to(parent2, LEFT, buff=0.3)
        parent2_group = VGroup(parent2_label, parent2)
        parent2_group.move_to(UP * 0.5)
        
        self.play(FadeIn(parent1_group), FadeIn(parent2_group))
        self.wait(1)
        
        # Highlight crossover points (positions 1-3, i.e., extracting middle segment)
        crossover_text = Text("Two-point crossover: extract middle segment").scale(0.45).move_to(DOWN * 0.5)
        self.play(Write(crossover_text))
        self.wait(1)
        
        # Highlight the middle segment from parent 1 (vessels B, E, C at indices 1-3)
        p1_segment_highlight = SurroundingRectangle(
            VGroup(parent1[1], parent1[2], parent1[3]),
            color=YELLOW,
            buff=0.05,
            stroke_width=3
        )
        self.play(Create(p1_segment_highlight))
        self.wait(1)
        
        # Show crossover operation - create children
        child1_seq = ["D", "B", "E", "C", "A"]  # Middle segment from P1, order from P2
        child2_seq = ["A", "C", "B", "E", "D"]  # Different combination
        
        child1 = create_sequence(child1_seq)
        child1_label = Text("Child 1", color=PURPLE).scale(0.4).next_to(child1, LEFT, buff=0.3)
        child1_group = VGroup(child1_label, child1)
        child1_group.move_to(DOWN * 1.5)
        
        child2 = create_sequence(child2_seq)
        child2_label = Text("Child 2", color=PURPLE).scale(0.4).next_to(child2, LEFT, buff=0.3)
        child2_group = VGroup(child2_label, child2)
        child2_group.move_to(DOWN * 2.5)
        
        self.play(
            FadeIn(child1_group),
            FadeIn(child2_group),
            FadeOut(p1_segment_highlight)
        )
        self.wait(2)
        
        self.play(
            FadeOut(parent1_group),
            FadeOut(parent2_group),
            FadeOut(child1_group),
            FadeOut(child2_group),
            FadeOut(crossover_text)
        )
        
        # Step 3: Mutation
        mut_title = Text("Step 3: Mutation (Random Variation)").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, mut_title))
        self.wait(0.5)
        
        # Show a sequence to mutate
        before_seq = ["B", "A", "D", "E", "C"]
        before_group = create_sequence(before_seq)
        before_label = Text("Before", color=WHITE).scale(0.45).next_to(before_group, LEFT, buff=0.3)
        before_full = VGroup(before_label, before_group)
        before_full.move_to(UP * 0.8)
        
        self.play(FadeIn(before_full))
        self.wait(0.5)
        
        mut_text = Text("Mutation swaps two random positions").scale(0.45).move_to(ORIGIN)
        self.play(Write(mut_text))
        self.wait(1)
        
        # Highlight two positions to swap (positions 1 and 3: A and E)
        pos1_circle = Circle(radius=0.35, color=RED, stroke_width=3).move_to(before_group[1].get_center())
        pos2_circle = Circle(radius=0.35, color=RED, stroke_width=3).move_to(before_group[3].get_center())
        
        self.play(Create(pos1_circle), Create(pos2_circle))
        self.wait(1)
        
        # Show the swap animation
        # Create copies for animation
        vessel_1_copy = before_group[1].copy()
        vessel_3_copy = before_group[3].copy()
        
        self.play(
            vessel_1_copy.animate.move_to(before_group[3].get_center()),
            vessel_3_copy.animate.move_to(before_group[1].get_center()),
            run_time=1.5
        )
        self.wait(0.5)
        
        self.play(
            FadeOut(pos1_circle),
            FadeOut(pos2_circle),
            FadeOut(vessel_1_copy),
            FadeOut(vessel_3_copy)
        )
        
        # Show after mutation
        after_seq = ["B", "E", "D", "A", "C"]  # A and E swapped
        after_group = create_sequence(after_seq)
        after_label = Text("After", color=GREEN).scale(0.45).next_to(after_group, LEFT, buff=0.3)
        after_full = VGroup(after_label, after_group)
        after_full.move_to(DOWN * 0.8)
        
        self.play(FadeIn(after_full))
        self.wait(2)
        
        self.play(FadeOut(before_full), FadeOut(after_full), FadeOut(mut_text))
        
        # Step 4: Selection & Iteration
        sel_title = Text("Step 4: Selection & Iteration").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, sel_title))
        self.wait(0.5)
        
        # Show population with delays, highlight best ones
        gen_pop = [
            (["A", "B", "E", "C", "D"], 5.8),
            (["D", "C", "B", "A", "E"], 4.2),  # Best
            (["C", "A", "D", "B", "E"], 6.5),
            (["B", "D", "C", "E", "A"], 3.9),  # Second best
        ]
        
        gen_sequences = VGroup()
        for seq, delay in gen_pop:
            seq_group = create_sequence(seq, delay)
            gen_sequences.add(seq_group)
        
        gen_sequences.arrange(DOWN, buff=0.35)
        gen_sequences.move_to(UP * 0.5)
        
        self.play(LaggedStart(*[FadeIn(seq) for seq in gen_sequences], lag_ratio=0.15))
        self.wait(1)
        
        # Highlight best two (indices 1 and 3 have lowest delays)
        best1_box = SurroundingRectangle(gen_sequences[1], color=GREEN, buff=0.1, stroke_width=3)
        best2_box = SurroundingRectangle(gen_sequences[3], color=GREEN, buff=0.1, stroke_width=3)
        
        selection_text = Text("Best solutions survive, population evolves").scale(0.45).next_to(gen_sequences, DOWN, buff=0.5)
        self.play(
            Create(best1_box),
            Create(best2_box),
            Write(selection_text)
        )
        self.wait(1)
        
        # Show generation counter
        gen_counter = Text("Gen 1 → Gen 2 → Gen 3 → ... → Gen 15").scale(0.5).move_to(DOWN * 2)
        best_delay_text = Text("Best Delay: 6.5 → 4.8 → 3.5").scale(0.5).next_to(gen_counter, DOWN, buff=0.3)
        
        self.play(Write(gen_counter), Write(best_delay_text))
        self.wait(2)
        
        self.play(
            FadeOut(gen_sequences),
            FadeOut(best1_box),
            FadeOut(best2_box),
            FadeOut(selection_text),
            FadeOut(gen_counter),
            FadeOut(best_delay_text)
        )
        
        # Step 5: Final Result
        result_title = Text("Optimal Solution Found").scale(0.7).move_to(UP * 3)
        self.play(Transform(title, result_title))
        self.wait(0.5)
        
        final_seq = ["D", "C", "B", "A", "E"]
        final_group = create_sequence(final_seq, 3.5)
        final_group.move_to(UP * 0.5)
        
        final_box = SurroundingRectangle(final_group, color=GREEN, buff=0.2, stroke_width=4)
        
        gen_reached = Text("Generation: 15").scale(0.5).next_to(final_group, DOWN, buff=0.5)
        
        self.play(FadeIn(final_group), Create(final_box))
        self.play(Write(gen_reached))
        self.wait(2)
        
        self.play(FadeOut(final_group), FadeOut(final_box), FadeOut(gen_reached))
        
        # Step 6: Algorithm Characteristics
        summary_title = Text("Algorithm Characteristics").scale(0.8).move_to(UP * 3)
        self.play(Transform(title, summary_title))
        self.wait(0.5)
        
        summary = VGroup(
            Text("✓ Balances exploration and exploitation", color=GREEN).scale(0.52),
            Text("✓ Can escape local optima", color=GREEN).scale(0.52),
            Text("✓ Parallelizable population", color=GREEN).scale(0.52),
            Text("✗ Many parameters to tune", color=RED).scale(0.52),
            Text("✗ No optimality guarantee", color=RED).scale(0.52),
            Text("✗ Computational cost varies", color=RED).scale(0.52),
        )
        summary.arrange(DOWN, buff=0.35, aligned_edge=LEFT)
        summary.move_to(ORIGIN)
        
        self.play(LaggedStart(*[Write(line) for line in summary], lag_ratio=0.25))
        self.wait(3)
