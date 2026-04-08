using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Typed event channel carrying a float payload.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Events/Float Event")]
    public class FloatGameEvent : ScriptableObject
    {
        #region Fields

        private readonly List<Action<float>> _listeners = new();

        #endregion

        #region Public Methods

        public void Raise(float value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke(value);
            }
        }

        public void Register(Action<float> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void Unregister(Action<float> listener)
        {
            _listeners.Remove(listener);
        }

        #endregion
    }
}
