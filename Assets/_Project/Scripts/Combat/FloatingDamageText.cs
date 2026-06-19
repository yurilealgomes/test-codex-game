using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class FloatingDamageText : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 0.75f;
        [SerializeField] private float riseSpeed = 2.5f;

        private TextMesh textMesh;
        private TextMesh shadowMesh;
        private float remaining;
        private Color baseColor;
        private bool criticalHit;
        private Vector3 baseScale;

        private void Awake()
        {
            textMesh = GetComponent<TextMesh>();
            if (textMesh == null)
            {
                textMesh = gameObject.AddComponent<TextMesh>();
            }

            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64;
            textMesh.characterSize = 0.12f;

            GameObject shadow = new GameObject("Shadow");
            shadow.transform.SetParent(transform, false);
            shadow.transform.localPosition = new Vector3(0.035f, -0.035f, 0.02f);
            shadowMesh = shadow.AddComponent<TextMesh>();
            shadowMesh.anchor = TextAnchor.MiddleCenter;
            shadowMesh.alignment = TextAlignment.Center;
            shadowMesh.fontSize = 64;
            shadowMesh.characterSize = 0.12f;
            shadowMesh.color = new Color(0f, 0f, 0f, 0.85f);
            baseScale = Vector3.one;
        }

        public void Play(DamageInfo damageInfo)
        {
            criticalHit = damageInfo != null && damageInfo.IsCritical;
            float amount = damageInfo != null ? damageInfo.FinalDamage : 0f;
            remaining = criticalHit ? lifetime * 1.18f : lifetime;
            baseColor = damageInfo != null ? damageInfo.DisplayColor : Color.white;
            string text = criticalHit ? "CRIT! " + Mathf.CeilToInt(amount) : Mathf.CeilToInt(amount).ToString();
            textMesh.text = text;
            textMesh.color = baseColor;
            textMesh.characterSize = criticalHit ? 0.17f : 0.12f;
            shadowMesh.text = text;
            shadowMesh.characterSize = textMesh.characterSize;
            transform.localScale = criticalHit ? Vector3.one * 1.18f : Vector3.one;
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
            float pop = criticalHit ? 1f + Mathf.Sin(alpha * Mathf.PI) * 0.18f : 1f;
            transform.localScale = baseScale * pop;
            textMesh.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            if (shadowMesh != null)
            {
                shadowMesh.color = new Color(0f, 0f, 0f, alpha * 0.82f);
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
            if (textMesh != null)
            {
                textMesh.text = string.Empty;
            }

            if (shadowMesh != null)
            {
                shadowMesh.text = string.Empty;
            }
        }
    }
}
