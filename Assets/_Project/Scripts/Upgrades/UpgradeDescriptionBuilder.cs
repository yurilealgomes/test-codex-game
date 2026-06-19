using UnityEngine;

namespace ArcaneSurvival
{
    public static class UpgradeDescriptionBuilder
    {
        public static string Build(UpgradeData upgrade)
        {
            if (upgrade == null)
            {
                return string.Empty;
            }

            float power = UpgradeRarityUtility.GetPowerMultiplier(upgrade.Rarity);
            float value = upgrade.Amount * power;
            string target = string.IsNullOrEmpty(upgrade.TargetSkillName) ? "All Skills" : upgrade.TargetSkillName;

            switch (upgrade.Effect)
            {
                case UpgradeEffect.IncreaseDamage:
                    return "Spell Damage +" + Percent(value);
                case UpgradeEffect.ReduceCooldown:
                    return "Skill Cooldowns -" + Percent(value);
                case UpgradeEffect.IncreaseProjectileCount:
                    return "Projectiles +" + Mathf.Max(1, Mathf.RoundToInt(upgrade.Amount));
                case UpgradeEffect.IncreaseArea:
                    return "Spell Area +" + Percent(value);
                case UpgradeEffect.IncreaseDuration:
                    return "Spell Duration +" + Percent(value);
                case UpgradeEffect.IncreaseMoveSpeed:
                    return "Move Speed +" + value.ToString("0.0");
                case UpgradeEffect.IncreasePickupRadius:
                    return "Pickup Radius +" + value.ToString("0.0");
                case UpgradeEffect.IncreaseMaxHP:
                    return "Max HP +" + Mathf.CeilToInt(value) + " and heal to full";
                case UpgradeEffect.IncreaseCriticalChance:
                    return "Critical Chance +" + Percent(value);
                case UpgradeEffect.IncreaseCriticalDamage:
                    return "Critical Damage +" + Percent(value);
                case UpgradeEffect.IncreaseLuck:
                    return "Luck +" + value.ToString("0.0") + " for better upgrade odds";
                case UpgradeEffect.IncreaseChainCount:
                    return "Lightning Chain Count +" + Mathf.Max(1, Mathf.RoundToInt(upgrade.Amount));
                case UpgradeEffect.IncreaseChainRadius:
                    return "Lightning Chain Radius +" + value.ToString("0.0");
                case UpgradeEffect.UnlockNewSkill:
                    return "Unlock " + target + " as a new skill";
                case UpgradeEffect.UpgradeExistingSkill:
                    return target + " Level +1";
                case UpgradeEffect.ImproveElementalTag:
                    return upgrade.ElementTag + " Damage +" + Percent(value);
                case UpgradeEffect.ActivateOrStrengthenSynergy:
                    return "Unlock " + target + " and increase synergy damage +" + Percent(value * 0.35f);
                default:
                    return upgrade.Description;
            }
        }

        private static string Percent(float value)
        {
            return Mathf.RoundToInt(value * 100f) + "%";
        }
    }
}
