using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class EnemyMovement : MonoBehaviour
    {
        private EnemyController controller;
        private Transform target;
        private float flankSign;

        public void Initialize(EnemyController enemyController, Transform playerTarget)
        {
            controller = enemyController;
            target = playerTarget;
            flankSign = Random.value < 0.5f ? -1f : 1f;
        }

        public void Tick(float deltaTime)
        {
            if (controller == null || target == null || !controller.IsAlive)
            {
                return;
            }

            Vector3 direction = MathUtils.DirectionOnPlane(transform.position, target.position);
            if (controller.Data.MovementStyle == EnemyMovementStyle.Flanking)
            {
                Vector3 side = Vector3.Cross(Vector3.up, direction) * flankSign;
                direction = (direction + side * 0.42f).normalized;
            }

            float styleMultiplier = controller.Data.MovementStyle == EnemyMovementStyle.Heavy ? 0.82f : 1f;
            transform.position += direction * controller.CurrentMoveSpeed * styleMultiplier * deltaTime;

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), deltaTime * 8f);
            }
        }
    }
}
