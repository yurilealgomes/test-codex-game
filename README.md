# Arcane Survival

Arcane Survival is a Windows PC focused Unity 3D roguelite survival prototype. The player moves through an endless low-poly arena, survives escalating enemy waves, casts automatic spells, collects XP, chooses upgrades on level up, activates elemental synergies, and faces periodic bosses.

## Repository

The intended GitHub repository name is `test-codex-game`.

The GitHub CLI was not available in the local environment used to create this prototype, so the remote repository was not created automatically. To publish manually:

```bash
gh repo create test-codex-game --private --source . --remote origin --push
```

If `gh` is not installed, create `test-codex-game` on GitHub, then run:

```bash
git remote add origin https://github.com/YOUR_ACCOUNT/test-codex-game.git
git push -u origin main
```

## Unity Version

Recommended Unity version:

```text
Unity 2022.3.62f3 LTS
```

The project includes the Universal Render Pipeline package in `Packages/manifest.json`. If Unity asks to regenerate project files or import packages, allow it.

## Opening the Project

1. Open Unity Hub.
2. Add this folder as a Unity project.
3. Open it with Unity `2022.3.62f3` or a compatible `2022.3 LTS` editor.
4. Open the scene:

```text
Assets/_Project/Scenes/MainGame.unity
```

5. Press Play.

The scene is intentionally minimal. Runtime bootstrap code creates the camera, player, world chunks, pools, managers, UI, and data objects when the scene loads.

## How to Play

Controls:

```text
WASD: Move
Mouse: UI navigation
Esc: Pause
Enter or Space: Start run
```

Gameplay:

1. Start a run from the start screen.
2. Move with WASD.
3. Arcane Bolt and Flame Orbit cast automatically.
4. Enemies spawn outside the camera view and chase the player.
5. Defeated enemies drop XP orbs.
6. Collect XP by moving near orbs.
7. Leveling up pauses the game and presents three upgrade cards.
8. Choose an upgrade to continue.
9. Waves scale endlessly over time.
10. Bosses spawn at configured intervals with a warning and a boss health bar.

## Main Scene

```text
Assets/_Project/Scenes/MainGame.unity
```

The playable setup is created by:

```text
Assets/_Project/Scripts/Core/GameBootstrapper.cs
```

## Creating a New Skill

1. Add a new `SkillData` entry, preferably as a ScriptableObject asset in:

```text
Assets/_Project/ScriptableObjects/Skills
```

2. Assign name, description, type, tags, damage, cooldown, targeting mode, and visual color.
3. Add a `SkillEffectKind` value if the skill needs unique behavior.
4. Implement its execution in `SkillEffect`.
5. Add upgrades that unlock or improve it through `UpgradeData`.
6. Add synergy rules through `SynergyData` when it should combine with other tags.

## Creating a New Enemy

1. Create an `EnemyData` asset in:

```text
Assets/_Project/ScriptableObjects/Enemies
```

2. Configure health, speed, damage, XP drop, attack type, movement style, weight, color, and spawn rules.
3. Add it to the runtime database or a future asset-backed database.
4. Common enemies use simple vector movement through `EnemyMovement`; no NavMesh is required.

## Creating a New Upgrade

1. Create an `UpgradeData` asset in:

```text
Assets/_Project/ScriptableObjects/Upgrades
```

2. Choose rarity, effect type, amount, optional target skill, and affected skill label.
3. `UpgradeOptionGenerator` rolls three options using rarity weights.
4. `UpgradeManager` applies the selected effect and resumes the run.

## Creating a New Synergy

1. Create a `SynergyData` asset in:

```text
Assets/_Project/ScriptableObjects/Synergies
```

2. Define required elemental tags, description, effect type, priority, visual feedback, and stacking rules.
3. `SynergyManager` detects matching player skill tags and activates the synergy automatically.
4. Skill behavior can query active synergies through `SynergyManager.HasSynergy`.

Implemented synergies:

```text
Fire + Arcane: Explosive Arcanum
Ice + Lightning: Stormfrost
Void + Fire: Burning Abyss
Nature + Arcane: Living Runes
Lightning + Void: Gravity Storm
```

## Infinite World

`InfiniteWorldManager` keeps a fixed grid of reusable chunks around the player. When the player crosses into a new chunk coordinate, existing chunks are repositioned and repopulated with deterministic lightweight decorations. This creates the illusion of an endless world without generating infinite objects.

Core scripts:

```text
InfiniteWorldManager
WorldChunk
WorldChunkData
WorldDecorationSpawner
```

## Off-Screen Spawning

`EnemySpawnDirector` uses `SpawnRingCalculator` to place enemies in a ring around the player:

```text
Spawn Radius Min: 24
Spawn Radius Max: 36
Boss Spawn Radius: 42
Elite Spawn Radius: 32
```

The calculator checks camera viewport space so enemies spawn outside the visible area whenever possible.

## Project Structure

The project follows the requested folder layout under:

```text
Assets/_Project
Docs
```

Core gameplay code is split by responsibility into `Core`, `Player`, `Camera`, `Combat`, `Enemies`, `Bosses`, `Waves`, `Skills`, `Upgrades`, `Progression`, `World`, `UI`, `Data`, `Pooling`, and `Utils`.
