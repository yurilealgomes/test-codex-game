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
Max Alive Enemies: 350
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

## Infinite World

The infinite world is visual rather than mathematically infinite. `InfiniteWorldManager` recycles a 5x5 grid of chunks around the player. Chunks are repositioned when the player crosses chunk boundaries.

Decorations are deterministic per chunk coordinate, so repositioned chunks feel stable without storing endless world state.

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
```

All UI text is in English.

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
