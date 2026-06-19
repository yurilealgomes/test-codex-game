using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class GameOverPanel : MonoBehaviour
    {
        private GameObject root;
        private GameManager gameManager;

        private void Awake()
        {
            EventBus.GameOver += Show;
        }

        private void Start()
        {
            ServiceLocator.TryGet(out gameManager);
            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Game Over Panel", new Color(0.02f, 0.015f, 0.02f, 0.92f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Game Over", 42, Color.white, TextAnchor.MiddleCenter, new Vector2(0.33f, 0.58f), new Vector2(0.67f, 0.70f), Vector2.zero, Vector2.zero);
            UIFactory.CreateButton(root.transform, "Restart Button", "Restart Run", new Color(0.45f, 0.16f, 0.2f), () => gameManager.RestartRun(), new Vector2(0.40f, 0.42f), new Vector2(0.60f, 0.50f), Vector2.zero, Vector2.zero);
            root.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.GameOver -= Show;
        }

        private void Show()
        {
            if (root != null)
            {
                root.SetActive(true);
            }
        }
    }
}
