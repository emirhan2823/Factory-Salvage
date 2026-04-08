using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Core;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Displays resource counts, energy bar, wave counter, base health.
    /// Event-driven — no polling in Update.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        #region Fields

        [Header("Resources")]
        [SerializeField] private TextMeshProUGUI _resourceText;
        [SerializeField] private Inventory _playerInventory;
        [SerializeField] private ResourceDefinition[] _displayResources;

        [Header("Energy")]
        [SerializeField] private Slider _energySlider;
        [SerializeField] private FloatVariable _currentEnergy;
        [SerializeField] private FloatVariable _maxEnergy;

        [Header("Wave")]
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private IntVariable _currentWave;
        [SerializeField] private IntVariable _enemiesRemaining;

        [Header("Base Health")]
        [SerializeField] private Slider _baseHealthSlider;
        [SerializeField] private Health _baseHealth;

        [Header("Events")]
        [SerializeField] private GameEvent _onInventoryChanged;

        // Cached StringBuilder to avoid GC
        private readonly System.Text.StringBuilder _sb = new(128);

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (_onInventoryChanged != null) _onInventoryChanged.Register(UpdateResourceDisplay);
            if (_currentEnergy != null) _currentEnergy.OnValueChanged += UpdateEnergyDisplay;
            if (_currentWave != null) _currentWave.OnValueChanged += UpdateWaveDisplay;
            if (_enemiesRemaining != null) _enemiesRemaining.OnValueChanged += UpdateEnemiesDisplay;
            if (_baseHealth != null) _baseHealth.OnDamaged += OnBaseHealthChanged;

            RefreshAll();
        }

        private void OnDisable()
        {
            if (_onInventoryChanged != null) _onInventoryChanged.Unregister(UpdateResourceDisplay);
            if (_currentEnergy != null) _currentEnergy.OnValueChanged -= UpdateEnergyDisplay;
            if (_currentWave != null) _currentWave.OnValueChanged -= UpdateWaveDisplay;
            if (_enemiesRemaining != null) _enemiesRemaining.OnValueChanged -= UpdateEnemiesDisplay;
            if (_baseHealth != null) _baseHealth.OnDamaged -= OnBaseHealthChanged;
        }

        #endregion

        #region Private Methods

        private void RefreshAll()
        {
            UpdateResourceDisplay();
            UpdateEnergyDisplay(_currentEnergy != null ? _currentEnergy.Value : 0f);
            UpdateWaveDisplay(_currentWave != null ? _currentWave.Value : 0);
            UpdateBaseHealthDisplay();
        }

        private void UpdateResourceDisplay()
        {
            if (_resourceText == null || _playerInventory == null || _displayResources == null) return;

            _sb.Clear();
            foreach (var res in _displayResources)
            {
                if (res == null) continue;
                int amount = _playerInventory.GetAmount(res);
                _sb.Append(res.ResourceName).Append(": ").Append(amount).Append("  ");
            }

            _resourceText.SetText(_sb);
        }

        private void UpdateEnergyDisplay(float value)
        {
            if (_energySlider == null) return;
            float max = _maxEnergy != null ? _maxEnergy.Value : 1f;
            _energySlider.maxValue = max;
            _energySlider.value = Mathf.Max(0f, value);
        }

        private void UpdateWaveDisplay(int wave)
        {
            if (_waveText == null) return;
            _sb.Clear();
            _sb.Append("Wave ").Append(wave);
            _waveText.SetText(_sb);
        }

        private void UpdateEnemiesDisplay(int count)
        {
            // Could update a separate enemies remaining text
        }

        private void OnBaseHealthChanged(float damage)
        {
            UpdateBaseHealthDisplay();
        }

        private void UpdateBaseHealthDisplay()
        {
            if (_baseHealthSlider == null || _baseHealth == null) return;
            _baseHealthSlider.maxValue = _baseHealth.MaxHealth;
            _baseHealthSlider.value = _baseHealth.CurrentHealth;
        }

        #endregion
    }
}
