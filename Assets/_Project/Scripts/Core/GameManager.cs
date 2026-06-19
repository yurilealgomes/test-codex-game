using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcaneSurvival
{
    public sealed class GameManager : MonoBehaviour
    {
        private GameStateManager stateManager;
        private bool gameOverRaised;

        private void Awake()
        {
            ServiceLocator.Register(this);
            EventBus.GameOver += HandleGameOver;
        }

        private void Start()
        {
            stateManager = ServiceLocator.Get<GameStateManager>();
        }

        private void OnDestroy()
        {
            EventBus.GameOver -= HandleGameOver;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && stateManager.CurrentState != GameState.MainMenu && stateManager.CurrentState != GameState.GameOver && stateManager.CurrentState != GameState.LevelUp && stateManager.CurrentState != GameState.PostBossChoice && stateManager.CurrentState != GameState.Victory)
            {
                stateManager.TogglePause();
            }
        }

        public void StartRun()
        {
            gameOverRaised = false;
            stateManager.SetState(GameState.Playing);
            EventBus.RaiseRunStarted();
        }

        public void StartRunWithSkill(SkillData startingSkill)
        {
            PlayerSkillInventory inventory;
            if (ServiceLocator.TryGet(out inventory))
            {
                inventory.UnlockSkill(startingSkill);
            }

            StartRun();
        }

        public void ResumeRun()
        {
            stateManager.SetState(GameState.Playing);
        }

        public void RestartRun()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleGameOver()
        {
            if (gameOverRaised)
            {
                return;
            }

            gameOverRaised = true;
            stateManager.SetState(GameState.GameOver);
        }
    }
}
