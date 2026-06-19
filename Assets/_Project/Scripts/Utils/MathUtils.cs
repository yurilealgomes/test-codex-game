using UnityEngine;

namespace ArcaneSurvival
{
    public static class MathUtils
    {
        public static Vector3 Flatten(Vector3 value)
        {
            value.y = 0f;
            return value;
        }

        public static Vector3 DirectionOnPlane(Vector3 from, Vector3 to)
        {
            Vector3 direction = Flatten(to - from);
            float magnitude = direction.magnitude;
            return magnitude <= 0.0001f ? Vector3.zero : direction / magnitude;
        }

        public static Vector3 RandomPointInRing(Vector3 center, float minRadius, float maxRadius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(minRadius, maxRadius);
            return center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        public static Vector3 WithY(Vector3 value, float y)
        {
            value.y = y;
            return value;
        }
    }
}
