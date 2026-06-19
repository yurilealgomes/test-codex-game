using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PickupManager : MonoBehaviour
    {
        private PoolManager poolManager;
        private PickupData magnetData;
        private PickupData healData;
        private Transform player;
        private PlayerHealth playerHealth;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out poolManager);

            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                player = playerController.transform;
            }

            ServiceLocator.TryGet(out playerHealth);

            GameDatabase database;
            if (ServiceLocator.TryGet(out database))
            {
                magnetData = database.FindPickup(PickupType.Magnet);
                healData = database.FindPickup(PickupType.Heal);
            }
        }

        public void TrySpawnMagnet(Vector3 position, float chance)
        {
            if (Random.value > chance)
            {
                return;
            }

            SpawnPickup(PickupType.Magnet, position);
        }

        public void TrySpawnHeal(Vector3 position, float chance)
        {
            if (Random.value > chance)
            {
                return;
            }

            SpawnPickup(PickupType.Heal, position);
        }

        public void SpawnPickup(PickupType type, Vector3 position)
        {
            if (poolManager == null)
            {
                return;
            }

            PickupData data = GetPickupData(type);
            if (data == null)
            {
                return;
            }

            GameObject pickupObject = poolManager.Spawn("SpecialPickup", position + Vector3.up * 0.45f, Quaternion.identity);
            if (pickupObject == null)
            {
                return;
            }

            SpecialPickup pickup = pickupObject.GetComponent<SpecialPickup>();
            if (pickup != null)
            {
                pickup.Initialize(data);
            }
        }

        public void ApplyPickup(SpecialPickup pickup)
        {
            if (pickup == null)
            {
                return;
            }

            if (pickup.Type == PickupType.Magnet)
            {
                XPOrb.PullAllTo(player);
                SkillEffect.SpawnVfx(player != null ? player.position + Vector3.up * 1.1f : pickup.transform.position, new Color(0.2f, 0.85f, 1f), Vector3.one * 1.8f, 0.35f);
                EventBus.RaisePickupCollected("Magnet");
            }
            else if (pickup.Type == PickupType.Heal)
            {
                if (playerHealth == null)
                {
                    ServiceLocator.TryGet(out playerHealth);
                }

                if (playerHealth != null)
                {
                    playerHealth.Heal(pickup.Value);
                }

                SkillEffect.SpawnVfx(player != null ? player.position + Vector3.up * 1.1f : pickup.transform.position, new Color(0.25f, 1f, 0.42f), Vector3.one * 1.4f, 0.32f);
                EventBus.RaisePickupCollected("Heal +" + Mathf.CeilToInt(pickup.Value));
            }
        }

        private PickupData GetPickupData(PickupType type)
        {
            switch (type)
            {
                case PickupType.Magnet:
                    return magnetData;
                case PickupType.Heal:
                    return healData;
                default:
                    return null;
            }
        }
    }
}
