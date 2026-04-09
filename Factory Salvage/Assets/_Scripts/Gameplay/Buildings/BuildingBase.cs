using UnityEngine;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Base class for all placed buildings. Handles slot placement and leveling.
    /// </summary>
    public class BuildingBase : MonoBehaviour
    {
        #region Fields

        [SerializeField] private BuildingDefinition _definition;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private BuildingSlot _slot;
        private int _level = 1;

        #endregion

        #region Properties

        public BuildingDefinition Definition => _definition;
        public BuildingSlot Slot => _slot;
        public int Level => _level;
        public float LevelMultiplier => 1f + (_level - 1) * 0.25f;

        #endregion

        #region Public Methods

        public void Initialize(BuildingDefinition definition)
        {
            _definition = definition;
            _level = 1;
            UpdateVisual();
        }

        public void Place(BuildingSlot slot)
        {
            _slot = slot;
            slot.TryPlace(gameObject);
            OnPlaced();
        }

        public void Remove()
        {
            if (_slot != null)
            {
                _slot.Remove();
                _slot = null;
            }
            OnRemoved();
        }

        public bool CanLevelUp(Inventory inventory)
        {
            if (_definition == null) return false;
            if (_level >= _definition.MaxLevel) return false;
            if (_definition.BuildCost == null || inventory == null) return true;

            // Scaled cost
            foreach (var cost in _definition.BuildCost)
            {
                int scaledAmount = Mathf.CeilToInt(cost.Amount * Mathf.Pow(_definition.UpgradeCostMultiplier, _level));
                if (!inventory.HasEnoughResource(cost.Resource, scaledAmount)) return false;
            }
            return true;
        }

        public bool LevelUp(Inventory inventory)
        {
            if (!CanLevelUp(inventory)) return false;

            // Spend scaled cost
            if (_definition.BuildCost != null && inventory != null)
            {
                foreach (var cost in _definition.BuildCost)
                {
                    int scaledAmount = Mathf.CeilToInt(cost.Amount * Mathf.Pow(_definition.UpgradeCostMultiplier, _level));
                    inventory.RemoveResource(cost.Resource, scaledAmount);
                }
            }

            _level++;
            UpdateVisual();
            OnLevelUp();
            return true;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnPlaced() { }
        protected virtual void OnRemoved() { }
        protected virtual void OnLevelUp() { }

        #endregion

        #region Private Methods

        private void UpdateVisual()
        {
            if (_spriteRenderer == null) return;
            if (_definition != null)
            {
                _spriteRenderer.color = _definition.Color;
            }
        }

        #endregion
    }
}
