#!/usr/bin/env bash
SCENE=${1:-HelloScene}
FILE=${2:-scenes/hello_scene.py}
QUALITY=${3:--p}
FPS=${4:-60}

manim "$QUALITY" "$FILE" "$SCENE" --fps "$FPS"
