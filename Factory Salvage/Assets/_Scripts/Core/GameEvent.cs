using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// SO-based event channel. Decouples sender and receiver.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        #region Fields

        private readonly List<Action> _listeners = new();

        #endregion

        #region Public Methods

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke();
            }
        }

        public void Register(Action listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public void Unregister(Action listener)
        {
            _listeners.Remove(listener);
        }

        #endregion
    }
}
