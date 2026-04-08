using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Core;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Shows wave number, enemies remaining, start wave button.
    /// </summary>
    public class WaveInfoPanel : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TextMeshProUGUI _waveNumberText;
        [SerializeField] private TextMeshProUGUI _enemiesText;
        [SerializeField] private Button _startWaveButton;

        [SerializeField] private IntVariable _currentWave;
        [SerializeField] private IntVariable _enemiesRemaining;

        private readonly System.Text.StringBuilder _sb = new(64);

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (_startWaveButton != null)
            {
                _startWaveButton.onClick.AddListener(OnStartWaveClicked);
            }

            if (_currentWave != null) _currentWave.OnValueChanged += UpdateWaveNumber;
            if (_enemiesRemaining != null) _enemiesRemaining.OnValueChanged += UpdateEnemiesCount;

            UpdateWaveNumber(_currentWave != null ? _currentWave.Value : 0);
            UpdateEnemiesCount(_enemiesRemaining != null ? _enemiesRemaining.Value : 0);
        }

        private void OnDisable()
        {
            if (_startWaveButton != null)
            {
                _startWaveButton.onClick.RemoveListener(OnStartWaveClicked);
            }

            if (_currentWave != null) _currentWave.OnValueChanged -= UpdateWaveNumber;
            if (_enemiesRemaining != null) _enemiesRemaining.OnValueChanged -= UpdateEnemiesCount;
        }

        #endregion

        #region Private Methods

        private void OnStartWaveClicked()
        {
            if (ServiceLocator.TryGet<WaveManager>(out var waveManager))
            {
                waveManager.StartNextWave();
            }
        }

        private void UpdateWaveNumber(int wave)
        {
            if (_waveNumberText == null) return;
            _sb.Clear();
            _sb.Append("Wave ").Append(wave);
            _waveNumberText.SetText(_sb);
        }

        private void UpdateEnemiesCount(int count)
        {
            if (_enemiesText == null) return;
            _sb.Clear();
            _sb.Append("Enemies: ").Append(count);
            _enemiesText.SetText(_sb);
        }

        #endregion
    }
}
