using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        private PlayerStats stats;
        private PlayerVisualController visualController;
        private float currentHealth;
        private GameStateManager stateManager;

        public bool IsAlive { get { return currentHealth > 0f; } }
        public bool IsInvulnerable { get; private set; }
        public Transform Transform { get { return transform; } }
        public float CurrentHealth { get { return currentHealth; } }

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            visualController = GetComponent<PlayerVisualController>();
            currentHealth = stats.MaxHP;
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
            EventBus.RaisePlayerHealthChanged(currentHealth, stats.MaxHP);
        }

        private void Update()
        {
            if (stateManager == null || !stateManager.IsGameplayRunning || !IsAlive || stats.Regeneration <= 0f)
            {
                return;
            }

            currentHealth = Mathf.Min(stats.MaxHP, currentHealth + stats.Regeneration * Time.deltaTime);
            EventBus.RaisePlayerHealthChanged(currentHealth, stats.MaxHP);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive || damageInfo == null)
            {
                return;
            }

            if (IsInvulnerable)
            {
                EventBus.RaisePlayerHealthChanged(currentHealth, stats.MaxHP);
                return;
            }

            float mitigatedDamage = Mathf.Max(1f, damageInfo.FinalDamage - stats.Armor);
            currentHealth = Mathf.Max(0f, currentHealth - mitigatedDamage);
            EventBus.RaisePlayerHealthChanged(currentHealth, stats.MaxHP);

            if (visualController != null)
            {
                visualController.FlashDamage();
            }

            CameraShake shake;
            if (ServiceLocator.TryGet(out shake))
            {
                shake.Shake(0.08f, 0.12f);
            }

            if (currentHealth <= 0f)
            {
                EventBus.RaiseGameOver();
            }
        }

        public void HealToFull()
        {
            currentHealth = stats.MaxHP;
            EventBus.RaisePlayerHealthChanged(currentHealth, stats.MaxHP);
        }

        public void SetInvulnerable(bool invulnerable)
        {
            IsInvulnerable = invulnerable;
            if (invulnerable)
            {
                HealToFull();
            }
            else
            {
                EventBus.RaisePlayerHealthChanged(currentHealth, stats.MaxHP);
            }
        }
    }
}
