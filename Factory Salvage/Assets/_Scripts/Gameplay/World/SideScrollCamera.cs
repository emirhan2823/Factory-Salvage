using UnityEngine;
using UnityEngine.InputSystem;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Side-scroll camera: horizontal drag to pan, pinch to zoom.
    /// Fixed Y position, clamped X within world bounds.
    /// </summary>
    public class SideScrollCamera : MonoBehaviour
    {
        #region Fields

        [Header("Bounds")]
        [SerializeField] private float _minX = -15f;
        [SerializeField] private float _maxX = 25f;
        [SerializeField] private float _fixedY = 3f;

        [Header("Drag")]
        [SerializeField] private float _dragSensitivity = 0.02f;
        [SerializeField] private float _smoothSpeed = 8f;

        [Header("Zoom")]
        [SerializeField] private float _minZoom = 4f;
        [SerializeField] private float _maxZoom = 12f;
        [SerializeField] private float _zoomSpeed = 2f;

        private Camera _camera;
        private float _targetX;
        private float _targetZoom;
        private bool _isDragging;
        private Vector2 _lastDragPos;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _targetX = transform.position.x;
            _targetZoom = _camera.orthographicSize;
        }

        private void Update()
        {
            HandleDragInput();
            HandleZoomInput();
            ApplyCamera();
        }

        #endregion

        #region Public Methods

        public void SetBounds(float minX, float maxX)
        {
            _minX = minX;
            _maxX = maxX;
        }

        public void FocusOn(float worldX)
        {
            _targetX = Mathf.Clamp(worldX, _minX, _maxX);
        }

        #endregion

        #region Private Methods

        private void HandleDragInput()
        {
            if (Mouse.current == null) return;

            // Don't drag if over UI
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                _isDragging = false;
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _isDragging = true;
                _lastDragPos = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _isDragging = false;
            }

            if (_isDragging && Mouse.current.leftButton.isPressed)
            {
                var currentPos = Mouse.current.position.ReadValue();
                var delta = currentPos - _lastDragPos;

                // Drag right → camera moves left (natural scroll)
                _targetX -= delta.x * _dragSensitivity * _camera.orthographicSize * 0.1f;
                _targetX = Mathf.Clamp(_targetX, _minX, _maxX);

                _lastDragPos = currentPos;
            }
        }

        private void HandleZoomInput()
        {
            if (Mouse.current != null)
            {
                var scroll = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    _targetZoom -= scroll * 0.01f * _zoomSpeed;
                }
            }

            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        private void ApplyCamera()
        {
            var pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, _targetX, _smoothSpeed * Time.deltaTime);
            pos.y = _fixedY;
            pos.z = -10f;
            transform.position = pos;

            _camera.orthographicSize = Mathf.Lerp(
                _camera.orthographicSize, _targetZoom, _smoothSpeed * Time.deltaTime);
        }

        #endregion
    }
}
