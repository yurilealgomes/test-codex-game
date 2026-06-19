using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class PostBossChoicePanel : MonoBehaviour
    {
        private GameObject root;
        private Button endRunButton;
        private Button continueButton;
        private Outline endOutline;
        private Outline continueOutline;
        private GameStateManager stateManager;
        private EndlessModeManager endlessModeManager;
        private VictoryPanel victoryPanel;
        private bool choiceShown;
        private int selectedIndex;

        private void Awake()
        {
            ServiceLocator.Register(this);
            EventBus.BossDefeated += HandleBossDefeated;
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
            ServiceLocator.TryGet(out endlessModeManager);
            ServiceLocator.TryGet(out victoryPanel);

            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Post Boss Choice Panel", new Color(0.02f, 0.025f, 0.035f, 0.94f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Boss Defeated", 42, Color.white, TextAnchor.MiddleCenter, new Vector2(0.30f, 0.66f), new Vector2(0.70f, 0.78f), Vector2.zero, Vector2.zero);
            UIFactory.CreateText(root.transform, "Description", "Endless Mode will keep increasing difficulty over time.", 18, new Color(0.82f, 0.9f, 1f), TextAnchor.MiddleCenter, new Vector2(0.25f, 0.56f), new Vector2(0.75f, 0.63f), Vector2.zero, Vector2.zero);
            endRunButton = UIFactory.CreateButton(root.transform, "End Run Button", "End Run", new Color(0.42f, 0.18f, 0.18f), EndRun, new Vector2(0.29f, 0.40f), new Vector2(0.47f, 0.50f), Vector2.zero, Vector2.zero);
            continueButton = UIFactory.CreateButton(root.transform, "Continue Endless Button", "Continue Endless Mode", new Color(0.14f, 0.35f, 0.42f), ContinueEndlessMode, new Vector2(0.53f, 0.40f), new Vector2(0.74f, 0.50f), Vector2.zero, Vector2.zero);
            endOutline = endRunButton.gameObject.AddComponent<Outline>();
            continueOutline = continueButton.gameObject.AddComponent<Outline>();
            endOutline.effectColor = new Color(1f, 0.85f, 0.35f);
            continueOutline.effectColor = new Color(1f, 0.85f, 0.35f);
            endOutline.effectDistance = new Vector2(5f, 5f);
            continueOutline.effectDistance = new Vector2(5f, 5f);
            Hide();
        }

        private void OnDestroy()
        {
            EventBus.BossDefeated -= HandleBossDefeated;
        }

        private void Update()
        {
            if (root == null || !root.activeSelf)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectIndex(selectedIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectIndex(selectedIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (selectedIndex == 0)
                {
                    EndRun();
                }
                else
                {
                    ContinueEndlessMode();
                }
            }
        }

        private void HandleBossDefeated()
        {
            if (choiceShown)
            {
                return;
            }

            choiceShown = true;
            Show();
        }

        private void Show()
        {
            if (stateManager != null)
            {
                stateManager.SetState(GameState.PostBossChoice);
            }

            root.SetActive(true);
            SelectIndex(0);
        }

        private void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        private void SelectIndex(int index)
        {
            selectedIndex = (index + 2) % 2;
            endOutline.enabled = selectedIndex == 0;
            continueOutline.enabled = selectedIndex == 1;
        }

        private void EndRun()
        {
            Hide();
            if (victoryPanel != null)
            {
                victoryPanel.Show();
            }
        }

        private void ContinueEndlessMode()
        {
            Hide();
            if (endlessModeManager != null)
            {
                endlessModeManager.StartEndlessMode();
            }

            if (stateManager != null)
            {
                stateManager.SetState(GameState.Playing);
            }
        }
    }
}
