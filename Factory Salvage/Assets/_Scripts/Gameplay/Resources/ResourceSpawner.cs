using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Periodically spawns resource nodes on empty grid cells.
    /// </summary>
    public class ResourceSpawner : MonoBehaviour
    {
        #region Fields

        [Header("Spawning")]
        [SerializeField] private float _spawnInterval = 10f;
        [SerializeField] private int _maxActiveNodes = 15;
        [SerializeField] private int _amountPerGather = 1;
        [SerializeField] private int _maxGathers = 3;

        [Header("References")]
        [SerializeField] private ObjectPool _resourcePool;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private ResourceDefinition[] _possibleResources;

        private float _spawnTimer;
        private int _activeNodeCount;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            _spawnTimer = 0f; // spawn immediately on start
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer = _spawnInterval;
                TrySpawnResource();
            }
        }

        #endregion

        #region Private Methods

        private void TrySpawnResource()
        {
            if (_possibleResources == null || _possibleResources.Length == 0) return;
            if (_resourcePool == null || _gridManager == null) return;
            if (_activeNodeCount >= _maxActiveNodes) return;

            // Try to find a random buildable cell
            for (int attempt = 0; attempt < 20; attempt++)
            {
                var randomCell = new Vector2Int(
                    Random.Range(0, _gridManager.MapSize.x),
                    Random.Range(0, _gridManager.MapSize.y)
                );

                if (!_gridManager.IsCellBuildable(randomCell)) continue;

                var worldPos = _gridManager.CellToWorld(randomCell);
                var resource = _possibleResources[Random.Range(0, _possibleResources.Length)];

                var obj = _resourcePool.Get(worldPos, Quaternion.identity);
                var node = obj.GetComponent<ResourceNode>();
                if (node != null)
                {
                    node.Initialize(resource, _amountPerGather, _maxGathers);
                }

                _activeNodeCount++;
                return;
            }
        }

        #endregion

        #region Public Methods

        public void OnNodeDepleted()
        {
            _activeNodeCount = Mathf.Max(0, _activeNodeCount - 1);
        }

        #endregion
    }
}
