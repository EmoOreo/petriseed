# Clean-Room Notes

The planning document is the source of truth for reuse boundaries. The referenced artificial-life projects are concept references only.

## Allowed Conceptual References

- Microbe-style resource loops and species-level thinking.
- Local movement ideas such as separation, cohesion, alignment, chemotaxis, and spatial partitioning.
- General genetic algorithm concepts such as mutation, heredity, selection, and fitness scoring.
- Continuous field ideas such as diffusion, decay, nutrient gradients, toxin gradients, and emergent colony behavior.

## Not Allowed

- Copying source code.
- Copying art, icons, shaders, UI, assets, or data files.
- Reusing project-specific names, exact class structures, file names, scene hierarchy, or balancing constants.
- Pasting source snippets into notes or docs.
- Implementing behavior by translating a reference implementation line by line.

## Phase 0 Statement

The Phase 0 foundation in this repository is original code written for PetriSeed. It implements only generic project infrastructure: a Godot C# project, a basic scene, a fixed tick loop, and a seedable RNG service.

## Future Implementation Rule

Future gameplay work should start from behavior specs written in plain English, then close reference material before writing code. Constants should be selected through PetriSeed playtesting and debugging.
