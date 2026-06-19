using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EndlessModeManager : MonoBehaviour
    {
        private RunTimer runTimer;
        private float endlessStartTime;

        public bool IsEndlessMode { get; private set; }
        public float EndlessMinutes { get { return IsEndlessMode && runTimer != null ? Mathf.Max(0f, (runTimer.ElapsedTime - endlessStartTime) / 60f) : 0f; } }

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out runTimer);
        }

        public void StartEndlessMode()
        {
            if (IsEndlessMode)
            {
                return;
            }

            IsEndlessMode = true;
            endlessStartTime = runTimer != null ? runTimer.ElapsedTime : 0f;
            EventBus.RaiseSynergyActivated("Endless Mode Started", "Difficulty will keep increasing over time.");
        }
    }
}
