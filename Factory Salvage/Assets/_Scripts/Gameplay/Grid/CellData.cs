using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Data for a single grid cell. Tracks type and what occupies it.
    /// </summary>
    [System.Serializable]
    public struct CellData
    {
        #region Fields

        public CellType Type;
        public GameObject OccupyingObject;

        #endregion

        #region Properties

        public bool IsBuildable => Type == CellType.Ground || Type == CellType.Empty;
        public bool IsWalkable => Type != CellType.Wall && Type != CellType.Occupied;

        #endregion

        #region Public Methods

        public static CellData CreateGround()
        {
            return new CellData { Type = CellType.Ground, OccupyingObject = null };
        }

        public static CellData CreateWall()
        {
            return new CellData { Type = CellType.Wall, OccupyingObject = null };
        }

        #endregion
    }
}
