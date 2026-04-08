using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Attach to any machine that requires energy to operate.
    /// ProcessingMachine checks HasPower before processing.
    /// </summary>
    public class EnergyConsumer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _energyRequired = 1f;

        private EnergyManager _energyManager;
        private bool _isRegistered;

        #endregion

        #region Properties

        public float EnergyRequired => _energyRequired;

        public bool HasPower
        {
            get
            {
                if (_energyManager == null) return true; // No energy system = free power
                return _energyManager.HasSurplus;
            }
        }

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (ServiceLocator.TryGet<EnergyManager>(out var manager))
            {
                _energyManager = manager;
                _energyManager.RegisterConsumer(_energyRequired);
                _isRegistered = true;
            }
        }

        private void OnDisable()
        {
            if (_isRegistered && _energyManager != null)
            {
                _energyManager.UnregisterConsumer(_energyRequired);
                _isRegistered = false;
            }
        }

        #endregion

        #region Public Methods

        public void SetEnergyRequired(float amount)
        {
            if (_isRegistered && _energyManager != null)
            {
                _energyManager.UnregisterConsumer(_energyRequired);
                _energyRequired = amount;
                _energyManager.RegisterConsumer(_energyRequired);
            }
            else
            {
                _energyRequired = amount;
            }
        }

        #endregion
    }
}
