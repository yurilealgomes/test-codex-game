using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class BossWarningUI : MonoBehaviour
    {
        private Text warningText;
        private float remaining;

        private void Awake()
        {
            EventBus.BossWarning += ShowWarning;
        }

        private void Start()
        {
            Canvas canvas = ServiceLocator.Get<Canvas>();
            warningText = UIFactory.CreateText(canvas.transform, "Boss Warning", "", 36, new Color(1f, 0.34f, 0.18f), TextAnchor.MiddleCenter, new Vector2(0.20f, 0.72f), new Vector2(0.80f, 0.82f), Vector2.zero, Vector2.zero);
            warningText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.BossWarning -= ShowWarning;
        }

        private void Update()
        {
            if (remaining <= 0f || warningText == null)
            {
                return;
            }

            remaining -= Time.unscaledDeltaTime;
            warningText.gameObject.SetActive(remaining > 0f);
        }

        private void ShowWarning(string bossName)
        {
            if (warningText == null)
            {
                return;
            }

            warningText.text = "Boss Incoming: " + bossName;
            warningText.gameObject.SetActive(true);
            remaining = 2.8f;
        }
    }
}
