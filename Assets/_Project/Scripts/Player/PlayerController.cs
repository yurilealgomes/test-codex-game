using UnityEngine;

namespace ArcaneSurvival
{
    [RequireComponent(typeof(PlayerStats))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerStats stats;
        private GameStateManager stateManager;
        private Vector3 velocity;

        public Vector3 MoveDirection { get; private set; }

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out stateManager);
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
            MoveDirection = input.sqrMagnitude > 1f ? input.normalized : input;
            velocity = MoveDirection * stats.MoveSpeed;
            transform.position += velocity * Time.deltaTime;

            if (MoveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(MoveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 12f);
            }
        }
    }
}
