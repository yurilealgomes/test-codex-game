using System.Collections.Generic;

namespace ArcaneSurvival
{
    public sealed class GameDatabase
    {
        public readonly List<SkillData> Skills = new List<SkillData>();
        public readonly List<EnemyData> Enemies = new List<EnemyData>();
        public readonly List<BossData> Bosses = new List<BossData>();
        public readonly List<UpgradeData> Upgrades = new List<UpgradeData>();
        public readonly List<SynergyData> Synergies = new List<SynergyData>();

        public PerformanceSettings PerformanceSettings;
        public WorldChunkData WorldChunkData;

        public SkillData FindSkill(string skillName)
        {
            return Skills.Find(skill => skill != null && skill.SkillName == skillName);
        }
    }
}
