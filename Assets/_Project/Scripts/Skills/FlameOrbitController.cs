using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class FlameOrbitController : MonoBehaviour
    {
        private readonly List<GameObject> orbiters = new List<GameObject>();
        private readonly Dictionary<IDamageable, float> hitCooldowns = new Dictionary<IDamageable, float>();
        private SkillRuntime runtime;
        private SkillCaster caster;
        private float angle;

        public void Configure(SkillRuntime skillRuntime, SkillCaster skillCaster)
        {
            runtime = skillRuntime;
            caster = skillCaster;
            EnsureOrbiters();
        }

        private void Update()
        {
            if (runtime == null || caster == null)
            {
                return;
            }

            EnsureOrbiters();
            angle += Time.deltaTime * (85f + runtime.Level * 8f);
            float radius = runtime.Data.Area * caster.Stats.AreaMultiplier;

            for (int i = 0; i < orbiters.Count; i++)
            {
                if (!orbiters[i].activeSelf)
                {
                    continue;
                }

                float orbAngle = angle + i * (360f / ActiveOrbCount());
                Vector3 offset = new Vector3(Mathf.Cos(orbAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(orbAngle * Mathf.Deg2Rad)) * radius;
                orbiters[i].transform.position = transform.position + offset + Vector3.up * 0.75f;
                DamageNearOrb(orbiters[i].transform.position);
            }
        }

        private void EnsureOrbiters()
        {
            int count = ActiveOrbCount();
            while (orbiters.Count < count)
            {
                GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                orb.name = "Flame Orbit Orb";
                orb.transform.localScale = Vector3.one * 0.55f;
                Object.Destroy(orb.GetComponent<Collider>());
                Renderer renderer = orb.GetComponent<Renderer>();
                renderer.material.color = new Color(1f, 0.28f, 0.08f);
                orbiters.Add(orb);
            }

            for (int i = 0; i < orbiters.Count; i++)
            {
                orbiters[i].SetActive(i < count);
            }
        }

        private int ActiveOrbCount()
        {
            return Mathf.Clamp(runtime.Data.ProjectileCount + runtime.Level / 2 + caster.Stats.ExtraProjectiles, 2, 8);
        }

        private void DamageNearOrb(Vector3 position)
        {
            List<IDamageable> targets = SkillTargeting.FindEnemies(position, 1.15f * caster.Stats.AreaMultiplier, 8);
            for (int i = 0; i < targets.Count; i++)
            {
                float nextAllowedTime;
                if (hitCooldowns.TryGetValue(targets[i], out nextAllowedTime) && Time.time < nextAllowedTime)
                {
                    continue;
                }

                DamageInfo damage = caster.BuildDamage(runtime.Data, 0.36f);
                targets[i].TakeDamage(damage);
                hitCooldowns[targets[i]] = Time.time + 0.45f;

                SynergyManager synergyManager;
                if (ServiceLocator.TryGet(out synergyManager) && synergyManager.HasSynergy(SynergyEffect.BurningAbyss) && Random.value < 0.08f)
                {
                    DamageInfo abyssDamage = caster.BuildDamage(runtime.Data, 0.22f);
                    abyssDamage.ElementTags.Add(SkillTag.Void);
                    SkillEffect.SpawnArea(targets[i].Transform.position, 1.4f, 1.8f, 0.45f, abyssDamage, DamageTarget.Enemies, new Color(0.18f, 0.04f, 0.25f));
                }
            }
        }
    }
}
