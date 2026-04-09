using UnityEngine;

namespace FactorySalvage.Gameplay
{
    public enum ZoneType
    {
        Resource,
        Village,
        Defense
    }

    /// <summary>
    /// Marks a zone in the side-scroll world with boundaries and visual.
    /// </summary>
    public class WorldZone : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ZoneType _zoneType;
        [SerializeField] private float _minX = -10f;
        [SerializeField] private float _maxX = 0f;
        [SerializeField] private Color _gizmoColor = Color.green;

        #endregion

        #region Properties

        public ZoneType ZoneType => _zoneType;
        public float MinX => _minX;
        public float MaxX => _maxX;
        public float Width => _maxX - _minX;
        public float CenterX => (_minX + _maxX) * 0.5f;

        #endregion

        #region Public Methods

        public bool ContainsX(float x)
        {
            return x >= _minX && x <= _maxX;
        }

        #endregion

        #region Editor

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            var center = new Vector3(CenterX, 2f, 0f);
            var size = new Vector3(Width, 8f, 0f);
            Gizmos.DrawWireCube(center, size);

            // Label
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(
                new Vector3(CenterX, 6.5f, 0f),
                _zoneType.ToString(),
                new GUIStyle { fontSize = 14, normal = { textColor = _gizmoColor } });
            #endif
        }

        #endregion
    }
}
