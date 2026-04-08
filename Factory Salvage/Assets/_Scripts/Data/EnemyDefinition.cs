using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines an enemy type: stats, visuals, loot.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        #region Fields

        [Header("Info")]
        [SerializeField] private string _enemyName;
        [SerializeField] private GameObject _prefab;

        [Header("Stats")]
        [SerializeField] private float _health = 10f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackDamage = 5f;
        [SerializeField] private float _attackRate = 1f;
        [SerializeField] private float _attackRange = 1f;

        [Header("Loot")]
        [SerializeField] private LootEntry[] _lootTable;

        #endregion

        #region Properties

        public string EnemyName => _enemyName;
        public GameObject Prefab => _prefab;
        public float Health => _health;
        public float MoveSpeed => _moveSpeed;
        public float AttackDamage => _attackDamage;
        public float AttackRate => _attackRate;
        public float AttackRange => _attackRange;
        public LootEntry[] LootTable => _lootTable;

        #endregion
    }

    [System.Serializable]
    public struct LootEntry
    {
        public ResourceDefinition Resource;
        public int Amount;
        [Range(0f, 1f)] public float DropChance;
    }
}
