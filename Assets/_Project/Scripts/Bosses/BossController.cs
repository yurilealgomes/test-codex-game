using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class BossController : MonoBehaviour, IDamageable, IPoolable
    {
        public static readonly List<BossController> ActiveBosses = new List<BossController>();

        private HealthComponent health;
        private Renderer bodyRenderer;
        private Transform target;
        private Color baseColor;
        private float damageMultiplier;
        private float moveSpeedMultiplier;
        private float attackTimer;
        private float specialTimer;
        private bool deathHandled;
        private bool charging;
        private Vector3 chargeDirection;
        private float chargeRemaining;

        public BossData Data { get; private set; }
        public bool IsAlive { get { return health != null && health.IsAlive; } }
        public Transform Transform { get { return transform; } }
        public float HealthPercent { get { return health == null ? 0f : health.CurrentHealth / health.MaxHealth; } }
        public float CurrentHealth { get { return health != null ? health.CurrentHealth : 0f; } }
        public float MaxHealth { get { return health != null ? health.MaxHealth : 0f; } }
        public string BossName { get { return Data != null ? Data.BossName : "Boss"; } }

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            if (health == null)
            {
                health = gameObject.AddComponent<HealthComponent>();
            }

            health.Damaged += HandleDamaged;
            health.Died += HandleDied;
            bodyRenderer = GetComponentInChildren<Renderer>();
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Damaged -= HandleDamaged;
                health.Died -= HandleDied;
            }
        }

        public void Initialize(BossData data, Transform playerTarget, int bossesDefeated)
        {
            Data = data;
            target = playerTarget;
            damageMultiplier = 1f + bossesDefeated * 0.2f;
            moveSpeedMultiplier = 1f + bossesDefeated * 0.05f;
            deathHandled = false;
            charging = false;
            attackTimer = 1.2f;
            specialTimer = 4f;

            health.Initialize(data.MaxHP * (1f + bossesDefeated * 0.35f));
            baseColor = data.BodyColor;
            transform.localScale = data.Kind == BossKind.RuneGuardian ? Vector3.one * 2.4f : Vector3.one * 1.8f;

            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = baseColor;
            }

            if (!ActiveBosses.Contains(this))
            {
                ActiveBosses.Add(this);
            }
        }

        private void Update()
        {
            if (target == null || Data == null || !IsAlive)
            {
                return;
            }

            if (Data.Kind == BossKind.RuneGuardian)
            {
                TickRuneGuardian();
            }
            else
            {
                TickAstralWitch();
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            health.TakeDamage(damageInfo);
        }

        private void TickRuneGuardian()
        {
            if (charging)
            {
                transform.position += chargeDirection * Data.MoveSpeed * moveSpeedMultiplier * 3.6f * Time.deltaTime;
                chargeRemaining -= Time.deltaTime;
                if (chargeRemaining <= 0f)
                {
                    charging = false;
                    DoMeleePulse(3.2f, 1.15f);
                }

                return;
            }

            MoveTowardTarget(1.1f);
            attackTimer -= Time.deltaTime;
            specialTimer -= Time.deltaTime;

            if (specialTimer <= 0f)
            {
                chargeDirection = MathUtils.DirectionOnPlane(transform.position, target.position);
                charging = true;
                chargeRemaining = 0.75f;
                specialTimer = 6f;
                return;
            }

            if (attackTimer <= 0f && MathUtils.DistanceXZ(transform.position, target.position) < 4f)
            {
                DoMeleePulse(3.8f, 1f);
                attackTimer = 2.6f;
            }
        }

        private void TickAstralWitch()
        {
            float distance = MathUtils.DistanceXZ(transform.position, target.position);
            if (distance < 10f)
            {
                Vector3 away = MathUtils.DirectionOnPlane(target.position, transform.position);
                transform.position += away * Data.MoveSpeed * moveSpeedMultiplier * Time.deltaTime;
            }
            else if (distance > 15f)
            {
                MoveTowardTarget(0.8f);
            }

            attackTimer -= Time.deltaTime;
            specialTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                ShootAtPlayer();
                attackTimer = 1.6f;
            }

            if (specialTimer <= 0f)
            {
                CreateDangerZone();
                TeleportNearPlayer();
                specialTimer = 7.5f;
            }
        }

        private void MoveTowardTarget(float speedScale)
        {
            Vector3 direction = MathUtils.DirectionOnPlane(transform.position, target.position);
            transform.position += direction * Data.MoveSpeed * moveSpeedMultiplier * speedScale * Time.deltaTime;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * 6f);
            }
        }

        private void DoMeleePulse(float radius, float multiplier)
        {
            DamageInfo damage = new DamageInfo
            {
                BaseDamage = Data.Damage,
                FinalDamage = Data.Damage * damageMultiplier * multiplier,
                Source = gameObject
            };
            SkillEffect.SpawnArea(transform.position, radius, 0.18f, 0.05f, damage, DamageTarget.Player, new Color(0.8f, 0.12f, 0.08f));
        }

        private void ShootAtPlayer()
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            Vector3 direction = MathUtils.DirectionOnPlane(transform.position, target.position);
            GameObject projectileObject = poolManager.Spawn("EnemyProjectile", transform.position + Vector3.up * 1.2f, Quaternion.LookRotation(direction));
            if (projectileObject == null)
            {
                return;
            }

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                DamageInfo damage = new DamageInfo
                {
                    BaseDamage = Data.Damage,
                    FinalDamage = Data.Damage * damageMultiplier,
                    Source = gameObject
                };
                projectile.Launch(damage, direction, 12f, 5f, 0.6f, DamageTarget.Player, null);
            }
        }

        private void CreateDangerZone()
        {
            DamageInfo damage = new DamageInfo
            {
                BaseDamage = Data.Damage,
                FinalDamage = Data.Damage * damageMultiplier * 0.5f,
                Source = gameObject
            };
            SkillEffect.SpawnArea(target.position, 3.1f, 3.2f, 0.45f, damage, DamageTarget.Player, new Color(0.48f, 0.15f, 0.85f));
        }

        private void TeleportNearPlayer()
        {
            Vector3 destination = SpawnRingCalculator.CalculatePosition(target.position, 13f, 18f, Camera.main);
            transform.position = destination;
        }

        private void HandleDamaged(DamageInfo damageInfo)
        {
            if (bodyRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashRoutine());
            }
        }

        private void HandleDied(HealthComponent component)
        {
            if (deathHandled)
            {
                return;
            }

            deathHandled = true;
            ActiveBosses.Remove(this);

            RunProgressionManager progression;
            if (ServiceLocator.TryGet(out progression))
            {
                progression.SpawnXp(Data.XPReward, transform.position);
                progression.TryDropBossMagnet(transform.position);
            }

            EventBus.RaiseBossDefeated();

            PoolManager poolManager;
            if (ServiceLocator.TryGet(out poolManager))
            {
                poolManager.Despawn(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator FlashRoutine()
        {
            bodyRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.08f);
            bodyRenderer.material.color = baseColor;
        }

        private void OnDisable()
        {
            ActiveBosses.Remove(this);
        }

        public void OnSpawnedFromPool()
        {
            deathHandled = false;
        }

        public void OnReturnedToPool()
        {
            ActiveBosses.Remove(this);
        }
    }
}
