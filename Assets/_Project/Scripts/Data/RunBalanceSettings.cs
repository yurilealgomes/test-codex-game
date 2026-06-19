using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class RunBalanceSettings
    {
        public float BaseRequiredXp = 18f;
        public float XpGrowthMultiplier = 1.32f;
        public float FirstBossTimeSeconds = 900f;
        public float BossIntervalSeconds = 900f;
        public float BaseSpawnInterval = 3.25f;
        public float MinimumSpawnInterval = 0.18f;
        public float SpawnIntervalReductionPerMinute = 0.16f;
        public int BaseBatchSize = 1;
        public int MaxBatchSize = 42;
        public float RangedEnemyUnlockTimeSeconds = 240f;
        public float RangedEnemyWeightRampStartSeconds = 480f;
        public float EarlyXpRewardMultiplier = 0.75f;
    }
}
