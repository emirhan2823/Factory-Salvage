using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Top-down 2D camera. Follows a target with smooth lerp.
    /// Supports pinch-to-zoom on mobile and scroll wheel on desktop.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        #region Fields

        [Header("Follow")]
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private Vector3 _offset = new(0f, 0f, -10f);

        [Header("Bounds")]
        [SerializeField] private Vector2 _minBounds = new(-12f, -12f);
        [SerializeField] private Vector2 _maxBounds = new(12f, 12f);

        [Header("Zoom")]
        [SerializeField] private float _minZoom = 3f;
        [SerializeField] private float _maxZoom = 12f;
        [SerializeField] private float _zoomSpeed = 2f;

        private Camera _camera;
        private float _targetZoom;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _targetZoom = _camera.orthographicSize;
        }

        private void LateUpdate()
        {
            FollowTarget();
            HandleZoom();
        }

        #endregion

        #region Public Methods

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        #endregion

        #region Private Methods

        private void FollowTarget()
        {
            if (_target == null) return;

            var desiredPos = _target.position + _offset;

            desiredPos.x = Mathf.Clamp(desiredPos.x, _minBounds.x, _maxBounds.x);
            desiredPos.y = Mathf.Clamp(desiredPos.y, _minBounds.y, _maxBounds.y);
            desiredPos.z = _offset.z;

            transform.position = Vector3.Lerp(transform.position, desiredPos, _smoothSpeed * Time.deltaTime);
        }

        private void HandleZoom()
        {
            // Mobile pinch-to-zoom
            if (Input.touchCount == 2)
            {
                var touch0 = Input.GetTouch(0);
                var touch1 = Input.GetTouch(1);

                var prevMagnitude = (touch0.position - touch0.deltaPosition -
                                     (touch1.position - touch1.deltaPosition)).magnitude;
                var currentMagnitude = (touch0.position - touch1.position).magnitude;

                var diff = prevMagnitude - currentMagnitude;
                _targetZoom += diff * 0.01f * _zoomSpeed;
            }
            // Desktop scroll wheel
            else
            {
                var scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    _targetZoom -= scroll * _zoomSpeed;
                }
            }

            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, _smoothSpeed * Time.deltaTime);
        }

        #endregion
    }
}
