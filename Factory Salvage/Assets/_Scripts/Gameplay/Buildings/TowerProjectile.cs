using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Tower projectile: moves in direction, damages enemies on hit, auto-destroys.
    /// </summary>
    public class TowerProjectile : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _speed = 12f;
        [SerializeField] private float _maxLifetime = 3f;

        private Vector3 _direction;
        private float _damage;
        private float _lifetime;

        #endregion

        #region Public Methods

        public void Initialize(Vector3 direction, float damage)
        {
            _direction = direction.normalized;
            _damage = damage;
            _lifetime = _maxLifetime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        #endregion

        #region Unity Callbacks

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;

            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<SideScrollEnemy>();
            if (enemy == null) return;

            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }

        #endregion
    }
}
