using UnityEngine;
using TMPro;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Auto-attaches HUDController at runtime if missing.
    /// Put this on the Canvas or any always-active GameObject.
    /// Runs once on Start, then disables itself.
    /// </summary>
    public class HUDAutoSetup : MonoBehaviour
    {
        #region Unity Callbacks

        private void Start()
        {
            EnsureHUDController();
        }

        #endregion

        #region Private Methods

        private void EnsureHUDController()
        {
            // Already exists?
            var existing = FindAnyObjectByType<HUDController>();
            if (existing != null) return;

            // Find Canvas → HUD panel
            var canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null) return;

            var hudTransform = canvas.transform.Find("HUD");
            if (hudTransform == null)
            {
                // Create HUD panel
                var hudGo = new GameObject("HUD", typeof(RectTransform));
                hudGo.transform.SetParent(canvas.transform, false);
                var rect = hudGo.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
                hudTransform = hudGo.transform;
            }

            // Add HUDController
            var controller = hudTransform.gameObject.AddComponent<HUDController>();

            // Find or create ResourceText
            var textTransform = hudTransform.Find("ResourceText");
            TextMeshProUGUI tmp;

            if (textTransform != null)
            {
                tmp = textTransform.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                var textGo = new GameObject("ResourceText", typeof(RectTransform));
                textGo.transform.SetParent(hudTransform, false);
                var textRect = textGo.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.5f, 1f);
                textRect.anchorMax = new Vector2(0.5f, 1f);
                textRect.anchoredPosition = new Vector2(0f, -40f);
                textRect.sizeDelta = new Vector2(800f, 50f);

                tmp = textGo.AddComponent<TextMeshProUGUI>();
                tmp.fontSize = 22;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;
            }

            // Wire the reference using reflection (since field is private)
            var field = typeof(HUDController).GetField("_resourceText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(controller, tmp);
            }

            Debug.Log("[HUDAutoSetup] HUDController created and wired at runtime!");
        }

        #endregion
    }
}
