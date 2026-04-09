using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Core;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Popup showing building info: name, level, production, upgrade button.
    /// </summary>
    public class BuildingInfoUI : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeButtonText;

        private BuildingBase _selectedBuilding;
        private readonly System.Text.StringBuilder _sb = new(256);

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
            if (_panel != null) _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<BuildingInfoUI>();
        }

        private void Update()
        {
            if (_panel != null && _panel.activeSelf && _selectedBuilding != null)
            {
                UpdateInfo();
            }
        }

        #endregion

        #region Public Methods

        public void Show(BuildingBase building)
        {
            if (building == null || _panel == null) return;
            _selectedBuilding = building;

            if (_upgradeButton != null)
            {
                _upgradeButton.onClick.RemoveAllListeners();
                _upgradeButton.onClick.AddListener(OnUpgradeClicked);
            }

            UpdateInfo();
            _panel.SetActive(true);
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
            _selectedBuilding = null;
        }

        public bool IsVisible => _panel != null && _panel.activeSelf;

        #endregion

        #region Private Methods

        private void UpdateInfo()
        {
            if (_infoText == null || _selectedBuilding == null) return;

            var def = _selectedBuilding.Definition;
            if (def == null) return;

            var inventory = FindAnyObjectByType<Inventory>();

            _sb.Clear();
            _sb.Append(def.BuildingName).Append("  Lv.").Append(_selectedBuilding.Level).Append("\n");

            // Production info
            if (def.IsProducer && def.PassiveOutput != null)
            {
                float mult = _selectedBuilding.LevelMultiplier;
                foreach (var output in def.PassiveOutput)
                {
                    int amount = Mathf.Max(1, Mathf.FloorToInt(output.Amount * mult));
                    _sb.Append("+").Append(amount).Append(" ").Append(output.Resource.ResourceName)
                       .Append(" / ").Append(def.ProductionInterval.ToString("F1")).Append("s\n");
                }
            }

            if (def.IsCrafter && def.Recipe != null)
            {
                _sb.Append("Recipe: ");
                foreach (var input in def.Recipe.Inputs)
                {
                    _sb.Append(input.Amount).Append(" ").Append(input.Resource.ResourceName).Append(" ");
                }
                _sb.Append("-> ");
                foreach (var output in def.Recipe.Outputs)
                {
                    _sb.Append(output.Amount).Append(" ").Append(output.Resource.ResourceName);
                }
                _sb.Append("\n");
            }

            // Upgrade cost
            if (def.BuildCost != null && _selectedBuilding.Level < def.MaxLevel)
            {
                _sb.Append("\nUpgrade cost: ");
                foreach (var cost in def.BuildCost)
                {
                    int scaled = Mathf.CeilToInt(cost.Amount * Mathf.Pow(def.UpgradeCostMultiplier, _selectedBuilding.Level));
                    _sb.Append(scaled).Append(" ").Append(cost.Resource.ResourceName).Append("  ");
                }
            }

            _infoText.SetText(_sb);

            // Upgrade button
            if (_upgradeButton != null)
            {
                bool canUpgrade = _selectedBuilding.CanLevelUp(inventory);
                _upgradeButton.interactable = canUpgrade;
                if (_upgradeButtonText != null)
                {
                    _upgradeButtonText.text = canUpgrade ? "UPGRADE" : "Need Resources";
                }
            }
        }

        private void OnUpgradeClicked()
        {
            if (_selectedBuilding == null) return;
            var inventory = FindAnyObjectByType<Inventory>();
            if (_selectedBuilding.LevelUp(inventory))
            {
                Debug.Log($"[Upgrade] {_selectedBuilding.Definition.BuildingName} -> Lv.{_selectedBuilding.Level}");
                UpdateInfo();
            }
        }

        #endregion
    }
}
