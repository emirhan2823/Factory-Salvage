using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Produces resources passively on a timer. Core idle income mechanic.
    /// Output scales with building level.
    /// </summary>
    [RequireComponent(typeof(BuildingBase))]
    public class IdleProducer : MonoBehaviour
    {
        #region Fields

        private BuildingBase _building;
        private Inventory _inventory;
        private float _timer;

        #endregion

        #region Properties

        public float ProductionRate
        {
            get
            {
                if (_building == null || _building.Definition == null) return 0f;
                return _building.LevelMultiplier / _building.Definition.ProductionInterval;
            }
        }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _building = GetComponent<BuildingBase>();
        }

        private void Start()
        {
            FindInventory();
        }

        private void Update()
        {
            if (_building == null || _building.Definition == null) return;
            if (!_building.Definition.IsProducer) return;
            if (_inventory == null)
            {
                FindInventory();
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Produce();
                _timer = _building.Definition.ProductionInterval;
            }
        }

        #endregion

        #region Private Methods

        private void FindInventory()
        {
            _inventory = FindAnyObjectByType<Inventory>();
        }

        private void Produce()
        {
            var outputs = _building.Definition.PassiveOutput;
            if (outputs == null) return;

            float multiplier = _building.LevelMultiplier;

            foreach (var output in outputs)
            {
                int amount = Mathf.Max(1, Mathf.FloorToInt(output.Amount * multiplier));
                _inventory.AddResource(output.Resource, amount);
            }
        }

        #endregion
    }
}
