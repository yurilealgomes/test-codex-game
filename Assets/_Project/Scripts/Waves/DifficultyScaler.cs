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
        private EndlessModeManager endlessModeManager;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out runTimer);
            ServiceLocator.TryGet(out progressionManager);
            ServiceLocator.TryGet(out endlessModeManager);

            GameDatabase database;
            if (ServiceLocator.TryGet(out database))
            {
                balanceSettings = database.RunBalanceSettings;
            }
        }

        private void Update()
        {
            float minutesElapsed = runTimer != null ? runTimer.MinutesElapsed : 0f;
            float pressureMinutes = Mathf.Max(0f, minutesElapsed - 2f);
            float endlessMinutes = endlessModeManager != null ? endlessModeManager.EndlessMinutes : 0f;
            int bossesDefeated = progressionManager != null ? progressionManager.BossesDefeated : 0;

            EnemyHpMultiplier = 1f + minutesElapsed * 0.065f + bossesDefeated * 0.25f + endlessMinutes * 0.12f;
            EnemyDamageMultiplier = 1f + minutesElapsed * 0.035f + bossesDefeated * 0.15f + endlessMinutes * 0.06f;
            EnemySpeedMultiplier = 1f + minutesElapsed * 0.01f + endlessMinutes * 0.01f;
            SpawnRateMultiplier = 1f + pressureMinutes * 0.08f + endlessMinutes * 0.12f;
            EliteChance = Mathf.Min(0.005f + pressureMinutes * 0.005f + endlessMinutes * 0.008f, 0.28f);
        }
    }
}
