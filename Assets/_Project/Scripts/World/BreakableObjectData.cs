using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/World/Breakable Object Data")]
    public sealed class BreakableObjectData : ScriptableObject
    {
        public string ObjectName = "Arcane Crystal";
        public float MaxHealth = 28f;
        public float XpDrop = 2f;
        public Color BaseColor = new Color(0.35f, 0.75f, 1f);
        public Vector3 MinScale = new Vector3(0.35f, 0.8f, 0.35f);
        public Vector3 MaxScale = new Vector3(0.7f, 1.55f, 0.7f);
    }
}
