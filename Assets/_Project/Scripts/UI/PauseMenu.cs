using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PauseMenu : MonoBehaviour
    {
        private GameObject root;
        private GameStateManager stateManager;
        private GameManager gameManager;

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
            ServiceLocator.TryGet(out gameManager);
            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Pause Menu", new Color(0.02f, 0.025f, 0.035f, 0.88f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Paused", 38, Color.white, TextAnchor.MiddleCenter, new Vector2(0.35f, 0.62f), new Vector2(0.65f, 0.73f), Vector2.zero, Vector2.zero);
            UIFactory.CreateButton(root.transform, "Resume Button", "Resume", new Color(0.16f, 0.34f, 0.48f), () => gameManager.ResumeRun(), new Vector2(0.40f, 0.46f), new Vector2(0.60f, 0.54f), Vector2.zero, Vector2.zero);
            UIFactory.CreateButton(root.transform, "Restart Button", "Restart", new Color(0.32f, 0.18f, 0.28f), () => gameManager.RestartRun(), new Vector2(0.40f, 0.35f), new Vector2(0.60f, 0.43f), Vector2.zero, Vector2.zero);
            root.SetActive(false);
        }

        private void Update()
        {
            if (root != null && stateManager != null)
            {
                root.SetActive(stateManager.CurrentState == GameState.Paused);
            }
        }
    }
}
