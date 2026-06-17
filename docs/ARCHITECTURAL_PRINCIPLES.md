# Architectural Principles

PetriSeed is a simulation-first artificial-life project. These principles define system boundaries so future features remain deterministic, testable, and easy to reason about.

## Principle #1: Fixed Tick Deterministic Simulation

All biological and ecosystem state changes must occur through `SimulationManager` fixed ticks.

The simulation should not depend on frame rate, rendering cadence, camera behavior, UI refresh timing, or visual effects timing. If a change affects the state of the dish, organisms, nutrients, genetics, populations, or ecosystem interactions, it belongs in the fixed simulation tick path.

### Allowed In `_Process()`

- Rendering
- UI updates
- Visual interpolation
- Camera movement
- Input handling
- Visual effects

### Not Allowed In `_Process()`

- Reproduction
- Mutation
- Nutrient diffusion
- Energy updates
- Death checks
- Population changes
- Genome changes
- Quorum sensing
- Phage interactions

### Implementation Rule

Use `_Process()` only to collect time, refresh presentation, and handle player-facing interaction. Route simulation state changes through explicit fixed-tick methods owned or coordinated by `SimulationManager`.
