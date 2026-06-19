using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class DifficultyScaler : MonoBehaviour
    {
        public float EnemyHpMultiplier { get; private set; } = 1f;
        public float EnemyDamageMultiplier { get; private set; } = 1f;
        public float EnemySpeedMultiplier { get; private set; } = 1f;
        public float SpawnRateMultiplier { get; private set; } = 1f;
        public float EliteChance { get; private set; } = 0.02f;

        private RunTimer runTimer;
        private RunProgressionManager progressionManager;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out runTimer);
            ServiceLocator.TryGet(out progressionManager);
        }

        private void Update()
        {
            float minutesElapsed = runTimer != null ? runTimer.MinutesElapsed : 0f;
            int bossesDefeated = progressionManager != null ? progressionManager.BossesDefeated : 0;

            EnemyHpMultiplier = 1f + minutesElapsed * 0.12f + bossesDefeated * 0.25f;
            EnemyDamageMultiplier = 1f + minutesElapsed * 0.06f + bossesDefeated * 0.15f;
            EnemySpeedMultiplier = 1f + minutesElapsed * 0.02f;
            SpawnRateMultiplier = 1f + minutesElapsed * 0.08f;
            EliteChance = Mathf.Min(0.02f + minutesElapsed * 0.005f, 0.25f);
        }
    }
}
