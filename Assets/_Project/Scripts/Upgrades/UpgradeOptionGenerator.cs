using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class UpgradeOptionGenerator : MonoBehaviour
    {
        private GameDatabase database;
        private PlayerSkillInventory skillInventory;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out database);
            ServiceLocator.TryGet(out skillInventory);
        }

        public UpgradeData[] GenerateOptions(int count, int playerLevel)
        {
            List<UpgradeData> options = new List<UpgradeData>();
            if (database == null || database.Upgrades.Count == 0)
            {
                return options.ToArray();
            }

            List<UpgradeData> candidates = BuildCandidatePool(playerLevel);
            if (candidates.Count == 0)
            {
                return options.ToArray();
            }

            AddPreferredOwnedSkillUpgrade(options, candidates, count);

            int attempts = 0;
            while (options.Count < count && attempts < 60)
            {
                attempts++;
                UpgradeData option = WeightedRandom.Pick(candidates, GetUpgradeWeight);
                if (option != null && !options.Contains(option))
                {
                    options.Add(option);
                }
            }

            return options.ToArray();
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
            float weight = UpgradeRarityUtility.GetDropWeight(upgrade.Rarity);
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
    }
}
