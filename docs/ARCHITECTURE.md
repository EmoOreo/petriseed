# Architecture

PetriSeed follows the clean-room architecture from the planning document. Phase 1B adds passive microbes that can wander, sense nutrients, consume nutrients, store energy, and die from starvation.

## Runtime Shape

`SimulationManager` is the root runtime coordinator for the first scene. It owns the fixed tick loop and seedable random service. Simulation systems plug into the fixed tick path instead of spreading simulation state across frame-rate-dependent `_Process` methods.

Phase 1A wires `NutrientField` into this path. Phase 1B adds `PassiveMicrobeSimulation` after the nutrient update. Rendering never changes nutrient or microbe state.

## Module Boundaries

- `src/simulation/` owns tick timing, run state, deterministic services, and high-level orchestration.
- `src/environment/` owns dish bounds and field layers. Phase 1A includes `NutrientCell`, `NutrientFieldConfig`, `NutrientField`, and `NutrientVisualization`.
- `src/organisms/` owns organism state, passive movement, nutrient sensing, energy, and starvation death. Reproduction, mutation, genetics, and species are intentionally absent.
- `src/genetics/` will own original PetriSeed genome traits, mutation, strain grouping, and ancestry.
- `src/stats/` will own telemetry such as population, births, deaths, nutrient totals, and strain summaries.
- `src/save/` will own save files, experiment presets, and load validation.
- `src/ui/` will own HUD, controls, and player tools.

## Current Constraints

- No imported gameplay code from reference projects.
- No copied scene hierarchy, class structure, constants, assets, UI, or names from reference projects.
- No reproduction, mutation, genetics, species, toxin, waste, pH, heat, or save/load systems yet.
- Keep foundational services deterministic enough to debug.

## Tick Model

The simulation uses a fixed tick interval independent of rendered frame rate. `_Process` accumulates elapsed time, then advances zero or more fixed ticks. Later systems should expose explicit tick methods and receive deterministic inputs from `SimulationManager`.

## Randomness

Randomness is centralized through `RandomNumberService`, which wraps Godot's `RandomNumberGenerator` and accepts an explicit seed. This keeps future debug scenarios and regression tests reproducible.

## Nutrient Field

The nutrient field is a rectangular grid. Each `NutrientCell` stores `nutrientAmount`, `regenerationRate`, and `decayRate`.

Each fixed tick:

1. Nutrients regenerate by each cell's configured regeneration rate.
2. Nutrients decay by each cell's configured decay rate.
3. Nutrients diffuse between horizontal and vertical neighboring cells.
4. Telemetry updates total nutrients, average nutrients, and diffusion movement for debug output.

The diffusion step is deterministic and grid-local. It exchanges nutrient amounts across neighboring cell pairs based on the amount difference between those cells.

## Passive Microbes

Passive microbes are deterministic agents updated only on fixed simulation ticks. Each microbe stores position, velocity, energy, and alive/dead state. On each tick, a passive microbe:

1. Samples nearby nutrient cells.
2. Steers toward the richest nearby nutrient direction while retaining a seeded wander component.
3. Moves within the circular dish boundary.
4. Consumes nutrients from its current cell.
5. Converts consumed nutrients into stored energy.
6. Loses a small amount of energy and dies when energy reaches zero.

This phase does not include reproduction, mutation, genomes, species labels, attacks, or player-directed organism control.
