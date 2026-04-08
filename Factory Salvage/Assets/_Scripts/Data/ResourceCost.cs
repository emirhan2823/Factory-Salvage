using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// A resource + amount pair used for build costs, recipes, etc.
    /// </summary>
    [System.Serializable]
    public struct ResourceCost
    {
        public ResourceDefinition Resource;
        public int Amount;
    }
}
