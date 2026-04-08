using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Typed event channel carrying an int payload.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Events/Int Event")]
    public class IntGameEvent : ScriptableObject
    {
        #region Fields

        private readonly List<Action<int>> _listeners = new();

        #endregion

        #region Public Methods

        public void Raise(int value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke(value);
            }
        }

        public void Register(Action<int> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void Unregister(Action<int> listener)
        {
            _listeners.Remove(listener);
        }

        #endregion
    }
}
