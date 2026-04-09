using System.Collections;
using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Spawns infinite waves from the right side. Enemies walk left toward base.
    /// </summary>
    public class SideScrollWaveManager : MonoBehaviour
    {
        #region Fields

        [Header("Config")]
        [SerializeField] private float _timeBetweenWaves = 10f;
        [SerializeField] private float _spawnXOffset = 22f;
        [SerializeField] private float _spawnY = 1f;

        [Header("Enemy Definitions")]
        [SerializeField] private EnemyDefinition _baseEnemy;
        [SerializeField] private EnemyDefinition _fastEnemy;

        [Header("References")]
        [SerializeField] private Transform _baseTarget;
        [SerializeField] private TransformRuntimeSet _enemySet;

        [Header("Variables")]
        [SerializeField] private IntVariable _currentWaveNumber;
        [SerializeField] private IntVariable _enemiesRemaining;

        [Header("Events")]
        [SerializeField] private GameEvent _onWaveStart;
        [SerializeField] private GameEvent _onWaveEnd;

        private int _currentWave;
        private bool _waveInProgress;

        #endregion

        #region Properties

        public int CurrentWave => _currentWave;
        public bool WaveInProgress => _waveInProgress;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SideScrollWaveManager>();
        }

        #endregion

        #region Public Methods

        public void StartNextWave()
        {
            if (_waveInProgress) return;

            _currentWave++;
            var waveData = InfiniteWaveGenerator.GenerateWave(_currentWave, _baseEnemy, _fastEnemy);

            if (_currentWaveNumber != null) _currentWaveNumber.Value = _currentWave;

            StartCoroutine(SpawnWaveCoroutine(waveData));
        }

        #endregion

        #region Private Methods

        private IEnumerator SpawnWaveCoroutine(InfiniteWaveGenerator.WaveData wave)
        {
            _waveInProgress = true;
            _onWaveStart?.Raise();

            string waveType = wave.IsBoss ? "BOSS WAVE" : "Wave";
            Debug.Log($"[Wave] {waveType} {wave.WaveNumber}: {wave.TotalEnemies} enemies (HP x{wave.HealthMultiplier:F1})");

            int aliveCount = 0;

            // Spawn base enemies
            for (int i = 0; i < wave.BaseEnemyCount; i++)
            {
                SpawnEnemy(wave.BaseEnemy, wave.HealthMultiplier, wave.DamageMultiplier);
                aliveCount++;
                UpdateEnemiesRemaining(aliveCount);
                yield return new WaitForSeconds(wave.SpawnDelay);
            }

            // Spawn fast enemies
            for (int i = 0; i < wave.FastEnemyCount; i++)
            {
                SpawnEnemy(wave.FastEnemy ?? wave.BaseEnemy, wave.HealthMultiplier, wave.DamageMultiplier);
                aliveCount++;
                UpdateEnemiesRemaining(aliveCount);
                yield return new WaitForSeconds(wave.SpawnDelay * 0.5f);
            }

            // Wait for all enemies to die
            while (_enemySet != null && _enemySet.Count > 0)
            {
                UpdateEnemiesRemaining(_enemySet.Count);
                yield return null;
            }

            UpdateEnemiesRemaining(0);
            _waveInProgress = false;
            _onWaveEnd?.Raise();

            Debug.Log($"[Wave] Wave {wave.WaveNumber} complete!");
        }

        private void SpawnEnemy(EnemyDefinition enemyDef, float healthMult, float damageMult)
        {
            if (enemyDef == null) return;

            // Random Y variation
            float yOffset = Random.Range(-0.5f, 0.5f);
            var spawnPos = new Vector3(_spawnXOffset, _spawnY + yOffset, 0f);

            // Create enemy
            var enemyGo = new GameObject($"Enemy_{enemyDef.EnemyName}");
            enemyGo.transform.position = spawnPos;

            // Sprite (SpriteFactory generates creature shapes)
            var sr = enemyGo.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 8;
            bool isFast = _fastEnemy != null && enemyDef == _fastEnemy;
            var enemyColor = isFast ? Core.ColorPalette.EnemyFast : Core.ColorPalette.EnemyBase;
            sr.sprite = Core.SpriteFactory.CreateEnemy(isFast, enemyColor);

            // Collider
            var col = enemyGo.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            col.isTrigger = true;

            // Health
            var health = enemyGo.AddComponent<Health>();
            health.Initialize(enemyDef.Health * healthMult);

            // Enemy controller
            var enemy = enemyGo.AddComponent<SideScrollEnemy>();
            var srField = typeof(SideScrollEnemy).GetField("_spriteRenderer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            srField?.SetValue(enemy, sr);

            enemy.Initialize(enemyDef, _baseTarget, _enemySet);
        }

        private void UpdateEnemiesRemaining(int count)
        {
            if (_enemiesRemaining != null) _enemiesRemaining.Value = count;
        }

        #endregion
    }
}
