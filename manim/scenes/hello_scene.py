from manim import Scene, Text, Write


class HelloScene(Scene):
    def construct(self):
        text = Text("Hello, Manim!").scale(1.5)
        self.play(Write(text))
        self.wait(2)
