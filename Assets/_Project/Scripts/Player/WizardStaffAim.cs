using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class WizardStaffAim : MonoBehaviour
    {
        private Transform staffRoot;
        private PlayerController playerController;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        public void SetStaff(Transform staff)
        {
            staffRoot = staff;
        }

        private void LateUpdate()
        {
            if (staffRoot == null)
            {
                return;
            }

            Vector3 aimDirection = Vector3.zero;
            IDamageable target = SkillTargeting.FindNearestEnemy(transform.position, 18f);
            if (target != null)
            {
                aimDirection = MathUtils.DirectionOnPlane(transform.position, target.Transform.position);
            }
            else if (playerController != null)
            {
                aimDirection = playerController.MoveDirection;
            }

            if (aimDirection.sqrMagnitude > 0.01f)
            {
                Quaternion rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
                staffRoot.rotation = Quaternion.Slerp(staffRoot.rotation, rotation, Time.deltaTime * 16f);
            }
        }
    }
}
