using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public static class SkillEffect
    {
        public static void MaintainPassive(SkillRuntime runtime, SkillCaster caster)
        {
            if (runtime.Data.EffectKind == SkillEffectKind.FlameOrbit)
            {
                FlameOrbitController orbit = caster.GetComponent<FlameOrbitController>();
                if (orbit == null)
                {
                    orbit = caster.gameObject.AddComponent<FlameOrbitController>();
                }

                orbit.Configure(runtime, caster);
            }
        }

        public static void Execute(SkillRuntime runtime, SkillCaster caster)
        {
            switch (runtime.Data.EffectKind)
            {
                case SkillEffectKind.ArcaneBolt:
                    CastArcaneBolt(runtime, caster);
                    break;
                case SkillEffectKind.IceNova:
                    CastIceNova(runtime, caster);
                    break;
                case SkillEffectKind.LightningChain:
                    CastLightningChain(runtime, caster);
                    break;
                case SkillEffectKind.VoidZone:
                    CastVoidZone(runtime, caster);
                    break;
                case SkillEffectKind.NatureSpikes:
                    CastNatureSpikes(runtime, caster);
                    break;
            }
        }

        private static void CastArcaneBolt(SkillRuntime runtime, SkillCaster caster)
        {
            SkillData data = runtime.Data;
            int count = Mathf.Max(1, data.ProjectileCount + caster.Stats.ExtraProjectiles + runtime.Level / 3);
            IDamageable target = SkillTargeting.FindNearestEnemy(caster.transform.position, data.Range);
            if (target == null)
            {
                return;
            }

            SpawnVfx(caster.transform.position + Vector3.up * 1f, data.VisualColor, Vector3.one * 0.46f, 0.12f);
            for (int i = 0; i < count; i++)
            {
                Vector3 direction = MathUtils.DirectionOnPlane(caster.transform.position, target.Transform.position);
                direction = Quaternion.Euler(0f, (i - (count - 1) * 0.5f) * 8f, 0f) * direction;
                DamageInfo damage = caster.BuildDamage(data, 1f);
                SpawnProjectile("PlayerProjectile", caster.transform.position + Vector3.up * 1.1f, direction, data.ProjectileSpeed, 4f, 0.45f, damage, OnArcaneBoltHit);
            }
        }

        private static void OnArcaneBoltHit(Vector3 position, IDamageable target)
        {
            SynergyManager synergyManager;
            if (ServiceLocator.TryGet(out synergyManager) && synergyManager.HasSynergy(SynergyEffect.ExplosiveArcanum))
            {
                DamageInfo explosionDamage = new DamageInfo
                {
                    BaseDamage = 8f,
                    FinalDamage = 8f,
                    Source = null
                };
                explosionDamage.ElementTags.Add(SkillTag.Arcane);
                explosionDamage.ElementTags.Add(SkillTag.Fire);
                SpawnArea(position, 2.4f, 0.12f, 0.05f, explosionDamage, DamageTarget.Enemies, new Color(0.8f, 0.25f, 1f));
            }
        }

        private static void CastIceNova(SkillRuntime runtime, SkillCaster caster)
        {
            SkillData data = runtime.Data;
            DamageInfo damage = caster.BuildDamage(data, 1f);
            damage.StatusEffects.Add(new StatusEffect
            {
                Type = StatusEffectType.Slow,
                Magnitude = 0.45f,
                Duration = 2.4f * caster.Stats.DurationMultiplier
            });

            float radius = data.Area * caster.Stats.AreaMultiplier * (1f + runtime.Level * 0.05f);
            SpawnArea(caster.transform.position, radius, 0.2f, 0.05f, damage, DamageTarget.Enemies, data.VisualColor);
            Shake(0.05f, 0.08f);
        }

        private static void CastLightningChain(SkillRuntime runtime, SkillCaster caster)
        {
            SkillData data = runtime.Data;
            IDamageable current = SkillTargeting.FindNearestEnemy(caster.transform.position, data.Range);
            if (current == null)
            {
                return;
            }

            List<IDamageable> hitTargets = new List<IDamageable>();
            int jumps = Mathf.Max(1, data.ChainCount + runtime.Level / 2 + caster.Stats.ExtraChainCount);
            float chainRadius = data.ChainRadius + caster.Stats.ChainRadiusBonus;
            Vector3 lastPosition = caster.transform.position;

            for (int i = 0; i <= jumps && current != null; i++)
            {
                DamageInfo damage = caster.BuildDamage(data, 1f);
                SynergyManager synergyManager;
                if (ServiceLocator.TryGet(out synergyManager) && synergyManager.HasSynergy(SynergyEffect.Stormfrost) && IsSlowed(current))
                {
                    damage.FinalDamage *= 1.45f;
                }

                if (ServiceLocator.TryGet(out synergyManager) && synergyManager.HasSynergy(SynergyEffect.GravityStorm))
                {
                    damage.StatusEffects.Add(new StatusEffect { Type = StatusEffectType.Pull, Magnitude = 2f, Duration = 0.25f });
                }

                current.TakeDamage(damage);
                SpawnLightningVfx(lastPosition + Vector3.up * 0.8f, current.Transform.position + Vector3.up * 0.8f, data.VisualColor);
                hitTargets.Add(current);
                lastPosition = current.Transform.position;
                current = SkillTargeting.FindNearestEnemyExcluding(lastPosition, chainRadius, hitTargets);
            }

            Shake(0.04f, 0.06f);
        }

        private static void CastVoidZone(SkillRuntime runtime, SkillCaster caster)
        {
            SkillData data = runtime.Data;
            IDamageable target = SkillTargeting.FindNearestEnemy(caster.transform.position, data.Range);
            Vector3 position = target != null ? target.Transform.position : caster.transform.position + caster.transform.forward * 5f;
            DamageInfo damage = caster.BuildDamage(data, 0.38f);
            float radius = data.Area * caster.Stats.AreaMultiplier;
            float duration = data.Duration * caster.Stats.DurationMultiplier * (1f + runtime.Level * 0.08f);
            SpawnArea(position, radius, duration, 0.35f, damage, DamageTarget.Enemies, data.VisualColor);
        }

        private static void CastNatureSpikes(SkillRuntime runtime, SkillCaster caster)
        {
            SkillData data = runtime.Data;
            int count = Mathf.Max(1, data.ProjectileCount + runtime.Level / 2 + caster.Stats.ExtraProjectiles);
            SynergyManager synergyManager;
            if (ServiceLocator.TryGet(out synergyManager) && synergyManager.HasSynergy(SynergyEffect.LivingRunes))
            {
                count += 1;
            }

            List<IDamageable> targets = SkillTargeting.FindEnemies(caster.transform.position, data.Range, count);
            for (int i = 0; i < targets.Count; i++)
            {
                DamageInfo damage = caster.BuildDamage(data, 1f);
                targets[i].TakeDamage(damage);
                SpawnVfx(targets[i].Transform.position + Vector3.up * 0.5f, data.VisualColor, new Vector3(0.35f, 2.2f, 0.35f), 0.35f);
            }
        }

        public static void SpawnArea(Vector3 position, float radius, float duration, float tickInterval, DamageInfo damage, DamageTarget target, Color color)
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            GameObject areaObject = poolManager.Spawn("AreaDamage", MathUtils.WithY(position, 0.04f), Quaternion.identity);
            if (areaObject == null)
            {
                return;
            }

            Renderer renderer = areaObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }

            AreaDamage areaDamage = areaObject.GetComponent<AreaDamage>();
            if (areaDamage != null)
            {
                areaDamage.Configure(damage, radius, duration, tickInterval, target);
            }
        }

        private static void SpawnProjectile(string key, Vector3 position, Vector3 direction, float speed, float lifetime, float radius, DamageInfo damage, System.Action<Vector3, IDamageable> onHit)
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            GameObject projectileObject = poolManager.Spawn(key, position, Quaternion.LookRotation(direction));
            if (projectileObject == null)
            {
                return;
            }

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Launch(damage, direction, speed, lifetime, radius, DamageTarget.Enemies, onHit);
            }
        }

        public static void SpawnVfx(Vector3 position, Color color, Vector3 scale, float lifetime)
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            GameObject vfxObject = poolManager.Spawn("SkillVFX", position, Quaternion.identity);
            if (vfxObject == null)
            {
                return;
            }

            SimpleVFX vfx = vfxObject.GetComponent<SimpleVFX>();
            if (vfx != null)
            {
                vfx.Play(color, scale, lifetime);
            }
        }

        private static void SpawnLightningVfx(Vector3 start, Vector3 end, Color color)
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            GameObject effectObject = poolManager.Spawn("ChainLightningEffect", Vector3.zero, Quaternion.identity);
            if (effectObject == null)
            {
                return;
            }

            ChainLightningEffect effect = effectObject.GetComponent<ChainLightningEffect>();
            if (effect != null)
            {
                effect.Play(start, end, color, 0.16f);
            }
        }

        private static bool IsSlowed(IDamageable target)
        {
            EnemyHealth enemyHealth = target as EnemyHealth;
            if (enemyHealth != null)
            {
                return enemyHealth.Controller != null && enemyHealth.Controller.IsSlowed;
            }

            return false;
        }

        private static void Shake(float amplitude, float duration)
        {
            CameraShake shake;
            if (ServiceLocator.TryGet(out shake))
            {
                shake.Shake(amplitude, duration);
            }
        }
    }
}
