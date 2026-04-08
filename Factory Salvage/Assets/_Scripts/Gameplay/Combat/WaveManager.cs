using System.Collections;
using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Spawns enemies in waves. Tracks alive count. Raises wave start/end events.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        #region Fields

        [Header("Config")]
        [SerializeField] private WaveDefinition[] _waves;
        [SerializeField] private float _timeBetweenWaves = 15f;
        [SerializeField] private bool _autoStartWaves;

        [Header("References")]
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Transform _baseTarget;
        [SerializeField] private ObjectPool _enemyPool;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private TransformRuntimeSet _enemySet;

        [Header("Events")]
        [SerializeField] private GameEvent _onWaveStart;
        [SerializeField] private GameEvent _onWaveEnd;
        [SerializeField] private IntGameEvent _onWaveNumberChanged;

        [Header("Variables")]
        [SerializeField] private IntVariable _currentWaveNumber;
        [SerializeField] private IntVariable _enemiesRemaining;

        private int _currentWaveIndex;
        private int _aliveEnemyCount;
        private bool _waveInProgress;
        private bool _allWavesComplete;

        #endregion

        #region Properties

        public int CurrentWaveIndex => _currentWaveIndex;
        public int AliveEnemyCount => _aliveEnemyCount;
        public bool WaveInProgress => _waveInProgress;
        public bool AllWavesComplete => _allWavesComplete;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            ServiceLocator.Register(this);

            if (_autoStartWaves)
            {
                StartNextWave();
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<WaveManager>();
        }

        #endregion

        #region Public Methods

        public void StartNextWave()
        {
            if (_waveInProgress || _allWavesComplete) return;
            if (_waves == null || _currentWaveIndex >= _waves.Length)
            {
                _allWavesComplete = true;
                return;
            }

            StartCoroutine(SpawnWaveCoroutine(_waves[_currentWaveIndex]));
        }

        #endregion

        #region Private Methods

        private IEnumerator SpawnWaveCoroutine(WaveDefinition wave)
        {
            _waveInProgress = true;
            _aliveEnemyCount = 0;

            if (_currentWaveNumber != null)
            {
                _currentWaveNumber.Value = wave.WaveNumber;
            }

            _onWaveStart?.Raise();
            _onWaveNumberChanged?.Raise(wave.WaveNumber);

            foreach (var group in wave.EnemyGroups)
            {
                for (int i = 0; i < group.Count; i++)
                {
                    SpawnEnemy(group.Enemy);
                    _aliveEnemyCount++;
                    UpdateEnemiesRemaining();

                    if (group.SpawnDelay > 0f)
                    {
                        yield return new WaitForSeconds(group.SpawnDelay);
                    }
                }

                if (wave.TimeBetweenGroups > 0f)
                {
                    yield return new WaitForSeconds(wave.TimeBetweenGroups);
                }
            }

            // Wait for all enemies to die
            while (_aliveEnemyCount > 0)
            {
                yield return null;
                // Recount from runtime set to handle edge cases
                _aliveEnemyCount = _enemySet != null ? _enemySet.Count : 0;
                UpdateEnemiesRemaining();
            }

            _waveInProgress = false;
            _currentWaveIndex++;
            _onWaveEnd?.Raise();

            if (_currentWaveIndex >= _waves.Length)
            {
                _allWavesComplete = true;
            }
            else if (_autoStartWaves)
            {
                yield return new WaitForSeconds(_timeBetweenWaves);
                StartNextWave();
            }
        }

        private void SpawnEnemy(EnemyDefinition enemyDef)
        {
            if (enemyDef == null || _spawnPoints == null || _spawnPoints.Length == 0) return;

            var spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            GameObject enemyObj;

            if (_enemyPool != null)
            {
                enemyObj = _enemyPool.Get(spawnPoint.position, Quaternion.identity);
            }
            else if (enemyDef.Prefab != null)
            {
                enemyObj = Instantiate(enemyDef.Prefab, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                return;
            }

            var controller = enemyObj.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(enemyDef, _gridManager, _baseTarget);
            }
        }

        private void UpdateEnemiesRemaining()
        {
            if (_enemiesRemaining != null)
            {
                _enemiesRemaining.Value = _aliveEnemyCount;
            }
        }

        #endregion
    }
}
