using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class BreakableBreakEffect : MonoBehaviour, IPoolable
    {
        private const float Lifetime = 0.32f;

        private Renderer[] renderers;
        private Vector3[] baseLocalPositions;
        private Vector3[] directions;
        private float remaining;
        private Color color;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
            baseLocalPositions = new Vector3[renderers.Length];
            directions = new Vector3[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                baseLocalPositions[i] = renderers[i].transform.localPosition;
                directions[i] = Random.onUnitSphere;
                directions[i].y = Mathf.Abs(directions[i].y) + 0.35f;
                directions[i].Normalize();
            }
        }

        public static void Spawn(Vector3 position, Color breakColor)
        {
            PoolManager poolManager;
            if (!ServiceLocator.TryGet(out poolManager))
            {
                return;
            }

            GameObject effectObject = poolManager.Spawn("BreakableBreakEffect", position, Quaternion.identity);
            if (effectObject == null)
            {
                return;
            }

            BreakableBreakEffect effect = effectObject.GetComponent<BreakableBreakEffect>();
            if (effect != null)
            {
                effect.Play(breakColor);
            }
        }

        public void Play(Color breakColor)
        {
            color = breakColor;
            remaining = Lifetime;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].transform.localPosition = baseLocalPositions[i];
                renderers[i].transform.localScale = Vector3.one * 0.16f;
                renderers[i].material.color = color;
            }
        }

        private void Update()
        {
            remaining -= Time.deltaTime;
            float age = 1f - Mathf.Clamp01(remaining / Lifetime);
            for (int i = 0; i < renderers.Length; i++)
            {
                Transform piece = renderers[i].transform;
                piece.localPosition = baseLocalPositions[i] + directions[i] * age * 1.15f;
                piece.localScale = Vector3.one * Mathf.Lerp(0.16f, 0.03f, age);
                piece.Rotate(Vector3.up, 540f * Time.deltaTime, Space.Self);
                renderers[i].material.color = new Color(color.r, color.g, color.b, 1f - age);
            }

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
            remaining = Lifetime;
        }

        public void OnReturnedToPool()
        {
        }
    }
}
