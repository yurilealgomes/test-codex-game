using UnityEngine;

namespace ArcaneSurvival
{
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerStats stats;
        private GameStateManager stateManager;
        private Camera mainCamera;
        private Vector3 velocity;

        public Vector3 MoveDirection { get; private set; }

        [SerializeField] private float acceleration = 28f;
        [SerializeField] private float deceleration = 34f;
        [SerializeField] private float rotationSpeed = 14f;

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (stateManager == null || !stateManager.IsGameplayRunning)
            {
                return;
            }

            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
            if (Input.GetKey(KeyCode.D)) horizontal += 1f;
            if (Input.GetKey(KeyCode.S)) vertical -= 1f;
            if (Input.GetKey(KeyCode.W)) vertical += 1f;

            Vector3 input = new Vector3(horizontal, 0f, vertical);
            input = input.sqrMagnitude > 1f ? input.normalized : input;

            Vector3 desiredDirection = GetCameraRelativeDirection(input);
            Vector3 desiredVelocity = desiredDirection * stats.MoveSpeed;
            float smoothing = desiredDirection.sqrMagnitude > 0.001f ? acceleration : deceleration;
            velocity = Vector3.MoveTowards(velocity, desiredVelocity, smoothing * Time.deltaTime);
            MoveDirection = velocity.sqrMagnitude > 0.001f ? velocity.normalized : Vector3.zero;
            transform.position += velocity * Time.deltaTime;

            if (MoveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(MoveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        private Vector3 GetCameraRelativeDirection(Vector3 input)
        {
            if (input.sqrMagnitude <= 0.001f)
            {
                return Vector3.zero;
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera == null)
            {
                return input.normalized;
            }

            Vector3 screenRight = MathUtils.Flatten(mainCamera.transform.right).normalized;
            Vector3 screenUp = MathUtils.Flatten(mainCamera.transform.up).normalized;
            Vector3 direction = screenRight * input.x + screenUp * input.z;
            return direction.sqrMagnitude > 1f ? direction.normalized : direction;
        }
    }
}
