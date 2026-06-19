using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemyController : MonoBehaviour, IPoolable
    {
        public static readonly List<EnemyController> ActiveEnemies = new List<EnemyController>();

        private readonly List<StatusEffect> statuses = new List<StatusEffect>();
        private EnemyMovement movement;
        private EnemyAttack attack;
        private Renderer bodyRenderer;
        private Color baseColor;
        private Transform target;
        private float damageMultiplier = 1f;
        private float speedMultiplier = 1f;
        private bool deathHandled;

        public EnemyData Data { get; private set; }
        public EnemyHealth Health { get; private set; }
        public bool IsAlive { get { return Health != null && Health.IsAlive; } }
        public bool IsSlowed { get; private set; }
        public float CurrentMoveSpeed { get { return Data.MoveSpeed * speedMultiplier * (IsSlowed ? 0.55f : 1f); } }

        private void Awake()
        {
            Health = GetComponent<EnemyHealth>();
            if (Health == null)
            {
                Health = gameObject.AddComponent<EnemyHealth>();
            }

            movement = GetComponent<EnemyMovement>();
            if (movement == null)
            {
                movement = gameObject.AddComponent<EnemyMovement>();
            }

            attack = GetComponent<EnemyAttack>();
            if (attack == null)
            {
                attack = gameObject.AddComponent<EnemyAttack>();
            }

            bodyRenderer = GetComponentInChildren<Renderer>();
        }

        public void Initialize(EnemyData data, Transform playerTarget, float hpMultiplier, float damageScale, float speedScale, bool elite)
        {
            Data = data;
            target = playerTarget;
            damageMultiplier = damageScale * (elite ? 1.35f : 1f);
            speedMultiplier = speedScale * (elite ? 1.08f : 1f);
            deathHandled = false;
            statuses.Clear();
            IsSlowed = false;

            float maxHealth = data.MaxHP * hpMultiplier * (elite ? 2.4f : 1f);
            Health.Initialize(this, maxHealth);
            movement.Initialize(this, target);

            transform.localScale = elite ? Vector3.one * 1.35f : Vector3.one;
            baseColor = elite ? Color.Lerp(data.BodyColor, Color.white, 0.35f) : data.BodyColor;
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = baseColor;
            }

            if (!ActiveEnemies.Contains(this))
            {
                ActiveEnemies.Add(this);
            }
        }

        private void Update()
        {
            UpdateStatuses(Time.deltaTime);
            movement.Tick(Time.deltaTime);
            attack.Tick(this, target, damageMultiplier, Time.deltaTime);
        }

        private void OnDisable()
        {
            ActiveEnemies.Remove(this);
        }

        public void ApplyStatus(StatusEffect effect)
        {
            if (effect != null)
            {
                statuses.Add(effect.Clone());
            }
        }

        public void HandleDamaged()
        {
            if (bodyRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashRoutine());
            }
        }

        public void HandleDeath()
        {
            if (deathHandled)
            {
                return;
            }

            deathHandled = true;
            ActiveEnemies.Remove(this);

            RunProgressionManager progression;
            if (ServiceLocator.TryGet(out progression))
            {
                progression.RegisterEnemyDefeated(Data.XPDrop, transform.position);
            }

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

        private void UpdateStatuses(float deltaTime)
        {
            IsSlowed = false;
            for (int i = statuses.Count - 1; i >= 0; i--)
            {
                StatusEffect status = statuses[i];
                status.Duration -= deltaTime;
                if (status.Duration <= 0f)
                {
                    statuses.RemoveAt(i);
                    continue;
                }

                if (status.Type == StatusEffectType.Slow)
                {
                    IsSlowed = true;
                }
                else if (status.Type == StatusEffectType.Pull && target != null)
                {
                    Vector3 pullDirection = MathUtils.DirectionOnPlane(transform.position, target.position);
                    transform.position += pullDirection * status.Magnitude * deltaTime;
                }
            }
        }

        private System.Collections.IEnumerator FlashRoutine()
        {
            bodyRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.08f);
            bodyRenderer.material.color = baseColor;
        }

        public void OnSpawnedFromPool()
        {
            deathHandled = false;
        }

        public void OnReturnedToPool()
        {
            statuses.Clear();
            IsSlowed = false;
            ActiveEnemies.Remove(this);
        }
    }
}
