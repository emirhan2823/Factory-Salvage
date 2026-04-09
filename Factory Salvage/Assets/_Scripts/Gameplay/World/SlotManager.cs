using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Manages building slots per zone. Replaces GridManager for side-scroll layout.
    /// </summary>
    public class SlotManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private BuildingSlot[] _villageSlots;
        [SerializeField] private BuildingSlot[] _defenseSlots;
        [SerializeField] private BuildingSlot[] _resourceSlots;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SlotManager>();
        }

        #endregion

        #region Public Methods

        public BuildingSlot[] GetSlots(ZoneType zone)
        {
            return zone switch
            {
                ZoneType.Village => _villageSlots,
                ZoneType.Defense => _defenseSlots,
                ZoneType.Resource => _resourceSlots,
                _ => null
            };
        }

        public BuildingSlot GetSlotAt(int index, ZoneType zone)
        {
            var slots = GetSlots(zone);
            if (slots == null || index < 0 || index >= slots.Length) return null;
            return slots[index];
        }

        public List<BuildingSlot> GetAvailableSlots(ZoneType zone)
        {
            var result = new List<BuildingSlot>();
            var slots = GetSlots(zone);
            if (slots == null) return result;

            foreach (var slot in slots)
            {
                if (!slot.IsOccupied) result.Add(slot);
            }
            return result;
        }

        public void SetSlots(ZoneType zone, BuildingSlot[] slots)
        {
            switch (zone)
            {
                case ZoneType.Village: _villageSlots = slots; break;
                case ZoneType.Defense: _defenseSlots = slots; break;
                case ZoneType.Resource: _resourceSlots = slots; break;
            }
        }

        #endregion
    }
}
