using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class RunProgressionManager : MonoBehaviour
    {
        public int EnemiesDefeated { get; private set; }
        public int BossesDefeated { get; private set; }

        private PoolManager poolManager;
        private PerformanceSettings performanceSettings;
        private RunBalanceSettings balanceSettings;
        private RunTimer runTimer;

        private void Awake()
        {
            ServiceLocator.Register(this);
            EventBus.BossDefeated += HandleBossDefeated;
        }

        private void Start()
        {
            ServiceLocator.TryGet(out poolManager);
            GameDatabase database;
            if (ServiceLocator.TryGet(out database))
            {
                performanceSettings = database.PerformanceSettings;
                balanceSettings = database.RunBalanceSettings;
            }

            ServiceLocator.TryGet(out runTimer);
        }

        private void OnDestroy()
        {
            EventBus.BossDefeated -= HandleBossDefeated;
        }

        public void RegisterEnemyDefeated(float xpDrop, Vector3 position)
        {
            EnemiesDefeated++;
            SpawnXp(GetAdjustedEnemyXp(xpDrop), position);
        }

        public void SpawnXp(float amount, Vector3 position)
        {
            if (poolManager == null)
            {
                return;
            }

            GameObject orbObject = poolManager.Spawn("XPOrb", position + Vector3.up * 0.35f, Quaternion.identity);
            if (orbObject == null)
            {
                return;
            }

            XPOrb orb = orbObject.GetComponent<XPOrb>();
            if (orb != null)
            {
                float maxDistance = performanceSettings != null ? performanceSettings.XpOrbDespawnDistance : 60f;
                orb.Initialize(amount, maxDistance);
            }
        }

        private void HandleBossDefeated()
        {
            BossesDefeated++;
        }

        private float GetAdjustedEnemyXp(float xpDrop)
        {
            if (balanceSettings == null || runTimer == null)
            {
                return xpDrop;
            }

            if (runTimer.ElapsedTime < balanceSettings.RangedEnemyWeightRampStartSeconds)
            {
                return xpDrop * balanceSettings.EarlyXpRewardMultiplier;
            }

            return xpDrop;
        }
    }
}
