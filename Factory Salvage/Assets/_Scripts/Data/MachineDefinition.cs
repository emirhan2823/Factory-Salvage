using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines a machine type (Smelter, Assembler, WireMill, Generator, etc).
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Machine Definition")]
    public class MachineDefinition : ScriptableObject
    {
        #region Fields

        [Header("Info")]
        [SerializeField] private string _machineName;
        [SerializeField] private string _machineId;
        [SerializeField] private Sprite _icon;

        [Header("Placement")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Vector2Int _size = Vector2Int.one;

        [Header("Cost")]
        [SerializeField] private ResourceCost[] _buildCost;

        [Header("Processing")]
        [SerializeField] private float _processTime = 3f;
        [SerializeField] private float _energyConsumption = 1f;

        #endregion

        #region Properties

        public string MachineName => _machineName;
        public string MachineId => _machineId;
        public Sprite Icon => _icon;
        public GameObject Prefab => _prefab;
        public Vector2Int Size => _size;
        public ResourceCost[] BuildCost => _buildCost;
        public float ProcessTime => _processTime;
        public float EnergyConsumption => _energyConsumption;

        #endregion
    }
}
