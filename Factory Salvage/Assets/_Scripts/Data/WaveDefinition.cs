using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines a wave: which enemies, how many, timing.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Wave Definition")]
    public class WaveDefinition : ScriptableObject
    {
        #region Fields

        [SerializeField] private int _waveNumber;
        [SerializeField] private EnemyGroup[] _enemyGroups;
        [SerializeField] private float _timeBetweenGroups = 3f;

        #endregion

        #region Properties

        public int WaveNumber => _waveNumber;
        public EnemyGroup[] EnemyGroups => _enemyGroups;
        public float TimeBetweenGroups => _timeBetweenGroups;

        #endregion

        #region Public Methods

        public int GetTotalEnemyCount()
        {
            int total = 0;
            if (_enemyGroups == null) return total;
            foreach (var group in _enemyGroups)
            {
                total += group.Count;
            }
            return total;
        }

        #endregion
    }

    [System.Serializable]
    public struct EnemyGroup
    {
        public EnemyDefinition Enemy;
        public int Count;
        public float SpawnDelay;
    }
}
