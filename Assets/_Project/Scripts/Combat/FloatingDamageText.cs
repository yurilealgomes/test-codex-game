using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class FloatingDamageText : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 0.75f;
        [SerializeField] private float riseSpeed = 2.5f;

        private TextMesh textMesh;
        private float remaining;
        private Color baseColor;

        private void Awake()
        {
            textMesh = GetComponent<TextMesh>();
            if (textMesh == null)
            {
                textMesh = gameObject.AddComponent<TextMesh>();
            }

            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 36;
            textMesh.characterSize = 0.08f;
        }

        public void Play(float amount, bool critical)
        {
            remaining = lifetime;
            baseColor = critical ? new Color(1f, 0.85f, 0.2f) : Color.white;
            textMesh.text = critical ? Mathf.CeilToInt(amount) + "!" : Mathf.CeilToInt(amount).ToString();
            textMesh.color = baseColor;
            textMesh.characterSize = critical ? 0.12f : 0.08f;
        }

        private void Update()
        {
            remaining -= Time.deltaTime;
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            Camera camera = Camera.main;
            if (camera != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
            }

            float alpha = Mathf.Clamp01(remaining / lifetime);
            textMesh.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

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
            if (textMesh != null)
            {
                textMesh.text = string.Empty;
            }
        }
    }
}
