using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/World/World Chunk Data")]
    public sealed class WorldChunkData : ScriptableObject
    {
        public Color PrimaryGroundColor = new Color(0.16f, 0.2f, 0.18f);
        public Color SecondaryGroundColor = new Color(0.12f, 0.16f, 0.17f);
        public Color DecorationColor = new Color(0.28f, 0.32f, 0.3f);
        public int DecorationsPerChunk = 5;
        public int BreakablesPerChunk = 3;
        public BreakableObjectData[] BreakableObjects;
    }
}
