# Arcane Survival Design Notes

## Core Loop

Arcane Survival is built around a continuous survival loop:

1. The player starts in the center of an endless arena.
2. Enemies spawn outside the camera in an expanding ring.
3. The player moves manually and casts spells automatically.
4. Enemies die, drop XP, and increase run momentum.
5. Level ups pause the game and present three upgrade cards.
6. Upgrades improve stats, unlock skills, or strengthen existing skills.
7. Waves scale over time and bosses appear periodically.
8. The run continues without a fixed endpoint.

## Infinite Progression

The run has no hard completion condition. Difficulty scales through:

```text
Elapsed run time
Current wave
Bosses defeated
Player level
```

The player should feel powerful through upgrades and synergies, while enemy density and durability keep increasing.

## Waves

`WaveDirector` manages spawn cadence, batch size, wave number, elite chance, and boss timing. The first minutes are intentionally readable, then enemy batches grow until the screen becomes chaotic.

The current wave is derived from elapsed minutes, which keeps the system simple and predictable for a first playable prototype.

## Bosses

Bosses are periodic pressure spikes. They spawn outside the screen with a UI warning and show a boss health bar.

Implemented bosses:

```text
Rune Guardian: melee boss with charge and close area attacks.
Astral Witch: ranged boss with projectiles, danger zones, and repositioning.
```

Each boss defeat increases global difficulty through the progression manager and difficulty scaler.

## Skills

Skills are defined by `SkillData` and executed by `SkillRuntime`, `SkillCaster`, and `SkillEffect`.

Initial skills:

```text
Arcane Bolt
Flame Orbit
Ice Nova
Lightning Chain
Void Zone
Nature Spikes
```

The player starts with Arcane Bolt and Flame Orbit so the first run immediately demonstrates automatic casting and a starter synergy.

## Synergies

Synergies are tag-driven. `SynergyManager` watches the player's skill inventory and activates matching `SynergyData` rules.

The first synergy, Explosive Arcanum, is active when Arcane and Fire tags are both present. This makes Arcane Bolt explosions a visible early payoff without requiring a long unlock path.

## Balance Philosophy

Early balance targets:

```text
Player HP: 100
Player move speed: 6
XP for level 2: 10
XP growth: 1.25 per level
First boss interval: 5 minutes
```

Enemies begin weak so the player levels quickly. Upgrades are intentionally impactful because the game fantasy depends on large spell combinations becoming powerful and readable.

## Future Expansion

Recommended next additions:

```text
Asset-backed databases for skills, enemies, bosses, upgrades, and synergies
More boss telegraphs
More enemy formations
More skill-specific upgrades
Persistent meta progression
Audio and hit impact polish
URP renderer asset tuning
Save data and run summary screens
Controller support for PC
```
