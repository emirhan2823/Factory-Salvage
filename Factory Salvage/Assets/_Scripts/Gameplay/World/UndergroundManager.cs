using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Manages all underground layers: creation, unlocking, slot tracking.
    /// </summary>
    public class UndergroundManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private UndergroundLayer[] _layers;

        #endregion

        #region Properties

        public IReadOnlyList<UndergroundLayer> Layers => _layers;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<UndergroundManager>();
        }

        #endregion

        #region Public Methods

        public void SetLayers(UndergroundLayer[] layers)
        {
            _layers = layers;
        }

        public UndergroundLayer GetLayer(int depth)
        {
            if (_layers == null) return null;
            foreach (var layer in _layers)
            {
                if (layer != null && layer.Definition != null && layer.Definition.Depth == depth)
                    return layer;
            }
            return null;
        }

        public bool TryUnlockLayer(int depth, Inventory inventory)
        {
            var layer = GetLayer(depth);
            if (layer == null) return false;
            return layer.TryUnlock(inventory);
        }

        #endregion
    }
}
