using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Shows available machines as buttons. Tap to enter build mode.
    /// </summary>
    public class BuildMenuPanel : MonoBehaviour
    {
        #region Fields

        [SerializeField] private BuildSystem _buildSystem;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private GameObject _buttonPrefab;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            PopulateButtons();
        }

        #endregion

        #region Private Methods

        private void PopulateButtons()
        {
            if (_buildSystem == null || _buttonContainer == null || _buttonPrefab == null) return;

            // Clear existing buttons
            foreach (Transform child in _buttonContainer)
            {
                Destroy(child.gameObject);
            }

            var machines = _buildSystem.AvailableMachines;
            if (machines == null) return;

            foreach (var machine in machines)
            {
                if (machine == null) continue;

                var buttonObj = Instantiate(_buttonPrefab, _buttonContainer);
                var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = machine.MachineName;
                }

                var button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    var machineDef = machine; // capture for closure
                    button.onClick.AddListener(() => OnMachineSelected(machineDef));
                }
            }
        }

        private void OnMachineSelected(MachineDefinition machine)
        {
            if (_buildSystem != null)
            {
                _buildSystem.EnterBuildMode(machine);
            }

            // Close panel
            if (Core.ServiceLocator.TryGet<UIManager>(out var uiManager))
            {
                uiManager.CloseActivePanel();
            }
        }

        #endregion
    }
}
