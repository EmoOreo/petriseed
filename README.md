# PetriSeed

PetriSeed is a clean-room Godot 4.6 C# artificial-life project about a 2D petri dish simulation. Phase 0 establishes the project shell only: the Godot project, documentation, source folders, a basic main scene, a fixed tick simulation manager, and a seedable random number service.

## Current Status

This repository is in Phase 0 from the clean-room reuse plan. It intentionally does not include full gameplay, microbes, nutrient fields, mutation, selection pressure, UI tools, or save/load behavior yet.

## Clean-Room Rule

Referenced artificial-life projects are study material only. Do not copy code, assets, file names, class structures, tuning constants, UI, art, or data from Thrive, Lenia, Godot boids projects, genetic algorithm examples, or similar repositories. Implement PetriSeed from original behavior specs and original code.

## Requirements

- Godot 4.6 .NET build
- .NET SDK compatible with Godot 4.6 C# projects

## Run

1. Open this folder in Godot 4.6 .NET.
2. Let Godot restore/build the C# project if prompted.
3. Run the main scene at `res://scenes/Main.tscn`.

## Test

From Godot:

1. Open `res://scenes/Main.tscn`.
2. Press Play.
3. Confirm the Output panel logs a Phase 0 ready message and fixed simulation tick messages.
4. Stop the scene.

From the command line, if Godot is on your PATH:

```powershell
godot --headless --path . --quit-after 2
dotnet build
```

## Structure

- `docs/` - architecture, roadmap, and clean-room notes.
- `scenes/` - Godot scenes.
- `src/simulation/` - simulation runtime shell, fixed tick loop, and RNG service.
- `src/organisms/` - future organism agent code.
- `src/environment/` - future field grid and dish environment code.
- `src/genetics/` - future genome and mutation code.
- `src/stats/` - future telemetry and run statistics code.
- `src/save/` - future persistence code.
- `src/ui/` - future HUD and tool UI code.

## Phase 0 Deliverables

- Godot 4.6 C# project foundation.
- Basic main scene.
- `SimulationManager` C# script.
- Fixed tick loop.
- Seedable RNG service.
- Clean-room documentation.
