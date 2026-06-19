using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class BossSpawnWarning : MonoBehaviour
    {
        [SerializeField] private float pulseSpeed = 5f;
        private Renderer cachedRenderer;
        private Color baseColor;

        private void Awake()
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
            if (cachedRenderer != null)
            {
                baseColor = cachedRenderer.material.color;
            }
        }

        private void Update()
        {
            if (cachedRenderer == null)
            {
                return;
            }

            float pulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
            cachedRenderer.material.color = Color.Lerp(baseColor, Color.white, pulse);
        }
    }
}
