using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class VictoryPanel : MonoBehaviour
    {
        private GameObject root;
        private GameStateManager stateManager;
        private GameManager gameManager;
        private RunProgressionManager progressionManager;
        private RunTimer runTimer;
        private UnityEngine.UI.Text summaryText;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
            ServiceLocator.TryGet(out gameManager);
            ServiceLocator.TryGet(out progressionManager);
            ServiceLocator.TryGet(out runTimer);

            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Victory Panel", new Color(0.02f, 0.025f, 0.035f, 0.95f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Run Complete", 44, Color.white, TextAnchor.MiddleCenter, new Vector2(0.32f, 0.62f), new Vector2(0.68f, 0.74f), Vector2.zero, Vector2.zero);
            summaryText = UIFactory.CreateText(root.transform, "Summary", "Boss defeated. Your run has been recorded.", 20, new Color(0.82f, 0.9f, 1f), TextAnchor.MiddleCenter, new Vector2(0.26f, 0.52f), new Vector2(0.74f, 0.60f), Vector2.zero, Vector2.zero);
            UIFactory.CreateButton(root.transform, "Victory Restart Button", "Try Again", new Color(0.16f, 0.34f, 0.48f), () => gameManager.RestartRun(), new Vector2(0.40f, 0.38f), new Vector2(0.60f, 0.47f), Vector2.zero, Vector2.zero);
            Hide();
        }

        private void Update()
        {
            if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.R) && gameManager != null)
            {
                gameManager.RestartRun();
            }
        }

        public void Show()
        {
            if (stateManager != null)
            {
                stateManager.SetState(GameState.Victory);
            }

            if (root != null)
            {
                if (summaryText != null)
                {
                    int seconds = runTimer != null ? Mathf.FloorToInt(runTimer.ElapsedTime) : 0;
                    int minutes = seconds / 60;
                    seconds %= 60;
                    int enemies = progressionManager != null ? progressionManager.EnemiesDefeated : 0;
                    int bosses = progressionManager != null ? progressionManager.BossesDefeated : 0;
                    summaryText.text = "Time " + minutes.ToString("00") + ":" + seconds.ToString("00") + "  Enemies " + enemies + "  Bosses " + bosses;
                }

                root.SetActive(true);
            }
        }

        private void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }
    }
}
