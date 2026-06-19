using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public static class WeightedRandom
    {
        public static T Pick<T>(IList<T> items, Func<T, float> weightSelector)
        {
            if (items == null || items.Count == 0)
            {
                return default(T);
            }

            float totalWeight = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                totalWeight += Mathf.Max(0f, weightSelector(items[i]));
            }

            if (totalWeight <= 0f)
            {
                return items[UnityEngine.Random.Range(0, items.Count)];
            }

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            for (int i = 0; i < items.Count; i++)
            {
                roll -= Mathf.Max(0f, weightSelector(items[i]));
                if (roll <= 0f)
                {
                    return items[i];
                }
            }

            return items[items.Count - 1];
        }
    }
}
