using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class HUDController : MonoBehaviour
    {
        private Image healthFill;
        private Image xpFill;
        private Text levelText;
        private Text healthText;
        private Text xpText;
        private Text lowHealthText;
        private Text timerText;
        private Text waveText;
        private Text enemyCountText;

        private void Awake()
        {
            ServiceLocator.Register(this);
            EventBus.PlayerHealthChanged += HandleHealthChanged;
            EventBus.PlayerExperienceChanged += HandleExperienceChanged;
            EventBus.RunStatsChanged += HandleRunStatsChanged;
        }

        private void Start()
        {
            Canvas canvas = ServiceLocator.Get<Canvas>();
            RectTransform root = UIFactory.CreateRect(canvas.transform, "HUD", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            healthFill = UIFactory.CreateBar(root, "Health Bar", new Color(0.95f, 0.16f, 0.14f), new Vector2(0.02f, 0.92f), new Vector2(0.34f, 0.965f), Vector2.zero, Vector2.zero);
            xpFill = UIFactory.CreateBar(root, "XP Bar", new Color(0.25f, 0.62f, 1f), new Vector2(0.02f, 0.875f), new Vector2(0.34f, 0.91f), Vector2.zero, Vector2.zero);
            UIFactory.SetBarFill(xpFill, 0f);
            healthText = UIFactory.CreateText(root, "Health Text", "HP 100 / 100", 16, Color.white, TextAnchor.MiddleLeft, new Vector2(0.035f, 0.92f), new Vector2(0.32f, 0.965f), Vector2.zero, Vector2.zero);
            xpText = UIFactory.CreateText(root, "XP Text", "XP 0 / 18", 15, Color.white, TextAnchor.MiddleLeft, new Vector2(0.035f, 0.875f), new Vector2(0.32f, 0.91f), Vector2.zero, Vector2.zero);
            levelText = UIFactory.CreateText(root, "Level Text", "Level 1", 18, Color.white, TextAnchor.MiddleLeft, new Vector2(0.035f, 0.82f), new Vector2(0.22f, 0.86f), Vector2.zero, Vector2.zero);
            lowHealthText = UIFactory.CreateText(root, "Low Health", "Low Health", 18, new Color(1f, 0.18f, 0.12f), TextAnchor.MiddleLeft, new Vector2(0.22f, 0.815f), new Vector2(0.42f, 0.865f), Vector2.zero, Vector2.zero);
            lowHealthText.gameObject.SetActive(false);
            timerText = UIFactory.CreateText(root, "Timer", "00:00", 22, Color.white, TextAnchor.UpperCenter, new Vector2(0.44f, 0.925f), new Vector2(0.56f, 0.98f), Vector2.zero, Vector2.zero);
            waveText = UIFactory.CreateText(root, "Wave", "Wave 1", 18, Color.white, TextAnchor.MiddleRight, new Vector2(0.78f, 0.925f), new Vector2(0.97f, 0.965f), Vector2.zero, Vector2.zero);
            enemyCountText = UIFactory.CreateText(root, "Enemy Count", "Enemies: 0", 16, Color.white, TextAnchor.MiddleRight, new Vector2(0.78f, 0.885f), new Vector2(0.97f, 0.925f), Vector2.zero, Vector2.zero);

            PlayerHealth playerHealth;
            PlayerStats playerStats;
            if (ServiceLocator.TryGet(out playerHealth) && ServiceLocator.TryGet(out playerStats))
            {
                HandleHealthChanged(playerHealth.CurrentHealth, playerStats.MaxHP);
            }

            PlayerExperience playerExperience;
            if (ServiceLocator.TryGet(out playerExperience))
            {
                HandleExperienceChanged(playerExperience.Level, playerExperience.CurrentXp, playerExperience.RequiredXp);
            }
        }

        private void OnDestroy()
        {
            EventBus.PlayerHealthChanged -= HandleHealthChanged;
            EventBus.PlayerExperienceChanged -= HandleExperienceChanged;
            EventBus.RunStatsChanged -= HandleRunStatsChanged;
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (healthFill != null)
            {
                float percent = max <= 0f ? 0f : current / max;
                UIFactory.SetBarFill(healthFill, percent);
                healthFill.color = GetHealthColor(percent);
                if (lowHealthText != null)
                {
                    lowHealthText.gameObject.SetActive(percent <= 0.28f);
                    if (lowHealthText.gameObject.activeSelf)
                    {
                        float pulse = 0.65f + Mathf.Sin(Time.unscaledTime * 8f) * 0.35f;
                        lowHealthText.color = new Color(1f, 0.18f, 0.12f, pulse);
                    }
                }
            }

            if (healthText != null)
            {
                healthText.text = "HP " + Mathf.CeilToInt(current) + " / " + Mathf.CeilToInt(max);
            }
        }

        private void HandleExperienceChanged(int level, float current, float required)
        {
            if (xpFill != null)
            {
                UIFactory.SetBarFill(xpFill, required <= 0f ? 0f : current / required);
            }

            if (levelText != null)
            {
                levelText.text = "Level " + level;
            }

            if (xpText != null)
            {
                xpText.text = "XP " + Mathf.FloorToInt(current) + " / " + Mathf.CeilToInt(required);
            }
        }

        private void HandleRunStatsChanged(float bossCountdownSeconds, int wave, int enemiesAlive)
        {
            int minutes = Mathf.FloorToInt(bossCountdownSeconds / 60f);
            int seconds = Mathf.FloorToInt(bossCountdownSeconds % 60f);
            timerText.text = "Boss in " + minutes.ToString("00") + ":" + seconds.ToString("00");
            waveText.text = "Wave " + wave;
            enemyCountText.text = "Enemies: " + enemiesAlive;
        }

        private Color GetHealthColor(float percent)
        {
            if (percent <= 0.28f)
            {
                return new Color(0.95f, 0.12f, 0.08f);
            }

            if (percent <= 0.58f)
            {
                return new Color(1f, 0.65f, 0.12f);
            }

            return new Color(0.25f, 0.85f, 0.32f);
        }
    }
}
