namespace ArcaneSurvival
{
    public sealed class SynergyCondition
    {
        private readonly SynergyData data;

        public SynergyCondition(SynergyData synergyData)
        {
            data = synergyData;
        }

        public bool IsMet(PlayerSkillInventory inventory)
        {
            if (data == null || inventory == null || data.RequiredTags == null)
            {
                return false;
            }

            for (int i = 0; i < data.RequiredTags.Length; i++)
            {
                if (!inventory.HasTag(data.RequiredTags[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
