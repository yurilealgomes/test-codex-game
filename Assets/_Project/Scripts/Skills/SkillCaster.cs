using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SkillCaster : MonoBehaviour
    {
        public PlayerStats Stats { get; private set; }
        public PlayerSkillInventory Inventory { get; private set; }

        private GameStateManager stateManager;

        private void Awake()
        {
            Stats = GetComponent<PlayerStats>();
            Inventory = GetComponent<PlayerSkillInventory>();
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
        }

        private void Update()
        {
            if (stateManager == null || !stateManager.IsGameplayRunning)
            {
                return;
            }

            for (int i = 0; i < Inventory.Skills.Count; i++)
            {
                Inventory.Skills[i].Update(this, Time.deltaTime);
            }
        }

        public DamageInfo BuildDamage(SkillData data, float multiplier)
        {
            float levelScale = 1f + (Inventory.GetSkillLevel(data.SkillName) - 1) * 0.18f;
            return Stats.BuildDamage(data.BaseDamage * multiplier * levelScale, data.CanCrit, gameObject, data.ElementTags);
        }
    }
}
