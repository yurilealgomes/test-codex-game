using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class BossHealthBar : MonoBehaviour
    {
        private GameObject root;
        private Image fill;
        private Text nameText;
        private Text healthText;
        private BossController currentBoss;

        private void Awake()
        {
            EventBus.BossSpawned += HandleBossSpawned;
        }

        private void Start()
        {
            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Boss Health Root", new Color(0f, 0f, 0f, 0f), new Vector2(0.24f, 0.04f), new Vector2(0.76f, 0.12f), Vector2.zero, Vector2.zero).gameObject;
            fill = UIFactory.CreateBar(root.transform, "Boss Health Bar", new Color(0.8f, 0.12f, 0.2f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            nameText = UIFactory.CreateText(root.transform, "Boss Name", "", 18, Color.white, TextAnchor.MiddleCenter, new Vector2(0f, 0.48f), new Vector2(1f, 0.95f), Vector2.zero, Vector2.zero);
            healthText = UIFactory.CreateText(root.transform, "Boss Health Value", "", 14, new Color(1f, 0.88f, 0.82f), TextAnchor.MiddleCenter, new Vector2(0f, 0.08f), new Vector2(1f, 0.48f), Vector2.zero, Vector2.zero);
            root.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.BossSpawned -= HandleBossSpawned;
        }

        private void Update()
        {
            if (currentBoss == null || !currentBoss.IsAlive)
            {
                if (root != null)
                {
                    root.SetActive(false);
                }
                return;
            }

            root.SetActive(true);
            float percent = currentBoss.HealthPercent;
            UIFactory.SetBarFill(fill, percent);
            fill.color = GetBossHealthColor(percent);
            nameText.text = currentBoss.BossName;
            healthText.text = Mathf.CeilToInt(currentBoss.CurrentHealth) + " / " + Mathf.CeilToInt(currentBoss.MaxHealth) + " HP";
        }

        private void HandleBossSpawned(BossController boss)
        {
            currentBoss = boss;
        }

        private Color GetBossHealthColor(float percent)
        {
            if (percent <= 0.25f)
            {
                float pulse = 0.5f + Mathf.Sin(Time.unscaledTime * 8f) * 0.5f;
                return Color.Lerp(new Color(0.85f, 0.05f, 0.08f), new Color(1f, 0.62f, 0.16f), pulse);
            }

            return Color.Lerp(new Color(0.9f, 0.12f, 0.18f), new Color(0.55f, 0.05f, 0.12f), percent);
        }
    }
}
