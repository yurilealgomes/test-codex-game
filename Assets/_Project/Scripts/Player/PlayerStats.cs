using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerStats : MonoBehaviour
    {
        public float MaxHP = 100f;
        public float MoveSpeed = 6f;
        public float PickupRadius = 3f;
        public float GlobalDamageMultiplier = 1f;
        public float CooldownReduction = 0f;
        public int ExtraProjectiles = 0;
        public float AreaMultiplier = 1f;
        public float DurationMultiplier = 1f;
        public float CriticalChance = 0.05f;
        public float CriticalMultiplier = 1.5f;
        public float Armor = 0f;
        public float Regeneration = 0f;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public DamageInfo BuildDamage(float baseDamage, bool canCrit, GameObject source, IEnumerable<SkillTag> tags)
        {
            bool critical = canCrit && Random.value <= CriticalChance;
            float finalDamage = baseDamage * GlobalDamageMultiplier * (critical ? CriticalMultiplier : 1f);

            DamageInfo info = new DamageInfo
            {
                BaseDamage = baseDamage,
                FinalDamage = finalDamage,
                IsCritical = critical,
                Source = source
            };

            if (tags != null)
            {
                info.ElementTags.AddRange(tags);
            }

            return info;
        }

        public float ApplyCooldownReduction(float cooldown)
        {
            return Mathf.Max(0.1f, cooldown * (1f - Mathf.Clamp(CooldownReduction, 0f, 0.75f)));
        }
    }
}
