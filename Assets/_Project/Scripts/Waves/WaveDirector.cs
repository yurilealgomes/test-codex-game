using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class WaveDirector : MonoBehaviour
    {
        [SerializeField] private float bossIntervalSeconds = 900f;
        [SerializeField] private int maxAliveEnemies = 350;

        private GameDatabase database;
        private EnemySpawnDirector spawnDirector;
        private DifficultyScaler difficultyScaler;
        private RunTimer runTimer;
        private GameStateManager stateManager;
        private float spawnTimer;
        private float nextBossTime;
        private RunBalanceSettings balanceSettings;

        public int CurrentWave { get; private set; } = 1;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out database);
            ServiceLocator.TryGet(out spawnDirector);
            ServiceLocator.TryGet(out difficultyScaler);
            ServiceLocator.TryGet(out runTimer);
            ServiceLocator.TryGet(out stateManager);

            if (database != null && database.PerformanceSettings != null)
            {
                maxAliveEnemies = database.PerformanceSettings.MaxAliveEnemies;
            }

            if (database != null && database.RunBalanceSettings != null)
            {
                balanceSettings = database.RunBalanceSettings;
                bossIntervalSeconds = balanceSettings.BossIntervalSeconds;
                nextBossTime = balanceSettings.FirstBossTimeSeconds;
                return;
            }

            nextBossTime = bossIntervalSeconds;
        }

        private void Update()
        {
            if (stateManager == null || !stateManager.IsGameplayRunning || database == null || spawnDirector == null)
            {
                return;
            }

            CurrentWave = Mathf.Max(1, Mathf.FloorToInt(runTimer.ElapsedTime / 60f) + 1);
            EventBus.RaiseRunStatsChanged(GetBossCountdownSeconds(), CurrentWave, EnemyController.ActiveEnemies.Count);

            TrySpawnEnemies();
            TrySpawnBoss();
        }

        private void TrySpawnEnemies()
        {
            if (EnemyController.ActiveEnemies.Count >= maxAliveEnemies)
            {
                return;
            }

            float spawnRate = difficultyScaler != null ? difficultyScaler.SpawnRateMultiplier : 1f;
            spawnTimer -= Time.deltaTime * spawnRate;
            if (spawnTimer > 0f)
            {
                return;
            }

            int baseBatchSize = balanceSettings != null ? balanceSettings.BaseBatchSize : 2;
            int maxBatchSize = balanceSettings != null ? balanceSettings.MaxBatchSize : 18;
            float pressureMinutes = Mathf.Max(0f, runTimer.MinutesElapsed - 1f);
            int batchSize = Mathf.Clamp(baseBatchSize + Mathf.FloorToInt(pressureMinutes * 0.4f) + Mathf.FloorToInt(CurrentWave / 6f), baseBatchSize, maxBatchSize);
            for (int i = 0; i < batchSize && EnemyController.ActiveEnemies.Count < maxAliveEnemies; i++)
            {
                EnemyData data = WeightedRandom.Pick(database.Enemies, GetSpawnWeight);
                if (data == null)
                {
                    continue;
                }

                bool elite = Random.value <= (difficultyScaler != null ? difficultyScaler.EliteChance : 0.02f);
                spawnDirector.SpawnEnemy(data, elite);
            }

            float baseInterval = balanceSettings != null ? balanceSettings.BaseSpawnInterval : 1.4f;
            float minimumInterval = balanceSettings != null ? balanceSettings.MinimumSpawnInterval : 0.12f;
            float reduction = balanceSettings != null ? balanceSettings.SpawnIntervalReductionPerMinute : 0.06f;
            spawnTimer = Mathf.Max(minimumInterval, baseInterval - pressureMinutes * reduction);
        }

        private float GetSpawnWeight(EnemyData enemy)
        {
            if (enemy == null)
            {
                return 0f;
            }

            if (enemy.AttackType != EnemyAttackType.Ranged || balanceSettings == null)
            {
                return enemy.Weight;
            }

            if (runTimer.ElapsedTime < balanceSettings.RangedEnemyUnlockTimeSeconds)
            {
                return 0f;
            }

            float rampDuration = Mathf.Max(1f, balanceSettings.RangedEnemyWeightRampStartSeconds - balanceSettings.RangedEnemyUnlockTimeSeconds);
            float earlyRamp = Mathf.Clamp01((runTimer.ElapsedTime - balanceSettings.RangedEnemyUnlockTimeSeconds) / rampDuration);
            float lateRamp = runTimer.ElapsedTime >= balanceSettings.RangedEnemyWeightRampStartSeconds
                ? Mathf.Clamp01((runTimer.ElapsedTime - balanceSettings.RangedEnemyWeightRampStartSeconds) / 240f)
                : 0f;
            return enemy.Weight * Mathf.Lerp(0.18f, 1f, Mathf.Max(earlyRamp * 0.65f, lateRamp));
        }

        private void TrySpawnBoss()
        {
            if (runTimer.ElapsedTime < nextBossTime || database.Bosses.Count == 0)
            {
                return;
            }

            BossData bossData = database.Bosses[Random.Range(0, database.Bosses.Count)];
            EventBus.RaiseBossWarning(bossData.BossName);
            spawnDirector.SpawnBoss(bossData);
            nextBossTime += bossIntervalSeconds;
        }

        public void SpawnBossNow()
        {
            if (database == null || spawnDirector == null || runTimer == null || database.Bosses.Count == 0)
            {
                return;
            }

            BossData bossData = database.Bosses[Random.Range(0, database.Bosses.Count)];
            EventBus.RaiseBossWarning(bossData.BossName);
            spawnDirector.SpawnBoss(bossData);
            nextBossTime = runTimer.ElapsedTime + bossIntervalSeconds;
        }

        private float GetBossCountdownSeconds()
        {
            if (runTimer == null)
            {
                return 0f;
            }

            return Mathf.Max(0f, nextBossTime - runTimer.ElapsedTime);
        }
    }
}
