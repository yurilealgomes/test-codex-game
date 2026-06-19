namespace ArcaneSurvival
{
    public sealed class Timer
    {
        public float Duration { get; private set; }
        public float Elapsed { get; private set; }
        public bool IsComplete { get { return Elapsed >= Duration; } }
        public float Progress { get { return Duration <= 0f ? 1f : Elapsed / Duration; } }

        public Timer(float duration)
        {
            Reset(duration);
        }

        public void Reset(float duration)
        {
            Duration = duration;
            Elapsed = 0f;
        }

        public void Tick(float deltaTime)
        {
            Elapsed += deltaTime;
        }
    }
}
