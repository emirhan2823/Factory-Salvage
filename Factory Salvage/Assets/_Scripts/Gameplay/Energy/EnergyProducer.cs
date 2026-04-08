using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A machine that produces energy. Registers with EnergyManager on placement.
    /// </summary>
    public class EnergyProducer : MachineBase
    {
        #region Fields

        [SerializeField] private GeneratorDefinition _generatorDef;

        private EnergyManager _energyManager;
        private bool _isRegistered;

        #endregion

        #region Properties

        public GeneratorDefinition GeneratorDef => _generatorDef;

        #endregion

        #region Protected Methods

        protected override void OnPlaced()
        {
            base.OnPlaced();
            RegisterEnergy();
        }

        protected override void OnRemoved()
        {
            UnregisterEnergy();
            base.OnRemoved();
        }

        #endregion

        #region Unity Callbacks

        protected override void OnDestroy()
        {
            UnregisterEnergy();
            base.OnDestroy();
        }

        #endregion

        #region Public Methods

        public void SetGeneratorDefinition(GeneratorDefinition def)
        {
            _generatorDef = def;
        }

        #endregion

        #region Private Methods

        private void RegisterEnergy()
        {
            if (_isRegistered || _generatorDef == null) return;

            if (ServiceLocator.TryGet<EnergyManager>(out var manager))
            {
                _energyManager = manager;
                _energyManager.RegisterProducer(_generatorDef.EnergyOutput);
                _isRegistered = true;
            }
        }

        private void UnregisterEnergy()
        {
            if (!_isRegistered || _energyManager == null) return;

            _energyManager.UnregisterProducer(_generatorDef.EnergyOutput);
            _isRegistered = false;
        }

        #endregion
    }
}
