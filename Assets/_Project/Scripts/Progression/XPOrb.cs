using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class XPOrb : MonoBehaviour, IPoolable
    {
        public static readonly System.Collections.Generic.List<XPOrb> ActiveOrbs = new System.Collections.Generic.List<XPOrb>();

        [SerializeField] private float value = 1f;
        [SerializeField] private float attractionSpeed = 14f;

        private Transform player;
        private Renderer cachedRenderer;
        private float despawnDistance = 60f;
        private bool collected;
        private bool magnetized;

        private void Awake()
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
        }

        public void Initialize(float xpValue, float maxDistance)
        {
            value = Mathf.Max(1f, xpValue);
            despawnDistance = maxDistance;
            collected = false;
            magnetized = false;

            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                player = playerController.transform;
            }

            ApplyVisualTier();
        }

        private void Update()
        {
            if (player == null || collected)
            {
                return;
            }

            float distance = MathUtils.DistanceXZ(transform.position, player.position);
            if (magnetized || distance < 5f)
            {
                float speed = magnetized ? attractionSpeed * 3.8f : attractionSpeed;
                transform.position = Vector3.MoveTowards(transform.position, player.position + Vector3.up * 0.8f, speed * Time.deltaTime);
            }

            if (!magnetized && distance > despawnDistance)
            {
                ReturnToPool();
            }
        }

        public void Collect(PlayerExperience experience)
        {
            if (collected)
            {
                return;
            }

            collected = true;
            if (experience != null)
            {
                experience.AddExperience(value);
            }

            if (value >= 7f)
            {
                SkillEffect.SpawnVfx(transform.position, GetTierColor(value), Vector3.one * 0.55f, 0.16f);
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
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
            collected = false;
            if (!ActiveOrbs.Contains(this))
            {
                ActiveOrbs.Add(this);
            }
        }

        public void OnReturnedToPool()
        {
            collected = false;
            magnetized = false;
            ActiveOrbs.Remove(this);
        }

        public void ForceMagnetize(Transform target)
        {
            if (target != null)
            {
                player = target;
            }

            magnetized = true;
        }

        public static void PullAllTo(Transform target)
        {
            for (int i = 0; i < ActiveOrbs.Count; i++)
            {
                XPOrb orb = ActiveOrbs[i];
                if (orb != null)
                {
                    orb.ForceMagnetize(target);
                }
            }
        }

        private void ApplyVisualTier()
        {
            if (cachedRenderer == null)
            {
                return;
            }

            cachedRenderer.material.color = GetTierColor(value);
            float scale = value >= 20f ? 0.72f : value >= 8f ? 0.6f : value >= 4f ? 0.52f : 0.45f;
            transform.localScale = Vector3.one * scale;
        }

        private static Color GetTierColor(float xpValue)
        {
            if (xpValue >= 35f)
            {
                return new Color(1f, 0.74f, 0.18f);
            }

            if (xpValue >= 16f)
            {
                return new Color(0.72f, 0.28f, 1f);
            }

            if (xpValue >= 7f)
            {
                return new Color(0.2f, 0.48f, 1f);
            }

            if (xpValue >= 3f)
            {
                return new Color(0.24f, 0.86f, 0.32f);
            }

            return Color.white;
        }
    }
}
