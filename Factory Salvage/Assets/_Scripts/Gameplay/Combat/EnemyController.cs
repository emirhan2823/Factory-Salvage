using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Enemy AI: pathfinds to base, attacks on arrival. Returns to pool on death.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private EnemyDefinition _definition;
        [SerializeField] private Health _health;
        [SerializeField] private TransformRuntimeSet _enemySet;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private GridManager _gridManager;
        private Transform _baseTarget;
        private List<Vector2Int> _path;
        private int _pathIndex;
        private float _attackTimer;
        private bool _isAttacking;

        #endregion

        #region Properties

        public EnemyDefinition Definition => _definition;
        public Health Health => _health;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (_health == null) _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (_enemySet != null) _enemySet.Add(transform);
            if (_health != null) _health.OnDied += OnDied;
        }

        private void OnDisable()
        {
            if (_enemySet != null) _enemySet.Remove(transform);
            if (_health != null) _health.OnDied -= OnDied;
        }

        private void Update()
        {
            if (_health == null || _health.IsDead) return;

            if (_isAttacking)
            {
                AttackBase();
            }
            else
            {
                FollowPath();
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(EnemyDefinition definition, GridManager gridManager, Transform baseTarget)
        {
            _definition = definition;
            _gridManager = gridManager;
            _baseTarget = baseTarget;

            if (_health != null)
            {
                _health.Initialize(definition.Health);
            }

            _isAttacking = false;
            _pathIndex = 0;
            _attackTimer = 0f;

            CalculatePath();
        }

        #endregion

        #region Private Methods

        private void CalculatePath()
        {
            if (_gridManager == null || _baseTarget == null) return;

            var start = _gridManager.WorldToCell(transform.position);
            var end = _gridManager.WorldToCell(_baseTarget.position);

            _path = GridPathfinding.FindPath(start, end, _gridManager);
            _pathIndex = 0;
        }

        private void FollowPath()
        {
            if (_path == null || _path.Count == 0 || _gridManager == null) return;

            if (_pathIndex >= _path.Count)
            {
                // Reached the end of path — start attacking
                _isAttacking = true;
                return;
            }

            var targetWorldPos = _gridManager.CellToWorld(_path[_pathIndex]);
            var direction = (targetWorldPos - transform.position);
            direction.z = 0f;

            if (direction.magnitude < 0.1f)
            {
                _pathIndex++;
                return;
            }

            var speed = _definition != null ? _definition.MoveSpeed : 2f;
            transform.position += direction.normalized * speed * Time.deltaTime;
        }

        private void AttackBase()
        {
            if (_baseTarget == null) return;

            var distance = Vector2.Distance(transform.position, _baseTarget.position);
            var range = _definition != null ? _definition.AttackRange : 1f;

            if (distance > range + 0.5f)
            {
                // Move closer
                var dir = (_baseTarget.position - transform.position).normalized;
                var speed = _definition != null ? _definition.MoveSpeed : 2f;
                transform.position += dir * speed * Time.deltaTime;
                return;
            }

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f)
            {
                var rate = _definition != null ? _definition.AttackRate : 1f;
                _attackTimer = 1f / Mathf.Max(0.1f, rate);

                var baseHealth = _baseTarget.GetComponent<Health>();
                if (baseHealth != null)
                {
                    var damage = _definition != null ? _definition.AttackDamage : 5f;
                    baseHealth.TakeDamage(damage);
                }
            }
        }

        private void OnDied(Health health)
        {
            DropLoot();

            var pooled = GetComponent<PooledObject>();
            if (pooled != null)
            {
                pooled.ReturnToPool();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void DropLoot()
        {
            if (_definition == null || _definition.LootTable == null) return;

            // Find player inventory (simple approach)
            var player = Object.FindAnyObjectByType<PlayerController>();
            if (player == null) return;

            var inventory = player.GetComponent<Inventory>();
            if (inventory == null) return;

            foreach (var loot in _definition.LootTable)
            {
                if (Random.value <= loot.DropChance)
                {
                    inventory.AddResource(loot.Resource, loot.Amount);
                }
            }
        }

        #endregion
    }
}
