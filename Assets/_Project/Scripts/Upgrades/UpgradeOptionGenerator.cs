using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class UpgradeOptionGenerator : MonoBehaviour
    {
        private GameDatabase database;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out database);
        }

        public UpgradeData[] GenerateOptions(int count)
        {
            List<UpgradeData> options = new List<UpgradeData>();
            if (database == null || database.Upgrades.Count == 0)
            {
                return options.ToArray();
            }

            int attempts = 0;
            while (options.Count < count && attempts < 40)
            {
                attempts++;
                UpgradeData option = WeightedRandom.Pick(database.Upgrades, upgrade => UpgradeRarityUtility.GetDropWeight(upgrade.Rarity));
                if (option != null && !options.Contains(option))
                {
                    options.Add(option);
                }
            }

            return options.ToArray();
        }
    }
}
