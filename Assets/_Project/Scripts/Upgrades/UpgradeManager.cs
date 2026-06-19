using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class UpgradeManager : MonoBehaviour
    {
        private UpgradeOptionGenerator optionGenerator;
        private GameStateManager stateManager;
        private PlayerStats playerStats;
        private PlayerSkillInventory skillInventory;
        private PlayerHealth playerHealth;
        private LevelUpPanel levelUpPanel;
        private GameDatabase database;

        private void Awake()
        {
            ServiceLocator.Register(this);
            EventBus.PlayerLevelUp += HandleLevelUp;
        }

        private void Start()
        {
            ServiceLocator.TryGet(out optionGenerator);
            ServiceLocator.TryGet(out stateManager);
            ServiceLocator.TryGet(out playerStats);
            ServiceLocator.TryGet(out skillInventory);
            ServiceLocator.TryGet(out playerHealth);
            ServiceLocator.TryGet(out levelUpPanel);
            ServiceLocator.TryGet(out database);
        }

        private void OnDestroy()
        {
            EventBus.PlayerLevelUp -= HandleLevelUp;
        }

        private void HandleLevelUp(int level)
        {
            if (stateManager == null || optionGenerator == null || levelUpPanel == null)
            {
                return;
            }

            stateManager.SetState(GameState.LevelUp);
            levelUpPanel.Show(optionGenerator.GenerateOptions(3, level), ApplyUpgrade);
        }

        private void ApplyUpgrade(UpgradeData upgrade)
        {
            if (upgrade == null)
            {
                Resume();
                return;
            }

            float power = UpgradeRarityUtility.GetPowerMultiplier(upgrade.Rarity);
            switch (upgrade.Effect)
            {
                case UpgradeEffect.IncreaseDamage:
                    playerStats.GlobalDamageMultiplier += upgrade.Amount * power;
                    break;
                case UpgradeEffect.ReduceCooldown:
                    playerStats.CooldownReduction += upgrade.Amount * power;
                    break;
                case UpgradeEffect.IncreaseProjectileCount:
                    playerStats.ExtraProjectiles += Mathf.Max(1, Mathf.RoundToInt(upgrade.Amount));
                    break;
                case UpgradeEffect.IncreaseArea:
                    playerStats.AreaMultiplier += upgrade.Amount * power;
                    break;
                case UpgradeEffect.IncreaseDuration:
                    playerStats.DurationMultiplier += upgrade.Amount * power;
                    break;
                case UpgradeEffect.IncreaseMoveSpeed:
                    playerStats.MoveSpeed += upgrade.Amount * power;
                    break;
                case UpgradeEffect.IncreasePickupRadius:
                    playerStats.PickupRadius += upgrade.Amount * power;
                    break;
                case UpgradeEffect.IncreaseMaxHP:
                    playerStats.MaxHP += upgrade.Amount * power;
                    if (playerHealth != null)
                    {
                        playerHealth.HealToFull();
                    }
                    break;
                case UpgradeEffect.IncreaseCriticalChance:
                    playerStats.CriticalChance += upgrade.Amount * power;
                    break;
                case UpgradeEffect.IncreaseCriticalDamage:
                    playerStats.CriticalMultiplier += upgrade.Amount * power;
                    break;
                case UpgradeEffect.UnlockNewSkill:
                    UnlockTargetSkill(upgrade.TargetSkillName);
                    break;
                case UpgradeEffect.UpgradeExistingSkill:
                    UpgradeTargetSkill(upgrade.TargetSkillName);
                    break;
                case UpgradeEffect.ImproveElementalTag:
                    playerStats.GlobalDamageMultiplier += upgrade.Amount * power * 0.6f;
                    break;
                case UpgradeEffect.ActivateOrStrengthenSynergy:
                    UnlockTargetSkill(upgrade.TargetSkillName);
                    playerStats.GlobalDamageMultiplier += upgrade.Amount * power * 0.35f;
                    break;
            }

            Resume();
        }

        private void UnlockTargetSkill(string skillName)
        {
            if (database == null || skillInventory == null)
            {
                return;
            }

            SkillData skill = database.FindSkill(skillName);
            if (skill != null)
            {
                skillInventory.UnlockSkill(skill);
            }
        }

        private void UpgradeTargetSkill(string skillName)
        {
            if (database == null || skillInventory == null)
            {
                return;
            }

            SkillData skill = database.FindSkill(skillName);
            if (skill != null)
            {
                skillInventory.UpgradeSkill(skill);
            }
        }

        private void Resume()
        {
            if (levelUpPanel != null)
            {
                levelUpPanel.Hide();
            }

            if (stateManager != null)
            {
                stateManager.SetState(GameState.Playing);
            }
        }
    }
}
