using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemyAttack : MonoBehaviour
    {
        private EnemyContactDamage contactDamage;
        private EnemyProjectileAttack projectileAttack;

        private void Awake()
        {
            contactDamage = GetComponent<EnemyContactDamage>();
            if (contactDamage == null)
            {
                contactDamage = gameObject.AddComponent<EnemyContactDamage>();
            }

            projectileAttack = GetComponent<EnemyProjectileAttack>();
            if (projectileAttack == null)
            {
                projectileAttack = gameObject.AddComponent<EnemyProjectileAttack>();
            }
        }

        public void Tick(EnemyController controller, Transform target, float damageMultiplier, float deltaTime)
        {
            if (controller.Data.AttackType == EnemyAttackType.Ranged)
            {
                projectileAttack.Tick(controller, target, damageMultiplier, deltaTime);
            }

            contactDamage.Tick(controller, target, damageMultiplier, deltaTime);
        }
    }
}
