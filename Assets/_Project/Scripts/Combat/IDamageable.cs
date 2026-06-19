using UnityEngine;

namespace ArcaneSurvival
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        Transform Transform { get; }
        void TakeDamage(DamageInfo damageInfo);
    }
}
