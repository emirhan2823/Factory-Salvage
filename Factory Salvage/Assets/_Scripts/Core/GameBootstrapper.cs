using UnityEngine;
using FactorySalvage.UI;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Auto-bootstrapper that runs on every scene load.
    /// Ensures HUD and other critical systems are wired.
    /// No need to manually add to scene — uses RuntimeInitializeOnLoadMethod.
    /// </summary>
    public static class GameBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            EnsureHUD();
        }

        private static void EnsureHUD()
        {
            var hud = Object.FindAnyObjectByType<HUDController>();
            if (hud != null) return;

            var canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null) return;

            // Add HUDAutoSetup — it handles everything
            if (canvas.GetComponent<HUDAutoSetup>() == null)
            {
                canvas.gameObject.AddComponent<HUDAutoSetup>();
                Debug.Log("[Bootstrapper] Added HUDAutoSetup to Canvas");
            }
        }
    }
}
