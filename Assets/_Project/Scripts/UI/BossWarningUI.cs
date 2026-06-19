using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class BossWarningUI : MonoBehaviour
    {
        private Image warningPanel;
        private Text warningText;
        private float remaining;
        private float duration;

        private void Awake()
        {
            EventBus.BossWarning += ShowWarning;
        }

        private void Start()
        {
            Canvas canvas = ServiceLocator.Get<Canvas>();
            warningPanel = UIFactory.CreatePanel(canvas.transform, "Boss Warning Panel", new Color(0.24f, 0.03f, 0.02f, 0.72f), new Vector2(0.22f, 0.72f), new Vector2(0.78f, 0.84f), Vector2.zero, Vector2.zero);
            warningPanel.raycastTarget = false;
            warningText = UIFactory.CreateText(warningPanel.transform, "Boss Warning", "", 36, new Color(1f, 0.34f, 0.18f), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            warningText.raycastTarget = false;
            warningPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.BossWarning -= ShowWarning;
        }

        private void Update()
        {
            if (remaining <= 0f || warningText == null || warningPanel == null)
            {
                return;
            }

            remaining -= Time.unscaledDeltaTime;
            bool active = remaining > 0f;
            warningPanel.gameObject.SetActive(active);
            if (!active)
            {
                return;
            }

            float percent = duration <= 0f ? 0f : Mathf.Clamp01(remaining / duration);
            float pulse = 0.5f + Mathf.Sin(Time.unscaledTime * 10f) * 0.5f;
            warningPanel.color = new Color(0.24f, 0.03f, 0.02f, 0.48f + pulse * 0.28f);
            warningText.color = Color.Lerp(new Color(1f, 0.34f, 0.18f), Color.white, pulse * 0.35f);
            warningText.transform.localScale = Vector3.one * (1f + (1f - percent) * 0.08f + pulse * 0.04f);
        }

        private void ShowWarning(string bossName)
        {
            if (warningText == null || warningPanel == null)
            {
                return;
            }

            warningText.text = "Boss Incoming: " + bossName;
            warningPanel.gameObject.SetActive(true);
            duration = 5.8f;
            remaining = duration;
        }
    }
}
