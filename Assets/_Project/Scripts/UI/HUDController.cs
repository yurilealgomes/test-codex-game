using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class HUDController : MonoBehaviour
    {
        private Image healthFill;
        private Image xpFill;
        private Text levelText;
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
            xpFill.fillAmount = 0f;
            levelText = UIFactory.CreateText(root, "Level Text", "Level 1", 18, Color.white, TextAnchor.MiddleLeft, new Vector2(0.035f, 0.82f), new Vector2(0.22f, 0.86f), Vector2.zero, Vector2.zero);
            timerText = UIFactory.CreateText(root, "Timer", "00:00", 22, Color.white, TextAnchor.UpperCenter, new Vector2(0.44f, 0.925f), new Vector2(0.56f, 0.98f), Vector2.zero, Vector2.zero);
            waveText = UIFactory.CreateText(root, "Wave", "Wave 1", 18, Color.white, TextAnchor.MiddleRight, new Vector2(0.78f, 0.925f), new Vector2(0.97f, 0.965f), Vector2.zero, Vector2.zero);
            enemyCountText = UIFactory.CreateText(root, "Enemy Count", "Enemies: 0", 16, Color.white, TextAnchor.MiddleRight, new Vector2(0.78f, 0.885f), new Vector2(0.97f, 0.925f), Vector2.zero, Vector2.zero);
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
                healthFill.fillAmount = max <= 0f ? 0f : current / max;
            }
        }

        private void HandleExperienceChanged(int level, float current, float required)
        {
            if (xpFill != null)
            {
                xpFill.fillAmount = required <= 0f ? 0f : current / required;
            }

            if (levelText != null)
            {
                levelText.text = "Level " + level;
            }
        }

        private void HandleRunStatsChanged(float elapsedTime, int wave, int enemiesAlive)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            waveText.text = "Wave " + wave;
            enemyCountText.text = "Enemies: " + enemiesAlive;
        }
    }
}
