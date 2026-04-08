using UnityEngine;
using UnityEngine.Events;

namespace FactorySalvage.Core
{
    /// <summary>
    /// MonoBehaviour that listens to a GameEvent and invokes a UnityEvent response.
    /// Wire up in the Inspector.
    /// </summary>
    public class GameEventListener : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameEvent _gameEvent;
        [SerializeField] private UnityEvent _response;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (_gameEvent != null)
            {
                _gameEvent.Register(OnEventRaised);
            }
        }

        private void OnDisable()
        {
            if (_gameEvent != null)
            {
                _gameEvent.Unregister(OnEventRaised);
            }
        }

        #endregion

        #region Private Methods

        private void OnEventRaised()
        {
            _response?.Invoke();
        }

        #endregion
    }
}
