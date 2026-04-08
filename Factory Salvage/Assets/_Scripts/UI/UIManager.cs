using UnityEngine;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Manages UI panels. Only one panel open at a time (besides HUD).
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private GameObject _buildMenuPanel;
        [SerializeField] private GameObject _waveInfoPanel;
        [SerializeField] private GameObject _upgradePanel;

        private GameObject _activePanel;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            Core.ServiceLocator.Register(this);
            CloseAllPanels();
        }

        private void OnDestroy()
        {
            Core.ServiceLocator.Unregister<UIManager>();
        }

        private void Update()
        {
            // Escape closes active panel
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (_activePanel != null)
                {
                    CloseActivePanel();
                }
            }
        }

        #endregion

        #region Public Methods

        public void ToggleInventory() => TogglePanel(_inventoryPanel);
        public void ToggleBuildMenu() => TogglePanel(_buildMenuPanel);
        public void ToggleWaveInfo() => TogglePanel(_waveInfoPanel);
        public void ToggleUpgradePanel() => TogglePanel(_upgradePanel);

        public void OpenPanel(GameObject panel)
        {
            if (panel == null) return;
            if (_activePanel == panel) return;

            CloseActivePanel();
            panel.SetActive(true);
            _activePanel = panel;
        }

        public void CloseActivePanel()
        {
            if (_activePanel != null)
            {
                _activePanel.SetActive(false);
                _activePanel = null;
            }
        }

        public void CloseAllPanels()
        {
            SetPanelActive(_inventoryPanel, false);
            SetPanelActive(_buildMenuPanel, false);
            SetPanelActive(_waveInfoPanel, false);
            SetPanelActive(_upgradePanel, false);
            _activePanel = null;
        }

        public bool IsAnyPanelOpen => _activePanel != null;

        #endregion

        #region Private Methods

        private void TogglePanel(GameObject panel)
        {
            if (panel == null) return;

            if (_activePanel == panel)
            {
                CloseActivePanel();
            }
            else
            {
                OpenPanel(panel);
            }
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null) panel.SetActive(active);
        }

        #endregion
    }
}
