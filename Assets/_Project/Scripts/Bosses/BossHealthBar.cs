using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class BossHealthBar : MonoBehaviour
    {
        private GameObject root;
        private Image fill;
        private Text nameText;
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
            nameText = UIFactory.CreateText(root.transform, "Boss Name", "", 16, Color.white, TextAnchor.MiddleCenter, new Vector2(0f, 0.18f), new Vector2(1f, 0.82f), Vector2.zero, Vector2.zero);
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
            fill.fillAmount = currentBoss.HealthPercent;
            nameText.text = currentBoss.BossName;
        }

        private void HandleBossSpawned(BossController boss)
        {
            currentBoss = boss;
        }
    }
}
