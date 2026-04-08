using UnityEngine;
using UnityEngine.InputSystem;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Player movement via tap-to-move or virtual joystick.
    /// Uses New Input System.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _stoppingDistance = 0.1f;

        [Header("References")]
        [SerializeField] private GridManager _gridManager;

        private Rigidbody2D _rb;
        private Camera _mainCamera;
        private Vector3 _moveTarget;
        private bool _isMoving;

        #endregion

        #region Properties

        public bool IsMoving => _isMoving;
        public GridManager GridManager => _gridManager;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _mainCamera = Camera.main;
            _moveTarget = transform.position;
        }

        private void Update()
        {
            HandleTapInput();
        }

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        #endregion

        #region Public Methods

        public void SetGridManager(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public void MoveTo(Vector3 worldPosition)
        {
            _moveTarget = worldPosition;
            _moveTarget.z = 0f;
            _isMoving = true;
        }

        public void Stop()
        {
            _isMoving = false;
            _rb.linearVelocity = Vector2.zero;
        }

        #endregion

        #region Private Methods

        private void HandleTapInput()
        {
            // Mouse/touch tap
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                var screenPos = Mouse.current.position.ReadValue();

                // Don't move if tapping on UI
                if (UnityEngine.EventSystems.EventSystem.current != null &&
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
                worldPos.z = 0f;

                // Check if target cell is walkable
                if (_gridManager != null)
                {
                    var cellPos = _gridManager.WorldToCell(worldPos);
                    if (!_gridManager.IsCellWalkable(cellPos))
                    {
                        return;
                    }
                }

                MoveTo(worldPos);
            }
        }

        private void MoveToTarget()
        {
            if (!_isMoving) return;

            var direction = (_moveTarget - transform.position);
            direction.z = 0f;

            if (direction.magnitude <= _stoppingDistance)
            {
                Stop();
                return;
            }

            var velocity = direction.normalized * _moveSpeed;
            _rb.linearVelocity = velocity;
        }

        #endregion
    }
}
