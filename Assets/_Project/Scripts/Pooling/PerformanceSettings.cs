using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/Settings/Performance Settings")]
    public sealed class PerformanceSettings : ScriptableObject
    {
        public int MaxAliveEnemies = 520;
        public int MaxAliveProjectiles = 500;
        public int MaxFloatingDamageTexts = 80;
        public int MaxVfx = 120;
        public float EnemyDespawnDistance = 70f;
        public float XpOrbDespawnDistance = 60f;
    }
}
