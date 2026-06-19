using UnityEngine;

namespace ArcaneSurvival
{
    public static class SpawnRingCalculator
    {
        public static Vector3 CalculatePosition(Vector3 center, float minRadius, float maxRadius, Camera camera)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector3 candidate = MathUtils.RandomPointInRing(center, minRadius, maxRadius);
                if (camera == null || IsOutsideCamera(candidate, camera))
                {
                    return candidate;
                }
            }

            return MathUtils.RandomPointInRing(center, minRadius, maxRadius);
        }

        private static bool IsOutsideCamera(Vector3 worldPosition, Camera camera)
        {
            Vector3 viewport = camera.WorldToViewportPoint(worldPosition + Vector3.up);
            return viewport.z < 0f || viewport.x < -0.08f || viewport.x > 1.08f || viewport.y < -0.08f || viewport.y > 1.08f;
        }
    }
}
