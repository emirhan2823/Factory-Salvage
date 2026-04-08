using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Audio
{
    /// <summary>
    /// Centralized audio manager. SFX via pool, music via dedicated source.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Fields

        [Header("Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Settings")]
        [SerializeField] private float _musicVolume = 0.5f;
        [SerializeField] private float _sfxVolume = 1f;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);

            if (_musicSource != null)
            {
                _musicSource.loop = true;
                _musicSource.volume = _musicVolume;
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AudioManager>();
        }

        #endregion

        #region Public Methods

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume);
        }

        public void PlaySFX(AudioClip clip, Vector3 position)
        {
            if (clip == null) return;
            AudioSource.PlayClipAtPoint(clip, position, _sfxVolume);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || _musicSource == null) return;
            _musicSource.clip = clip;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            if (_musicSource != null) _musicSource.Stop();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            if (_musicSource != null) _musicSource.volume = _musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        #endregion
    }
}
