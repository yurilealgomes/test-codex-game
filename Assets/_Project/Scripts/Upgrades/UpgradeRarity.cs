using UnityEngine;

namespace ArcaneSurvival
{
    public enum UpgradeRarity
    {
        Common,
        Uncommon,
        Magic,
        Epic,
        Legendary
    }

    public static class UpgradeRarityUtility
    {
        public static float GetDropWeight(UpgradeRarity rarity)
        {
            return GetDropWeight(rarity, 0f);
        }

        public static float GetDropWeight(UpgradeRarity rarity, float luck)
        {
            float safeLuck = Mathf.Max(0f, luck);
            switch (rarity)
            {
                case UpgradeRarity.Uncommon: return 22f + safeLuck * 0.8f;
                case UpgradeRarity.Magic: return 9f + safeLuck * 0.45f;
                case UpgradeRarity.Epic: return 3.5f + safeLuck * 0.18f;
                case UpgradeRarity.Legendary: return 0.5f + safeLuck * 0.035f;
                default: return Mathf.Max(28f, 65f - safeLuck * 1.35f);
            }
        }

        public static float GetPowerMultiplier(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Uncommon: return 1.18f;
                case UpgradeRarity.Magic: return 1.4f;
                case UpgradeRarity.Epic: return 1.8f;
                case UpgradeRarity.Legendary: return 2.6f;
                default: return 1f;
            }
        }

        public static Color GetColor(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Uncommon: return new Color(0.24f, 0.86f, 0.32f);
                case UpgradeRarity.Magic: return new Color(0.2f, 0.48f, 1f);
                case UpgradeRarity.Epic: return new Color(0.72f, 0.28f, 1f);
                case UpgradeRarity.Legendary: return new Color(1f, 0.74f, 0.22f);
                default: return Color.white;
            }
        }
    }
}
