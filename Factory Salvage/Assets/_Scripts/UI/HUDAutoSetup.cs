using UnityEngine;
using TMPro;
using FactorySalvage.Gameplay;

namespace FactorySalvage.UI
{
    /// <summary>
    /// Ensures HUDController exists and is wired at runtime.
    /// Works by finding Canvas → ResourceText, then adding HUDController.
    /// </summary>
    public class HUDAutoSetup : MonoBehaviour
    {
        private void Start()
        {
            var existing = FindAnyObjectByType<HUDController>();
            if (existing != null) return;

            // Find ResourceText in Canvas
            var canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null) return;

            TextMeshProUGUI targetText = null;

            // Search all TMP texts in canvas for "ResourceText" named object
            var allTexts = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in allTexts)
            {
                if (t.gameObject.name == "ResourceText")
                {
                    targetText = t;
                    break;
                }
            }

            // Fallback: use first TMP text found
            if (targetText == null && allTexts.Length > 0)
            {
                targetText = allTexts[0];
            }

            if (targetText == null) return;

            // Add HUDController to the text's parent (or canvas)
            var hudGo = targetText.transform.parent != null ? targetText.transform.parent.gameObject : canvas.gameObject;
            var hud = hudGo.AddComponent<HUDController>();

            // Wire reference
            var field = typeof(HUDController).GetField("_resourceText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(hud, targetText);
            }

            Debug.Log("[HUDAutoSetup] HUDController wired to ResourceText");
        }
    }
}
