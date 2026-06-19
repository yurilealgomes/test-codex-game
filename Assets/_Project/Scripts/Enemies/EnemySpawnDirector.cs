using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemySpawnDirector : MonoBehaviour
    {
        public const float SpawnRadiusMin = 24f;
        public const float SpawnRadiusMax = 36f;
        public const float BossSpawnRadius = 42f;
        public const float EliteSpawnRadius = 32f;

        private Transform player;
        private PoolManager poolManager;
        private DifficultyScaler difficultyScaler;
        private RunProgressionManager progressionManager;
        private Camera mainCamera;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                player = playerController.transform;
            }

            ServiceLocator.TryGet(out poolManager);
            ServiceLocator.TryGet(out difficultyScaler);
            ServiceLocator.TryGet(out progressionManager);
            mainCamera = Camera.main;
        }

        public EnemyController SpawnEnemy(EnemyData data, bool elite)
        {
            if (player == null || poolManager == null || data == null)
            {
                return null;
            }

            float minRadius = elite ? EliteSpawnRadius : SpawnRadiusMin;
            float maxRadius = elite ? SpawnRadiusMax + 4f : SpawnRadiusMax;
            Vector3 position = SpawnRingCalculator.CalculatePosition(player.position, minRadius, maxRadius, mainCamera);
            GameObject enemyObject = poolManager.Spawn("Enemy", position, Quaternion.identity);
            if (enemyObject == null)
            {
                return null;
            }

            EnemyController controller = enemyObject.GetComponent<EnemyController>();
            if (controller != null)
            {
                float hp = difficultyScaler != null ? difficultyScaler.EnemyHpMultiplier : 1f;
                float damage = difficultyScaler != null ? difficultyScaler.EnemyDamageMultiplier : 1f;
                float speed = difficultyScaler != null ? difficultyScaler.EnemySpeedMultiplier : 1f;
                controller.Initialize(data, player, hp, damage, speed, elite);
            }

            return controller;
        }

        public BossController SpawnBoss(BossData data)
        {
            if (player == null || poolManager == null || data == null)
            {
                return null;
            }

            Vector3 position = SpawnRingCalculator.CalculatePosition(player.position, BossSpawnRadius, BossSpawnRadius + 8f, mainCamera);
            GameObject bossObject = poolManager.Spawn("Boss", position, Quaternion.identity);
            if (bossObject == null)
            {
                return null;
            }

            BossController boss = bossObject.GetComponent<BossController>();
            if (boss != null)
            {
                int bossKills = progressionManager != null ? progressionManager.BossesDefeated : 0;
                boss.Initialize(data, player, bossKills);
                EventBus.RaiseBossSpawned(boss);
            }

            return boss;
        }

        private void OnDrawGizmos()
        {
            DebugGodModeController debugTools;
            if (!ServiceLocator.TryGet(out debugTools) || !debugTools.SpawnDebugEnabled || player == null)
            {
                return;
            }

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            Gizmos.DrawWireSphere(player.position, SpawnRadiusMin);
            Gizmos.DrawWireSphere(player.position, SpawnRadiusMax);
            Gizmos.color = new Color(1f, 0.7f, 0.2f, 0.35f);
            Gizmos.DrawWireSphere(player.position, BossSpawnRadius);
        }
    }
}
