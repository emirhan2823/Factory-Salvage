using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A building that consumes input resources from global Inventory
    /// and produces output resources on a timer. Auto-feeds from Inventory.
    /// </summary>
    [RequireComponent(typeof(BuildingBase))]
    public class CraftingBuilding : MonoBehaviour
    {
        #region Fields

        private BuildingBase _building;
        private Inventory _inventory;
        private float _processTimer;
        private bool _isProcessing;

        #endregion

        #region Properties

        public bool IsProcessing => _isProcessing;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _building = GetComponent<BuildingBase>();
        }

        private void Start()
        {
            _inventory = FindAnyObjectByType<Inventory>();
        }

        private void Update()
        {
            if (_building == null || _building.Definition == null) return;
            if (!_building.Definition.IsCrafter) return;
            if (_inventory == null) return;

            if (_isProcessing)
            {
                _processTimer -= Time.deltaTime;
                if (_processTimer <= 0f)
                {
                    CompleteProcessing();
                }
            }
            else
            {
                TryStartProcessing();
            }
        }

        #endregion

        #region Private Methods

        private void TryStartProcessing()
        {
            var recipe = _building.Definition.Recipe;
            if (recipe == null || recipe.Inputs == null) return;

            // Check all inputs available in global inventory
            foreach (var input in recipe.Inputs)
            {
                if (!_inventory.HasEnoughResource(input.Resource, input.Amount))
                    return;
            }

            // Consume inputs
            foreach (var input in recipe.Inputs)
            {
                _inventory.RemoveResource(input.Resource, input.Amount);
            }

            float speedMultiplier = _building.LevelMultiplier;
            _processTimer = recipe.ProcessTime / speedMultiplier;
            _isProcessing = true;
        }

        private void CompleteProcessing()
        {
            var recipe = _building.Definition.Recipe;
            if (recipe == null || recipe.Outputs == null) return;

            float multiplier = _building.LevelMultiplier;

            foreach (var output in recipe.Outputs)
            {
                int amount = Mathf.Max(1, Mathf.FloorToInt(output.Amount * multiplier));
                _inventory.AddResource(output.Resource, amount);
            }

            _isProcessing = false;
        }

        #endregion
    }
}
