using UnityEngine;

namespace ArcaneSurvival
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Playing,
        Paused,
        LevelUp,
        PostBossChoice,
        Victory,
        GameOver
    }

    public sealed class GameStateManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; }
        public bool IsGameplayRunning { get { return CurrentState == GameState.Playing; } }

        private void Awake()
        {
            ServiceLocator.Register(this);
            SetState(GameState.MainMenu);
        }

        public void SetState(GameState state)
        {
            CurrentState = state;
            Time.timeScale = state == GameState.Playing || state == GameState.MainMenu ? 1f : 0f;
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
            else if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }
    }
}
