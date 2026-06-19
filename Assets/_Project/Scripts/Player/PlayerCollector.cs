using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerCollector : MonoBehaviour
    {
        private PlayerStats stats;
        private PlayerExperience experience;
        private GameStateManager stateManager;

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

            Collider[] hits = Physics.OverlapSphere(transform.position, stats.PickupRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                XPOrb orb = hits[i].GetComponentInParent<XPOrb>();
                if (orb != null)
                {
                    orb.Collect(experience);
                }

                SpecialPickup pickup = hits[i].GetComponentInParent<SpecialPickup>();
                if (pickup != null)
                {
                    pickup.Collect();
                }
            }
        }
    }
}
