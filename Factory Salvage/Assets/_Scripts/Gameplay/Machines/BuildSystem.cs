using UnityEngine;
using UnityEngine.InputSystem;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Manages build mode: selecting machines, ghost preview, placement validation.
    /// </summary>
    public class BuildSystem : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private Inventory _playerInventory;
        [SerializeField] private MachineGhost _ghost;

        [Header("Config")]
        [SerializeField] private MachineDefinition[] _availableMachines;

        [Header("Events")]
        [SerializeField] private GameEvent _onMachinePlaced;
        [SerializeField] private GameEvent _onBuildModeChanged;

        private MachineDefinition _selectedMachine;
        private Camera _mainCamera;
        private bool _inBuildMode;

        #endregion

        #region Properties

        public bool InBuildMode => _inBuildMode;
        public MachineDefinition SelectedMachine => _selectedMachine;
        public MachineDefinition[] AvailableMachines => _availableMachines;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_ghost != null) _ghost.Hide();
        }

        private void Update()
        {
            if (!_inBuildMode) return;

            UpdateGhostPosition();
            HandleBuildInput();
        }

        #endregion

        #region Public Methods

        public void EnterBuildMode(MachineDefinition machine)
        {
            if (machine == null) return;

            _selectedMachine = machine;
            _inBuildMode = true;

            if (_ghost != null)
            {
                _ghost.SetSprite(machine.Icon);
                _ghost.Show();
            }

            _onBuildModeChanged?.Raise();
        }

        public void ExitBuildMode()
        {
            _selectedMachine = null;
            _inBuildMode = false;

            if (_ghost != null) _ghost.Hide();

            _onBuildModeChanged?.Raise();
        }

        public bool TryPlaceMachine(Vector2Int gridPos)
        {
            if (_selectedMachine == null) return false;
            if (_gridManager == null) return false;

            // Check cell buildable
            if (!_gridManager.IsCellBuildable(gridPos))
            {
                return false;
            }

            // Check resources
            if (_playerInventory != null && !_playerInventory.HasEnoughResources(_selectedMachine.BuildCost))
            {
                return false;
            }

            // Spend resources
            if (_playerInventory != null)
            {
                _playerInventory.SpendResources(_selectedMachine.BuildCost);
            }

            // Instantiate machine
            var prefab = _selectedMachine.Prefab;
            if (prefab == null)
            {
                Debug.LogWarning($"[BuildSystem] No prefab for {_selectedMachine.MachineName}");
                return false;
            }

            var worldPos = _gridManager.CellToWorld(gridPos);
            var machineObj = Instantiate(prefab, worldPos, Quaternion.identity);

            var machineBase = machineObj.GetComponent<MachineBase>();
            if (machineBase != null)
            {
                machineBase.SetDefinition(_selectedMachine);
                machineBase.Place(gridPos, _gridManager);
            }

            _onMachinePlaced?.Raise();
            return true;
        }

        #endregion

        #region Private Methods

        private void UpdateGhostPosition()
        {
            if (_ghost == null || _mainCamera == null) return;

            var mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            var worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;

            var gridPos = _gridManager.WorldToCell(worldPos);
            var snappedPos = _gridManager.CellToWorld(gridPos);

            _ghost.UpdatePosition(snappedPos);
            _ghost.SetValid(_gridManager.IsCellBuildable(gridPos));
        }

        private void HandleBuildInput()
        {
            if (Mouse.current == null) return;

            // Place on click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Don't build if over UI
                if (UnityEngine.EventSystems.EventSystem.current != null &&
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                var mousePos = Mouse.current.position.ReadValue();
                var worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
                var gridPos = _gridManager.WorldToCell(worldPos);

                TryPlaceMachine(gridPos);
            }

            // Cancel on right click or Escape
            if ((Mouse.current.rightButton != null && Mouse.current.rightButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame))
            {
                ExitBuildMode();
            }
        }

        #endregion
    }
}
