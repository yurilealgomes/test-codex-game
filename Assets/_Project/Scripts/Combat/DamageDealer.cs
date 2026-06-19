using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class DamageDealer : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float radius = 1f;
        [SerializeField] private float cooldown = 0.75f;
        [SerializeField] private DamageTarget target = DamageTarget.Enemies;

        private float cooldownRemaining;

        private void Update()
        {
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining -= Time.deltaTime;
                return;
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable damageable = DamageUtility.FindDamageable(hits[i], target);
                if (damageable == null)
                {
                    continue;
                }

                DamageInfo info = new DamageInfo
                {
                    BaseDamage = damage,
                    FinalDamage = damage,
                    Source = gameObject
                };
                damageable.TakeDamage(info);
                cooldownRemaining = cooldown;
                return;
            }
        }
    }
}
