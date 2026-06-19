using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerCollector : MonoBehaviour
    {
        private PlayerStats stats;
        private PlayerExperience experience;
        private GameStateManager stateManager;
        private readonly Collider[] pickupHits = new Collider[80];

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            experience = GetComponent<PlayerExperience>();
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

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, stats.PickupRadius, pickupHits);
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = pickupHits[i];
                if (hit == null)
                {
                    continue;
                }

                XPOrb orb = hit.GetComponentInParent<XPOrb>();
                if (orb != null)
                {
                    orb.Collect(experience);
                }

                SpecialPickup pickup = hit.GetComponentInParent<SpecialPickup>();
                if (pickup != null)
                {
                    pickup.Collect();
                }

                pickupHits[i] = null;
            }
        }
    }
}
