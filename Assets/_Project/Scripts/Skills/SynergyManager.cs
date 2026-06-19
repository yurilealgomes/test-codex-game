using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SynergyManager : MonoBehaviour
    {
        private readonly List<ActiveSynergy> activeSynergies = new List<ActiveSynergy>();
        private GameDatabase database;
        private PlayerSkillInventory inventory;

        private void Awake()
        {
            ServiceLocator.Register(this);
            EventBus.SkillInventoryChanged += EvaluateSynergies;
        }

        private void Start()
        {
            ServiceLocator.TryGet(out database);
            ServiceLocator.TryGet(out inventory);
            StartCoroutine(EvaluateAfterUiReady());
        }

        private IEnumerator EvaluateAfterUiReady()
        {
            yield return null;
            EvaluateSynergies();
        }

        private void OnDestroy()
        {
            EventBus.SkillInventoryChanged -= EvaluateSynergies;
        }

        public bool HasSynergy(SynergyEffect effect)
        {
            for (int i = 0; i < activeSynergies.Count; i++)
            {
                if (activeSynergies[i].Data.EffectType == effect)
                {
                    return true;
                }
            }

            return false;
        }

        private void EvaluateSynergies()
        {
            if (database == null || inventory == null)
            {
                return;
            }

            for (int i = 0; i < database.Synergies.Count; i++)
            {
                SynergyData data = database.Synergies[i];
                if (data == null || HasSynergy(data.EffectType))
                {
                    continue;
                }

                SynergyCondition condition = new SynergyCondition(data);
                if (condition.IsMet(inventory))
                {
                    activeSynergies.Add(new ActiveSynergy(data));
                    EventBus.RaiseSynergyActivated(data.SynergyName, data.Description);
                }
            }
        }
    }
}
