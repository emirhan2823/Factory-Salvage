using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Core;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Shows available upgrades with purchase buttons.
    /// </summary>
    public class UpgradePanel : MonoBehaviour
    {
        #region Fields

        [SerializeField] private UpgradeDefinition[] _availableUpgrades;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private GameObject _buttonPrefab;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            PopulateUpgrades();
        }

        #endregion

        #region Private Methods

        private void PopulateUpgrades()
        {
            if (_buttonContainer == null || _buttonPrefab == null) return;

            foreach (Transform child in _buttonContainer)
            {
                Destroy(child.gameObject);
            }

            if (_availableUpgrades == null) return;

            if (!ServiceLocator.TryGet<UpgradeManager>(out var upgradeManager)) return;

            foreach (var upgrade in _availableUpgrades)
            {
                if (upgrade == null) continue;

                var buttonObj = Instantiate(_buttonPrefab, _buttonContainer);
                var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    int level = upgradeManager.GetLevel(upgrade);
                    text.text = $"{upgrade.UpgradeName} (Lv.{level}/{upgrade.MaxLevel})";
                }

                var button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    var upgradeDef = upgrade;
                    button.onClick.AddListener(() => OnUpgradeClicked(upgradeDef));
                    button.interactable = upgradeManager.CanPurchase(upgrade);
                }
            }
        }

        private void OnUpgradeClicked(UpgradeDefinition upgrade)
        {
            if (!ServiceLocator.TryGet<UpgradeManager>(out var upgradeManager)) return;

            if (upgradeManager.Purchase(upgrade))
            {
                PopulateUpgrades(); // Refresh
            }
        }

        #endregion
    }
}
