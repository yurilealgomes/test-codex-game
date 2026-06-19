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
        private LineRenderer beaconRenderer;
        private Light beaconLight;
        private UpgradeRarity rarity = UpgradeRarity.Common;
        private float despawnDistance = 60f;
        private bool collected;
        private bool magnetized;
        private float age;

        private void Awake()
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
            beaconRenderer = GetComponentInChildren<LineRenderer>(true);
            beaconLight = GetComponentInChildren<Light>(true);
        }

        public void Initialize(float xpValue, float maxDistance)
        {
            value = Mathf.Max(1f, xpValue);
            despawnDistance = maxDistance;
            collected = false;
            magnetized = false;
            age = 0f;

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

            age += Time.deltaTime;
            float bob = Mathf.Sin(age * 4.5f) * 0.07f;
            Transform visual = cachedRenderer != null ? cachedRenderer.transform : transform;
            visual.localPosition = Vector3.up * bob;

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
                SkillEffect.SpawnVfx(transform.position, UpgradeRarityUtility.GetColor(rarity), Vector3.one * 0.55f, 0.16f);
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
            age = 0f;
            if (!ActiveOrbs.Contains(this))
            {
                ActiveOrbs.Add(this);
            }
        }

        public void OnReturnedToPool()
        {
            collected = false;
            magnetized = false;
            age = 0f;
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

            rarity = GetRarity(value);
            Color tierColor = UpgradeRarityUtility.GetColor(rarity);
            cachedRenderer.material.color = tierColor;
            cachedRenderer.transform.localPosition = Vector3.zero;

            float scale = rarity == UpgradeRarity.Legendary ? 0.82f
                : rarity == UpgradeRarity.Epic ? 0.70f
                : rarity == UpgradeRarity.Magic ? 0.60f
                : rarity == UpgradeRarity.Uncommon ? 0.52f
                : 0.44f;
            transform.localScale = Vector3.one * scale;

            ApplyBeacon(tierColor);
        }

        private void ApplyBeacon(Color tierColor)
        {
            if (beaconRenderer != null)
            {
                bool showStrongBeacon = rarity >= UpgradeRarity.Magic;
                beaconRenderer.enabled = true;
                beaconRenderer.startColor = new Color(tierColor.r, tierColor.g, tierColor.b, showStrongBeacon ? 0.9f : 0.45f);
                beaconRenderer.endColor = new Color(tierColor.r, tierColor.g, tierColor.b, 0f);
                beaconRenderer.startWidth = showStrongBeacon ? 0.08f : 0.035f;
                beaconRenderer.endWidth = showStrongBeacon ? 0.24f : 0.08f;
            }

            if (beaconLight != null)
            {
                beaconLight.enabled = rarity >= UpgradeRarity.Magic;
                beaconLight.color = tierColor;
                beaconLight.range = rarity >= UpgradeRarity.Epic ? 7f : rarity >= UpgradeRarity.Magic ? 5f : 3f;
                beaconLight.intensity = rarity >= UpgradeRarity.Epic ? 2.1f : rarity >= UpgradeRarity.Magic ? 1.35f : 0.65f;
            }
        }

        private static UpgradeRarity GetRarity(float xpValue)
        {
            if (xpValue >= 28f)
            {
                return UpgradeRarity.Legendary;
            }

            if (xpValue >= 14f)
            {
                return UpgradeRarity.Epic;
            }

            if (xpValue >= 7f)
            {
                return UpgradeRarity.Magic;
            }

            if (xpValue >= 3f)
            {
                return UpgradeRarity.Uncommon;
            }

            return UpgradeRarity.Common;
        }
    }
}
