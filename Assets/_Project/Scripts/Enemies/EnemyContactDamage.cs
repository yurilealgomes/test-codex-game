using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemyContactDamage : MonoBehaviour
    {
        [SerializeField] private float attackRange = 1.35f;
        [SerializeField] private float cooldown = 0.75f;
        private float cooldownRemaining;

        public void Tick(EnemyController controller, Transform target, float damageMultiplier, float deltaTime)
        {
            if (target == null || controller == null || !controller.IsAlive)
            {
                return;
            }

            cooldownRemaining -= deltaTime;
            if (cooldownRemaining > 0f)
            {
                return;
            }

            if (MathUtils.DistanceXZ(transform.position, target.position) > attackRange)
            {
                return;
            }

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                DamageInfo damage = new DamageInfo
                {
                    BaseDamage = controller.Data.Damage,
                    FinalDamage = controller.Data.Damage * damageMultiplier,
                    Source = gameObject
                };
                playerHealth.TakeDamage(damage);
                cooldownRemaining = cooldown;
            }
        }
    }
}
