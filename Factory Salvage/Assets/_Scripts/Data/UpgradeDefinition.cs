using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines an upgrade: cost, effect, prerequisites, max level.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        #region Fields

        [Header("Info")]
        [SerializeField] private string _upgradeName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        [Header("Cost")]
        [SerializeField] private ResourceCost[] _costPerLevel;

        [Header("Effect")]
        [SerializeField] private UpgradeEffect[] _effects;

        [Header("Progression")]
        [SerializeField] private int _maxLevel = 3;
        [SerializeField] private UpgradeDefinition _prerequisite;

        #endregion

        #region Properties

        public string UpgradeName => _upgradeName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public ResourceCost[] CostPerLevel => _costPerLevel;
        public UpgradeEffect[] Effects => _effects;
        public int MaxLevel => _maxLevel;
        public UpgradeDefinition Prerequisite => _prerequisite;

        #endregion
    }

    public enum UpgradeTargetType
    {
        MachineSpeed,
        TurretDamage,
        TurretRange,
        EnergyOutput,
        MaxHealth,
        GatherSpeed
    }

    [System.Serializable]
    public struct UpgradeEffect
    {
        public UpgradeTargetType Target;
        public float ValuePerLevel;
    }
}
