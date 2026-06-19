using UnityEngine;

namespace ArcaneSurvival
{
    public enum EnemyAttackType
    {
        Contact,
        Ranged
    }

    public enum EnemyMovementStyle
    {
        Direct,
        Heavy,
        Flanking
    }

    [CreateAssetMenu(menuName = "Arcane Survival/Enemies/Enemy Data")]
    public sealed class EnemyData : ScriptableObject
    {
        public string EnemyName;
        public float MaxHP = 10f;
        public float MoveSpeed = 3f;
        public float Damage = 6f;
        public float XPDrop = 1f;
        public EnemyAttackType AttackType = EnemyAttackType.Contact;
        public EnemyMovementStyle MovementStyle = EnemyMovementStyle.Direct;
        public GameObject PrefabPlaceholder;
        public float Weight = 1f;
        public string[] SpawnRules;
        public Color BodyColor = Color.white;
    }
}
