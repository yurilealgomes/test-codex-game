using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/Upgrades/Upgrade Data")]
    public sealed class UpgradeData : ScriptableObject
    {
        public string UpgradeName;
        [TextArea] public string Description;
        public Sprite Icon;
        public UpgradeRarity Rarity;
        public UpgradeEffect Effect;
        public float Amount = 0.1f;
        public string TargetSkillName;
        public SkillTag ElementTag;
        public string AffectedSkillLabel;
    }
}
