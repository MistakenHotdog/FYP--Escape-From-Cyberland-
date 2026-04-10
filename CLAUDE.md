# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Escape From Cyberland** — a single-player stealth/puzzle action game built in Unity 2022.3.57f1 (LTS) as a Final Year Project. The player navigates levels avoiding detection by cameras, drones, and guards, with hacking mini-games to disable surveillance.

## Build & Development

- **Engine:** Unity 2022.3.57f1 — open via Unity Hub
- **Language:** C# (no assembly definitions, all scripts compile together)
- **No CLI build pipeline** — build through Unity Editor (File > Build Settings)
- **No automated tests** — test-framework package is included but unused
- **IDE:** Visual Studio or JetBrains Rider (both integrations configured)

## Architecture

### Game Flow
MainMenu → LoadingScene (async with progress bar) → GameScene1/GameScene2 → TerminationUI (Game Over or Victory)

### Core Systems

**Detection & Alert Pipeline:**
`SurveillanceCamera` / `DoorwayLaser` / `DronePatrol` → `AlarmSystem` (broadcasts alert) → all `EnemyAI` instances enter alert state (30m detection range for 10 seconds). `StealthDetection` monitors all detection sources, applies continuous damage when detected, and drives UI color feedback.

**Player Systems:**
- `PlayerMove` — Rigidbody movement with camera-relative directions, supports two input modes (joystick vs buttons) switchable at runtime via `ControlSettings` (persisted in PlayerPrefs)
- `PlayerHealth` — 100 HP, damage flash, triggers `TerminationUI` on death, calls `AlarmSystem.StopAlarm()` then freezes game via `Time.timeScale = 0`
- `PlayerEffectsController` — temporary debuffs (slowdown) using PostProcessing v2 (Vignette, Depth of Field)

**Enemy Systems:**
- `EnemyAI` — NavMesh-based guard with state machine: Patrol → Approach → Shooting. Uses raycast bullets with line-of-sight checks, waypoint patrol, alert mode from AlarmSystem. Shooting deals `shootDamage` (default 10) to PlayerHealth
- `DronePatrol` — airborne patrol between points, downward laser detection, spawns `FlyingBugEnemy` on player detect
- `FlyingBugEnemy` — homing enemy with sinusoidal weaving movement, applies slowdown effect on collision then plays shrink death animation
- `BugPatrol3D` — simple ground patrol between two points

**Puzzle System:**
- `HackWireManager` + `HackNodeUI` — wire-connection puzzle. Max 2 attempts before alarm triggers. On success, disables all cameras in scene. Keeps `Time.timeScale = 1` (does not freeze time)

**UI/Flow:**
- `CutsceneController` — camera animation sequences with UI text, disables gameplay scripts during playback
- `TriggerShowButton` — context-sensitive action prompts in trigger zones
- `LoadingScreen` — async scene loading with fade transitions
- `TerminationUI` — end-state display with restart/menu/quit options

### Input System
Two control modes managed by static `ControlSettings.mode` (0 = Joystick, 1 = Buttons). `ControlSettings.OnControlChanged` event notifies listeners. `ButtonInput` provides static Horizontal/Vertical properties as the keyboard alternative to joystick.

## Key Conventions

- Cross-system references (AlarmSystem, cameras, enemies) are cached in Start/Awake via `FindObjectOfType<T>()` — loose coupling, no dependency injection
- Player GameObject must have the **"Player" tag** — used by raycasts and trigger checks throughout
- Timed sequences use **coroutines**, not async/await
- Animator parameters use `Animator.StringToHash()` for performance
- Laser visuals use `LineRenderer` (cameras, drones, doorway lasers)
- Detection uses `Physics.Raycast()` for line-of-sight checks

## Key Packages

- `com.unity.ai.navigation` (1.1.7) — NavMesh for enemy pathfinding
- `com.unity.cinemachine` (2.10.5) — camera management
- `com.unity.postprocessing` (3.4.0) — visual effects (vignette, DoF)
- Joystick Pack (asset) — mobile-style joystick UI input

## Gotchas

- `HackWireManager` deliberately keeps `Time.timeScale = 1` during the puzzle so audio continues playing — don't "fix" this to freeze time
- `AlarmSystem` uses `WaitForSecondsRealtime` — alarm runs in real time even when `Time.timeScale = 0`
- `AlarmSystem.StopAlarm()` must be called before freezing time on death — `PlayerHealth.Die()` handles this
- `PlayerMove` and `PlayerMovement` both exist — `PlayerMove` is the active movement script; `PlayerMovement` appears to be legacy
- PostProcessing requires a GameObject with `PostProcessLayer` component in each scene
- Scene names are centralized in `SceneNames` static class (`Assets/Scripts/SceneNames.cs`) — update constants there when renaming scenes
- `DoorwayLaser` deals `contactDamage` (default 15) on player contact in addition to triggering the alarm
- `DoorwayLaserTimer` supports `randomVariation` field (default 0) for less predictable laser timing
- Tutorials (`CyberTutorial`, `CyberTutorialPopup`) are skippable via mouse click, Space, or Escape after 0.5s delay
- `FlyingBugEnemy` has `waveFrequency`/`waveAmplitude` fields for tuning weaving movement
