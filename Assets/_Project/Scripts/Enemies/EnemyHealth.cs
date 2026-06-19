using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        private HealthComponent health;

        public EnemyController Controller { get; private set; }
        public bool IsAlive { get { return health != null && health.IsAlive; } }
        public Transform Transform { get { return transform; } }
        public float CurrentHealth { get { return health != null ? health.CurrentHealth : 0f; } }
        public float MaxHealth { get { return health != null ? health.MaxHealth : 1f; } }

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            if (health == null)
            {
                health = gameObject.AddComponent<HealthComponent>();
            }

            health.Damaged += HandleDamaged;
            health.Died += HandleDied;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Damaged -= HandleDamaged;
                health.Died -= HandleDied;
            }
        }

        public void Initialize(EnemyController controller, float maxHealth)
        {
            Controller = controller;
            health.Initialize(maxHealth);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (Controller != null && damageInfo != null)
            {
                for (int i = 0; i < damageInfo.StatusEffects.Count; i++)
                {
                    Controller.ApplyStatus(damageInfo.StatusEffects[i]);
                }
            }

            health.TakeDamage(damageInfo);
        }

        private void HandleDamaged(DamageInfo damageInfo)
        {
            if (Controller != null)
            {
                Controller.HandleDamaged();
            }
        }

        private void HandleDied(HealthComponent component)
        {
            if (Controller != null)
            {
                Controller.HandleDeath();
            }
        }
    }
}
