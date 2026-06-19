using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/Synergies/Synergy Data")]
    public sealed class SynergyData : ScriptableObject
    {
        public string SynergyName;
        [TextArea] public string Description;
        public SkillTag[] RequiredTags;
        public string[] RequiredSkillLevels;
        public SynergyEffect EffectType;
        public Color VisualFeedback = Color.white;
        public int Priority;
        public bool AllowStacking;
    }
}
