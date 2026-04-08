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
    /// Auto-finds references if not set in Inspector.
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
        private float _refreshTimer;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            AutoFindReferences();

            if (_onInventoryChanged != null) _onInventoryChanged.Register(UpdateResourceDisplay);

            RefreshAll();
        }

        private void OnDisable()
        {
            if (_onInventoryChanged != null) _onInventoryChanged.Unregister(UpdateResourceDisplay);
        }

        private void Update()
        {
            // Fallback: refresh every 0.5s in case events aren't wired
            _refreshTimer -= Time.deltaTime;
            if (_refreshTimer <= 0f)
            {
                _refreshTimer = 0.5f;
                UpdateResourceDisplay();
            }
        }

        #endregion

        #region Private Methods

        private void AutoFindReferences()
        {
            // Auto-find inventory
            if (_playerInventory == null)
            {
                var player = FindAnyObjectByType<PlayerController>();
                if (player != null)
                {
                    _playerInventory = player.GetComponent<Inventory>();
                }
            }

            // Auto-find resource text
            if (_resourceText == null)
            {
                _resourceText = GetComponentInChildren<TextMeshProUGUI>();
            }

            // Auto-find resource definitions
            if (_displayResources == null || _displayResources.Length == 0)
            {
                _displayResources = UnityEngine.Resources.FindObjectsOfTypeAll<ResourceDefinition>();
            }
        }

        private void RefreshAll()
        {
            UpdateResourceDisplay();
        }

        private void UpdateResourceDisplay()
        {
            if (_resourceText == null || _playerInventory == null) return;

            _sb.Clear();

            // Get all resources directly from inventory
            var allResources = _playerInventory.GetAllResources();
            if (allResources.Count > 0)
            {
                foreach (var kvp in allResources)
                {
                    _sb.Append(kvp.Key.ResourceName).Append(": ").Append(kvp.Value).Append("  ");
                }
            }
            else
            {
                _sb.Append("No resources yet — tap scrap piles to collect!");
            }

            _resourceText.SetText(_sb);
        }

        #endregion
    }
}
