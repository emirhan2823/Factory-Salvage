using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Runtime representation of an underground layer.
    /// Manages slots, visuals, and unlock state.
    /// </summary>
    public class UndergroundLayer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private LayerDefinition _definition;
        [SerializeField] private BuildingSlot[] _slots;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private GameObject _lockOverlay;

        private bool _isUnlocked;

        #endregion

        #region Properties

        public LayerDefinition Definition => _definition;
        public BuildingSlot[] Slots => _slots;
        public bool IsUnlocked => _isUnlocked;

        #endregion

        #region Public Methods

        public void Initialize(LayerDefinition definition, bool unlocked)
        {
            _definition = definition;
            _isUnlocked = unlocked || definition.UnlockedByDefault;

            UpdateVisuals();
        }

        public bool TryUnlock(Inventory inventory)
        {
            if (_isUnlocked) return true;
            if (_definition == null) return false;

            if (_definition.UnlockCost != null && inventory != null)
            {
                if (!inventory.HasEnoughResources(_definition.UnlockCost))
                    return false;
                inventory.SpendResources(_definition.UnlockCost);
            }

            _isUnlocked = true;
            UpdateVisuals();
            Debug.Log($"[Underground] Layer '{_definition.LayerName}' unlocked!");
            return true;
        }

        public void SetSlots(BuildingSlot[] slots)
        {
            _slots = slots;
        }

        #endregion

        #region Private Methods

        private void UpdateVisuals()
        {
            if (_lockOverlay != null)
            {
                _lockOverlay.SetActive(!_isUnlocked);
            }

            // Enable/disable slot colliders based on lock state
            if (_slots != null)
            {
                foreach (var slot in _slots)
                {
                    if (slot == null) continue;
                    var col = slot.GetComponent<Collider2D>();
                    if (col != null) col.enabled = _isUnlocked;
                }
            }
        }

        #endregion
    }
}
