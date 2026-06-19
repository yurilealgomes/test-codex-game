using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SimpleVFX : MonoBehaviour, IPoolable
    {
        private Renderer cachedRenderer;
        private float remaining;
        private float lifetime;
        private Color color;

        private void Awake()
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
        }

        public void Play(Color visualColor, Vector3 scale, float duration)
        {
            color = visualColor;
            lifetime = Mathf.Max(0.05f, duration);
            remaining = lifetime;
            transform.localScale = scale;
            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = color;
            }
        }

        private void Update()
        {
            remaining -= Time.deltaTime;
            transform.Rotate(Vector3.up, 180f * Time.deltaTime, Space.World);
            if (cachedRenderer != null)
            {
                float alpha = Mathf.Clamp01(remaining / lifetime);
                cachedRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
            }

            if (remaining <= 0f)
            {
                PooledObject pooledObject = GetComponent<PooledObject>();
                if (pooledObject != null)
                {
                    pooledObject.Despawn();
                }
            }
        }

        public void OnSpawnedFromPool()
        {
            remaining = lifetime;
        }

        public void OnReturnedToPool()
        {
        }
    }
}
