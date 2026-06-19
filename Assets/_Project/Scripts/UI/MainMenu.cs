using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class MainMenu : MonoBehaviour
    {
        private GameObject root;
        private GameManager gameManager;
        private StartingSkillSelectionPanel startingSkillSelectionPanel;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out gameManager);
            ServiceLocator.TryGet(out startingSkillSelectionPanel);
            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Start Screen", new Color(0.025f, 0.03f, 0.04f, 0.96f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Arcane Survival", 46, Color.white, TextAnchor.MiddleCenter, new Vector2(0.25f, 0.57f), new Vector2(0.75f, 0.71f), Vector2.zero, Vector2.zero);
            UIFactory.CreateText(root.transform, "Subtitle", "Endless spellcraft against impossible waves.", 20, new Color(0.82f, 0.9f, 1f), TextAnchor.MiddleCenter, new Vector2(0.25f, 0.50f), new Vector2(0.75f, 0.56f), Vector2.zero, Vector2.zero);
            Button startButton = UIFactory.CreateButton(root.transform, "Start Button", "Start Run", new Color(0.16f, 0.34f, 0.48f), StartRun, new Vector2(0.40f, 0.37f), new Vector2(0.60f, 0.46f), Vector2.zero, Vector2.zero);
            root.AddComponent<MenuButtonNavigator>().Configure(new[] { startButton });
        }

        private void StartRun()
        {
            if (root != null)
            {
                root.SetActive(false);
            }

            if (gameManager != null)
            {
                if (startingSkillSelectionPanel != null)
                {
                    startingSkillSelectionPanel.Show(gameManager.StartRunWithSkill);
                }
                else
                {
                    gameManager.StartRun();
                }
            }
        }
    }
}
