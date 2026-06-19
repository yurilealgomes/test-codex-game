using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class WaveDirector : MonoBehaviour
    {
        [SerializeField] private float bossIntervalSeconds = 300f;
        [SerializeField] private int maxAliveEnemies = 350;

        private GameDatabase database;
        private EnemySpawnDirector spawnDirector;
        private DifficultyScaler difficultyScaler;
        private RunTimer runTimer;
        private GameStateManager stateManager;
        private float spawnTimer;
        private float nextBossTime;

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

            nextBossTime = bossIntervalSeconds;
        }

        private void Update()
        {
            if (stateManager == null || !stateManager.IsGameplayRunning || database == null || spawnDirector == null)
            {
                return;
            }

            CurrentWave = Mathf.Max(1, Mathf.FloorToInt(runTimer.ElapsedTime / 60f) + 1);
            EventBus.RaiseRunStatsChanged(runTimer.ElapsedTime, CurrentWave, EnemyController.ActiveEnemies.Count);

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

            int batchSize = Mathf.Clamp(2 + CurrentWave + Mathf.FloorToInt(runTimer.MinutesElapsed), 2, 18);
            for (int i = 0; i < batchSize && EnemyController.ActiveEnemies.Count < maxAliveEnemies; i++)
            {
                EnemyData data = WeightedRandom.Pick(database.Enemies, enemy => enemy.Weight);
                bool elite = Random.value <= (difficultyScaler != null ? difficultyScaler.EliteChance : 0.02f);
                spawnDirector.SpawnEnemy(data, elite);
            }

            spawnTimer = Mathf.Max(0.12f, 1.4f - runTimer.MinutesElapsed * 0.06f);
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
    }
}
