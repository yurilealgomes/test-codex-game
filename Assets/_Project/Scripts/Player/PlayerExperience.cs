using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerExperience : MonoBehaviour
    {
        [SerializeField] private float baseRequiredXp = 10f;
        [SerializeField] private float levelGrowth = 1.25f;

        private float currentXp;

        public int Level { get; private set; } = 1;
        public float RequiredXp { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register(this);
            RequiredXp = baseRequiredXp;
        }

        private void Start()
        {
            EventBus.RaisePlayerExperienceChanged(Level, currentXp, RequiredXp);
        }

        public void AddExperience(float amount)
        {
            currentXp += Mathf.Max(0f, amount);

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
