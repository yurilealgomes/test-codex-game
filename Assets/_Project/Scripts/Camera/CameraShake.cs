using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private bool shakeEnabled = true;

        private float amplitude;
        private float remaining;

        public Vector3 CurrentOffset { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Update()
        {
            if (!shakeEnabled || remaining <= 0f)
            {
                CurrentOffset = Vector3.zero;
                return;
            }

            remaining -= Time.deltaTime;
            CurrentOffset = new Vector3(
                Random.Range(-amplitude, amplitude),
                Random.Range(-amplitude, amplitude),
                Random.Range(-amplitude, amplitude));
        }

        public void Shake(float newAmplitude, float duration)
        {
            if (!shakeEnabled)
            {
                return;
            }

            amplitude = Mathf.Max(amplitude, newAmplitude);
            remaining = Mathf.Max(remaining, duration);
        }

        public void SetEnabled(bool enabled)
        {
            shakeEnabled = enabled;
        }
    }
}
