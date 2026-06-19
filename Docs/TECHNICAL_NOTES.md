# Arcane Survival Technical Notes

## Architecture

The prototype is organized around small MonoBehaviours and ScriptableObject data definitions.

Important systems:

```text
GameBootstrapper: Creates the playable runtime scene.
GameStateManager: Controls main menu, playing, pause, level up, and game over.
EventBus: Decouples UI, progression, and gameplay notifications.
ServiceLocator: Provides simple access to core runtime systems.
PoolManager: Owns reusable object pools.
GameDatabase: Holds runtime-created data for the prototype.
PickupManager: Spawns and applies special pickups.
EndlessModeManager: Tracks Endless Mode scaling after the milestone boss.
```

The scene file is minimal by design. `GameBootstrapper` creates all required runtime objects after `MainGame` loads.

## Object Pooling

Pooling is implemented for:

```text
Enemies
Bosses
Player projectiles
Enemy projectiles
XP orbs
Floating damage text
Area damage effects
Simple VFX
Chain Lightning effects
Breakable destruction effects
Special pickups
```

Core pooling scripts:

```text
ObjectPool
PoolManager
PooledObject
IPoolable
DespawnWhenFarFromPlayer
PerformanceSettings
```

This avoids frequent gameplay `Instantiate` and `Destroy` calls for the most common runtime objects.

## Performance Limits

Initial limits:

```text
Max Alive Enemies: 520
Max Alive Projectiles: 500
Max Floating Damage Texts: 80
Enemy Despawn Distance: 70
XP Orb Despawn Distance: 60
```

`WaveDirector` respects the max alive enemy limit before spawning new batches.

## Movement and Combat

Common enemies use direct vector movement toward the player. This avoids NavMesh overhead for large hordes.

Damage flows through:

```text
DamageInfo
IDamageable
HealthComponent
Projectile
AreaDamage
StatusEffect
FloatingDamageText
```

Status effects currently support slow, burn-ready data, and pull.

Damage text uses pooled `TextMesh` objects with a shadow child. Critical hits display `CRIT!` with a larger gold pop animation. Damage colors are derived from skill elemental tags when possible.

When the floating damage text pool is close to its configured limit, non-critical damage text is skipped so critical hits and important feedback remain readable during dense horde moments.

## Infinite World

The infinite world is visual rather than mathematically infinite. `InfiniteWorldManager` recycles a 5x5 grid of chunks around the player. Chunks are repositioned when the player crosses chunk boundaries.

Decorations are disabled for the main gameplay pass to avoid gray non-interactive clutter. Breakable objects are deterministic per chunk coordinate and slot. Destroyed breakables are stored for the current run so recycled chunks do not respawn objects that the player already broke.

Breakable placement validates:

```text
Minimum distance from player
Camera viewport margin
Chunk coordinate and slot persistence
```

## Off-Screen Spawn

`SpawnRingCalculator` generates spawn positions in a ring around the player and validates camera viewport space. If all attempts fail, it still falls back to the configured ring to keep wave pressure consistent.

Spawn values:

```text
Min Radius: 24
Max Radius: 36
Elite Radius: 32
Boss Radius: 42
```

## UI

The UI is generated at runtime with Unity UI:

```text
HUDController
MainMenu
PauseMenu
GameOverPanel
LevelUpPanel
UpgradeCardUI
BossWarningUI
BossHealthBar
SynergyNotificationUI
PostBossChoicePanel
VictoryPanel
```

All UI text is in English.

`HUDController` smooths HP and XP bar fill values, shows short feedback for XP gains, level ups, and special pickups, and adds a subtle low-health pulse. The warning overlay is non-interactive so it does not block mouse input on gameplay choice panels.

## Debug Tools

`DebugGodModeController` provides editor-facing test commands:

```text
F1: Toggle debug overlay
F2: Toggle No Cooldowns
F3: Toggle Infinite XP
F4: Toggle Spawn Debug
F5: Toggle Chunk Debug
F6: Add XP
F7: Toggle God Mode
F8: Force Level Up
F9: Spawn Boss
F10: Clear Enemies
F11: Toggle Damage Debug
F12: Break nearby objects
```

Spawn and chunk debug draw simple gizmos in the Scene view when enabled.

## Upgrade Rarity And Luck

Upgrade rarity values are `Common`, `Uncommon`, `Magic`, `Epic`, and `Legendary`. `UpgradeRarityUtility` owns colors, power multipliers, and Luck-adjusted drop weights. `UpgradeDescriptionBuilder` generates specific card descriptions from upgrade data.

## Pickups

Special pickups use `PickupType`, `PickupData`, `SpecialPickup`, and `PickupManager`. `Magnet` is implemented and calls `XPOrb.PullAllTo` so all active XP Orbs move toward the player.

The Magnet placeholder is built from runtime Unity primitives instead of an external asset. `SpecialPickup` applies bobbing and pulse motion to make special drops visually distinct from XP orbs.

## Chain Lightning

`ChainLightningEffect` uses pooled `LineRenderer` instances with jittered segments. Chain count and radius can be increased by upgrades through `PlayerStats.ExtraChainCount` and `PlayerStats.ChainRadiusBonus`.

## Post Boss Flow

`PostBossChoicePanel` listens for the first boss defeat and pauses the run with `End Run` and `Continue Endless Mode`. `VictoryPanel` handles the simple run completion screen, and `EndlessModeManager` increases pressure after Endless Mode starts.

`WaveDirector` now issues the boss warning before the spawn time and stores the pending boss data so the warning matches the boss that appears. `BossHealthBar` shows current and max HP and shifts color when the boss is near defeat.

## Physics Query Performance

High-frequency overlap checks use non-allocating buffers:

```text
PlayerCollector: pickup scans
Projectile: projectile hit checks
AreaDamage: tick-based area damage checks
```

This keeps XP collection, projectile combat, and large damage zones from allocating new collider arrays every frame.

## Known Future Improvements

Recommended technical improvements:

```text
Move runtime-created data into saved ScriptableObject assets.
Add formal assembly definitions.
Add URP renderer asset and quality profiles.
Add automated play mode tests.
Add spatial partitioning for enemy targeting as enemy counts rise.
Replace simple overlap checks with batched or cached queries where needed.
Add a lightweight save system for meta progression.
Add audio pooling for SFX.
```
