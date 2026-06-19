using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class ChainLightningEffect : MonoBehaviour, IPoolable
    {
        private const int SegmentCount = 7;

        private LineRenderer lineRenderer;
        private float remaining;
        private float lifetime;
        private Color color;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = SegmentCount + 1;
            lineRenderer.widthMultiplier = 0.16f;
        }

        public void Play(Vector3 start, Vector3 end, Color visualColor, float duration)
        {
            color = visualColor;
            lifetime = Mathf.Max(0.05f, duration);
            remaining = lifetime;
            lineRenderer.enabled = true;
            lineRenderer.startColor = color;
            lineRenderer.endColor = Color.white;

            Vector3 direction = end - start;
            Vector3 side = Vector3.Cross(direction.normalized, Vector3.up);
            if (side.sqrMagnitude < 0.001f)
            {
                side = Vector3.right;
            }

            for (int i = 0; i <= SegmentCount; i++)
            {
                float t = i / (float)SegmentCount;
                Vector3 point = Vector3.Lerp(start, end, t);
                if (i > 0 && i < SegmentCount)
                {
                    float jitter = Random.Range(-0.45f, 0.45f);
                    point += side.normalized * jitter + Vector3.up * Random.Range(-0.18f, 0.18f);
                }

                lineRenderer.SetPosition(i, point);
            }
        }

        private void Update()
        {
            remaining -= Time.deltaTime;
            float alpha = Mathf.Clamp01(remaining / lifetime);
            lineRenderer.startColor = new Color(color.r, color.g, color.b, alpha);
            lineRenderer.endColor = new Color(1f, 1f, 1f, alpha);
            lineRenderer.widthMultiplier = Mathf.Lerp(0.03f, 0.16f, alpha);

            if (remaining <= 0f)
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
        }

        public void OnSpawnedFromPool()
        {
            remaining = lifetime;
        }

        public void OnReturnedToPool()
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
