using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class AreaDamage : MonoBehaviour, IPoolable
    {
        private DamageInfo damageInfo;
        private DamageTarget target;
        private float radius;
        private float duration;
        private float tickInterval;
        private float durationRemaining;
        private float tickRemaining;
        private bool active;

        public void Configure(DamageInfo info, float areaRadius, float areaDuration, float interval, DamageTarget targetTeam)
        {
            damageInfo = info;
            radius = areaRadius;
            duration = areaDuration;
            tickInterval = Mathf.Max(0.05f, interval);
            durationRemaining = duration;
            tickRemaining = 0f;
            target = targetTeam;
            active = true;
            transform.localScale = new Vector3(radius * 2f, 0.08f, radius * 2f);
        }

        private void Update()
        {
            if (!active)
            {
                return;
            }

            durationRemaining -= Time.deltaTime;
            tickRemaining -= Time.deltaTime;

            if (tickRemaining <= 0f)
            {
                DamageTargets();
                tickRemaining = tickInterval;
            }

            if (durationRemaining <= 0f)
            {
                Despawn();
            }
        }

        private void DamageTargets()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable damageable = DamageUtility.FindDamageable(hits[i], target);
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.TakeDamage(damageInfo.Clone());
                }
            }
        }

        private void Despawn()
        {
            active = false;
            PooledObject pooledObject = GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.Despawn();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnSpawnedFromPool()
        {
            active = false;
        }

        public void OnReturnedToPool()
        {
            active = false;
            damageInfo = null;
        }
    }
}
