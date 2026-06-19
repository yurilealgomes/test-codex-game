using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class DebugGodModeController : MonoBehaviour
    {
        private const float BreakRadius = 18f;

        private PlayerHealth playerHealth;
        private PlayerExperience playerExperience;
        private PlayerSkillInventory skillInventory;
        private GameDatabase database;
        private WaveDirector waveDirector;
        private Transform playerTransform;
        private Text titleText;
        private readonly List<Text> statusLines = new List<Text>();
        private float infiniteXpTimer;

        public bool OverlayVisible { get; private set; } = true;
        public bool GodModeEnabled { get; private set; }
        public bool NoCooldownsEnabled { get; private set; }
        public bool InfiniteXpEnabled { get; private set; }
        public bool SpawnDebugEnabled { get; private set; }
        public bool ChunkDebugEnabled { get; private set; }
        public bool DamageDebugEnabled { get; private set; }

        private void Awake()
        {
            if (!Application.isEditor && !Debug.isDebugBuild)
            {
                enabled = false;
                return;
            }

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
            if (Input.GetKeyDown(KeyCode.F1))
            {
                OverlayVisible = !OverlayVisible;
                RefreshOverlay();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                NoCooldownsEnabled = !NoCooldownsEnabled;
                RefreshOverlay();
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                InfiniteXpEnabled = !InfiniteXpEnabled;
                RefreshOverlay();
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                SpawnDebugEnabled = !SpawnDebugEnabled;
                RefreshOverlay();
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                ChunkDebugEnabled = !ChunkDebugEnabled;
                RefreshOverlay();
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                AddXp();
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                SetGodMode(!GodModeEnabled);
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                ForceLevelUp();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                SpawnBossNow();
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                ClearEnemies();
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                DamageDebugEnabled = !DamageDebugEnabled;
                RefreshOverlay();
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                BreakNearbyObjects();
            }

            TickInfiniteXp();
        }

        private void SetGodMode(bool enabled)
        {
            GodModeEnabled = enabled;
            if (playerHealth != null)
            {
                playerHealth.SetInvulnerable(enabled);
            }

            RefreshOverlay();
        }

        private void AddXp()
        {
            if (playerExperience != null)
            {
                playerExperience.AddExperience(Mathf.Max(8f, playerExperience.RequiredXp * 0.35f));
            }
        }

        private void ForceLevelUp()
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

        private void ClearEnemies()
        {
            for (int i = EnemyController.ActiveEnemies.Count - 1; i >= 0; i--)
            {
                EnemyController enemy = EnemyController.ActiveEnemies[i];
                if (enemy != null)
                {
                    enemy.HandleDeath();
                }
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

        private void TickInfiniteXp()
        {
            if (!InfiniteXpEnabled || playerExperience == null)
            {
                return;
            }

            infiniteXpTimer -= Time.unscaledDeltaTime;
            if (infiniteXpTimer <= 0f)
            {
                playerExperience.AddExperience(Mathf.Max(2f, playerExperience.RequiredXp * 0.08f));
                infiniteXpTimer = 0.35f;
            }
        }

        private void CreateOverlay()
        {
            Canvas canvas;
            if (!ServiceLocator.TryGet(out canvas))
            {
                return;
            }

            titleText = UIFactory.CreateText(canvas.transform, "Debug Powers Title", "Debug Powers", 16, new Color(0.85f, 1f, 0.9f), TextAnchor.UpperLeft, new Vector2(0.012f, 0.60f), new Vector2(0.31f, 0.64f), Vector2.zero, Vector2.zero);
            string[] names =
            {
                "God Mode",
                "No Cooldowns",
                "Infinite XP",
                "Spawn Debug",
                "Chunk Debug",
                "Damage Debug",
                "F6 Add XP",
                "F8 Level Up",
                "F9 Spawn Boss",
                "F10 Clear Enemies",
                "F12 Break Objects"
            };

            for (int i = 0; i < names.Length; i++)
            {
                float top = 0.595f - i * 0.025f;
                Text line = UIFactory.CreateText(canvas.transform, "Debug Power " + names[i], "", 13, Color.white, TextAnchor.UpperLeft, new Vector2(0.012f, top - 0.025f), new Vector2(0.31f, top), Vector2.zero, Vector2.zero);
                statusLines.Add(line);
            }
        }

        private void RefreshOverlay()
        {
            if (titleText == null)
            {
                return;
            }

            titleText.gameObject.SetActive(OverlayVisible);
            for (int i = 0; i < statusLines.Count; i++)
            {
                statusLines[i].gameObject.SetActive(OverlayVisible);
            }

            SetLine(0, "God Mode", GodModeEnabled);
            SetLine(1, "No Cooldowns", NoCooldownsEnabled);
            SetLine(2, "Infinite XP", InfiniteXpEnabled);
            SetLine(3, "Spawn Debug", SpawnDebugEnabled);
            SetLine(4, "Chunk Debug", ChunkDebugEnabled);
            SetLine(5, "Damage Debug", DamageDebugEnabled);
            SetActionLine(6, "F6 Add XP");
            SetActionLine(7, "F8 Level Up");
            SetActionLine(8, "F9 Spawn Boss");
            SetActionLine(9, "F10 Clear Enemies");
            SetActionLine(10, "F12 Break Objects");
        }

        private void SetLine(int index, string label, bool enabled)
        {
            if (index < 0 || index >= statusLines.Count)
            {
                return;
            }

            statusLines[index].text = label + ": " + (enabled ? "ON" : "OFF");
            statusLines[index].color = enabled ? new Color(0.45f, 1f, 0.45f) : new Color(0.62f, 0.66f, 0.68f);
        }

        private void SetActionLine(int index, string label)
        {
            if (index < 0 || index >= statusLines.Count)
            {
                return;
            }

            statusLines[index].text = label;
            statusLines[index].color = new Color(0.72f, 0.85f, 1f);
        }
    }
}
