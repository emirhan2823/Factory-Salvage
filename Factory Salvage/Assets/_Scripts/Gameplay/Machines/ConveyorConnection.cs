using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Logical connection between two machines. Transfers output → input on a timer.
    /// </summary>
    public class ConveyorConnection : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ProcessingMachine _source;
        [SerializeField] private ProcessingMachine _destination;
        [SerializeField] private float _transferInterval = 1f;

        private float _transferTimer;

        #endregion

        #region Properties

        public ProcessingMachine Source => _source;
        public ProcessingMachine Destination => _destination;

        #endregion

        #region Unity Callbacks

        private void Update()
        {
            _transferTimer -= Time.deltaTime;
            if (_transferTimer <= 0f)
            {
                _transferTimer = _transferInterval;
                TryTransfer();
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(ProcessingMachine source, ProcessingMachine destination)
        {
            _source = source;
            _destination = destination;
        }

        #endregion

        #region Private Methods

        private void TryTransfer()
        {
            if (_source == null || _destination == null) return;
            if (!_source.HasOutput()) return;

            if (_source.TryTakeOutput(out var resource, out int amount))
            {
                _destination.AcceptInput(resource, amount);
            }
        }

        #endregion
    }
}
