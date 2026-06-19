using UnityEngine;

namespace ArcaneSurvival
{
    public enum BossKind
    {
        RuneGuardian,
        AstralWitch
    }

    [CreateAssetMenu(menuName = "Arcane Survival/Bosses/Boss Data")]
    public sealed class BossData : ScriptableObject
    {
        public string BossName;
        public BossKind Kind;
        public float MaxHP = 600f;
        public float Damage = 18f;
        public float MoveSpeed = 3f;
        public string[] AttackPatterns;
        public float XPReward = 35f;
        public float UpgradeRewardChance = 0.3f;
        public string[] ScalingRules;
        public GameObject PrefabPlaceholder;
        public Color BodyColor = Color.white;
    }
}
