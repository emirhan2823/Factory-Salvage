using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines a generator's energy output and fuel consumption.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Generator Definition")]
    public class GeneratorDefinition : ScriptableObject
    {
        #region Fields

        [SerializeField] private float _energyOutput = 10f;
        [SerializeField] private float _fuelConsumptionRate;
        [SerializeField] private ResourceDefinition _fuelType;

        #endregion

        #region Properties

        public float EnergyOutput => _energyOutput;
        public float FuelConsumptionRate => _fuelConsumptionRate;
        public ResourceDefinition FuelType => _fuelType;
        public bool RequiresFuel => _fuelType != null && _fuelConsumptionRate > 0f;

        #endregion
    }
}
