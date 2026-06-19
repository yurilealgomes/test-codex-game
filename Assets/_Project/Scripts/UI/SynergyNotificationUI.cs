using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class SynergyNotificationUI : MonoBehaviour
    {
        private Text text;
        private float remaining;

        private void Awake()
        {
            EventBus.SynergyActivated += Show;
        }

        private void Start()
        {
            Canvas canvas = ServiceLocator.Get<Canvas>();
            text = UIFactory.CreateText(canvas.transform, "Synergy Notification", "", 22, new Color(0.75f, 0.95f, 1f), TextAnchor.MiddleCenter, new Vector2(0.22f, 0.79f), new Vector2(0.78f, 0.88f), Vector2.zero, Vector2.zero);
            text.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.SynergyActivated -= Show;
        }

        private void Update()
        {
            if (remaining <= 0f || text == null)
            {
                return;
            }

            remaining -= Time.unscaledDeltaTime;
            text.gameObject.SetActive(remaining > 0f);
        }

        private void Show(string synergyName, string description)
        {
            if (text == null)
            {
                return;
            }

            text.text = "Synergy Activated: " + synergyName;
            text.gameObject.SetActive(true);
            remaining = 3f;
        }
    }
}
