using UnityEngine;

namespace ArcaneSurvival
{
    [RequireComponent(typeof(Camera))]
    public sealed class IsometricCameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(-10f, 15f, -10f);
        [SerializeField] private float followSmoothness = 9.5f;
        [SerializeField] private float zoom = 12.5f;
        [SerializeField] private bool useOrthographic = true;

        private Transform target;
        private Camera cachedCamera;
        private CameraShake shake;

        private void Awake()
        {
            cachedCamera = GetComponent<Camera>();
            cachedCamera.orthographic = useOrthographic;
            cachedCamera.orthographicSize = zoom;
            transform.rotation = Quaternion.Euler(45f, 45f, 0f);
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            PlayerController player;
            if (ServiceLocator.TryGet(out player))
            {
                target = player.transform;
            }

            shake = GetComponent<CameraShake>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 shakeOffset = shake != null ? shake.CurrentOffset : Vector3.zero;
            Vector3 desiredPosition = target.position + offset + shakeOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSmoothness * Time.deltaTime));
        }

        public void SetZoom(float value)
        {
            zoom = Mathf.Clamp(value, 8f, 28f);
            if (cachedCamera != null)
            {
                cachedCamera.orthographicSize = zoom;
            }
        }
    }
}
