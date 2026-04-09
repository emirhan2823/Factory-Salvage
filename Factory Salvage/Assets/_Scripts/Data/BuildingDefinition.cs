using UnityEngine;

namespace FactorySalvage.Data
{
    public enum BuildingCategory
    {
        Resource,
        Processing,
        Military,
        Defense
    }

    /// <summary>
    /// Defines a building type: cost, production, category.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        #region Fields

        [Header("Info")]
        [SerializeField] private string _buildingName;
        [SerializeField] private string _buildingId;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private BuildingCategory _category;

        [Header("Cost")]
        [SerializeField] private ResourceCost[] _buildCost;

        [Header("Idle Production (Resource buildings)")]
        [SerializeField] private ResourceOutput[] _passiveOutput;
        [SerializeField] private float _productionInterval = 1f;

        [Header("Crafting (Processing buildings)")]
        [SerializeField] private RecipeDefinition _recipe;

        [Header("Upgrades")]
        [SerializeField] private int _maxLevel = 99;
        [SerializeField] private float _upgradeCostMultiplier = 1.5f;

        #endregion

        #region Properties

        public string BuildingName => _buildingName;
        public string BuildingId => _buildingId;
        public Sprite Icon => _icon;
        public Color Color => _color;
        public BuildingCategory Category => _category;
        public ResourceCost[] BuildCost => _buildCost;
        public ResourceOutput[] PassiveOutput => _passiveOutput;
        public float ProductionInterval => _productionInterval;
        public RecipeDefinition Recipe => _recipe;
        public int MaxLevel => _maxLevel;
        public float UpgradeCostMultiplier => _upgradeCostMultiplier;

        public bool IsProducer => _passiveOutput != null && _passiveOutput.Length > 0;
        public bool IsCrafter => _recipe != null;

        #endregion
    }
}
