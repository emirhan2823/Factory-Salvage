using UnityEngine;
using TMPro;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Simple HUD that polls inventory every frame.
    /// No event wiring needed — just finds Player and reads inventory.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TextMeshProUGUI _resourceText;

        private Inventory _inventory;
        private readonly System.Text.StringBuilder _sb = new(128);

        #endregion

        #region Unity Callbacks

        private void Update()
        {
            if (_inventory == null)
            {
                FindInventory();
                return;
            }

            if (_resourceText == null)
            {
                _resourceText = GetComponentInChildren<TextMeshProUGUI>();
                return;
            }

            UpdateDisplay();
        }

        #endregion

        #region Private Methods

        private void FindInventory()
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                _inventory = player.GetComponent<Inventory>();
            }
        }

        private void UpdateDisplay()
        {
            var resources = _inventory.GetAllResources();

            _sb.Clear();
            if (resources.Count > 0)
            {
                foreach (var kvp in resources)
                {
                    _sb.Append(kvp.Key.ResourceName).Append(": ").Append(kvp.Value).Append("  ");
                }
            }
            else
            {
                _sb.Append("Tap scrap piles to collect!");
            }

            _resourceText.SetText(_sb);
        }

        #endregion
    }
}
