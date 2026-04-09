using UnityEngine;
using TMPro;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Simple HUD that polls inventory every frame.
    /// Finds ANY Inventory in scene (works with both player-based and standalone).
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
                _inventory = FindAnyObjectByType<Inventory>();
                return;
            }

            if (_resourceText == null)
            {
                _resourceText = FindAnyObjectByType<TextMeshProUGUI>();
                return;
            }

            UpdateDisplay();
        }

        #endregion

        #region Private Methods

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
                _sb.Append("Build a Lumber Mill to start!");
            }

            _resourceText.SetText(_sb);
        }

        #endregion
    }
}
