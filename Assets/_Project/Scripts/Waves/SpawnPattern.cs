using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/Waves/Spawn Pattern")]
    public sealed class SpawnPattern : ScriptableObject
    {
        public string PatternName = "Default Swarm";
        public float SpawnInterval = 1.2f;
        public int BatchSize = 3;
        public float SpawnRadiusMin = 24f;
        public float SpawnRadiusMax = 36f;
    }
}
