using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A slot where a building can be placed. Shows empty indicator when unoccupied.
    /// </summary>
    public class BuildingSlot : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ZoneType _zone;
        [SerializeField] private int _slotIndex;
        [SerializeField] private SpriteRenderer _indicator;

        private GameObject _occupant;
        private bool _isOccupied;

        private static readonly Color EmptyColor = new(1f, 1f, 1f, 0.2f);
        private static readonly Color HoverColor = new(0f, 1f, 0f, 0.4f);

        #endregion

        #region Properties

        public ZoneType Zone => _zone;
        public int SlotIndex => _slotIndex;
        public bool IsOccupied => _isOccupied;
        public GameObject Occupant => _occupant;
        public Vector3 Position => transform.position;

        #endregion

        #region Public Methods

        public void Initialize(ZoneType zone, int index)
        {
            _zone = zone;
            _slotIndex = index;
        }

        public bool TryPlace(GameObject building)
        {
            if (_isOccupied || building == null) return false;

            _occupant = building;
            _isOccupied = true;
            building.transform.position = transform.position;
            building.transform.SetParent(transform);

            if (_indicator != null) _indicator.enabled = false;

            return true;
        }

        public GameObject Remove()
        {
            if (!_isOccupied) return null;

            var removed = _occupant;
            _occupant = null;
            _isOccupied = false;

            if (_indicator != null) _indicator.enabled = true;

            return removed;
        }

        public void SetHighlight(bool highlighted)
        {
            if (_indicator != null && !_isOccupied)
            {
                _indicator.color = highlighted ? HoverColor : EmptyColor;
            }
        }

        #endregion
    }
}
