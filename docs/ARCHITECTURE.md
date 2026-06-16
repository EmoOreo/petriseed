# Architecture

PetriSeed follows the clean-room architecture from the planning document. Phase 0 creates only the shell needed to grow the simulation safely.

## Runtime Shape

`SimulationManager` is the root runtime coordinator for the first scene. It owns the fixed tick loop and seedable random service. Future systems should plug into the fixed tick path instead of spreading simulation state across frame-rate-dependent `_Process` methods.

## Module Boundaries

- `src/simulation/` owns tick timing, run state, deterministic services, and high-level orchestration.
- `src/environment/` will own dish bounds and field layers such as nutrient, toxin, waste, heat, pH, and colony signal.
- `src/organisms/` will own organism state, movement, energy, aging, reproduction, and death.
- `src/genetics/` will own original PetriSeed genome traits, mutation, strain grouping, and ancestry.
- `src/stats/` will own telemetry such as population, births, deaths, nutrient totals, and strain summaries.
- `src/save/` will own save files, experiment presets, and load validation.
- `src/ui/` will own HUD, controls, and player tools.

## Phase 0 Constraints

- No imported gameplay code from reference projects.
- No copied scene hierarchy, class structure, constants, assets, UI, or names from reference projects.
- No full gameplay systems yet.
- Keep foundational services deterministic enough to debug.

## Tick Model

The simulation uses a fixed tick interval independent of rendered frame rate. `_Process` accumulates elapsed time, then advances zero or more fixed ticks. Later systems should expose explicit tick methods and receive deterministic inputs from `SimulationManager`.

## Randomness

Randomness is centralized through `RandomNumberService`, which wraps Godot's `RandomNumberGenerator` and accepts an explicit seed. This keeps future debug scenarios and regression tests reproducible.
