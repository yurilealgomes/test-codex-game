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
8. After the first major boss, the player chooses to end the run or continue into Endless Mode.

## Infinite Progression

The run has a milestone boss choice and optional Endless Mode. Difficulty scales through:

```text
Elapsed run time
Current wave
Bosses defeated
Player level
Endless Mode time
```

The player should feel powerful through upgrades and synergies, while enemy density and durability keep increasing. Endless Mode increases pressure faster after the first major boss if the player chooses to continue.

## Waves

`WaveDirector` manages spawn cadence, batch size, wave number, elite chance, and boss timing. The first minutes are intentionally readable, then enemy batches grow until the screen becomes chaotic.

The current wave is derived from elapsed minutes, which keeps the system simple and predictable for a first playable prototype.

## Bosses

Bosses are periodic pressure spikes. A warning appears shortly before the boss spawns, giving the player time to reposition before the boss enters from outside the screen. Bosses show a health bar with readable HP values.

Implemented bosses:

```text
Rune Guardian: melee boss with charge and close area attacks.
Astral Witch: ranged boss with projectiles, danger zones, and repositioning.
```

The first major boss defeat pauses the run and offers `End Run` or `Continue Endless Mode`. Future bosses continue to increase global difficulty through progression and endless scaling.

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

The player chooses one of six starting skills before the run begins. Additional skills are offered first on level 5 and every fifth level afterward while locked skills remain.

## Rarity And Luck

Upgrade rarities are `Common`, `Uncommon`, `Magic`, `Epic`, and `Legendary`. Card colors follow white, green, blue, purple, and gold respectively. The player `Luck` stat shifts upgrade odds toward better rarities without making Legendary common.

## Breakable Exploration Objects

Breakable objects are generated per world chunk and avoid the visible camera area and the player's immediate radius. Destroyed breakables are tracked for the current run, so they do not respawn when chunks recycle. This makes exploration matter without surprising the player with pop-in.

## Special Pickups

The first special pickup is `Magnet`. It pulls all active XP Orbs to the player from anywhere in the run. The pickup is rare and can drop from elites, bosses, and breakables. Its placeholder visual bobs and pulses so it feels like a special reward instead of another XP orb.

## HUD Readability

HP and XP bars should communicate state changes at a glance. Smooth fill motion, short XP/level feedback, and low-health pulsing are intentionally lightweight, readable, and restrained so they do not compete with combat effects.

## Chain Lightning

Lightning Chain uses a short-lived pooled LineRenderer effect with jittered segments. The chain count and chain radius can be improved by upgrades, and a single cast avoids repeatedly hitting the same target.

## Synergies

Synergies are tag-driven. `SynergyManager` watches the player's skill inventory and activates matching `SynergyData` rules.

The first synergy, Explosive Arcanum, is active when Arcane and Fire tags are both present. This makes Arcane Bolt explosions a visible early payoff without requiring a long unlock path.

## Balance Philosophy

Early balance targets:

```text
Player HP: 100
Player move speed: 6
XP for level 2: 18
XP growth: 1.32 per level
First boss interval: 15 minutes
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
