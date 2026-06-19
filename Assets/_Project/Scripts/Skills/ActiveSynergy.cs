namespace ArcaneSurvival
{
    public sealed class ActiveSynergy
    {
        public SynergyData Data { get; private set; }
        public int Stacks { get; private set; }

        public ActiveSynergy(SynergyData data)
        {
            Data = data;
            Stacks = 1;
        }

        public void AddStack()
        {
            Stacks++;
        }
    }
}
