namespace ArcaneSurvival
{
    public static class SkillFactory
    {
        public static SkillRuntime CreateRuntime(SkillData data)
        {
            return data == null ? null : new SkillRuntime(data);
        }
    }
}
