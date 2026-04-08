using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Tracks purchased upgrades and calculates stat modifiers.
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Inventory _playerInventory;
        [SerializeField] private GameEvent _onUpgradePurchased;

        private readonly Dictionary<UpgradeDefinition, int> _upgradeLevels = new();

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<UpgradeManager>();
        }

        #endregion

        #region Public Methods

        public int GetLevel(UpgradeDefinition upgrade)
        {
            if (upgrade == null) return 0;
            return _upgradeLevels.TryGetValue(upgrade, out int level) ? level : 0;
        }

        public bool CanPurchase(UpgradeDefinition upgrade)
        {
            if (upgrade == null) return false;

            // Check max level
            int currentLevel = GetLevel(upgrade);
            if (currentLevel >= upgrade.MaxLevel) return false;

            // Check prerequisite
            if (upgrade.Prerequisite != null && GetLevel(upgrade.Prerequisite) <= 0)
                return false;

            // Check resources
            if (_playerInventory != null && upgrade.CostPerLevel != null)
            {
                return _playerInventory.HasEnoughResources(upgrade.CostPerLevel);
            }

            return true;
        }

        public bool Purchase(UpgradeDefinition upgrade)
        {
            if (!CanPurchase(upgrade)) return false;

            // Spend resources
            if (_playerInventory != null && upgrade.CostPerLevel != null)
            {
                if (!_playerInventory.SpendResources(upgrade.CostPerLevel))
                    return false;
            }

            // Increment level
            if (_upgradeLevels.ContainsKey(upgrade))
            {
                _upgradeLevels[upgrade]++;
            }
            else
            {
                _upgradeLevels[upgrade] = 1;
            }

            _onUpgradePurchased?.Raise();
            return true;
        }

        /// <summary>
        /// Returns cumulative multiplier for a target type. 1.0 = no change.
        /// Example: 2 upgrades with +0.15 each = 1.30 multiplier.
        /// </summary>
        public float GetModifier(UpgradeTargetType targetType)
        {
            float modifier = 1f;

            foreach (var kvp in _upgradeLevels)
            {
                var upgrade = kvp.Key;
                int level = kvp.Value;

                if (upgrade.Effects == null) continue;

                foreach (var effect in upgrade.Effects)
                {
                    if (effect.Target == targetType)
                    {
                        modifier += effect.ValuePerLevel * level;
                    }
                }
            }

            return modifier;
        }

        public Dictionary<UpgradeDefinition, int> GetAllUpgrades()
        {
            return new Dictionary<UpgradeDefinition, int>(_upgradeLevels);
        }

        #endregion
    }
}
