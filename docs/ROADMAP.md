# Roadmap

This roadmap follows the clean-room reuse plan. Phase 1A establishes the first field simulation layer without adding organisms.

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

## Phase 1B: Field Tools and Debug Controls

- Paintable food.
- Field reset controls.
- Configurable display modes.

## Phase 1C: Additional Fields

- Toxin grid.
- Waste grid.
- Heat and pH stress placeholders.

## Phase 2: Microbes

- Spawn microbes.
- Movement, energy, eating, aging, death, and asexual reproduction.

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
