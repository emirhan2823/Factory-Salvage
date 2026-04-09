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

            // Sprite (SpriteFactory generates shaped sprites)
            var sr = buildingGo.AddComponent<SpriteRenderer>();
            sr.sprite = Core.SpriteFactory.CreateBuilding(definition.Category, definition.Color, 1);
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

            // Defense buildings get a tower component
            if (definition.Category == FactorySalvage.Data.BuildingCategory.Defense)
            {
                var towerType = System.Type.GetType("FactorySalvage.Gameplay.DefenseTower, FactorySalvage");
                if (towerType != null)
                {
                    var tower = (Component)buildingGo.AddComponent(towerType);
                    var setStats = towerType.GetMethod("SetStats");
                    setStats?.Invoke(tower, new object[] { 10f, 1f, 8f });
                }
            }

            // Animations
            var animator = buildingGo.AddComponent<SpriteAnimator>();
            animator.SetAnimation(SpriteAnimator.AnimationType.IdleBounce, 1f, 0.03f);
            animator.PlaySpawnPop();

            // Place in slot
            buildingBase.Place(slot);

            _onBuildingPlaced?.Raise();
            Core.SimpleParticleSystem.Emit(slot.Position, Core.SimpleParticleSystem.ParticlePreset.LevelUp, 8);
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
                // Open build menu if available, otherwise auto-build
                if (Core.ServiceLocator.TryGet<UI.BuildMenuUI>(out var buildMenu))
                {
                    buildMenu.Show(slot);
                    return;
                }

                // Fallback: auto-build first affordable building
                if (_availableBuildings == null || _availableBuildings.Length == 0) return;
                bool isDefenseSlot = slot.Zone == ZoneType.Defense;
                foreach (var building in _availableBuildings)
                {
                    if (building == null) continue;
                    bool isDefenseBuilding = building.Category == Data.BuildingCategory.Defense;
                    if (isDefenseSlot != isDefenseBuilding) continue;
                    if (_inventory == null || _inventory.HasEnoughResources(building.BuildCost))
                    {
                        TryBuild(building, slot);
                        return;
                    }
                }
                Debug.Log("[Build] Not enough resources!");
                return;
            }

            // Check if tapped on a building — show info popup
            var buildingBase = hit.GetComponentInParent<BuildingBase>();
            if (buildingBase != null)
            {
                if (Core.ServiceLocator.TryGet<UI.BuildingInfoUI>(out var infoUI))
                {
                    infoUI.Show(buildingBase);
                }
                else
                {
                    // Fallback: direct upgrade
                    if (buildingBase.CanLevelUp(_inventory))
                    {
                        buildingBase.LevelUp(_inventory);
                        Debug.Log($"[Upgrade] {buildingBase.Definition.BuildingName} -> Lv.{buildingBase.Level}");
                    }
                }
            }
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
