using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class BreakableObjectSpawner : MonoBehaviour
    {
        public void Configure(GameObject breakableObject, BreakableObjectData data, System.Random random)
        {
            if (breakableObject == null || data == null || random == null)
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

            Vector3 scale = Vector3.Lerp(data.MinScale, data.MaxScale, (float)random.NextDouble());
            breakableObject.transform.localScale = scale;
            float tint = 0.85f + (float)random.NextDouble() * 0.3f;
            breakable.Initialize(data, data.BaseColor * tint);
        }
    }
}
