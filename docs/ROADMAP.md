# Roadmap

This roadmap follows the clean-room reuse plan. Phase 1B adds passive microbes without reproduction, mutation, genetics, or species.

## Phase 0: Project Shell

- Godot 4.6 C# project.
- Basic main scene.
- Simulation manager with pause/play state, speed multiplier, fixed tick loop, and population cap placeholder.
- Seedable RNG service.
- Clean-room documentation and source folders.

## Phase 1A: Nutrient Field

- Deterministic nutrient cell grid.
- Cell nutrient amount, regeneration rate, and decay rate.
- Fixed-tick regeneration, decay, and neighbor diffusion.
- Seeded initial nutrient patches.
- Nutrient density visualization.
- Debug telemetry for total nutrients, average nutrients, and diffusion movement.

## Phase 1B: Passive Microbes

- Spawn passive microbes.
- Wander inside the circular dish.
- Sense nearby nutrients.
- Consume nutrients and store energy.
- Die from starvation.
- Visualize microbes and energy.

## Phase 1C: Field Tools and Debug Controls

- Paintable food.
- Field reset controls.
- Configurable display modes.

## Phase 1D: Additional Fields

- Toxin grid.
- Waste grid.
- Heat and pH stress placeholders.

## Phase 2: Active Microbe Life Cycle

- Aging, asexual reproduction, and life-cycle tuning.

## Phase 3: Genetics

- Original PetriSeed genome traits.
- Mutation engine.
- Basic inheritance.

## Phase 4: Selection Pressure

- Toxin or antibiotic field.
- Temperature and pH stress.
- Waste accumulation.
- Population graphs.

## Phase 5: Species Emergence

- Strain labels from genome similarity.
- Lineage history.
- Extinction events.
- Dominant trait summaries.

## Phase 6: Polish Vertical Slice

- Experiment presets.
- Tutorial shell.
- Save/load.
- Performance cap.
- Release build.
