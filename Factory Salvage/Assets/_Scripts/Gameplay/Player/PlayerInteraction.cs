using UnityEngine;
using UnityEngine.InputSystem;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Handles player interaction with IInteractable objects.
    /// Tap on an interactable within range to interact.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _interactRange = 1.5f;
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private PlayerController _playerController;

        private Camera _mainCamera;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_playerController == null)
            {
                _playerController = GetComponent<PlayerController>();
            }
        }

        private void Update()
        {
            HandleInteractionInput();
        }

        #endregion

        #region Private Methods

        private void HandleInteractionInput()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
                return;

            // Don't interact if tapping on UI
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            // Check for interactable at tap position
            var hit = Physics2D.OverlapPoint(worldPos, _interactableLayer);
            if (hit == null) return;

            var interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) return;

            // Check range
            var distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance > _interactRange)
            {
                return;
            }

            // Stop moving and interact
            _playerController.Stop();
            interactable.Interact(_playerController);
        }

        #endregion
    }
}
