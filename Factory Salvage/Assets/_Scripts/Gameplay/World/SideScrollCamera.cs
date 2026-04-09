using UnityEngine;
using UnityEngine.InputSystem;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Side-scroll camera: drag to pan horizontally AND vertically.
    /// Scroll up = surface (village/defense), scroll down = underground layers.
    /// </summary>
    public class SideScrollCamera : MonoBehaviour
    {
        #region Fields

        [Header("Horizontal Bounds")]
        [SerializeField] private float _minX = -15f;
        [SerializeField] private float _maxX = 25f;

        [Header("Vertical Bounds")]
        [SerializeField] private float _minY = -25f;
        [SerializeField] private float _maxY = 5f;
        [SerializeField] private float _defaultY = 3f;

        [Header("Drag")]
        [SerializeField] private float _dragSensitivity = 0.02f;
        [SerializeField] private float _smoothSpeed = 8f;

        [Header("Zoom")]
        [SerializeField] private float _minZoom = 4f;
        [SerializeField] private float _maxZoom = 12f;
        [SerializeField] private float _zoomSpeed = 2f;

        private Camera _camera;
        private float _targetX;
        private float _targetY;
        private float _targetZoom;
        private bool _isDragging;
        private Vector2 _lastDragPos;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _targetX = transform.position.x;
            _targetY = _defaultY;
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

        public void SetBounds(float minX, float maxX, float minY, float maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public void FocusOn(float worldX, float worldY)
        {
            _targetX = Mathf.Clamp(worldX, _minX, _maxX);
            _targetY = Mathf.Clamp(worldY, _minY, _maxY);
        }

        public void GoToSurface()
        {
            _targetY = _defaultY;
        }

        public void GoToLayer(float layerY)
        {
            _targetY = Mathf.Clamp(layerY, _minY, _maxY);
        }

        #endregion

        #region Private Methods

        private void HandleDragInput()
        {
            if (Mouse.current == null) return;

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
                float zoomFactor = _camera.orthographicSize * 0.1f;

                // Horizontal drag
                _targetX -= delta.x * _dragSensitivity * zoomFactor;
                _targetX = Mathf.Clamp(_targetX, _minX, _maxX);

                // Vertical drag
                _targetY -= delta.y * _dragSensitivity * zoomFactor;
                _targetY = Mathf.Clamp(_targetY, _minY, _maxY);

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
            pos.y = Mathf.Lerp(pos.y, _targetY, _smoothSpeed * Time.deltaTime);
            pos.z = -10f;
            transform.position = pos;

            _camera.orthographicSize = Mathf.Lerp(
                _camera.orthographicSize, _targetZoom, _smoothSpeed * Time.deltaTime);
        }

        #endregion
    }
}
