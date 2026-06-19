using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class DamageInfo
    {
        public float BaseDamage;
        public float FinalDamage;
        public bool IsCritical;
        public GameObject Source;
        public Vector3 Knockback;
        public Color DisplayColor = Color.white;
        public readonly List<SkillTag> ElementTags = new List<SkillTag>();
        public readonly List<StatusEffect> StatusEffects = new List<StatusEffect>();

        public DamageInfo Clone()
        {
            DamageInfo clone = new DamageInfo
            {
                BaseDamage = BaseDamage,
                FinalDamage = FinalDamage,
                IsCritical = IsCritical,
                Source = Source,
                Knockback = Knockback,
                DisplayColor = DisplayColor
            };

            clone.ElementTags.AddRange(ElementTags);
            for (int i = 0; i < StatusEffects.Count; i++)
            {
                clone.StatusEffects.Add(StatusEffects[i].Clone());
            }

            return clone;
        }
    }
}
