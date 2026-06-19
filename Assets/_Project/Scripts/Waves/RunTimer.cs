using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class RunTimer : MonoBehaviour
    {
        public float ElapsedTime { get; private set; }
        public float MinutesElapsed { get { return ElapsedTime / 60f; } }

        private GameStateManager stateManager;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
        }

        private void Update()
        {
            if (stateManager != null && stateManager.IsGameplayRunning)
            {
                ElapsedTime += Time.deltaTime;
            }
        }
    }
}
