using UnityEngine;

namespace ArcaneSurvival
{
    public enum SkillType
    {
        Active,
        Passive
    }

    public enum SkillTargetingMode
    {
        NearestEnemy,
        AroundPlayer,
        GroundAtEnemy,
        Chain,
        NearbyEnemies
    }

    public enum SkillEffectKind
    {
        ArcaneBolt,
        FlameOrbit,
        IceNova,
        LightningChain,
        VoidZone,
        NatureSpikes
    }

    [CreateAssetMenu(menuName = "Arcane Survival/Skills/Skill Data")]
    public sealed class SkillData : ScriptableObject
    {
        public string SkillName;
        [TextArea] public string Description;
        public Sprite Icon;
        public SkillType SkillType;
        public SkillEffectKind EffectKind;
        public SkillTag[] ElementTags;
        public float BaseDamage = 10f;
        public float Cooldown = 1f;
        public float Range = 14f;
        public float Area = 2f;
        public float Duration = 1f;
        public int ProjectileCount = 1;
        public float ProjectileSpeed = 14f;
        public int ChainCount = 3;
        public float ChainRadius = 8f;
        public bool CanCrit = true;
        public SkillTargetingMode TargetingMode;
        public string[] UpgradePool;
        public string[] SynergyRules;
        public Color VisualColor = Color.white;

        public bool HasTag(SkillTag tag)
        {
            if (ElementTags == null)
            {
                return false;
            }

            for (int i = 0; i < ElementTags.Length; i++)
            {
                if (ElementTags[i] == tag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
