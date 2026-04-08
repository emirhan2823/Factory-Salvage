using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Base class for all placed machines. Handles grid registration and interaction.
    /// </summary>
    public class MachineBase : MonoBehaviour, IInteractable
    {
        #region Fields

        [SerializeField] private MachineDefinition _definition;

        private Vector2Int _gridPosition;
        private GridManager _gridManager;
        private bool _isPlaced;

        #endregion

        #region Properties

        public MachineDefinition Definition => _definition;
        public Vector2Int GridPosition => _gridPosition;
        public bool IsPlaced => _isPlaced;

        #endregion

        #region Public Methods

        public virtual void Place(Vector2Int gridPos, GridManager gridManager)
        {
            _gridPosition = gridPos;
            _gridManager = gridManager;
            _isPlaced = true;

            _gridManager.OccupyCell(gridPos, gameObject);
            transform.position = _gridManager.CellToWorld(gridPos);

            OnPlaced();
        }

        public virtual void Remove()
        {
            if (_isPlaced && _gridManager != null)
            {
                _gridManager.FreeCell(_gridPosition);
            }
            _isPlaced = false;

            OnRemoved();
        }

        public virtual void Interact(PlayerController player)
        {
            // Override in subclasses for specific behavior
            Debug.Log($"Interacted with {_definition?.MachineName ?? "Unknown Machine"}");
        }

        public virtual string GetInteractionPrompt()
        {
            return _definition != null ? _definition.MachineName : "Machine";
        }

        public void SetDefinition(MachineDefinition definition)
        {
            _definition = definition;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnPlaced() { }
        protected virtual void OnRemoved() { }

        #endregion

        #region Unity Callbacks

        protected virtual void OnDestroy()
        {
            if (_isPlaced)
            {
                Remove();
            }
        }

        #endregion
    }
}
