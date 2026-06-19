using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class DebugGodModeController : MonoBehaviour
    {
        private const float BreakRadius = 18f;

        private bool godModeEnabled;
        private PlayerHealth playerHealth;
        private PlayerExperience playerExperience;
        private PlayerSkillInventory skillInventory;
        private GameDatabase database;
        private WaveDirector waveDirector;
        private Transform playerTransform;
        private Text overlayText;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out playerHealth);
            ServiceLocator.TryGet(out playerExperience);
            ServiceLocator.TryGet(out skillInventory);
            ServiceLocator.TryGet(out database);
            ServiceLocator.TryGet(out waveDirector);

            PlayerController playerController;
            if (ServiceLocator.TryGet(out playerController))
            {
                playerTransform = playerController.transform;
            }

            CreateOverlay();
            RefreshOverlay();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                SetGodMode(!godModeEnabled);
            }

            if (!godModeEnabled)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                UnlockAllSkills();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                GrantLevelUp();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                SpawnBossNow();
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                BreakNearbyObjects();
            }
        }

        private void SetGodMode(bool enabled)
        {
            godModeEnabled = enabled;
            if (playerHealth != null)
            {
                playerHealth.SetInvulnerable(enabled);
            }

            RefreshOverlay();
        }

        private void GrantLevelUp()
        {
            if (playerExperience != null)
            {
                playerExperience.AddExperience(playerExperience.RequiredXp);
            }
        }

        private void UnlockAllSkills()
        {
            if (database == null || skillInventory == null)
            {
                return;
            }

            for (int i = 0; i < database.Skills.Count; i++)
            {
                skillInventory.UnlockSkill(database.Skills[i]);
            }
        }

        private void SpawnBossNow()
        {
            if (waveDirector != null)
            {
                waveDirector.SpawnBossNow();
            }
        }

        private void BreakNearbyObjects()
        {
            Vector3 origin = playerTransform != null ? playerTransform.position : Vector3.zero;
            int brokenCount = 0;

            for (int i = BreakableObject.ActiveBreakables.Count - 1; i >= 0; i--)
            {
                BreakableObject breakable = BreakableObject.ActiveBreakables[i];
                if (breakable == null || !breakable.IsAlive)
                {
                    continue;
                }

                if ((breakable.transform.position - origin).sqrMagnitude <= BreakRadius * BreakRadius)
                {
                    breakable.BreakInstantly();
                    brokenCount++;
                }
            }

            if (brokenCount == 0)
            {
                BreakNearestObjects(origin, 3);
            }
        }

        private void BreakNearestObjects(Vector3 origin, int count)
        {
            for (int broken = 0; broken < count; broken++)
            {
                BreakableObject nearest = null;
                float bestDistance = float.MaxValue;

                for (int i = 0; i < BreakableObject.ActiveBreakables.Count; i++)
                {
                    BreakableObject breakable = BreakableObject.ActiveBreakables[i];
                    if (breakable == null || !breakable.IsAlive)
                    {
                        continue;
                    }

                    float distance = (breakable.transform.position - origin).sqrMagnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        nearest = breakable;
                    }
                }

                if (nearest == null)
                {
                    return;
                }

                nearest.BreakInstantly();
            }
        }

        private void CreateOverlay()
        {
            Canvas canvas;
            if (!ServiceLocator.TryGet(out canvas))
            {
                return;
            }

            overlayText = UIFactory.CreateText(canvas.transform, "Debug God Mode Overlay", "", 15, new Color(0.55f, 1f, 0.65f), TextAnchor.UpperLeft, new Vector2(0.012f, 0.68f), new Vector2(0.31f, 0.81f), Vector2.zero, Vector2.zero);
        }

        private void RefreshOverlay()
        {
            if (overlayText == null)
            {
                return;
            }

            overlayText.gameObject.SetActive(godModeEnabled);
            overlayText.text = "GOD MODE\nF6 Unlock Skills\nF8 Level Up\nF9 Spawn Boss\nF10 Break Objects";
        }
    }
}
