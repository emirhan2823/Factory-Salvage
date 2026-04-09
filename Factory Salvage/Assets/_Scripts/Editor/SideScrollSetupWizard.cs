using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Gameplay;

namespace FactorySalvage.Editor
{
    /// <summary>
    /// Setup wizard for the side-scroll idle TD game.
    /// Menu: FactorySalvage → Side-Scroll Setup
    /// </summary>
    public static class SideScrollSetupWizard
    {
        [MenuItem("FactorySalvage/Side-Scroll/Setup New Scene")]
        public static void SetupSideScrollScene()
        {
            // Create new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            CreateGround();
            CreateZones();
            SetupCamera();
            CreateBase();
            CreateCanvas();
            CreateManagers();

            // Save scene
            EditorSceneManager.SaveScene(scene, "Assets/_Scenes/SideScrollMain.unity");

            Debug.Log("[SideScroll Setup] Scene created! Press Play to test.");
        }

        private static void CreateGround()
        {
            // Long horizontal ground
            var groundGo = new GameObject("Ground");

            var sr = groundGo.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColorSprite("GroundLong", new Color(0.25f, 0.2f, 0.15f), 1024, 64);
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(50f, 2f);
            sr.sortingOrder = -1;

            groundGo.transform.position = new Vector3(5f, -1f, 0f);

            // Ground collider
            var col = groundGo.AddComponent<BoxCollider2D>();
            col.size = new Vector2(50f, 2f);

            // Resource zone background (left)
            CreateZoneBackground("ResourceBG", new Color(0.15f, 0.3f, 0.15f, 0.3f), -10f, 7f);

            // Village zone background (center)
            CreateZoneBackground("VillageBG", new Color(0.2f, 0.2f, 0.35f, 0.3f), 0f, 12f);

            // Defense zone background (right)
            CreateZoneBackground("DefenseBG", new Color(0.35f, 0.15f, 0.15f, 0.3f), 12f, 8f);

            Debug.Log("[Setup] Ground and zone backgrounds created");
        }

        private static void CreateZoneBackground(string name, Color color, float startX, float width)
        {
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColorSprite(name + "Sprite", color, 32, 32);
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(width, 8f);
            sr.sortingOrder = -2;
            go.transform.position = new Vector3(startX + width * 0.5f, 3f, 0f);
        }

        private static void CreateZones()
        {
            var zonesParent = new GameObject("Zones");

            // Resource Zone (left)
            var resourceZone = new GameObject("ResourceZone");
            resourceZone.transform.SetParent(zonesParent.transform);
            var rz = resourceZone.AddComponent<WorldZone>();
            SetField(rz, "_zoneType", ZoneType.Resource);
            SetField(rz, "_minX", -10f);
            SetField(rz, "_maxX", 0f);
            SetField(rz, "_gizmoColor", Color.green);

            // Village Zone (center)
            var villageZone = new GameObject("VillageZone");
            villageZone.transform.SetParent(zonesParent.transform);
            var vz = villageZone.AddComponent<WorldZone>();
            SetField(vz, "_zoneType", ZoneType.Village);
            SetField(vz, "_minX", 0f);
            SetField(vz, "_maxX", 12f);
            SetField(vz, "_gizmoColor", Color.blue);

            // Defense Zone (right)
            var defenseZone = new GameObject("DefenseZone");
            defenseZone.transform.SetParent(zonesParent.transform);
            var dz = defenseZone.AddComponent<WorldZone>();
            SetField(dz, "_zoneType", ZoneType.Defense);
            SetField(dz, "_minX", 12f);
            SetField(dz, "_maxX", 20f);
            SetField(dz, "_gizmoColor", Color.red);

            Debug.Log("[Setup] 3 zones created: Resource(-10~0), Village(0~12), Defense(12~20)");
        }

        private static void SetupCamera()
        {
            var cam = Camera.main;
            if (cam == null) return;

            // Remove old CameraController if exists
            var oldCam = cam.GetComponent<CameraController>();
            if (oldCam != null) Object.DestroyImmediate(oldCam);

            // Add side-scroll camera
            var ssCam = cam.gameObject.AddComponent<SideScrollCamera>();
            SetField(ssCam, "_minX", -10f);
            SetField(ssCam, "_maxX", 20f);
            SetField(ssCam, "_fixedY", 3f);
            SetField(ssCam, "_minZoom", 4f);
            SetField(ssCam, "_maxZoom", 10f);

            cam.orthographicSize = 6f;
            cam.transform.position = new Vector3(5f, 3f, -10f);
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f);

            Debug.Log("[Setup] Side-scroll camera configured");
        }

        private static void CreateBase()
        {
            // Player base at village/defense boundary
            var baseGo = new GameObject("PlayerBase");
            baseGo.transform.position = new Vector3(12f, 1.5f, 0f);

            var sr = baseGo.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColorSprite("BaseSprite2", new Color(0f, 0.7f, 1f), 48, 64);
            sr.sortingOrder = 5;

            var health = baseGo.AddComponent<Combat.Health>();
            SetField(health, "_maxHealth", 100f);

            var col = baseGo.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1.5f, 2f);

            Debug.Log("[Setup] Player base created at x=12");
        }

        private static void CreateCanvas()
        {
            // EventSystem
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

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

            // Resource text (top)
            var resTextGo = new GameObject("ResourceText", typeof(RectTransform));
            resTextGo.transform.SetParent(canvasGo.transform, false);
            var resRect = resTextGo.GetComponent<RectTransform>();
            resRect.anchorMin = new Vector2(0f, 1f);
            resRect.anchorMax = new Vector2(1f, 1f);
            resRect.anchoredPosition = new Vector2(0f, -30f);
            resRect.sizeDelta = new Vector2(0f, 50f);
            var resTmp = resTextGo.AddComponent<TextMeshProUGUI>();
            resTmp.text = "Tap and drag to scroll!";
            resTmp.fontSize = 22;
            resTmp.alignment = TextAlignmentOptions.Center;
            resTmp.color = Color.white;

            // Bottom buttons
            CreateButton(canvasGo.transform, "BuildBtn", "Build", 0.15f);
            CreateButton(canvasGo.transform, "WaveBtn", "Start Wave", 0.5f);
            CreateButton(canvasGo.transform, "UpgradeBtn", "Upgrades", 0.85f);

            // Zone labels (world space)
            CreateWorldLabel("ResourceLabel", "KAYNAKLAR", -5f, Color.green);
            CreateWorldLabel("VillageLabel", "KOY", 6f, Color.cyan);
            CreateWorldLabel("DefenseLabel", "SAVUNMA", 16f, Color.red);

            Debug.Log("[Setup] Canvas and UI created");
        }

        private static void CreateManagers()
        {
            var managersGo = new GameObject("Managers");

            // Inventory (standalone, no player needed for idle game)
            var invGo = new GameObject("GlobalInventory");
            invGo.transform.SetParent(managersGo.transform);
            invGo.AddComponent<Resources.Inventory>();

            // Audio
            var audioGo = new GameObject("AudioManager");
            audioGo.transform.SetParent(managersGo.transform);
            var am = audioGo.AddComponent<Audio.AudioManager>();
            var musicSrc = audioGo.AddComponent<AudioSource>();
            musicSrc.loop = true;
            musicSrc.playOnAwake = false;
            var sfxSrc = audioGo.AddComponent<AudioSource>();
            sfxSrc.playOnAwake = false;
            SetField(am, "_musicSource", musicSrc);
            SetField(am, "_sfxSource", sfxSrc);

            // Save
            var saveGo = new GameObject("SaveManager");
            saveGo.transform.SetParent(managersGo.transform);
            saveGo.AddComponent<Data.SaveManager>();

            Debug.Log("[Setup] Managers created (Inventory, Audio, Save)");
        }

        #region Helpers

        private static void CreateButton(Transform parent, string name, string text, float anchorX)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(anchorX, 0f);
            rect.anchorMax = new Vector2(anchorX, 0f);
            rect.anchoredPosition = new Vector2(0f, 50f);
            rect.sizeDelta = new Vector2(160f, 50f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.3f, 0.9f);
            go.AddComponent<Button>();

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private static void CreateWorldLabel(string name, string text, float x, Color color)
        {
            var go = new GameObject(name);
            go.transform.position = new Vector3(x, 6.5f, 0f);
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.fontSize = 6;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = color;
            tmp.sortingOrder = 20;
        }

        private static Sprite CreateColorSprite(string name, Color color, int w, int h)
        {
            var path = $"Assets/_Art/Sprites/{name}.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            GameSetupWizard_EnsureDirectory("Assets/_Art/Sprites");

            var tex = new Texture2D(w, h);
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();

            var pngData = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, pngData);
            AssetDatabase.ImportAsset(path);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void GameSetupWizard_EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }
        }

        private static void SetField(object target, string fieldName, object value)
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
        }

        #endregion
    }
}
