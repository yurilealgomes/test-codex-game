using System;
using UnityEngine;

namespace ArcaneSurvival
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        public event Action<DamageInfo> Damaged;
        public event Action<HealthComponent> Died;

        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float currentHealth = 10f;

        public bool IsAlive { get { return currentHealth > 0f; } }
        public float CurrentHealth { get { return currentHealth; } }
        public float MaxHealth { get { return maxHealth; } }
        public Transform Transform { get { return transform; } }

        public void Initialize(float newMaxHealth)
        {
            maxHealth = Mathf.Max(1f, newMaxHealth);
            currentHealth = maxHealth;
        }

        public virtual void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive || damageInfo == null)
            {
                return;
            }

            float amount = Mathf.Max(0f, damageInfo.FinalDamage);
            currentHealth = Mathf.Max(0f, currentHealth - amount);
            SpawnFloatingText(amount, damageInfo.IsCritical);

            if (Damaged != null)
            {
                Damaged.Invoke(damageInfo);
            }

            if (currentHealth <= 0f && Died != null)
            {
                Died.Invoke(this);
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive)
            {
                return;
            }

            currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Max(0f, amount));
        }

        private void SpawnFloatingText(float amount, bool critical)
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            GameObject textObject = poolManager.Spawn("FloatingDamageText", transform.position + Vector3.up * 1.8f, Quaternion.identity);
            if (textObject == null)
            {
                return;
            }

            FloatingDamageText floatingText = textObject.GetComponent<FloatingDamageText>();
            if (floatingText != null)
            {
                floatingText.Play(amount, critical);
            }
        }
    }
}
