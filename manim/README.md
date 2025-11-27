Boilerplate Manim project

Contents:
- `scenes/hello_scene.py`: simple example scene
- `requirements.txt`: lists `manim` (install into a venv)
- `run.ps1` / `run.sh`: simple helpers to render the example

Quick start (PowerShell):

1. Create & activate a venv:

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
```

2. Install dependencies:

```powershell
python -m pip install --upgrade pip
pip install -r requirements.txt
```

3. Render the example scene:

```powershell
# from repository root
cd manim
manim -pql scenes/hello_scene.py HelloScene
```

Notes:
- `-pql` = preview, quality=low (fast). Use `-pqh` or `-pqm` for higher quality.
- Output video files go to `manim/media/` by default.
- On Windows you might run `manim.exe` or `python -m manim` if the CLI isn't on PATH.
