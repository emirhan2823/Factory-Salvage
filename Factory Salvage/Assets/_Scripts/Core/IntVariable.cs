using System;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Observable int stored as a ScriptableObject.
    /// Use for shared runtime data (scores, counts, wave numbers, etc).
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Variables/Int")]
    public class IntVariable : ScriptableObject
    {
        #region Fields

        [SerializeField] private int _initialValue;
        private int _runtimeValue;

        #endregion

        #region Properties

        public int Value
        {
            get => _runtimeValue;
            set
            {
                if (_runtimeValue != value)
                {
                    _runtimeValue = value;
                    OnValueChanged?.Invoke(_runtimeValue);
                }
            }
        }

        #endregion

        #region Events

        public event Action<int> OnValueChanged;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _runtimeValue = _initialValue;
        }

        #endregion

        #region Public Methods

        public void Add(int amount)
        {
            Value += amount;
        }

        public void ResetToInitial()
        {
            Value = _initialValue;
        }

        #endregion
    }
}
