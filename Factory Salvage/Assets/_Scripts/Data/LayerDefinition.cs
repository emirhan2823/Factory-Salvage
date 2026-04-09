using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines an underground layer: depth, slot count, resources, unlock cost.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Layer Definition")]
    public class LayerDefinition : ScriptableObject
    {
        #region Fields

        [Header("Info")]
        [SerializeField] private string _layerName;
        [SerializeField] private int _depth; // 1, 2, 3...

        [Header("Position")]
        [SerializeField] private float _worldY = -5f;
        [SerializeField] private int _slotCount = 6;

        [Header("Visuals")]
        [SerializeField] private Color _backgroundColor = new(0.3f, 0.2f, 0.15f);
        [SerializeField] private Color _slotColor = new(0.5f, 0.4f, 0.3f, 0.3f);

        [Header("Unlock")]
        [SerializeField] private ResourceCost[] _unlockCost;
        [SerializeField] private bool _unlockedByDefault;

        [Header("Available Buildings")]
        [SerializeField] private BuildingDefinition[] _availableBuildings;

        #endregion

        #region Properties

        public string LayerName => _layerName;
        public int Depth => _depth;
        public float WorldY => _worldY;
        public int SlotCount => _slotCount;
        public Color BackgroundColor => _backgroundColor;
        public Color SlotColor => _slotColor;
        public ResourceCost[] UnlockCost => _unlockCost;
        public bool UnlockedByDefault => _unlockedByDefault;
        public BuildingDefinition[] AvailableBuildings => _availableBuildings;

        #endregion
    }
}
