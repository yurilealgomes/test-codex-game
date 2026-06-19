using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerExperience : MonoBehaviour
    {
        [SerializeField] private float baseRequiredXp = 18f;
        [SerializeField] private float levelGrowth = 1.32f;

        private float currentXp;

        public int Level { get; private set; } = 1;
        public float CurrentXp { get { return currentXp; } }
        public float RequiredXp { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register(this);
            RequiredXp = baseRequiredXp;
        }

        private void Start()
        {
            GameDatabase database;
            if (ServiceLocator.TryGet(out database) && database.RunBalanceSettings != null)
            {
                baseRequiredXp = database.RunBalanceSettings.BaseRequiredXp;
                levelGrowth = database.RunBalanceSettings.XpGrowthMultiplier;
                RequiredXp = baseRequiredXp;
            }

            EventBus.RaisePlayerExperienceChanged(Level, currentXp, RequiredXp);
        }

        public void AddExperience(float amount)
        {
            float gained = Mathf.Max(0f, amount);
            if (gained <= 0f)
            {
                return;
            }

            currentXp += gained;
            EventBus.RaisePlayerExperienceGained(gained);

            while (currentXp >= RequiredXp)
            {
                currentXp -= RequiredXp;
                Level++;
                RequiredXp = Mathf.Ceil(baseRequiredXp * Mathf.Pow(levelGrowth, Level - 1));
                EventBus.RaisePlayerLevelUp(Level);
            }

            EventBus.RaisePlayerExperienceChanged(Level, currentXp, RequiredXp);
        }
    }
}
