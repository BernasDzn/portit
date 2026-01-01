from manim import Scene, Text, Write, Unwrite


class ByeScene(Scene):
    def construct(self):
        text = Text("1221402\n\n1231090\n\n1231092\n\n1231402").scale(1)
        self.play(Write(text))
        self.wait(2)
        self.play(Unwrite(text))
        self.wait(.1)
