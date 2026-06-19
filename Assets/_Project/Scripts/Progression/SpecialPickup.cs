using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SpecialPickup : MonoBehaviour, IPoolable
    {
        private PickupData data;
        private Renderer cachedRenderer;
        private bool collected;

        public PickupType Type { get { return data != null ? data.Type : PickupType.Magnet; } }

        private void Awake()
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
        }

        public void Initialize(PickupData pickupData)
        {
            data = pickupData;
            collected = false;
            if (cachedRenderer != null && data != null)
            {
                cachedRenderer.material.color = data.VisualColor;
            }
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, 120f * Time.deltaTime, Space.World);
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
        }

        public void OnReturnedToPool()
        {
            collected = false;
        }
    }
}
