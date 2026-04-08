using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Visual line between connected machines with animated dot showing flow.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ConveyorVisual : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _dotSpeed = 2f;
        [SerializeField] private Color _lineColor = new(0.5f, 0.8f, 1f, 0.6f);
        [SerializeField] private float _lineWidth = 0.05f;

        private Transform _source;
        private Transform _destination;
        private LineRenderer _lineRenderer;
        private float _dotProgress;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            SetupLineRenderer();
        }

        private void Update()
        {
            if (_source == null || _destination == null) return;

            UpdateLine();
            AnimateDot();
        }

        #endregion

        #region Public Methods

        public void Initialize(Transform source, Transform destination)
        {
            _source = source;
            _destination = destination;
        }

        #endregion

        #region Private Methods

        private void SetupLineRenderer()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
            _lineRenderer.startColor = _lineColor;
            _lineRenderer.endColor = _lineColor;
            _lineRenderer.sortingOrder = 5;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void UpdateLine()
        {
            _lineRenderer.SetPosition(0, _source.position);
            _lineRenderer.SetPosition(1, _destination.position);
        }

        private void AnimateDot()
        {
            _dotProgress += Time.deltaTime * _dotSpeed;
            if (_dotProgress > 1f)
            {
                _dotProgress -= 1f;
            }
        }

        #endregion
    }
}
