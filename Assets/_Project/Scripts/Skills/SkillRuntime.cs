using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class SkillRuntime
    {
        public SkillData Data { get; private set; }
        public int Level { get; private set; }
        public float CooldownRemaining { get; private set; }

        public SkillRuntime(SkillData data)
        {
            Data = data;
            Level = 1;
            CooldownRemaining = Random.Range(0f, data.Cooldown * 0.4f);
        }

        public void Update(SkillCaster caster, float deltaTime)
        {
            if (Data.SkillType == SkillType.Passive)
            {
                SkillEffect.MaintainPassive(this, caster);
                return;
            }

            CooldownRemaining -= deltaTime;
            if (CooldownRemaining <= 0f)
            {
                SkillEffect.Execute(this, caster);
                DebugGodModeController debugTools;
                bool noCooldowns = ServiceLocator.TryGet(out debugTools) && debugTools.NoCooldownsEnabled;
                CooldownRemaining = noCooldowns ? 0.05f : caster.Stats.ApplyCooldownReduction(Data.Cooldown);
            }
        }

        public void LevelUp()
        {
            Level++;
        }
    }
}
