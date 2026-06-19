using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public static class GameBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (Object.FindObjectOfType<GameManager>() != null)
            {
                return;
            }

            EventBus.Clear();
            ServiceLocator.Clear();
            EnemyController.ActiveEnemies.Clear();
            BossController.ActiveBosses.Clear();
            BreakableObject.ActiveBreakables.Clear();

            GameDatabase database = CreateDatabase();
            ServiceLocator.Register(database);

            CreateCanvas();
            GameObject systems = CreateSystems();
            CreatePools(systems.GetComponent<PoolManager>(), database);
            GameObject player = CreatePlayer(database);
            CreateCamera(player.transform);
            CreateLights();
        }

        private static GameObject CreateSystems()
        {
            GameObject systems = new GameObject("Game Systems");
            systems.AddComponent<GameStateManager>();
            systems.AddComponent<GameManager>();
            systems.AddComponent<PoolManager>();
            systems.AddComponent<RunTimer>();
            systems.AddComponent<DifficultyScaler>();
            systems.AddComponent<RunProgressionManager>();
            systems.AddComponent<PickupManager>();
            systems.AddComponent<EndlessModeManager>();
            systems.AddComponent<EnemySpawnDirector>();
            systems.AddComponent<WaveDirector>();
            systems.AddComponent<UpgradeOptionGenerator>();
            systems.AddComponent<UpgradeManager>();
            systems.AddComponent<SynergyManager>();
            systems.AddComponent<InfiniteWorldManager>();
            systems.AddComponent<HUDController>();
            systems.AddComponent<LevelUpPanel>();
            systems.AddComponent<PauseMenu>();
            systems.AddComponent<GameOverPanel>();
            systems.AddComponent<PostBossChoicePanel>();
            systems.AddComponent<VictoryPanel>();
            systems.AddComponent<MainMenu>();
            systems.AddComponent<StartingSkillSelectionPanel>();
            systems.AddComponent<DebugGodModeController>();
            systems.AddComponent<BossWarningUI>();
            systems.AddComponent<BossHealthBar>();
            systems.AddComponent<SynergyNotificationUI>();
            systems.AddComponent<MetaProgressionPlaceholder>();
            return systems;
        }

        private static void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Game Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            ServiceLocator.Register(canvas);

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("Event System", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystem.transform.SetParent(canvasObject.transform);
            }
        }

        private static GameObject CreatePlayer(GameDatabase database)
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.layer = LayerOrDefault("Player");
            player.transform.position = Vector3.zero;
            player.transform.localScale = new Vector3(1f, 1.25f, 1f);
            SetColor(player, new Color(0.08f, 0.38f, 0.42f));

            PlayerStats stats = player.AddComponent<PlayerStats>();
            player.AddComponent<PlayerVisualController>();
            player.AddComponent<PlayerHealth>();
            player.AddComponent<PlayerExperience>();
            player.AddComponent<PlayerSkillInventory>();
            player.AddComponent<PlayerCollector>();
            player.AddComponent<PlayerController>();
            player.AddComponent<SkillCaster>();

            stats.MaxHP = 100f;
            return player;
        }

        private static void CreateCamera(Transform target)
        {
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = target.position + new Vector3(-10f, 15f, -10f);
            cameraObject.transform.rotation = Quaternion.Euler(45f, 45f, 0f);

            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.045f, 0.055f, 0.07f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 500f;

            cameraObject.AddComponent<CameraShake>();
            cameraObject.AddComponent<IsometricCameraFollow>();
        }

        private static void CreateLights()
        {
            GameObject lightObject = new GameObject("Key Light", typeof(Light));
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color(1f, 0.94f, 0.84f);
            lightObject.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }

        private static void CreatePools(PoolManager poolManager, GameDatabase database)
        {
            GameObject prefabRoot = new GameObject("Runtime Pool Prefabs");
            prefabRoot.SetActive(false);

            poolManager.RegisterPool("Enemy", CreateEnemyPrefab(prefabRoot.transform), 80, database.PerformanceSettings.MaxAliveEnemies);
            poolManager.RegisterPool("Boss", CreateBossPrefab(prefabRoot.transform), 2, 8);
            poolManager.RegisterPool("PlayerProjectile", CreateProjectilePrefab(prefabRoot.transform, "Player Projectile Prefab", new Color(0.32f, 0.68f, 1f), 0.34f), 80, database.PerformanceSettings.MaxAliveProjectiles);
            poolManager.RegisterPool("EnemyProjectile", CreateProjectilePrefab(prefabRoot.transform, "Enemy Projectile Prefab", new Color(1f, 0.22f, 0.02f), 0.38f), 40, 180);
            poolManager.RegisterPool("XPOrb", CreateXpOrbPrefab(prefabRoot.transform), 80, 500);
            poolManager.RegisterPool("SpecialPickup", CreateSpecialPickupPrefab(prefabRoot.transform), 8, 48);
            poolManager.RegisterPool("FloatingDamageText", CreateFloatingTextPrefab(prefabRoot.transform), 24, database.PerformanceSettings.MaxFloatingDamageTexts);
            poolManager.RegisterPool("AreaDamage", CreateAreaDamagePrefab(prefabRoot.transform), 24, 160);
            poolManager.RegisterPool("SkillVFX", CreateVfxPrefab(prefabRoot.transform), 40, database.PerformanceSettings.MaxVfx);
            poolManager.RegisterPool("ChainLightningEffect", CreateChainLightningPrefab(prefabRoot.transform), 16, 120);
            poolManager.RegisterPool("BreakableBreakEffect", CreateBreakableBreakPrefab(prefabRoot.transform), 12, 80);
        }

        private static GameObject CreateEnemyPrefab(Transform parent)
        {
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Enemy Prefab";
            enemy.transform.SetParent(parent);
            enemy.layer = LayerOrDefault("Enemy");
            enemy.AddComponent<PooledObject>();
            enemy.AddComponent<EnemyController>();
            enemy.AddComponent<DespawnWhenFarFromPlayer>().SetDistance(70f);
            return enemy;
        }

        private static GameObject CreateBossPrefab(Transform parent)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Boss Prefab";
            boss.transform.SetParent(parent);
            boss.layer = LayerOrDefault("Enemy");
            boss.AddComponent<PooledObject>();
            boss.AddComponent<BossController>();
            boss.AddComponent<BossSpawnWarning>();
            return boss;
        }

        private static GameObject CreateProjectilePrefab(Transform parent, string name, Color color, float size)
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = name;
            projectile.transform.SetParent(parent);
            projectile.transform.localScale = Vector3.one * size;
            projectile.layer = LayerOrDefault("Projectile");
            SetColor(projectile, color);
            projectile.AddComponent<PooledObject>();
            projectile.AddComponent<Projectile>();
            return projectile;
        }

        private static GameObject CreateXpOrbPrefab(Transform parent)
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "XP Orb Prefab";
            orb.transform.SetParent(parent);
            orb.transform.localScale = Vector3.one * 0.45f;
            orb.layer = LayerOrDefault("Pickup");
            SetColor(orb, new Color(0.25f, 0.7f, 1f));
            AddDropBeacon(orb, new Color(0.25f, 0.7f, 1f), 3.2f, 0.06f, 4f, 1f);
            orb.AddComponent<PooledObject>();
            orb.AddComponent<XPOrb>();
            return orb;
        }

        private static GameObject CreateSpecialPickupPrefab(Transform parent)
        {
            GameObject pickup = new GameObject("Special Pickup Prefab");
            pickup.name = "Special Pickup Prefab";
            pickup.transform.SetParent(parent);
            pickup.layer = LayerOrDefault("Pickup");
            SphereCollider pickupCollider = pickup.AddComponent<SphereCollider>();
            pickupCollider.radius = 0.75f;
            pickupCollider.isTrigger = true;

            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "Pickup Core";
            core.transform.SetParent(pickup.transform, false);
            core.transform.localScale = Vector3.one * 0.52f;
            RemoveCollider(core);
            SetColor(core, new Color(0.2f, 0.85f, 1f));

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Magnet Ring";
            ring.transform.SetParent(pickup.transform, false);
            ring.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            ring.transform.localScale = new Vector3(0.82f, 0.035f, 0.82f);
            RemoveCollider(ring);
            SetColor(ring, new Color(0.8f, 0.95f, 1f));

            GameObject leftPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPole.name = "Magnet Left Pole";
            leftPole.transform.SetParent(pickup.transform, false);
            leftPole.transform.localPosition = new Vector3(-0.28f, 0f, 0f);
            leftPole.transform.localScale = new Vector3(0.14f, 0.54f, 0.14f);
            RemoveCollider(leftPole);
            SetColor(leftPole, new Color(0.35f, 0.95f, 1f));

            GameObject rightPole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPole.name = "Magnet Right Pole";
            rightPole.transform.SetParent(pickup.transform, false);
            rightPole.transform.localPosition = new Vector3(0.28f, 0f, 0f);
            rightPole.transform.localScale = new Vector3(0.14f, 0.54f, 0.14f);
            RemoveCollider(rightPole);
            SetColor(rightPole, new Color(1f, 0.95f, 0.35f));

            GameObject healVertical = GameObject.CreatePrimitive(PrimitiveType.Cube);
            healVertical.name = "Heal Vertical Bar";
            healVertical.transform.SetParent(pickup.transform, false);
            healVertical.transform.localScale = new Vector3(0.16f, 0.62f, 0.16f);
            RemoveCollider(healVertical);
            SetColor(healVertical, new Color(0.25f, 1f, 0.42f));

            GameObject healHorizontal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            healHorizontal.name = "Heal Horizontal Bar";
            healHorizontal.transform.SetParent(pickup.transform, false);
            healHorizontal.transform.localScale = new Vector3(0.62f, 0.16f, 0.16f);
            RemoveCollider(healHorizontal);
            SetColor(healHorizontal, new Color(0.25f, 1f, 0.42f));

            AddDropBeacon(pickup, new Color(0.2f, 0.85f, 1f), 4.2f, 0.09f, 7f, 2.2f);
            pickup.AddComponent<PooledObject>();
            pickup.AddComponent<SpecialPickup>();
            return pickup;
        }

        private static GameObject CreateFloatingTextPrefab(Transform parent)
        {
            GameObject text = new GameObject("Floating Damage Text Prefab", typeof(TextMesh));
            text.transform.SetParent(parent);
            text.AddComponent<PooledObject>();
            text.AddComponent<FloatingDamageText>();
            return text;
        }

        private static GameObject CreateAreaDamagePrefab(Transform parent)
        {
            GameObject area = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            area.name = "Area Damage Prefab";
            area.transform.SetParent(parent);
            area.layer = LayerOrDefault("Projectile");
            SetColor(area, new Color(0.5f, 0.15f, 0.8f, 0.72f));
            area.AddComponent<PooledObject>();
            area.AddComponent<AreaDamage>();
            return area;
        }

        private static GameObject CreateVfxPrefab(Transform parent)
        {
            GameObject vfx = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vfx.name = "Skill VFX Prefab";
            vfx.transform.SetParent(parent);
            SetColor(vfx, Color.white);
            RemoveCollider(vfx);
            vfx.AddComponent<PooledObject>();
            vfx.AddComponent<SimpleVFX>();
            return vfx;
        }

        private static GameObject CreateChainLightningPrefab(Transform parent)
        {
            GameObject effect = new GameObject("Chain Lightning Effect Prefab", typeof(LineRenderer));
            effect.transform.SetParent(parent);
            LineRenderer lineRenderer = effect.GetComponent<LineRenderer>();
            Shader shader = Shader.Find("Sprites/Default");
            lineRenderer.material = new Material(shader != null ? shader : Shader.Find("Standard"));
            lineRenderer.textureMode = LineTextureMode.Stretch;
            effect.AddComponent<PooledObject>();
            effect.AddComponent<ChainLightningEffect>();
            return effect;
        }

        private static GameObject CreateBreakableBreakPrefab(Transform parent)
        {
            GameObject root = new GameObject("Breakable Break Effect Prefab");
            root.transform.SetParent(parent);
            for (int i = 0; i < 6; i++)
            {
                GameObject shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shard.name = "Shard " + (i + 1);
                shard.transform.SetParent(root.transform, false);
                shard.transform.localPosition = Random.onUnitSphere * 0.12f;
                shard.transform.localScale = Vector3.one * 0.16f;
                Collider collider = shard.GetComponent<Collider>();
                if (collider != null)
                {
                    Object.Destroy(collider);
                }
                SetColor(shard, Color.white);
            }

            root.AddComponent<PooledObject>();
            root.AddComponent<BreakableBreakEffect>();
            return root;
        }

        private static GameDatabase CreateDatabase()
        {
            GameDatabase database = new GameDatabase();
            database.PerformanceSettings = ScriptableObject.CreateInstance<PerformanceSettings>();
            database.RunBalanceSettings = new RunBalanceSettings();
            database.WorldChunkData = CreateWorldChunkData();
            CreateSkills(database);
            CreateEnemies(database);
            CreateBosses(database);
            CreatePickups(database);
            CreateUpgrades(database);
            CreateSynergies(database);
            return database;
        }

        private static WorldChunkData CreateWorldChunkData()
        {
            WorldChunkData data = ScriptableObject.CreateInstance<WorldChunkData>();
            data.PrimaryGroundColor = new Color(0.14f, 0.20f, 0.18f);
            data.SecondaryGroundColor = new Color(0.11f, 0.16f, 0.18f);
            data.DecorationColor = new Color(0.26f, 0.30f, 0.29f);
            data.DecorationsPerChunk = 0;
            data.BreakablesPerChunk = 3;
            data.MinBreakableDistanceFromPlayer = 18f;
            data.CameraBreakableMargin = 0.12f;
            data.BreakablePlacementAttempts = 10;
            data.BreakableObjects = new[]
            {
                CreateBreakableData("Arcane Crystal", 16f, 2f, new Color(0.34f, 0.74f, 1f), new Vector3(0.36f, 0.85f, 0.36f), new Vector3(0.68f, 1.55f, 0.68f)),
                CreateBreakableData("Rune Stone", 24f, 3f, new Color(0.48f, 0.42f, 0.34f), new Vector3(0.55f, 0.45f, 0.55f), new Vector3(1f, 0.85f, 1f)),
                CreateBreakableData("Small Relic Pillar", 32f, 4f, new Color(0.52f, 0.38f, 0.78f), new Vector3(0.45f, 0.9f, 0.45f), new Vector3(0.8f, 1.8f, 0.8f))
            };
            return data;
        }

        private static BreakableObjectData CreateBreakableData(string name, float health, float xpDrop, Color color, Vector3 minScale, Vector3 maxScale)
        {
            BreakableObjectData data = ScriptableObject.CreateInstance<BreakableObjectData>();
            data.ObjectName = name;
            data.MaxHealth = health;
            data.XpDrop = xpDrop;
            data.BaseColor = color;
            data.MinScale = minScale;
            data.MaxScale = maxScale;
            return data;
        }

        private static void CreateSkills(GameDatabase database)
        {
            database.Skills.Add(CreateSkill("Arcane Bolt", "Fires seeking arcane projectiles at the nearest enemy.", SkillEffectKind.ArcaneBolt, SkillType.Active, new[] { SkillTag.Arcane }, 16f, 0.75f, 18f, 2.2f, 0f, 1, 19f, true, SkillTargetingMode.NearestEnemy, new Color(0.7f, 0.25f, 1f)));
            database.Skills.Add(CreateSkill("Flame Orbit", "Creates fire orbs that orbit the caster and burn nearby enemies.", SkillEffectKind.FlameOrbit, SkillType.Passive, new[] { SkillTag.Fire }, 7f, 0f, 0f, 2.8f, 0f, 2, 0f, true, SkillTargetingMode.AroundPlayer, new Color(1f, 0.32f, 0.08f)));
            database.Skills.Add(CreateSkill("Ice Nova", "Releases a freezing burst around the player and slows enemies.", SkillEffectKind.IceNova, SkillType.Active, new[] { SkillTag.Ice }, 20f, 4.6f, 0f, 4.2f, 2.5f, 1, 0f, true, SkillTargetingMode.AroundPlayer, new Color(0.45f, 0.85f, 1f)));
            database.Skills.Add(CreateSkill("Lightning Chain", "Strikes an enemy and jumps to nearby targets.", SkillEffectKind.LightningChain, SkillType.Active, new[] { SkillTag.Lightning }, 22f, 3.1f, 17f, 0f, 0f, 1, 0f, true, SkillTargetingMode.Chain, new Color(1f, 0.9f, 0.18f)));
            database.Skills.Add(CreateSkill("Void Zone", "Opens a damaging void field under a nearby enemy.", SkillEffectKind.VoidZone, SkillType.Active, new[] { SkillTag.Void }, 9f, 5.2f, 16f, 3.2f, 4f, 1, 0f, true, SkillTargetingMode.GroundAtEnemy, new Color(0.11f, 0.02f, 0.18f)));
            database.Skills.Add(CreateSkill("Nature Spikes", "Summons piercing spikes below nearby enemies.", SkillEffectKind.NatureSpikes, SkillType.Active, new[] { SkillTag.Nature }, 32f, 3.8f, 15f, 1f, 0f, 3, 0f, true, SkillTargetingMode.NearbyEnemies, new Color(0.25f, 0.78f, 0.35f)));
        }

        private static SkillData CreateSkill(string name, string description, SkillEffectKind kind, SkillType type, SkillTag[] tags, float damage, float cooldown, float range, float area, float duration, int projectileCount, float projectileSpeed, bool canCrit, SkillTargetingMode targeting, Color color)
        {
            SkillData data = ScriptableObject.CreateInstance<SkillData>();
            data.SkillName = name;
            data.Description = description;
            data.EffectKind = kind;
            data.SkillType = type;
            data.ElementTags = tags;
            data.BaseDamage = damage;
            data.Cooldown = cooldown;
            data.Range = range;
            data.Area = area;
            data.Duration = duration;
            data.ProjectileCount = projectileCount;
            data.ProjectileSpeed = projectileSpeed;
            data.ChainCount = kind == SkillEffectKind.LightningChain ? 3 : 0;
            data.ChainRadius = kind == SkillEffectKind.LightningChain ? 8f : 0f;
            data.CanCrit = canCrit;
            data.TargetingMode = targeting;
            data.VisualColor = color;
            data.UpgradePool = new[] { "Damage", "Cooldown", "Area", "Projectiles" };
            data.SynergyRules = new[] { "Elemental tag matching" };
            return data;
        }

        private static void CreateEnemies(GameDatabase database)
        {
            database.Enemies.Add(CreateEnemy("Wisp", 18f, 3.3f, 5f, 1f, EnemyAttackType.Contact, EnemyMovementStyle.Direct, 48f, new Color(0.55f, 0.85f, 1f)));
            database.Enemies.Add(CreateEnemy("Golem Shard", 64f, 1.65f, 12f, 4f, EnemyAttackType.Contact, EnemyMovementStyle.Heavy, 12f, new Color(0.55f, 0.50f, 0.42f)));
            database.Enemies.Add(CreateEnemy("Hex Bat", 14f, 4.25f, 6f, 2f, EnemyAttackType.Contact, EnemyMovementStyle.Flanking, 28f, new Color(0.62f, 0.25f, 0.82f)));
            database.Enemies.Add(CreateEnemy("Cultist", 34f, 2.2f, 9f, 3f, EnemyAttackType.Ranged, EnemyMovementStyle.Direct, 16f, new Color(0.75f, 0.18f, 0.25f)));
        }

        private static EnemyData CreateEnemy(string name, float hp, float speed, float damage, float xp, EnemyAttackType attack, EnemyMovementStyle movement, float weight, Color color)
        {
            EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
            data.EnemyName = name;
            data.MaxHP = hp;
            data.MoveSpeed = speed;
            data.Damage = damage;
            data.XPDrop = xp;
            data.AttackType = attack;
            data.MovementStyle = movement;
            data.Weight = weight;
            data.BodyColor = color;
            data.SpawnRules = new[] { "Spawn outside the camera ring" };
            return data;
        }

        private static void CreateBosses(GameDatabase database)
        {
            BossData guardian = ScriptableObject.CreateInstance<BossData>();
            guardian.BossName = "Rune Guardian";
            guardian.Kind = BossKind.RuneGuardian;
            guardian.MaxHP = 900f;
            guardian.Damage = 22f;
            guardian.MoveSpeed = 2.3f;
            guardian.XPReward = 45f;
            guardian.UpgradeRewardChance = 0.35f;
            guardian.AttackPatterns = new[] { "Charge", "Close area slam" };
            guardian.ScalingRules = new[] { "Health and damage increase after each boss defeat" };
            guardian.BodyColor = new Color(0.28f, 0.62f, 0.55f);
            database.Bosses.Add(guardian);

            BossData witch = ScriptableObject.CreateInstance<BossData>();
            witch.BossName = "Astral Witch";
            witch.Kind = BossKind.AstralWitch;
            witch.MaxHP = 720f;
            witch.Damage = 18f;
            witch.MoveSpeed = 3.1f;
            witch.XPReward = 45f;
            witch.UpgradeRewardChance = 0.35f;
            witch.AttackPatterns = new[] { "Magic projectile", "Void hazard", "Reposition" };
            witch.ScalingRules = new[] { "Health and damage increase after each boss defeat" };
            witch.BodyColor = new Color(0.58f, 0.25f, 0.86f);
            database.Bosses.Add(witch);
        }

        private static void CreatePickups(GameDatabase database)
        {
            PickupData magnet = ScriptableObject.CreateInstance<PickupData>();
            magnet.PickupName = "Magnet";
            magnet.Type = PickupType.Magnet;
            magnet.VisualColor = new Color(0.2f, 0.85f, 1f);
            magnet.Value = 1f;
            database.Pickups.Add(magnet);

            PickupData heal = ScriptableObject.CreateInstance<PickupData>();
            heal.PickupName = "Heal";
            heal.Type = PickupType.Heal;
            heal.VisualColor = new Color(0.25f, 1f, 0.42f);
            heal.Value = 32f;
            database.Pickups.Add(heal);
        }

        private static void CreateUpgrades(GameDatabase database)
        {
            AddUpgrade(database, "Sharpened Spellwork", "Spell Damage +12%.", UpgradeRarity.Common, UpgradeEffect.IncreaseDamage, 0.12f, "", "All Skills");
            AddUpgrade(database, "Quickened Casting", "Skill Cooldowns -6%.", UpgradeRarity.Uncommon, UpgradeEffect.ReduceCooldown, 0.06f, "", "All Skills");
            AddUpgrade(database, "Split Focus", "Projectiles +1.", UpgradeRarity.Magic, UpgradeEffect.IncreaseProjectileCount, 1f, "", "Projectile Skills");
            AddUpgrade(database, "Widened Sigils", "Increase spell area.", UpgradeRarity.Common, UpgradeEffect.IncreaseArea, 0.15f, "", "Area Skills");
            AddUpgrade(database, "Lingering Power", "Spell Duration +18%.", UpgradeRarity.Uncommon, UpgradeEffect.IncreaseDuration, 0.18f, "", "Duration Skills");
            AddUpgrade(database, "Fleet Steps", "Increase movement speed.", UpgradeRarity.Common, UpgradeEffect.IncreaseMoveSpeed, 0.55f, "", "Player");
            AddUpgrade(database, "Magnetized Runes", "Increase XP pickup radius.", UpgradeRarity.Common, UpgradeEffect.IncreasePickupRadius, 0.75f, "", "Player");
            AddUpgrade(database, "Vital Ward", "Max HP +22 and heal to full.", UpgradeRarity.Magic, UpgradeEffect.IncreaseMaxHP, 22f, "", "Player");
            AddUpgrade(database, "Keen Arcana", "Critical Chance +4%.", UpgradeRarity.Magic, UpgradeEffect.IncreaseCriticalChance, 0.04f, "", "Player");
            AddUpgrade(database, "Ruinous Crits", "Increase critical damage.", UpgradeRarity.Epic, UpgradeEffect.IncreaseCriticalDamage, 0.28f, "", "Player");
            AddUpgrade(database, "Lucky Charm", "Luck +1.", UpgradeRarity.Uncommon, UpgradeEffect.IncreaseLuck, 1f, "", "Player");
            AddUpgrade(database, "Better Omens", "Luck +2.", UpgradeRarity.Magic, UpgradeEffect.IncreaseLuck, 2f, "", "Player");
            AddUpgrade(database, "Arcane Aegis", "Armor +2.0.", UpgradeRarity.Uncommon, UpgradeEffect.IncreaseArmor, 2f, "", "Player");
            AddUpgrade(database, "Runic Plating", "Armor +4.0.", UpgradeRarity.Magic, UpgradeEffect.IncreaseArmor, 4f, "", "Player");
            AddUpgrade(database, "Restorative Ward", "Health Regeneration +1.2.", UpgradeRarity.Uncommon, UpgradeEffect.IncreaseRegeneration, 1.2f, "", "Player");
            AddUpgrade(database, "Living Barrier", "Health Regeneration +2.0.", UpgradeRarity.Epic, UpgradeEffect.IncreaseRegeneration, 2f, "", "Player");
            AddUpgrade(database, "Learn Arcane Bolt", "Unlock Arcane Bolt.", UpgradeRarity.Magic, UpgradeEffect.UnlockNewSkill, 1f, "Arcane Bolt", "Arcane Bolt");
            AddUpgrade(database, "Learn Flame Orbit", "Unlock Flame Orbit.", UpgradeRarity.Magic, UpgradeEffect.UnlockNewSkill, 1f, "Flame Orbit", "Flame Orbit");
            AddUpgrade(database, "Learn Ice Nova", "Unlock Ice Nova.", UpgradeRarity.Magic, UpgradeEffect.UnlockNewSkill, 1f, "Ice Nova", "Ice Nova");
            AddUpgrade(database, "Learn Lightning Chain", "Unlock Lightning Chain.", UpgradeRarity.Magic, UpgradeEffect.UnlockNewSkill, 1f, "Lightning Chain", "Lightning Chain");
            AddUpgrade(database, "Learn Void Zone", "Unlock Void Zone.", UpgradeRarity.Epic, UpgradeEffect.UnlockNewSkill, 1f, "Void Zone", "Void Zone");
            AddUpgrade(database, "Learn Nature Spikes", "Unlock Nature Spikes.", UpgradeRarity.Epic, UpgradeEffect.UnlockNewSkill, 1f, "Nature Spikes", "Nature Spikes");
            AddUpgrade(database, "Arcane Bolt Mastery", "Upgrade Arcane Bolt.", UpgradeRarity.Common, UpgradeEffect.UpgradeExistingSkill, 1f, "Arcane Bolt", "Arcane Bolt");
            AddUpgrade(database, "Flame Orbit Mastery", "Upgrade Flame Orbit.", UpgradeRarity.Common, UpgradeEffect.UpgradeExistingSkill, 1f, "Flame Orbit", "Flame Orbit");
            AddUpgrade(database, "Ice Nova Mastery", "Upgrade Ice Nova.", UpgradeRarity.Common, UpgradeEffect.UpgradeExistingSkill, 1f, "Ice Nova", "Ice Nova");
            AddUpgrade(database, "Lightning Chain Mastery", "Upgrade Lightning Chain.", UpgradeRarity.Common, UpgradeEffect.UpgradeExistingSkill, 1f, "Lightning Chain", "Lightning Chain");
            AddUpgrade(database, "Branching Lightning", "Lightning Chain Count +1.", UpgradeRarity.Magic, UpgradeEffect.IncreaseChainCount, 1f, "Lightning Chain", "Lightning Chain");
            AddUpgrade(database, "Conductive Air", "Lightning Chain Radius +2.0.", UpgradeRarity.Uncommon, UpgradeEffect.IncreaseChainRadius, 2f, "Lightning Chain", "Lightning Chain");
            AddUpgrade(database, "Void Zone Mastery", "Upgrade Void Zone.", UpgradeRarity.Common, UpgradeEffect.UpgradeExistingSkill, 1f, "Void Zone", "Void Zone");
            AddUpgrade(database, "Nature Spikes Mastery", "Upgrade Nature Spikes.", UpgradeRarity.Common, UpgradeEffect.UpgradeExistingSkill, 1f, "Nature Spikes", "Nature Spikes");
            AddUpgrade(database, "Elemental Convergence", "Strengthen elemental synergies.", UpgradeRarity.Legendary, UpgradeEffect.ActivateOrStrengthenSynergy, 0.18f, "Lightning Chain", "Synergy");
        }

        private static void AddUpgrade(GameDatabase database, string name, string description, UpgradeRarity rarity, UpgradeEffect effect, float amount, string targetSkill, string affectedSkill)
        {
            UpgradeData data = ScriptableObject.CreateInstance<UpgradeData>();
            data.UpgradeName = name;
            data.Description = description;
            data.Rarity = rarity;
            data.Effect = effect;
            data.Amount = amount;
            data.TargetSkillName = targetSkill;
            data.AffectedSkillLabel = affectedSkill;
            database.Upgrades.Add(data);
        }

        private static void CreateSynergies(GameDatabase database)
        {
            AddSynergy(database, "Explosive Arcanum", "Arcane projectiles explode when they hit enemies.", SynergyEffect.ExplosiveArcanum, new[] { SkillTag.Fire, SkillTag.Arcane }, new Color(0.95f, 0.28f, 1f), 5);
            AddSynergy(database, "Stormfrost", "Lightning deals extra damage to slowed enemies.", SynergyEffect.Stormfrost, new[] { SkillTag.Ice, SkillTag.Lightning }, new Color(0.48f, 0.85f, 1f), 4);
            AddSynergy(database, "Burning Abyss", "Flame damage can leave short-lived void burns.", SynergyEffect.BurningAbyss, new[] { SkillTag.Void, SkillTag.Fire }, new Color(0.5f, 0.12f, 0.1f), 3);
            AddSynergy(database, "Living Runes", "Nature spikes gain an extra duplicated strike.", SynergyEffect.LivingRunes, new[] { SkillTag.Nature, SkillTag.Arcane }, new Color(0.4f, 1f, 0.46f), 3);
            AddSynergy(database, "Gravity Storm", "Lightning applies a light pull during chains.", SynergyEffect.GravityStorm, new[] { SkillTag.Lightning, SkillTag.Void }, new Color(0.65f, 0.45f, 1f), 4);
        }

        private static void AddSynergy(GameDatabase database, string name, string description, SynergyEffect effect, SkillTag[] tags, Color color, int priority)
        {
            SynergyData data = ScriptableObject.CreateInstance<SynergyData>();
            data.SynergyName = name;
            data.Description = description;
            data.EffectType = effect;
            data.RequiredTags = tags;
            data.RequiredSkillLevels = new string[0];
            data.VisualFeedback = color;
            data.Priority = priority;
            data.AllowStacking = false;
            database.Synergies.Add(data);
        }

        private static int LayerOrDefault(string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            return layer < 0 ? 0 : layer;
        }

        private static void SetColor(GameObject gameObject, Color color)
        {
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateMaterial(color);
            }
        }

        private static void RemoveCollider(GameObject gameObject)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }
        }

        private static void AddDropBeacon(GameObject root, Color color, float height, float width, float lightRange, float lightIntensity)
        {
            GameObject beacon = new GameObject("Drop Beacon", typeof(LineRenderer));
            beacon.transform.SetParent(root.transform, false);
            LineRenderer lineRenderer = beacon.GetComponent<LineRenderer>();
            Shader shader = Shader.Find("Sprites/Default");
            lineRenderer.material = new Material(shader != null ? shader : Shader.Find("Standard"));
            lineRenderer.useWorldSpace = false;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.up * height);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width * 3f;
            lineRenderer.startColor = new Color(color.r, color.g, color.b, 0.85f);
            lineRenderer.endColor = new Color(color.r, color.g, color.b, 0f);

            GameObject lightObject = new GameObject("Drop Beacon Light", typeof(Light));
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.localPosition = Vector3.up * 1.1f;
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = lightRange;
            light.intensity = lightIntensity;
        }

        private static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}
