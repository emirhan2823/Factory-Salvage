using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using FactorySalvage.Core;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;
using FactorySalvage.Audio;

namespace FactorySalvage.Editor
{
    /// <summary>
    /// One-click game setup wizard. Creates all SO assets, prefabs, and scene hierarchy.
    /// Menu: FactorySalvage → Setup Game
    /// </summary>
    public static class GameSetupWizard
    {
        #region Menu Items

        [MenuItem("FactorySalvage/1. Create All SO Assets")]
        public static void CreateAllAssets()
        {
            CreateResourceAssets();
            CreateMachineAssets();
            CreateEnemyAssets();
            CreateWaveAssets();
            CreateEventAssets();
            CreateVariableAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Setup] All SO assets created!");
        }

        [MenuItem("FactorySalvage/2. Create All Prefabs")]
        public static void CreateAllPrefabs()
        {
            CreatePlayerPrefab();
            CreateResourceNodePrefab();
            CreateMachinePrefabs();
            CreateEnemyPrefabs();
            CreateProjectilePrefab();
            CreateWallPrefab();
            CreateTurretPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Setup] All prefabs created!");
        }

        [MenuItem("FactorySalvage/3. Setup Game Scene")]
        public static void SetupGameScene()
        {
            SetupGrid();
            SetupPlayer();
            SetupCamera();
            SetupManagers();
            SetupUI();
            Debug.Log("[Setup] Game scene setup complete! Press Play to test.");
        }

        [MenuItem("FactorySalvage/Run Full Setup (1+2+3)")]
        public static void RunFullSetup()
        {
            CreateAllAssets();
            CreateAllPrefabs();
            SetupGameScene();
            Debug.Log("[Setup] === FULL SETUP COMPLETE ===");
        }

        #endregion

        #region Resource Assets

        private static void CreateResourceAssets()
        {
            CreateResource("ScrapMetal", "scrap_metal", new Color(0.6f, 0.6f, 0.6f));
            CreateResource("Wire", "wire", new Color(0.8f, 0.5f, 0.2f));
            CreateResource("Circuit", "circuit", new Color(0.2f, 0.8f, 0.3f));
            CreateResource("Gear", "gear", new Color(0.4f, 0.4f, 0.8f));
        }

        private static void CreateResource(string name, string id, Color color)
        {
            var path = $"Assets/_Data/Resources/{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<ResourceDefinition>(path) != null) return;

            var resource = ScriptableObject.CreateInstance<ResourceDefinition>();
            SetPrivateField(resource, "_resourceName", name);
            SetPrivateField(resource, "_resourceId", id);
            SetPrivateField(resource, "_color", color);
            SetPrivateField(resource, "_maxStack", 999);

            EnsureDirectory("Assets/_Data/Resources");
            AssetDatabase.CreateAsset(resource, path);
            Debug.Log($"[Setup] Created resource: {name}");
        }

        #endregion

        #region Machine Assets

        private static void CreateMachineAssets()
        {
            var scrap = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/ScrapMetal.asset");
            var wire = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/Wire.asset");

            // Smelter
            CreateMachineDefinition("Smelter", "smelter", new Color(1f, 0.4f, 0.2f), 3f, 2f,
                new ResourceCost[] { new() { Resource = scrap, Amount = 5 } });

            // WireMill
            CreateMachineDefinition("Wire Mill", "wire_mill", new Color(0.8f, 0.5f, 0.2f), 3f, 2f,
                new ResourceCost[] { new() { Resource = scrap, Amount = 3 }, new() { Resource = wire, Amount = 2 } });

            // Assembler
            CreateMachineDefinition("Assembler", "assembler", new Color(0.3f, 0.6f, 0.9f), 5f, 3f,
                new ResourceCost[] { new() { Resource = scrap, Amount = 8 } });

            // Generator
            CreateMachineDefinition("Generator", "generator", new Color(1f, 0.9f, 0.2f), 0f, 0f,
                new ResourceCost[] { new() { Resource = scrap, Amount = 10 } });

            // Recipes
            CreateRecipes();

            // Generator Definition
            CreateGeneratorDefinition();
        }

        private static void CreateMachineDefinition(string name, string id, Color color, float processTime, float energyCost, ResourceCost[] cost)
        {
            var path = $"Assets/_Data/Machines/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<MachineDefinition>(path) != null) return;

            var machine = ScriptableObject.CreateInstance<MachineDefinition>();
            SetPrivateField(machine, "_machineName", name);
            SetPrivateField(machine, "_machineId", id);
            SetPrivateField(machine, "_size", Vector2Int.one);
            SetPrivateField(machine, "_processTime", processTime);
            SetPrivateField(machine, "_energyConsumption", energyCost);
            SetPrivateField(machine, "_buildCost", cost);

            EnsureDirectory("Assets/_Data/Machines");
            AssetDatabase.CreateAsset(machine, path);
        }

        private static void CreateRecipes()
        {
            var scrap = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/ScrapMetal.asset");
            var wire = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/Wire.asset");
            var circuit = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/Circuit.asset");
            var gear = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/Gear.asset");

            // SmeltScrap: 2 Scrap -> 1 Gear
            CreateRecipe("SmeltScrap", "Smelt Scrap", 3f,
                new ResourceCost[] { new() { Resource = scrap, Amount = 2 } },
                new ResourceOutput[] { new() { Resource = gear, Amount = 1 } });

            // MillWire: 2 Wire -> 1 Circuit
            CreateRecipe("MillWire", "Mill Wire", 3f,
                new ResourceCost[] { new() { Resource = wire, Amount = 2 } },
                new ResourceOutput[] { new() { Resource = circuit, Amount = 1 } });

            // AssembleParts: 1 Gear + 1 Circuit -> 3 Scrap (recycling)
            CreateRecipe("AssembleParts", "Assemble Parts", 5f,
                new ResourceCost[] { new() { Resource = gear, Amount = 1 }, new() { Resource = circuit, Amount = 1 } },
                new ResourceOutput[] { new() { Resource = scrap, Amount = 5 } });
        }

        private static void CreateRecipe(string fileName, string name, float processTime, ResourceCost[] inputs, ResourceOutput[] outputs)
        {
            var path = $"Assets/_Data/Machines/{fileName}.asset";
            if (AssetDatabase.LoadAssetAtPath<RecipeDefinition>(path) != null) return;

            var recipe = ScriptableObject.CreateInstance<RecipeDefinition>();
            SetPrivateField(recipe, "_recipeName", name);
            SetPrivateField(recipe, "_processTime", processTime);
            SetPrivateField(recipe, "_inputs", inputs);
            SetPrivateField(recipe, "_outputs", outputs);

            AssetDatabase.CreateAsset(recipe, path);
        }

        private static void CreateGeneratorDefinition()
        {
            var path = "Assets/_Data/Machines/SmallGenerator.asset";
            if (AssetDatabase.LoadAssetAtPath<GeneratorDefinition>(path) != null) return;

            var genDef = ScriptableObject.CreateInstance<GeneratorDefinition>();
            SetPrivateField(genDef, "_energyOutput", 10f);

            AssetDatabase.CreateAsset(genDef, path);
        }

        #endregion

        #region Enemy Assets

        private static void CreateEnemyAssets()
        {
            var scrap = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/ScrapMetal.asset");

            // Scrapper
            CreateEnemyDefinition("Scrapper", "scrapper", 15f, 2f, 5f, 1f,
                new LootEntry[] { new() { Resource = scrap, Amount = 2, DropChance = 0.8f } });

            // Sparker
            CreateEnemyDefinition("Sparker", "sparker", 8f, 3.5f, 3f, 1.5f,
                new LootEntry[] { new() { Resource = scrap, Amount = 1, DropChance = 0.5f } });

            EnsureDirectory("Assets/_Data/Enemies");
        }

        private static void CreateEnemyDefinition(string name, string id, float health, float speed, float damage, float attackRate, LootEntry[] loot)
        {
            var path = $"Assets/_Data/Enemies/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<EnemyDefinition>(path) != null) return;

            var enemy = ScriptableObject.CreateInstance<EnemyDefinition>();
            SetPrivateField(enemy, "_enemyName", name);
            SetPrivateField(enemy, "_health", health);
            SetPrivateField(enemy, "_moveSpeed", speed);
            SetPrivateField(enemy, "_attackDamage", damage);
            SetPrivateField(enemy, "_attackRate", attackRate);
            SetPrivateField(enemy, "_lootTable", loot);

            EnsureDirectory("Assets/_Data/Enemies");
            AssetDatabase.CreateAsset(enemy, path);
        }

        #endregion

        #region Wave Assets

        private static void CreateWaveAssets()
        {
            var scrapper = AssetDatabase.LoadAssetAtPath<EnemyDefinition>("Assets/_Data/Enemies/scrapper.asset");
            var sparker = AssetDatabase.LoadAssetAtPath<EnemyDefinition>("Assets/_Data/Enemies/sparker.asset");

            EnsureDirectory("Assets/_Data/Waves");

            // Wave 1: 5 Scrappers
            CreateWave("Wave1", 1,
                new EnemyGroup[] { new() { Enemy = scrapper, Count = 5, SpawnDelay = 1f } });

            // Wave 2: 8 Scrappers + 3 Sparkers
            CreateWave("Wave2", 2,
                new EnemyGroup[] {
                    new() { Enemy = scrapper, Count = 8, SpawnDelay = 0.8f },
                    new() { Enemy = sparker, Count = 3, SpawnDelay = 0.5f }
                });

            // Wave 3: 12 Scrappers + 6 Sparkers
            CreateWave("Wave3", 3,
                new EnemyGroup[] {
                    new() { Enemy = scrapper, Count = 12, SpawnDelay = 0.6f },
                    new() { Enemy = sparker, Count = 6, SpawnDelay = 0.4f }
                });
        }

        private static void CreateWave(string fileName, int waveNum, EnemyGroup[] groups)
        {
            var path = $"Assets/_Data/Waves/{fileName}.asset";
            if (AssetDatabase.LoadAssetAtPath<WaveDefinition>(path) != null) return;

            var wave = ScriptableObject.CreateInstance<WaveDefinition>();
            SetPrivateField(wave, "_waveNumber", waveNum);
            SetPrivateField(wave, "_enemyGroups", groups);
            SetPrivateField(wave, "_timeBetweenGroups", 3f);

            AssetDatabase.CreateAsset(wave, path);
        }

        #endregion

        #region Event & Variable Assets

        private static void CreateEventAssets()
        {
            EnsureDirectory("Assets/_Data/Events");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnGameStart.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnGamePause.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnWaveStart.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnWaveEnd.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnInventoryChanged.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnMachinePlaced.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnBuildModeChanged.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnEnergyDepleted.asset");
            CreateSOAsset<GameEvent>("Assets/_Data/Events/OnUpgradePurchased.asset");
            CreateSOAsset<IntGameEvent>("Assets/_Data/Events/OnWaveNumberChanged.asset");
            CreateSOAsset<IntGameEvent>("Assets/_Data/Events/OnResourceChanged.asset");
        }

        private static void CreateVariableAssets()
        {
            EnsureDirectory("Assets/_Data/Variables");
            CreateSOAsset<FloatVariable>("Assets/_Data/Variables/CurrentEnergy.asset");
            CreateSOAsset<FloatVariable>("Assets/_Data/Variables/MaxEnergy.asset");
            CreateSOAsset<IntVariable>("Assets/_Data/Variables/CurrentWave.asset");
            CreateSOAsset<IntVariable>("Assets/_Data/Variables/EnemiesRemaining.asset");
            CreateSOAsset<TransformRuntimeSet>("Assets/_Data/Variables/EnemySet.asset");
        }

        #endregion

        #region Prefab Creation

        private static void CreatePlayerPrefab()
        {
            var path = "Assets/_Prefabs/Player/Player.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("Player");

            // Sprite
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("PlayerSprite", Color.cyan, 32);
            sr.sortingOrder = 10;

            // Physics
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 0.8f);

            // Components
            go.AddComponent<PlayerController>();
            go.AddComponent<PlayerInteraction>();
            go.AddComponent<Inventory>();

            EnsureDirectory("Assets/_Prefabs/Player");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log("[Setup] Created Player prefab");
        }

        private static void CreateResourceNodePrefab()
        {
            var path = "Assets/_Prefabs/Resources/ResourceNode.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("ResourceNode");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("ResourceSprite", Color.yellow, 24);
            sr.sortingOrder = 5;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;

            var node = go.AddComponent<ResourceNode>();
            SetPrivateField(node, "_spriteRenderer", sr);

            go.AddComponent<PooledObject>();

            EnsureDirectory("Assets/_Prefabs/Resources");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log("[Setup] Created ResourceNode prefab");
        }

        private static void CreateMachinePrefabs()
        {
            CreateMachinePrefab("Smelter", new Color(1f, 0.4f, 0.2f), true);
            CreateMachinePrefab("WireMill", new Color(0.8f, 0.5f, 0.2f), true);
            CreateMachinePrefab("Assembler", new Color(0.3f, 0.6f, 0.9f), true);
            CreateMachinePrefab("Generator", new Color(1f, 0.9f, 0.2f), false);
        }

        private static void CreateMachinePrefab(string name, Color color, bool isProcessor)
        {
            var path = $"Assets/_Prefabs/Machines/{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite($"{name}Sprite", color, 30);
            sr.sortingOrder = 8;

            go.AddComponent<BoxCollider2D>();

            if (isProcessor)
            {
                go.AddComponent<ProcessingMachine>();
                go.AddComponent<EnergyConsumer>();
            }
            else
            {
                go.AddComponent<EnergyProducer>();
            }

            EnsureDirectory("Assets/_Prefabs/Machines");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateEnemyPrefabs()
        {
            CreateEnemyPrefab("Scrapper", new Color(0.8f, 0.2f, 0.2f));
            CreateEnemyPrefab("Sparker", new Color(0.9f, 0.5f, 0.1f));
        }

        private static void CreateEnemyPrefab(string name, Color color)
        {
            var path = $"Assets/_Prefabs/Enemies/{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite($"{name}Sprite", color, 28);
            sr.sortingOrder = 9;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            col.isTrigger = true;

            go.AddComponent<Health>();
            var enemy = go.AddComponent<EnemyController>();
            SetPrivateField(enemy, "_spriteRenderer", sr);

            go.AddComponent<PooledObject>();

            EnsureDirectory("Assets/_Prefabs/Enemies");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateProjectilePrefab()
        {
            var path = "Assets/_Prefabs/Projectiles/Bullet.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("Bullet");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("BulletSprite", Color.white, 8);
            sr.sortingOrder = 11;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.15f;
            col.isTrigger = true;

            go.AddComponent<Projectile>();
            go.AddComponent<PooledObject>();

            EnsureDirectory("Assets/_Prefabs/Projectiles");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateWallPrefab()
        {
            var path = "Assets/_Prefabs/Machines/Wall.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("Wall");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("WallSprite", new Color(0.5f, 0.5f, 0.5f), 32);
            sr.sortingOrder = 7;

            go.AddComponent<BoxCollider2D>();
            go.AddComponent<Health>();
            go.AddComponent<WallBlock>();

            EnsureDirectory("Assets/_Prefabs/Machines");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateTurretPrefab()
        {
            var path = "Assets/_Prefabs/Machines/BasicTurret.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("BasicTurret");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlaceholderSprite("TurretSprite", new Color(0.2f, 0.7f, 0.2f), 30);
            sr.sortingOrder = 8;

            go.AddComponent<BoxCollider2D>();
            go.AddComponent<TurretController>();

            // Fire point
            var firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(go.transform);
            firePoint.transform.localPosition = new Vector3(0f, 0.5f, 0f);

            EnsureDirectory("Assets/_Prefabs/Machines");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        #endregion

        #region Scene Setup

        private static void SetupGrid()
        {
            // Check if grid already exists
            if (Object.FindAnyObjectByType<Grid>() != null)
            {
                Debug.Log("[Setup] Grid already exists, skipping");
                return;
            }

            // Create Grid
            var gridGo = new GameObject("Grid");
            var grid = gridGo.AddComponent<Grid>();
            grid.cellSize = new Vector3(1f, 1f, 1f);

            // Ground Tilemap
            var groundGo = new GameObject("Ground");
            groundGo.transform.SetParent(gridGo.transform);
            var groundTilemap = groundGo.AddComponent<Tilemap>();
            var groundRenderer = groundGo.AddComponent<TilemapRenderer>();
            groundRenderer.sortingOrder = 0;

            // Buildings Tilemap
            var buildingsGo = new GameObject("Buildings");
            buildingsGo.transform.SetParent(gridGo.transform);
            buildingsGo.AddComponent<Tilemap>();
            var buildingsRenderer = buildingsGo.AddComponent<TilemapRenderer>();
            buildingsRenderer.sortingOrder = 1;

            // Paint ground tiles (20x20)
            var groundTile = CreatePlaceholderTile("GroundTile", new Color(0.3f, 0.25f, 0.2f));
            for (int x = -10; x < 10; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }

            // Add GridManager
            var gridManager = gridGo.AddComponent<GridManager>();
            SetPrivateField(gridManager, "_grid", grid);
            SetPrivateField(gridManager, "_groundTilemap", groundTilemap);
            SetPrivateField(gridManager, "_buildingTilemap", buildingsGo.GetComponent<Tilemap>());

            Debug.Log("[Setup] Grid created with 20x20 ground tiles");
        }

        private static void SetupPlayer()
        {
            if (Object.FindAnyObjectByType<PlayerController>() != null) return;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Prefabs/Player/Player.prefab");
            if (prefab == null)
            {
                Debug.LogWarning("[Setup] Player prefab not found. Run 'Create All Prefabs' first.");
                return;
            }

            var player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            player.transform.position = Vector3.zero;

            // Wire GridManager reference
            var gridManager = Object.FindAnyObjectByType<GridManager>();
            if (gridManager != null)
            {
                var pc = player.GetComponent<PlayerController>();
                SetPrivateField(pc, "_gridManager", gridManager);
            }

            // Wire inventory event
            var invChangedEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets/_Data/Events/OnInventoryChanged.asset");
            if (invChangedEvent != null)
            {
                var inv = player.GetComponent<Inventory>();
                SetPrivateField(inv, "_onInventoryChanged", invChangedEvent);
            }

            Debug.Log("[Setup] Player placed at origin");
        }

        private static void SetupCamera()
        {
            var cam = Camera.main;
            if (cam == null) return;

            var controller = cam.GetComponent<CameraController>();
            if (controller == null)
            {
                controller = cam.gameObject.AddComponent<CameraController>();
            }

            // Wire player as target
            var player = Object.FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                SetPrivateField(controller, "_target", player.transform);
            }

            cam.orthographicSize = 7f;
            Debug.Log("[Setup] Camera configured");
        }

        private static void SetupManagers()
        {
            // --- Managers parent ---
            var managersGo = GameObject.Find("Managers") ?? new GameObject("Managers");

            // BuildSystem
            if (Object.FindAnyObjectByType<BuildSystem>() == null)
            {
                var buildGo = new GameObject("BuildSystem");
                buildGo.transform.SetParent(managersGo.transform);
                var buildSystem = buildGo.AddComponent<BuildSystem>();

                // Wire references
                var gridManager = Object.FindAnyObjectByType<GridManager>();
                var player = Object.FindAnyObjectByType<PlayerController>();
                SetPrivateField(buildSystem, "_gridManager", gridManager);
                if (player != null)
                    SetPrivateField(buildSystem, "_playerInventory", player.GetComponent<Inventory>());

                // Load machine definitions
                var smelter = AssetDatabase.LoadAssetAtPath<MachineDefinition>("Assets/_Data/Machines/smelter.asset");
                var wireMill = AssetDatabase.LoadAssetAtPath<MachineDefinition>("Assets/_Data/Machines/wire_mill.asset");
                var assembler = AssetDatabase.LoadAssetAtPath<MachineDefinition>("Assets/_Data/Machines/assembler.asset");
                var generator = AssetDatabase.LoadAssetAtPath<MachineDefinition>("Assets/_Data/Machines/generator.asset");
                SetPrivateField(buildSystem, "_availableMachines", new MachineDefinition[] { smelter, wireMill, assembler, generator });

                // Ghost
                var ghostGo = new GameObject("MachineGhost");
                ghostGo.transform.SetParent(buildGo.transform);
                var ghostSr = ghostGo.AddComponent<SpriteRenderer>();
                ghostSr.sortingOrder = 20;
                var ghost = ghostGo.AddComponent<MachineGhost>();
                SetPrivateField(ghost, "_spriteRenderer", ghostSr);
                ghostGo.SetActive(false);
                SetPrivateField(buildSystem, "_ghost", ghost);
            }

            // EnergyManager
            if (Object.FindAnyObjectByType<EnergyManager>() == null)
            {
                var energyGo = new GameObject("EnergyManager");
                energyGo.transform.SetParent(managersGo.transform);
                var em = energyGo.AddComponent<EnergyManager>();

                var currentEnergy = AssetDatabase.LoadAssetAtPath<FloatVariable>("Assets/_Data/Variables/CurrentEnergy.asset");
                var maxEnergy = AssetDatabase.LoadAssetAtPath<FloatVariable>("Assets/_Data/Variables/MaxEnergy.asset");
                SetPrivateField(em, "_currentEnergy", currentEnergy);
                SetPrivateField(em, "_maxEnergy", maxEnergy);
            }

            // ConnectionManager
            if (Object.FindAnyObjectByType<ConnectionManager>() == null)
            {
                var connGo = new GameObject("ConnectionManager");
                connGo.transform.SetParent(managersGo.transform);
                connGo.AddComponent<ConnectionManager>();
            }

            // WaveManager
            if (Object.FindAnyObjectByType<WaveManager>() == null)
            {
                var waveGo = new GameObject("WaveManager");
                waveGo.transform.SetParent(managersGo.transform);
                var wm = waveGo.AddComponent<WaveManager>();

                // Load waves
                var w1 = AssetDatabase.LoadAssetAtPath<WaveDefinition>("Assets/_Data/Waves/Wave1.asset");
                var w2 = AssetDatabase.LoadAssetAtPath<WaveDefinition>("Assets/_Data/Waves/Wave2.asset");
                var w3 = AssetDatabase.LoadAssetAtPath<WaveDefinition>("Assets/_Data/Waves/Wave3.asset");
                SetPrivateField(wm, "_waves", new WaveDefinition[] { w1, w2, w3 });

                var enemySet = AssetDatabase.LoadAssetAtPath<TransformRuntimeSet>("Assets/_Data/Variables/EnemySet.asset");
                SetPrivateField(wm, "_enemySet", enemySet);

                // Spawn points
                var spawnParent = new GameObject("SpawnPoints");
                spawnParent.transform.SetParent(waveGo.transform);
                var sp1 = CreateSpawnPoint(spawnParent.transform, new Vector3(-10f, 0f, 0f), "SpawnLeft");
                var sp2 = CreateSpawnPoint(spawnParent.transform, new Vector3(10f, 0f, 0f), "SpawnRight");
                var sp3 = CreateSpawnPoint(spawnParent.transform, new Vector3(0f, 10f, 0f), "SpawnTop");
                var sp4 = CreateSpawnPoint(spawnParent.transform, new Vector3(0f, -10f, 0f), "SpawnBottom");
                SetPrivateField(wm, "_spawnPoints", new Transform[] { sp1, sp2, sp3, sp4 });

                // Base target (player base)
                var baseGo = new GameObject("PlayerBase");
                baseGo.transform.position = Vector3.zero;
                baseGo.AddComponent<Health>();
                var baseSr = baseGo.AddComponent<SpriteRenderer>();
                baseSr.sprite = CreatePlaceholderSprite("BaseSprite", new Color(0f, 0.8f, 1f), 48);
                baseSr.sortingOrder = 3;
                SetPrivateField(wm, "_baseTarget", baseGo.transform);

                var gridManager = Object.FindAnyObjectByType<GridManager>();
                SetPrivateField(wm, "_gridManager", gridManager);

                // Enemy pool
                var enemyPoolGo = new GameObject("EnemyPool");
                enemyPoolGo.transform.SetParent(waveGo.transform);
                var enemyPool = enemyPoolGo.AddComponent<ObjectPool>();
                var scrapperPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Prefabs/Enemies/Scrapper.prefab");
                SetPrivateField(enemyPool, "_prefab", scrapperPrefab);
                SetPrivateField(enemyPool, "_initialSize", 20);
                SetPrivateField(wm, "_enemyPool", enemyPool);
            }

            // UpgradeManager
            if (Object.FindAnyObjectByType<UpgradeManager>() == null)
            {
                var upgradeGo = new GameObject("UpgradeManager");
                upgradeGo.transform.SetParent(managersGo.transform);
                var um = upgradeGo.AddComponent<UpgradeManager>();

                var player = Object.FindAnyObjectByType<PlayerController>();
                if (player != null)
                    SetPrivateField(um, "_playerInventory", player.GetComponent<Inventory>());
            }

            // AudioManager
            if (Object.FindAnyObjectByType<AudioManager>() == null)
            {
                var audioGo = new GameObject("AudioManager");
                audioGo.transform.SetParent(managersGo.transform);
                var am = audioGo.AddComponent<AudioManager>();

                var musicSource = audioGo.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;

                var sfxSource = audioGo.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;

                SetPrivateField(am, "_musicSource", musicSource);
                SetPrivateField(am, "_sfxSource", sfxSource);
            }

            // SaveManager
            if (Object.FindAnyObjectByType<SaveManager>() == null)
            {
                var saveGo = new GameObject("SaveManager");
                saveGo.transform.SetParent(managersGo.transform);
                saveGo.AddComponent<SaveManager>();
            }

            // ResourceSpawner
            if (Object.FindAnyObjectByType<ResourceSpawner>() == null)
            {
                var spawnerGo = new GameObject("ResourceSpawner");
                spawnerGo.transform.SetParent(managersGo.transform);
                var spawner = spawnerGo.AddComponent<ResourceSpawner>();

                var gridManager = Object.FindAnyObjectByType<GridManager>();
                SetPrivateField(spawner, "_gridManager", gridManager);

                var scrap = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/ScrapMetal.asset");
                var wire = AssetDatabase.LoadAssetAtPath<ResourceDefinition>("Assets/_Data/Resources/Wire.asset");
                SetPrivateField(spawner, "_possibleResources", new ResourceDefinition[] { scrap, wire });

                // Resource pool
                var resPoolGo = new GameObject("ResourcePool");
                resPoolGo.transform.SetParent(spawnerGo.transform);
                var resPool = resPoolGo.AddComponent<ObjectPool>();
                var resPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Prefabs/Resources/ResourceNode.prefab");
                SetPrivateField(resPool, "_prefab", resPrefab);
                SetPrivateField(resPool, "_initialSize", 15);
                SetPrivateField(spawner, "_resourcePool", resPool);
            }

            Debug.Log("[Setup] All managers created");
        }

        private static void SetupUI()
        {
            // Check if Canvas exists
            if (Object.FindAnyObjectByType<Canvas>() != null)
            {
                Debug.Log("[Setup] Canvas already exists, skipping UI setup");
                return;
            }

            // EventSystem
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            // Canvas
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // HUD (always visible)
            var hudGo = CreateUIPanel(canvasGo.transform, "HUD", true);
            var hudText = CreateUIText(hudGo.transform, "ResourceText", "Scrap: 0  Wire: 0  Circuit: 0  Gear: 0",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f));

            // Bottom buttons
            CreateUIButton(hudGo.transform, "BuildBtn", "Build [B]", new Vector2(0.1f, 0f), new Vector2(0.1f, 0f), new Vector2(0f, 60f));
            CreateUIButton(hudGo.transform, "WaveBtn", "Wave [W]", new Vector2(0.3f, 0f), new Vector2(0.3f, 0f), new Vector2(0f, 60f));
            CreateUIButton(hudGo.transform, "UpgradeBtn", "Upgrade [U]", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 60f));

            Debug.Log("[Setup] UI Canvas created with HUD");
        }

        #endregion

        #region Helper Methods

        private static Sprite CreatePlaceholderSprite(string name, Color color, int size)
        {
            var path = $"Assets/_Art/Sprites/{name}.png";
            if (AssetDatabase.LoadAssetAtPath<Sprite>(path) != null)
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);

            EnsureDirectory("Assets/_Art/Sprites");

            var tex = new Texture2D(size, size);
            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;

            // Simple border
            for (int x = 0; x < size; x++)
            {
                pixels[x] = Color.black;
                pixels[x + (size - 1) * size] = Color.black;
                pixels[x * size] = Color.black;
                pixels[x * size + size - 1] = Color.black;
            }

            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();

            var pngData = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, pngData);
            AssetDatabase.ImportAsset(path);

            // Configure as sprite
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = size;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static UnityEngine.Tilemaps.Tile CreatePlaceholderTile(string name, Color color)
        {
            var path = $"Assets/_Art/Tiles/{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Tilemaps.Tile>(path) != null)
                return AssetDatabase.LoadAssetAtPath<UnityEngine.Tilemaps.Tile>(path);

            EnsureDirectory("Assets/_Art/Tiles");

            var tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            tile.sprite = CreatePlaceholderSprite($"{name}Sprite", color, 32);
            tile.color = Color.white;

            AssetDatabase.CreateAsset(tile, path);
            return tile;
        }

        private static Transform CreateSpawnPoint(Transform parent, Vector3 position, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = position;
            return go.transform;
        }

        private static GameObject CreateUIPanel(Transform parent, string name, bool startActive)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            go.SetActive(startActive);
            return go;
        }

        private static TextMeshProUGUI CreateUIText(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(600f, 40f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        private static Button CreateUIButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(150f, 50f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var button = go.AddComponent<Button>();

            // Text child
            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return button;
        }

        private static void CreateSOAsset<T>(string path) where T : ScriptableObject
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) != null) return;
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        AssetDatabase.CreateFolder(current, parts[i]);
                    }
                    current = next;
                }
            }
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                var field = type.GetField(fieldName,
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(target, value);
                    if (target is Object unityObj)
                        EditorUtility.SetDirty(unityObj);
                    return;
                }
                type = type.BaseType;
            }
            Debug.LogWarning($"[Setup] Field '{fieldName}' not found on {target.GetType().Name}");
        }

        #endregion
    }
}
