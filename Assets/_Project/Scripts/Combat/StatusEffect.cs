using System;

namespace ArcaneSurvival
{
    public enum StatusEffectType
    {
        Slow,
        Burn,
        Pull
    }

    [Serializable]
    public sealed class StatusEffect
    {
        public StatusEffectType Type;
        public float Magnitude;
        public float Duration;

        public StatusEffect Clone()
        {
            return new StatusEffect
            {
                Type = Type,
                Magnitude = Magnitude,
                Duration = Duration
            };
        }
    }
}
