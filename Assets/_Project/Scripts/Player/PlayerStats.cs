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
        public float Luck = 0f;
        public int ExtraChainCount = 0;
        public float ChainRadiusBonus = 0f;

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
                Source = source,
                DisplayColor = GetDamageColor(tags, critical)
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

        private static Color GetDamageColor(IEnumerable<SkillTag> tags, bool critical)
        {
            if (critical)
            {
                return new Color(1f, 0.84f, 0.18f);
            }

            if (tags == null)
            {
                return new Color(0.92f, 0.88f, 1f);
            }

            foreach (SkillTag tag in tags)
            {
                switch (tag)
                {
                    case SkillTag.Fire: return new Color(1f, 0.36f, 0.12f);
                    case SkillTag.Ice: return new Color(0.48f, 0.9f, 1f);
                    case SkillTag.Lightning: return new Color(1f, 0.9f, 0.22f);
                    case SkillTag.Void: return new Color(0.72f, 0.32f, 1f);
                    case SkillTag.Nature: return new Color(0.3f, 0.92f, 0.42f);
                    case SkillTag.Arcane: return new Color(0.86f, 0.58f, 1f);
                }
            }

            return new Color(0.92f, 0.88f, 1f);
        }
    }
}
