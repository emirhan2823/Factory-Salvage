using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactorySalvage.Core;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Build menu popup: shows available buildings for the tapped slot.
    /// Creates buttons dynamically based on slot zone type.
    /// </summary>
    public class BuildMenuUI : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject _panel;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private TextMeshProUGUI _titleText;

        private BuildingSlot _targetSlot;
        private SlotBuildSystem _buildSystem;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
            if (_panel != null) _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<BuildMenuUI>();
        }

        #endregion

        #region Public Methods

        public void Show(BuildingSlot slot)
        {
            if (slot == null || _panel == null) return;

            _targetSlot = slot;
            if (_buildSystem == null)
                ServiceLocator.TryGet(out _buildSystem);

            if (_buildSystem == null) return;

            // Set title
            if (_titleText != null)
            {
                _titleText.text = slot.Zone == ZoneType.Defense ? "Build Defense" : "Build Structure";
            }

            PopulateButtons();
            _panel.SetActive(true);
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
            _targetSlot = null;
        }

        public bool IsVisible => _panel != null && _panel.activeSelf;

        #endregion

        #region Private Methods

        private void PopulateButtons()
        {
            if (_buttonContainer == null) return;

            // Clear existing
            foreach (Transform child in _buttonContainer)
            {
                Destroy(child.gameObject);
            }

            var buildings = _buildSystem.AvailableBuildings;
            if (buildings == null) return;

            bool isDefense = _targetSlot.Zone == ZoneType.Defense;
            var inventory = FindAnyObjectByType<Inventory>();

            foreach (var building in buildings)
            {
                if (building == null) continue;

                bool isDefenseBuilding = building.Category == BuildingCategory.Defense;
                if (isDefense != isDefenseBuilding) continue;

                CreateBuildButton(building, inventory);
            }

            // Close button
            CreateCloseButton();
        }

        private void CreateBuildButton(BuildingDefinition building, Inventory inventory)
        {
            var go = new GameObject(building.BuildingName, typeof(RectTransform));
            go.transform.SetParent(_buttonContainer, false);

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280f, 60f);

            // Background
            var img = go.AddComponent<Image>();
            bool canAfford = inventory == null || inventory.HasEnoughResources(building.BuildCost);
            img.color = canAfford ? new Color(0.2f, 0.35f, 0.2f, 0.9f) : new Color(0.35f, 0.2f, 0.2f, 0.9f);

            // Button
            var btn = go.AddComponent<Button>();
            btn.interactable = canAfford;
            var def = building;
            btn.onClick.AddListener(() => OnBuildSelected(def));

            // Text
            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(10f, 0f);

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            var sb = new System.Text.StringBuilder();
            sb.Append(building.BuildingName);

            if (building.BuildCost != null && building.BuildCost.Length > 0)
            {
                sb.Append("  (");
                for (int i = 0; i < building.BuildCost.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(building.BuildCost[i].Amount).Append(" ").Append(building.BuildCost[i].Resource.ResourceName);
                }
                sb.Append(")");
            }
            else
            {
                sb.Append("  (Free)");
            }

            if (building.IsProducer && building.PassiveOutput.Length > 0)
            {
                sb.Append("\n+").Append(building.PassiveOutput[0].Amount).Append(" ")
                  .Append(building.PassiveOutput[0].Resource.ResourceName).Append("/s");
            }

            tmp.text = sb.ToString();
            tmp.fontSize = 16;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = canAfford ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }

        private void CreateCloseButton()
        {
            var go = new GameObject("CloseBtn", typeof(RectTransform));
            go.transform.SetParent(_buttonContainer, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(280f, 40f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.4f, 0.2f, 0.2f, 0.9f);

            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(Hide);

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Cancel";
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private void OnBuildSelected(BuildingDefinition building)
        {
            if (_targetSlot == null || _buildSystem == null) return;

            _buildSystem.TryBuild(building, _targetSlot);
            Hide();
        }

        #endregion
    }
}
