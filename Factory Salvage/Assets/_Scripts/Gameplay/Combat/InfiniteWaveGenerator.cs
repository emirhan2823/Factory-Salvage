using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Generates infinite waves with scaling difficulty.
    /// Every 25 waves = boss wave.
    /// </summary>
    public static class InfiniteWaveGenerator
    {
        #region Public Methods

        public static WaveData GenerateWave(int waveNumber, EnemyDefinition baseEnemy, EnemyDefinition fastEnemy)
        {
            bool isBoss = waveNumber % 25 == 0;

            int baseCount;
            int fastCount;
            float healthMultiplier;
            float damageMultiplier;
            float spawnDelay;

            if (isBoss)
            {
                baseCount = 3 + waveNumber / 5;
                fastCount = 2 + waveNumber / 10;
                healthMultiplier = 3f + waveNumber * 0.2f;
                damageMultiplier = 2f + waveNumber * 0.1f;
                spawnDelay = 0.3f;
            }
            else
            {
                baseCount = 3 + waveNumber;
                fastCount = Mathf.Max(0, waveNumber - 3);
                healthMultiplier = 1f + waveNumber * 0.1f;
                damageMultiplier = 1f + waveNumber * 0.05f;
                spawnDelay = Mathf.Max(0.3f, 1.5f - waveNumber * 0.05f);
            }

            return new WaveData
            {
                WaveNumber = waveNumber,
                IsBoss = isBoss,
                BaseEnemyCount = baseCount,
                FastEnemyCount = fastCount,
                HealthMultiplier = healthMultiplier,
                DamageMultiplier = damageMultiplier,
                SpawnDelay = spawnDelay,
                BaseEnemy = baseEnemy,
                FastEnemy = fastEnemy
            };
        }

        #endregion

        #region WaveData

        public struct WaveData
        {
            public int WaveNumber;
            public bool IsBoss;
            public int BaseEnemyCount;
            public int FastEnemyCount;
            public float HealthMultiplier;
            public float DamageMultiplier;
            public float SpawnDelay;
            public EnemyDefinition BaseEnemy;
            public EnemyDefinition FastEnemy;

            public int TotalEnemies => BaseEnemyCount + FastEnemyCount;
        }

        #endregion
    }
}
