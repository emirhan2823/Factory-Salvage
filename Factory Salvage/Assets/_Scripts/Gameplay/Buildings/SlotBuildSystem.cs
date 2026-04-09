using UnityEngine;
using UnityEngine.InputSystem;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Handles tapping on empty slots to open build menu and place buildings.
    /// Replaces the old grid-based BuildSystem.
    /// </summary>
    public class SlotBuildSystem : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Inventory _inventory;
        [SerializeField] private BuildingDefinition[] _availableBuildings;

        [Header("Events")]
        [SerializeField] private GameEvent _onBuildingPlaced;

        private Camera _mainCamera;
        private BuildingSlot _selectedSlot;

        #endregion

        #region Properties

        public BuildingDefinition[] AvailableBuildings => _availableBuildings;
        public BuildingSlot SelectedSlot => _selectedSlot;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _mainCamera = Camera.main;
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SlotBuildSystem>();
        }

        private void Start()
        {
            if (_inventory == null)
                _inventory = FindAnyObjectByType<Inventory>();
        }

        private void Update()
        {
            HandleTapInput();
        }

        #endregion

        #region Public Methods

        public bool TryBuild(BuildingDefinition definition, BuildingSlot slot)
        {
            if (definition == null || slot == null || slot.IsOccupied) return false;

            // Check cost
            if (_inventory != null && definition.BuildCost != null)
            {
                if (!_inventory.HasEnoughResources(definition.BuildCost))
                    return false;
                _inventory.SpendResources(definition.BuildCost);
            }

            // Create building
            var buildingGo = new GameObject(definition.BuildingName);

            // Sprite
            var sr = buildingGo.AddComponent<SpriteRenderer>();
            sr.color = definition.Color;
            sr.sprite = CreatePlaceholderSprite(definition.Color);
            sr.sortingOrder = 5;

            // Building base
            var buildingBase = buildingGo.AddComponent<BuildingBase>();
            buildingBase.Initialize(definition);

            // Set sprite renderer reference
            SetField(buildingBase, "_spriteRenderer", sr);

            // Add behavior based on category
            if (definition.IsProducer)
            {
                buildingGo.AddComponent<IdleProducer>();
            }
            else if (definition.IsCrafter)
            {
                buildingGo.AddComponent<CraftingBuilding>();
            }

            // Place in slot
            buildingBase.Place(slot);

            _onBuildingPlaced?.Raise();

            Debug.Log($"[Build] Placed {definition.BuildingName} at slot {slot.SlotIndex}");
            return true;
        }

        public void SelectSlot(BuildingSlot slot)
        {
            _selectedSlot = slot;
        }

        public void ClearSelection()
        {
            _selectedSlot = null;
        }

        #endregion

        #region Private Methods

        private void HandleTapInput()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            // Check if tapped on a slot
            var hit = Physics2D.OverlapPoint(worldPos);
            if (hit == null) return;

            var slot = hit.GetComponent<BuildingSlot>();
            if (slot != null && !slot.IsOccupied)
            {
                // Auto-build first available building for testing
                // TODO: Replace with proper build menu UI
                if (_availableBuildings != null && _availableBuildings.Length > 0)
                {
                    // Cycle through buildings — pick first one we can afford
                    foreach (var building in _availableBuildings)
                    {
                        if (_inventory == null || _inventory.HasEnoughResources(building.BuildCost))
                        {
                            TryBuild(building, slot);
                            return;
                        }
                    }
                    Debug.Log("[Build] Not enough resources for any building!");
                }
            }

            // Check if tapped on a building (for info/upgrade)
            var buildingBase = hit.GetComponentInParent<BuildingBase>();
            if (buildingBase != null)
            {
                if (buildingBase.LevelUp(_inventory))
                {
                    Debug.Log($"[Build] {buildingBase.Definition.BuildingName} upgraded to level {buildingBase.Level}!");
                }
            }
        }

        private Sprite CreatePlaceholderSprite(Color color)
        {
            var tex = new Texture2D(32, 48);
            var pixels = new Color[32 * 48];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            // Border
            for (int x = 0; x < 32; x++)
            {
                pixels[x] = Color.black;
                pixels[x + 47 * 32] = Color.black;
            }
            for (int y = 0; y < 48; y++)
            {
                pixels[y * 32] = Color.black;
                pixels[y * 32 + 31] = Color.black;
            }
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 32, 48), new Vector2(0.5f, 0.25f), 32f);
        }

        private void SetField(object target, string name, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                var field = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null) { field.SetValue(target, value); return; }
                type = type.BaseType;
            }
        }

        #endregion
    }
}
