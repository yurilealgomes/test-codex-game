using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class DespawnWhenFarFromPlayer : MonoBehaviour
    {
        [SerializeField] private float distance = 70f;
        private Transform player;
        private PoolManager poolManager;

        private void Start()
        {
            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                player = playerController.transform;
            }

            ServiceLocator.TryGet(out poolManager);
        }

        private void Update()
        {
            if (player == null || poolManager == null)
            {
                return;
            }

            if (MathUtils.DistanceXZ(transform.position, player.position) > distance)
            {
                poolManager.Despawn(gameObject);
            }
        }

        public void SetDistance(float value)
        {
            distance = value;
        }
    }
}
