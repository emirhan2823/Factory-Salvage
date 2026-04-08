using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Player inventory. Tracks resource quantities and raises events on change.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameEvent _onInventoryChanged;

        private readonly Dictionary<ResourceDefinition, int> _resources = new();

        #endregion

        #region Public Methods

        public void AddResource(ResourceDefinition resource, int amount)
        {
            if (resource == null || amount <= 0) return;

            if (_resources.ContainsKey(resource))
            {
                _resources[resource] += amount;
            }
            else
            {
                _resources[resource] = amount;
            }

            _onInventoryChanged?.Raise();
        }

        public bool RemoveResource(ResourceDefinition resource, int amount)
        {
            if (resource == null || amount <= 0) return false;
            if (!HasEnoughResource(resource, amount)) return false;

            _resources[resource] -= amount;
            if (_resources[resource] <= 0)
            {
                _resources.Remove(resource);
            }

            _onInventoryChanged?.Raise();
            return true;
        }

        public bool HasEnoughResource(ResourceDefinition resource, int amount)
        {
            if (resource == null) return false;
            return _resources.TryGetValue(resource, out int current) && current >= amount;
        }

        public bool HasEnoughResources(ResourceCost[] costs)
        {
            if (costs == null) return true;
            foreach (var cost in costs)
            {
                if (!HasEnoughResource(cost.Resource, cost.Amount))
                    return false;
            }
            return true;
        }

        public bool SpendResources(ResourceCost[] costs)
        {
            if (!HasEnoughResources(costs)) return false;
            foreach (var cost in costs)
            {
                RemoveResource(cost.Resource, cost.Amount);
            }
            return true;
        }

        public int GetAmount(ResourceDefinition resource)
        {
            if (resource == null) return 0;
            return _resources.TryGetValue(resource, out int amount) ? amount : 0;
        }

        public Dictionary<ResourceDefinition, int> GetAllResources()
        {
            return new Dictionary<ResourceDefinition, int>(_resources);
        }

        public void Clear()
        {
            _resources.Clear();
            _onInventoryChanged?.Raise();
        }

        #endregion
    }
}
