using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Moves in a direction, deals damage on hit, returns to pool.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _maxLifetime = 3f;

        private Vector3 _direction;
        private float _damage;
        private float _lifetime;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _lifetime = _maxLifetime;
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;

            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0f)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<Health>();
            if (health == null) return;

            // Only damage enemies (check for EnemyController)
            var enemy = other.GetComponent<EnemyController>();
            if (enemy == null) return;

            health.TakeDamage(_damage);
            ReturnToPool();
        }

        #endregion

        #region Public Methods

        public void Initialize(Vector3 direction, float damage)
        {
            _direction = direction.normalized;
            _damage = damage;
            _lifetime = _maxLifetime;

            // Rotate to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        #endregion

        #region Private Methods

        private void ReturnToPool()
        {
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

        #endregion
    }
}
