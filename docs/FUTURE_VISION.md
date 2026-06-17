# Future Vision

This document collects long-range PetriSeed ideas as future systems. They are not current implementation requirements, and they should not be added until the fixed-tick simulation foundation is stable.

Each future system must follow the clean-room rule and the architectural principle that biological and ecosystem state changes happen through deterministic fixed ticks.

## Future Systems

### Quorum Sensing

Future microbes may coordinate through local signal fields, density thresholds, or shared environmental cues. This should be implemented as deterministic field or agent state, not as frame-rate-dependent behavior.

### Phage Therapy

Future phage systems may add targeted population pressure, infection-like dynamics, or experimental treatment tools. This should wait until passive microbes, lifecycle behavior, and population telemetry are stable.

### Captain Mode

Future player-facing direction may include a high-level command or experiment-management mode. This should be tool/probe driven rather than direct character control.

### Dr. Blob-Inspired Containment

The dish should eventually feel like an active containment arena with meaningful boundaries, probes, and interventions. This is high-level presentation inspiration only: do not copy code, assets, names, formulas, UI, or exact mechanics.

### D/R/M Integration

Future design may explore D/R/M-style systems as a higher-level framework for experiment structure, progression, or control. Define the terms and scope before implementation.

### Particle-Life Forces

Future organism or colony motion may explore simple attraction/repulsion forces. These forces must be original, deterministic, fixed-tick systems and should not replace the current clean nutrient/microbe foundation prematurely.

### Multi-Dish Support

Future experiments may support multiple dishes, linked environments, or isolated samples. This should wait until one-dish simulation state, save/load, and telemetry boundaries are clean.

### Project Genesis Integration

Future PetriSeed work may connect to Project Genesis if the product direction calls for it. Treat that as integration work after PetriSeed has a stable standalone simulation loop.

## Current Boundary

Do not implement these systems during Phase 1B. Current work remains limited to deterministic nutrient fields and passive microbes unless a later phase explicitly expands scope.
