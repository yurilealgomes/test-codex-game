using UnityEngine;

namespace ArcaneSurvival
{
    public enum UpgradeRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public static class UpgradeRarityUtility
    {
        public static float GetDropWeight(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Rare: return 22f;
                case UpgradeRarity.Epic: return 7f;
                case UpgradeRarity.Legendary: return 1f;
                default: return 70f;
            }
        }

        public static float GetPowerMultiplier(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Rare: return 1.35f;
                case UpgradeRarity.Epic: return 1.8f;
                case UpgradeRarity.Legendary: return 2.6f;
                default: return 1f;
            }
        }

        public static Color GetColor(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Rare: return new Color(0.2f, 0.48f, 1f);
                case UpgradeRarity.Epic: return new Color(0.72f, 0.28f, 1f);
                case UpgradeRarity.Legendary: return new Color(1f, 0.74f, 0.22f);
                default: return new Color(0.86f, 0.88f, 0.9f);
            }
        }
    }
}
