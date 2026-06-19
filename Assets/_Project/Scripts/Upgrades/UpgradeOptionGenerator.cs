using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class UpgradeOptionGenerator : MonoBehaviour
    {
        private GameDatabase database;
        private PlayerSkillInventory skillInventory;
        private PlayerStats playerStats;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out database);
            ServiceLocator.TryGet(out skillInventory);
            ServiceLocator.TryGet(out playerStats);
        }

        public UpgradeData[] GenerateOptions(int count, int playerLevel)
        {
            List<UpgradeData> options = new List<UpgradeData>();
            if (database == null || database.Upgrades.Count == 0)
            {
                return options.ToArray();
            }

            List<UpgradeData> lockedSkillOptions = BuildLockedSkillOptions(playerLevel);
            if (lockedSkillOptions.Count > 0)
            {
                AddUniqueWeightedOptions(options, lockedSkillOptions, count);
                if (options.Count >= count || lockedSkillOptions.Count >= count)
                {
                    return options.ToArray();
                }
            }

            List<UpgradeData> candidates = BuildCandidatePool(playerLevel);
            if (candidates.Count == 0)
            {
                return options.ToArray();
            }

            if (options.Count == 0)
            {
                AddPreferredOwnedSkillUpgrade(options, candidates, count);
            }

            AddUniqueWeightedOptions(options, candidates, count);

            return options.ToArray();
        }

        private List<UpgradeData> BuildLockedSkillOptions(int playerLevel)
        {
            List<UpgradeData> options = new List<UpgradeData>();
            if (!IsNewSkillLevel(playerLevel))
            {
                return options;
            }

            for (int i = 0; i < database.Upgrades.Count; i++)
            {
                UpgradeData upgrade = database.Upgrades[i];
                if (upgrade != null && upgrade.Effect == UpgradeEffect.UnlockNewSkill && !HasSkill(upgrade.TargetSkillName))
                {
                    options.Add(upgrade);
                }
            }

            return options;
        }

        private List<UpgradeData> BuildCandidatePool(int playerLevel)
        {
            List<UpgradeData> candidates = new List<UpgradeData>();
            for (int i = 0; i < database.Upgrades.Count; i++)
            {
                UpgradeData upgrade = database.Upgrades[i];
                if (upgrade != null && IsEligible(upgrade, playerLevel))
                {
                    candidates.Add(upgrade);
                }
            }

            return candidates;
        }

        private bool IsEligible(UpgradeData upgrade, int playerLevel)
        {
            switch (upgrade.Effect)
            {
                case UpgradeEffect.UnlockNewSkill:
                    return IsNewSkillLevel(playerLevel) && !HasSkill(upgrade.TargetSkillName);
                case UpgradeEffect.UpgradeExistingSkill:
                    return HasSkill(upgrade.TargetSkillName);
                case UpgradeEffect.ActivateOrStrengthenSynergy:
                    return IsNewSkillLevel(playerLevel) && !HasSkill(upgrade.TargetSkillName);
                default:
                    if (!string.IsNullOrEmpty(upgrade.TargetSkillName) && !HasSkill(upgrade.TargetSkillName))
                    {
                        return false;
                    }

                    return true;
            }
        }

        private void AddPreferredOwnedSkillUpgrade(List<UpgradeData> options, List<UpgradeData> candidates, int count)
        {
            if (options.Count >= count || skillInventory == null || string.IsNullOrEmpty(skillInventory.StartingSkillName))
            {
                return;
            }

            List<UpgradeData> ownedSkillUpgrades = candidates.FindAll(upgrade => upgrade.Effect == UpgradeEffect.UpgradeExistingSkill && HasSkill(upgrade.TargetSkillName));
            UpgradeData preferred = WeightedRandom.Pick(ownedSkillUpgrades, GetUpgradeWeight);
            if (preferred != null && !options.Contains(preferred))
            {
                options.Add(preferred);
            }
        }

        private float GetUpgradeWeight(UpgradeData upgrade)
        {
            float luck = playerStats != null ? playerStats.Luck : 0f;
            float weight = UpgradeRarityUtility.GetDropWeight(upgrade.Rarity, luck);
            if (upgrade.Effect == UpgradeEffect.UpgradeExistingSkill && HasSkill(upgrade.TargetSkillName))
            {
                weight *= 2.35f;
                if (skillInventory != null && upgrade.TargetSkillName == skillInventory.StartingSkillName)
                {
                    weight *= 1.5f;
                }
            }

            if (upgrade.Effect == UpgradeEffect.UnlockNewSkill || upgrade.Effect == UpgradeEffect.ActivateOrStrengthenSynergy)
            {
                weight *= 0.7f;
            }

            return weight;
        }

        private bool HasSkill(string skillName)
        {
            return skillInventory != null && !string.IsNullOrEmpty(skillName) && skillInventory.HasSkill(skillName);
        }

        private static bool IsNewSkillLevel(int playerLevel)
        {
            return playerLevel > 1 && playerLevel % 5 == 0;
        }

        private void AddUniqueWeightedOptions(List<UpgradeData> options, List<UpgradeData> candidates, int count)
        {
            int attempts = 0;
            while (options.Count < count && attempts < 80)
            {
                attempts++;
                UpgradeData option = WeightedRandom.Pick(candidates, GetUpgradeWeight);
                if (option != null && !options.Contains(option))
                {
                    options.Add(option);
                }
            }
        }
    }
}
