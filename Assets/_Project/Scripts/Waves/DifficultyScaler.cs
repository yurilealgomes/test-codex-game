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
        private RunBalanceSettings balanceSettings;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out runTimer);
            ServiceLocator.TryGet(out progressionManager);

            GameDatabase database;
            if (ServiceLocator.TryGet(out database))
            {
                balanceSettings = database.RunBalanceSettings;
            }
        }

        private void Update()
        {
            float minutesElapsed = runTimer != null ? runTimer.MinutesElapsed : 0f;
            int bossesDefeated = progressionManager != null ? progressionManager.BossesDefeated : 0;

            EnemyHpMultiplier = 1f + minutesElapsed * 0.08f + bossesDefeated * 0.25f;
            EnemyDamageMultiplier = 1f + minutesElapsed * 0.04f + bossesDefeated * 0.15f;
            EnemySpeedMultiplier = 1f + minutesElapsed * 0.015f;
            SpawnRateMultiplier = 1f + minutesElapsed * 0.055f;
            EliteChance = Mathf.Min(0.01f + minutesElapsed * 0.004f, 0.22f);
        }
    }
}
