using System.Collections;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerVisualController : MonoBehaviour
    {
        private Renderer bodyRenderer;
        private Color baseColor = new Color(0.45f, 0.88f, 1f);

        private void Awake()
        {
            bodyRenderer = GetComponentInChildren<Renderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = baseColor;
            }
        }

        private void OnEnable()
        {
            EventBus.PlayerLevelUp += HandleLevelUp;
        }

        private void OnDisable()
        {
            EventBus.PlayerLevelUp -= HandleLevelUp;
        }

        public void FlashDamage()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(FlashRoutine(Color.red, 0.12f));
            }
        }

        private void HandleLevelUp(int level)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(FlashRoutine(new Color(0.7f, 1f, 0.55f), 0.28f));
            }
        }

        private IEnumerator FlashRoutine(Color flashColor, float duration)
        {
            if (bodyRenderer == null)
            {
                yield break;
            }

            bodyRenderer.material.color = flashColor;
            yield return new WaitForSecondsRealtime(duration);
            bodyRenderer.material.color = baseColor;
        }
    }
}
