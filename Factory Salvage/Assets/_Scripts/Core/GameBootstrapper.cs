using UnityEngine;
using UnityEngine.Tilemaps;
using FactorySalvage.Gameplay;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Runtime bootstrapper: creates placeholder sprites, sets up the game scene.
    /// Attach to a GameObject in the scene and press Play.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        #region Fields

        [Header("Auto Setup")]
        [SerializeField] private bool _autoSetup = true;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (_autoSetup)
            {
                SetupScene();
            }
        }

        #endregion

        #region Public Methods

        public void SetupScene()
        {
            Debug.Log("[Bootstrapper] Setting up game scene...");

            // The scene should already have Grid, Tilemaps, Camera etc.
            // This just validates everything is connected
            ValidateScene();

            Debug.Log("[Bootstrapper] Scene setup complete!");
        }

        #endregion

        #region Private Methods

        private void ValidateScene()
        {
            // Check for required components
            var grid = FindAnyObjectByType<GridManager>();
            if (grid == null) Debug.LogWarning("[Bootstrapper] No GridManager found!");

            var camera = FindAnyObjectByType<CameraController>();
            if (camera == null) Debug.LogWarning("[Bootstrapper] No CameraController found!");

            var player = FindAnyObjectByType<PlayerController>();
            if (player == null) Debug.LogWarning("[Bootstrapper] No PlayerController found!");

            var buildSystem = FindAnyObjectByType<BuildSystem>();
            if (buildSystem == null) Debug.LogWarning("[Bootstrapper] No BuildSystem found!");
        }

        #endregion
    }
}
