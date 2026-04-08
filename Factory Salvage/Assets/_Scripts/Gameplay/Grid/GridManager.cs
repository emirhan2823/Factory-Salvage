using UnityEngine;
using UnityEngine.Tilemaps;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Manages a logical grid overlaid on Unity Tilemaps.
    /// Tracks cell state for building, pathfinding, and occupancy.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private Tilemap _buildingTilemap;

        [Header("Config")]
        [SerializeField] private Vector2Int _mapSize = new(20, 20);
        [SerializeField] private Vector2Int _mapOffset = new(-10, -10);

        private CellData[,] _cells;

        #endregion

        #region Properties

        public Vector2Int MapSize => _mapSize;
        public Vector2Int MapOffset => _mapOffset;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            InitializeGrid();
        }

        #endregion

        #region Public Methods

        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            var cellPos = _grid.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x - _mapOffset.x, cellPos.y - _mapOffset.y);
        }

        public Vector3 CellToWorld(Vector2Int gridPos)
        {
            var cellPos = new Vector3Int(gridPos.x + _mapOffset.x, gridPos.y + _mapOffset.y, 0);
            return _grid.GetCellCenterWorld(cellPos);
        }

        public bool IsInBounds(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.x < _mapSize.x &&
                   gridPos.y >= 0 && gridPos.y < _mapSize.y;
        }

        public bool IsCellBuildable(Vector2Int gridPos)
        {
            if (!IsInBounds(gridPos)) return false;
            return _cells[gridPos.x, gridPos.y].IsBuildable;
        }

        public bool IsCellWalkable(Vector2Int gridPos)
        {
            if (!IsInBounds(gridPos)) return false;
            return _cells[gridPos.x, gridPos.y].IsWalkable;
        }

        public CellData GetCellData(Vector2Int gridPos)
        {
            if (!IsInBounds(gridPos)) return new CellData { Type = CellType.Wall };
            return _cells[gridPos.x, gridPos.y];
        }

        public bool OccupyCell(Vector2Int gridPos, GameObject obj)
        {
            if (!IsInBounds(gridPos)) return false;
            if (!_cells[gridPos.x, gridPos.y].IsBuildable) return false;

            _cells[gridPos.x, gridPos.y].Type = CellType.Occupied;
            _cells[gridPos.x, gridPos.y].OccupyingObject = obj;
            return true;
        }

        public void FreeCell(Vector2Int gridPos)
        {
            if (!IsInBounds(gridPos)) return;

            _cells[gridPos.x, gridPos.y].Type = CellType.Ground;
            _cells[gridPos.x, gridPos.y].OccupyingObject = null;
        }

        public void SetCellType(Vector2Int gridPos, CellType type)
        {
            if (!IsInBounds(gridPos)) return;
            _cells[gridPos.x, gridPos.y].Type = type;
        }

        #endregion

        #region Private Methods

        private void InitializeGrid()
        {
            _cells = new CellData[_mapSize.x, _mapSize.y];

            for (int x = 0; x < _mapSize.x; x++)
            {
                for (int y = 0; y < _mapSize.y; y++)
                {
                    var tilePos = new Vector3Int(x + _mapOffset.x, y + _mapOffset.y, 0);
                    bool hasGroundTile = _groundTilemap != null && _groundTilemap.HasTile(tilePos);

                    _cells[x, y] = hasGroundTile
                        ? CellData.CreateGround()
                        : new CellData { Type = CellType.Empty };
                }
            }
        }

        #endregion

        #region Editor

        private void OnDrawGizmosSelected()
        {
            if (_grid == null) return;

            Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
            for (int x = 0; x < _mapSize.x; x++)
            {
                for (int y = 0; y < _mapSize.y; y++)
                {
                    var cellPos = new Vector3Int(x + _mapOffset.x, y + _mapOffset.y, 0);
                    var worldPos = _grid.GetCellCenterWorld(cellPos);
                    Gizmos.DrawWireCube(worldPos, _grid.cellSize);
                }
            }
        }

        #endregion
    }
}
