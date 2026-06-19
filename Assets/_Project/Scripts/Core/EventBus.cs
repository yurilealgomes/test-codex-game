using System;

namespace ArcaneSurvival
{
    public static class EventBus
    {
        public static event Action RunStarted;
        public static event Action GameOver;
        public static event Action<float, float> PlayerHealthChanged;
        public static event Action<int, float, float> PlayerExperienceChanged;
        public static event Action<float> PlayerExperienceGained;
        public static event Action<int> PlayerLevelUp;
        public static event Action<float, int, int> RunStatsChanged;
        public static event Action<string> BossWarning;
        public static event Action<BossController> BossSpawned;
        public static event Action BossDefeated;
        public static event Action<string> PickupCollected;
        public static event Action<string, string> SynergyActivated;
        public static event Action SkillInventoryChanged;

        public static void Clear()
        {
            RunStarted = null;
            GameOver = null;
            PlayerHealthChanged = null;
            PlayerExperienceChanged = null;
            PlayerExperienceGained = null;
            PlayerLevelUp = null;
            RunStatsChanged = null;
            BossWarning = null;
            BossSpawned = null;
            BossDefeated = null;
            PickupCollected = null;
            SynergyActivated = null;
            SkillInventoryChanged = null;
        }

        public static void RaiseRunStarted()
        {
            if (RunStarted != null)
            {
                RunStarted.Invoke();
            }
        }

        public static void RaiseGameOver()
        {
            if (GameOver != null)
            {
                GameOver.Invoke();
            }
        }

        public static void RaisePlayerHealthChanged(float current, float max)
        {
            if (PlayerHealthChanged != null)
            {
                PlayerHealthChanged.Invoke(current, max);
            }
        }

        public static void RaisePlayerExperienceChanged(int level, float current, float required)
        {
            if (PlayerExperienceChanged != null)
            {
                PlayerExperienceChanged.Invoke(level, current, required);
            }
        }

        public static void RaisePlayerExperienceGained(float amount)
        {
            if (PlayerExperienceGained != null)
            {
                PlayerExperienceGained.Invoke(amount);
            }
        }

        public static void RaisePlayerLevelUp(int level)
        {
            if (PlayerLevelUp != null)
            {
                PlayerLevelUp.Invoke(level);
            }
        }

        public static void RaiseRunStatsChanged(float elapsedTime, int wave, int enemiesAlive)
        {
            if (RunStatsChanged != null)
            {
                RunStatsChanged.Invoke(elapsedTime, wave, enemiesAlive);
            }
        }

        public static void RaiseBossWarning(string bossName)
        {
            if (BossWarning != null)
            {
                BossWarning.Invoke(bossName);
            }
        }

        public static void RaiseBossSpawned(BossController boss)
        {
            if (BossSpawned != null)
            {
                BossSpawned.Invoke(boss);
            }
        }

        public static void RaiseBossDefeated()
        {
            if (BossDefeated != null)
            {
                BossDefeated.Invoke();
            }
        }

        public static void RaisePickupCollected(string pickupName)
        {
            if (PickupCollected != null)
            {
                PickupCollected.Invoke(pickupName);
            }
        }

        public static void RaiseSynergyActivated(string name, string description)
        {
            if (SynergyActivated != null)
            {
                SynergyActivated.Invoke(name, description);
            }
        }

        public static void RaiseSkillInventoryChanged()
        {
            if (SkillInventoryChanged != null)
            {
                SkillInventoryChanged.Invoke();
            }
        }
    }
}
