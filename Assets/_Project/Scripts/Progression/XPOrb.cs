using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class XPOrb : MonoBehaviour, IPoolable
    {
        [SerializeField] private float value = 1f;
        [SerializeField] private float attractionSpeed = 14f;

        private Transform player;
        private float despawnDistance = 60f;
        private bool collected;

        public void Initialize(float xpValue, float maxDistance)
        {
            value = Mathf.Max(1f, xpValue);
            despawnDistance = maxDistance;
            collected = false;

            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                player = playerController.transform;
            }
        }

        private void Update()
        {
            if (player == null || collected)
            {
                return;
            }

            float distance = MathUtils.DistanceXZ(transform.position, player.position);
            if (distance < 5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position + Vector3.up * 0.8f, attractionSpeed * Time.deltaTime);
            }

            if (distance > despawnDistance)
            {
                ReturnToPool();
            }
        }

        public void Collect(PlayerExperience experience)
        {
            if (collected)
            {
                return;
            }

            collected = true;
            if (experience != null)
            {
                experience.AddExperience(value);
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            PooledObject pooledObject = GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.Despawn();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnSpawnedFromPool()
        {
            collected = false;
        }

        public void OnReturnedToPool()
        {
            collected = false;
        }
    }
}
