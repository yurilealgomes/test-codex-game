using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SpecialPickup : MonoBehaviour, IPoolable
    {
        private PickupData data;
        private Renderer[] renderers;
        private Transform magnetRing;
        private Transform magnetLeftPole;
        private Transform magnetRightPole;
        private Transform healVerticalBar;
        private Transform healHorizontalBar;
        private LineRenderer beaconRenderer;
        private Light beaconLight;
        private bool collected;
        private Vector3 spawnPosition;
        private Vector3 baseScale;
        private float age;

        public PickupType Type { get { return data != null ? data.Type : PickupType.Magnet; } }
        public float Value { get { return data != null ? data.Value : 0f; } }

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
            magnetRing = transform.Find("Magnet Ring");
            magnetLeftPole = transform.Find("Magnet Left Pole");
            magnetRightPole = transform.Find("Magnet Right Pole");
            healVerticalBar = transform.Find("Heal Vertical Bar");
            healHorizontalBar = transform.Find("Heal Horizontal Bar");
            beaconRenderer = GetComponentInChildren<LineRenderer>(true);
            beaconLight = GetComponentInChildren<Light>(true);
            baseScale = transform.localScale;
        }

        public void Initialize(PickupData pickupData)
        {
            data = pickupData;
            collected = false;
            spawnPosition = transform.position;
            age = 0f;
            if (data != null)
            {
                ApplyShape(data.Type);
                ApplyVisualColor(data.VisualColor);
                ApplyBeacon(data.VisualColor);
            }
        }

        private void Update()
        {
            age += Time.deltaTime;
            transform.Rotate(Vector3.up, 120f * Time.deltaTime, Space.World);
            transform.position = spawnPosition + Vector3.up * (Mathf.Sin(age * 4f) * 0.12f);
            transform.localScale = baseScale * (1f + Mathf.Sin(age * 6f) * 0.06f);
        }

        public void Collect()
        {
            if (collected)
            {
                return;
            }

            collected = true;
            PickupManager pickupManager;
            if (ServiceLocator.TryGet(out pickupManager))
            {
                pickupManager.ApplyPickup(this);
            }

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
            spawnPosition = transform.position;
            age = 0f;
        }

        public void OnReturnedToPool()
        {
            collected = false;
            transform.localScale = baseScale;
        }

        private void ApplyVisualColor(Color color)
        {
            if (renderers == null || renderers.Length == 0)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                {
                    continue;
                }

                float tint = i == 0 ? 0f : i % 2 == 0 ? 0.45f : 0.72f;
                renderers[i].material.color = Color.Lerp(color, Color.white, tint);
            }
        }

        private void ApplyShape(PickupType type)
        {
            bool heal = type == PickupType.Heal;
            SetActive(magnetRing, !heal);
            SetActive(magnetLeftPole, !heal);
            SetActive(magnetRightPole, !heal);
            SetActive(healVerticalBar, heal);
            SetActive(healHorizontalBar, heal);
        }

        private void ApplyBeacon(Color color)
        {
            if (beaconRenderer != null)
            {
                beaconRenderer.enabled = true;
                beaconRenderer.startColor = new Color(color.r, color.g, color.b, 0.95f);
                beaconRenderer.endColor = new Color(color.r, color.g, color.b, 0f);
                beaconRenderer.startWidth = 0.09f;
                beaconRenderer.endWidth = 0.32f;
            }

            if (beaconLight != null)
            {
                beaconLight.color = color;
                beaconLight.range = 7f;
                beaconLight.intensity = 2.2f;
            }
        }

        private static void SetActive(Transform target, bool active)
        {
            if (target != null)
            {
                target.gameObject.SetActive(active);
            }
        }
    }
}
