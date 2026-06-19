using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class XPDropper : MonoBehaviour
    {
        public void Drop(float amount, Vector3 position)
        {
            RunProgressionManager progressionManager;
            if (ServiceLocator.TryGet(out progressionManager))
            {
                progressionManager.SpawnXp(amount, position);
            }
        }
    }
}
