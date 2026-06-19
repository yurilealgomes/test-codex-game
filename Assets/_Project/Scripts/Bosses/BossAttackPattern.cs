using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/Bosses/Boss Attack Pattern")]
    public sealed class BossAttackPattern : ScriptableObject
    {
        public string PatternName;
        public float Cooldown = 3f;
        public float Range = 10f;
        public float DamageMultiplier = 1f;
        public float TelegraphSeconds = 0.4f;
    }
}
