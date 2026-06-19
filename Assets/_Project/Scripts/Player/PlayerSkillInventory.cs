using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerSkillInventory : MonoBehaviour
    {
        private readonly List<SkillRuntime> skills = new List<SkillRuntime>();

        public IReadOnlyList<SkillRuntime> Skills { get { return skills; } }

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void AddStartingSkill(SkillRuntime runtime)
        {
            if (runtime == null || skills.Contains(runtime))
            {
                return;
            }

            skills.Add(runtime);
            EventBus.RaiseSkillInventoryChanged();
        }

        public SkillRuntime UnlockSkill(SkillData data)
        {
            if (data == null)
            {
                return null;
            }

            SkillRuntime existing = FindRuntime(data);
            if (existing != null)
            {
                existing.LevelUp();
                EventBus.RaiseSkillInventoryChanged();
                return existing;
            }

            SkillRuntime runtime = SkillFactory.CreateRuntime(data);
            skills.Add(runtime);
            EventBus.RaiseSkillInventoryChanged();
            return runtime;
        }

        public void UpgradeSkill(SkillData data)
        {
            SkillRuntime runtime = FindRuntime(data);
            if (runtime == null)
            {
                UnlockSkill(data);
                return;
            }

            runtime.LevelUp();
            EventBus.RaiseSkillInventoryChanged();
        }

        public bool HasTag(SkillTag tag)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].Data.HasTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public int GetSkillLevel(string skillName)
        {
            SkillRuntime runtime = skills.Find(skill => skill.Data.SkillName == skillName);
            return runtime == null ? 0 : runtime.Level;
        }

        private SkillRuntime FindRuntime(SkillData data)
        {
            return skills.Find(skill => skill.Data == data || skill.Data.SkillName == data.SkillName);
        }
    }
}
