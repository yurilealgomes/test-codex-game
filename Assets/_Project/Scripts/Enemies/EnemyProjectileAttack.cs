using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemyProjectileAttack : MonoBehaviour
    {
        [SerializeField] private float range = 13f;
        [SerializeField] private float cooldown = 2.1f;
        [SerializeField] private float projectileSpeed = 9f;
        private float cooldownRemaining;

        public void Tick(EnemyController controller, Transform target, float damageMultiplier, float deltaTime)
        {
            if (target == null || controller == null || !controller.IsAlive)
            {
                return;
            }

            cooldownRemaining -= deltaTime;
            float distance = MathUtils.DistanceXZ(transform.position, target.position);
            if (distance > range || cooldownRemaining > 0f)
            {
                return;
            }

            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            Vector3 direction = MathUtils.DirectionOnPlane(transform.position, target.position);
            GameObject projectileObject = poolManager.Spawn("EnemyProjectile", transform.position + Vector3.up * 0.8f, Quaternion.LookRotation(direction));
            if (projectileObject == null)
            {
                return;
            }

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                DamageInfo damage = new DamageInfo
                {
                    BaseDamage = controller.Data.Damage,
                    FinalDamage = controller.Data.Damage * damageMultiplier,
                    Source = gameObject
                };
                projectile.Launch(damage, direction, projectileSpeed, 4f, 0.5f, DamageTarget.Player, null);
            }

            cooldownRemaining = cooldown;
        }
    }
}
