using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Audio
{
    /// <summary>
    /// Plays an SFX clip when a GameEvent is raised.
    /// Attach to any object that should trigger sound on an event.
    /// </summary>
    public class SFXTrigger : MonoBehaviour
    {
        #region Fields

        [SerializeField] private AudioClip _clip;
        [SerializeField] private GameEvent _triggerEvent;
        [SerializeField] private bool _usePosition;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (_triggerEvent != null)
            {
                _triggerEvent.Register(OnTriggered);
            }
        }

        private void OnDisable()
        {
            if (_triggerEvent != null)
            {
                _triggerEvent.Unregister(OnTriggered);
            }
        }

        #endregion

        #region Private Methods

        private void OnTriggered()
        {
            if (!ServiceLocator.TryGet<AudioManager>(out var audioManager)) return;

            if (_usePosition)
            {
                audioManager.PlaySFX(_clip, transform.position);
            }
            else
            {
                audioManager.PlaySFX(_clip);
            }
        }

        #endregion
    }
}
