using System;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Observable float stored as a ScriptableObject.
    /// Use for shared runtime data (health, energy, etc).
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Variables/Float")]
    public class FloatVariable : ScriptableObject
    {
        #region Fields

        [SerializeField] private float _initialValue;
        private float _runtimeValue;

        #endregion

        #region Properties

        public float Value
        {
            get => _runtimeValue;
            set
            {
                if (Math.Abs(_runtimeValue - value) > float.Epsilon)
                {
                    _runtimeValue = value;
                    OnValueChanged?.Invoke(_runtimeValue);
                }
            }
        }

        #endregion

        #region Events

        public event Action<float> OnValueChanged;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _runtimeValue = _initialValue;
        }

        #endregion

        #region Public Methods

        public void Add(float amount)
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
