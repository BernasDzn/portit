param(
    [string]$Scene = "HelloScene",
    [string]$File = "scenes/hello_scene.py",
    [string]$Quality = "-p",
    [string]$Fps = "--fps"
)

# Usage: .\run.ps1 -Scene HelloScene -File scenes/hello_scene.py -Quality -pql
manim $Quality $File $Scene $Fps 60
