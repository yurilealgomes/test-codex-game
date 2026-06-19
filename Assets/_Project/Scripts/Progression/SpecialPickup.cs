using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SpecialPickup : MonoBehaviour, IPoolable
    {
        private PickupData data;
        private Renderer[] renderers;
        private bool collected;
        private Vector3 spawnPosition;
        private Vector3 baseScale;
        private float age;

        public PickupType Type { get { return data != null ? data.Type : PickupType.Magnet; } }

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
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
                ApplyVisualColor(data.VisualColor);
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
    }
}
