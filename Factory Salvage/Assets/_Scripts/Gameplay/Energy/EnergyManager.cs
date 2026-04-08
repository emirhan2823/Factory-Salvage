using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Tracks total energy production and consumption.
    /// Machines check HasSufficientEnergy before processing.
    /// </summary>
    public class EnergyManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private FloatVariable _currentEnergy;
        [SerializeField] private FloatVariable _maxEnergy;
        [SerializeField] private GameEvent _onEnergyDepleted;

        private float _totalProduction;
        private float _totalConsumption;

        #endregion

        #region Properties

        public float TotalProduction => _totalProduction;
        public float TotalConsumption => _totalConsumption;
        public float NetEnergy => _totalProduction - _totalConsumption;
        public bool HasSurplus => NetEnergy >= 0f;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<EnergyManager>();
        }

        private void Update()
        {
            UpdateEnergyVariables();
        }

        #endregion

        #region Public Methods

        public void RegisterProducer(float output)
        {
            _totalProduction += output;
        }

        public void UnregisterProducer(float output)
        {
            _totalProduction -= output;
            _totalProduction = Mathf.Max(0f, _totalProduction);
        }

        public void RegisterConsumer(float consumption)
        {
            _totalConsumption += consumption;
        }

        public void UnregisterConsumer(float consumption)
        {
            _totalConsumption -= consumption;
            _totalConsumption = Mathf.Max(0f, _totalConsumption);
        }

        public bool HasSufficientEnergy(float amount)
        {
            return NetEnergy >= amount;
        }

        #endregion

        #region Private Methods

        private void UpdateEnergyVariables()
        {
            if (_currentEnergy != null)
            {
                _currentEnergy.Value = NetEnergy;
            }
            if (_maxEnergy != null)
            {
                _maxEnergy.Value = _totalProduction;
            }

            if (!HasSurplus)
            {
                _onEnergyDepleted?.Raise();
            }
        }

        #endregion
    }
}
