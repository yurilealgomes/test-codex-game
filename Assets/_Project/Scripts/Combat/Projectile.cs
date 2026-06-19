using System;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class Projectile : MonoBehaviour, IPoolable
    {
        private DamageInfo damageInfo;
        private DamageTarget target;
        private Action<Vector3, IDamageable> onHit;
        private Vector3 direction;
        private float speed;
        private float radius;
        private float lifeRemaining;
        private bool active;

        public void Launch(DamageInfo info, Vector3 moveDirection, float projectileSpeed, float lifetime, float hitRadius, DamageTarget targetTeam, Action<Vector3, IDamageable> hitCallback)
        {
            damageInfo = info;
            direction = MathUtils.Flatten(moveDirection).normalized;
            speed = projectileSpeed;
            lifeRemaining = lifetime;
            radius = hitRadius;
            target = targetTeam;
            onHit = hitCallback;
            active = true;
        }

        private void Update()
        {
            if (!active)
            {
                return;
            }

            transform.position += direction * speed * Time.deltaTime;
            lifeRemaining -= Time.deltaTime;
            TryHit();

            if (lifeRemaining <= 0f)
            {
                Despawn();
            }
        }

        private void TryHit()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable damageable = DamageUtility.FindDamageable(hits[i], target);
                if (damageable == null || !damageable.IsAlive)
                {
                    continue;
                }

                if (damageInfo.Source != null && damageable.Transform == damageInfo.Source.transform)
                {
                    continue;
                }

                damageable.TakeDamage(damageInfo.Clone());
                if (onHit != null)
                {
                    onHit.Invoke(transform.position, damageable);
                }

                Despawn();
                return;
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
            onHit = null;
            damageInfo = null;
        }
    }
}
