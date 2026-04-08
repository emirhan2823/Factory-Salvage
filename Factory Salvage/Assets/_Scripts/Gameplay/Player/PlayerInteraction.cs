using UnityEngine;
using UnityEngine.InputSystem;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Handles player interaction with IInteractable objects.
    /// Tap on an interactable: if in range, interact immediately.
    /// If out of range, move to it then interact on arrival.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _interactRange = 2f;
        [SerializeField] private PlayerController _playerController;

        private Camera _mainCamera;
        private IInteractable _pendingInteraction;
        private Transform _pendingTarget;

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
            CheckPendingInteraction();
        }

        #endregion

        #region Private Methods

        private void HandleInteractionInput()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            // Check for interactable at tap position
            var hit = Physics2D.OverlapPoint(worldPos);
            if (hit == null)
            {
                _pendingInteraction = null;
                _pendingTarget = null;
                return;
            }

            var interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
            {
                _pendingInteraction = null;
                _pendingTarget = null;
                return;
            }

            var distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance <= _interactRange)
            {
                // In range — interact now
                _playerController.Stop();
                interactable.Interact(_playerController);
                _pendingInteraction = null;
                _pendingTarget = null;
            }
            else
            {
                // Out of range — move toward it, interact on arrival
                _pendingInteraction = interactable;
                _pendingTarget = hit.transform;
                _playerController.MoveTo(hit.transform.position);
            }
        }

        private void CheckPendingInteraction()
        {
            if (_pendingInteraction == null || _pendingTarget == null) return;

            var distance = Vector2.Distance(transform.position, _pendingTarget.position);
            if (distance <= _interactRange)
            {
                _playerController.Stop();
                _pendingInteraction.Interact(_playerController);
                _pendingInteraction = null;
                _pendingTarget = null;
            }
        }

        #endregion
    }
}
