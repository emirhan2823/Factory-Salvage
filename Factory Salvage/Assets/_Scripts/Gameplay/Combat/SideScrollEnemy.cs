using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Side-scroll enemy: walks left toward base, attacks on arrival.
    /// No pathfinding needed — simple horizontal movement.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class SideScrollEnemy : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TransformRuntimeSet _enemySet;

        private EnemyDefinition _definition;
        private Transform _baseTarget;
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
                MoveLeft();
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(EnemyDefinition definition, Transform baseTarget, TransformRuntimeSet enemySet)
        {
            _definition = definition;
            _baseTarget = baseTarget;
            _enemySet = enemySet;
            _isAttacking = false;
            _attackTimer = 0f;

            if (_health != null)
            {
                _health.Initialize(definition.Health);
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.red;
            }

            // Register in set
            if (_enemySet != null)
            {
                _enemySet.Add(transform);
            }
        }

        #endregion

        #region Private Methods

        private void MoveLeft()
        {
            if (_baseTarget == null) return;

            float baseX = _baseTarget.position.x;
            float speed = _definition != null ? _definition.MoveSpeed : 2f;
            float range = _definition != null ? _definition.AttackRange : 1.5f;

            // Move left toward base
            if (transform.position.x > baseX + range)
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            else
            {
                _isAttacking = true;
            }
        }

        private void AttackBase()
        {
            if (_baseTarget == null) return;

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
            Core.SimpleParticleSystem.Emit(transform.position,
                Core.SimpleParticleSystem.ParticlePreset.EnemyDeath, 10);

            var animator = GetComponent<SpriteAnimator>();
            if (animator != null)
            {
                animator.PlayDeathShrink(() => {
                    var pooled = GetComponent<PooledObject>();
                    if (pooled != null) pooled.ReturnToPool();
                    else gameObject.SetActive(false);
                });
            }
            else
            {
                var pooled = GetComponent<PooledObject>();
                if (pooled != null) pooled.ReturnToPool();
                else gameObject.SetActive(false);
            }
        }

        private void DropLoot()
        {
            if (_definition == null || _definition.LootTable == null) return;

            var inventory = FindAnyObjectByType<Inventory>();
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
