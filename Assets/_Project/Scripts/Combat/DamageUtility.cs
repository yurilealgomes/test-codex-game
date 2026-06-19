using UnityEngine;

namespace ArcaneSurvival
{
    public enum DamageTarget
    {
        Enemies,
        Player
    }

    public static class DamageUtility
    {
        public static IDamageable FindDamageable(Collider collider, DamageTarget target)
        {
            if (collider == null)
            {
                return null;
            }

            if (target == DamageTarget.Player)
            {
                return collider.GetComponentInParent<PlayerHealth>();
            }

            EnemyHealth enemyHealth = collider.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                return enemyHealth;
            }

            BossController bossController = collider.GetComponentInParent<BossController>();
            return bossController;
        }
    }
}
