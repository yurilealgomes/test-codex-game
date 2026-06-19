using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class BreakableObjectSpawner : MonoBehaviour
    {
        public void Configure(GameObject breakableObject, BreakableObjectData data, float scaleRoll, float tint, string breakableKey, InfiniteWorldManager worldManager)
        {
            if (breakableObject == null || data == null)
            {
                return;
            }

            BreakableObject breakable = breakableObject.GetComponent<BreakableObject>();
            if (breakable == null)
            {
                breakable = breakableObject.AddComponent<BreakableObject>();
            }

            Collider collider = breakableObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            Vector3 scale = Vector3.Lerp(data.MinScale, data.MaxScale, Mathf.Clamp01(scaleRoll));
            breakableObject.transform.localScale = scale;
            breakable.Initialize(data, data.BaseColor * tint, breakableKey, worldManager);
        }
    }
}
