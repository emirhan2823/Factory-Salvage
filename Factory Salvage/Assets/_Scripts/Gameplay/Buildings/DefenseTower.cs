using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A defense tower placed in defense slots.
    /// Auto-targets nearest enemy via RuntimeSet and fires projectiles.
    /// </summary>
    [RequireComponent(typeof(BuildingBase))]
    public class DefenseTower : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _fireRate = 1f;
        [SerializeField] private float _range = 6f;
        [SerializeField] private TransformRuntimeSet _enemySet;

        private BuildingBase _building;
        private float _fireTimer;
        private Transform _currentTarget;

        #endregion

        #region Properties

        public float Damage => _damage * (_building != null ? _building.LevelMultiplier : 1f);
        public float Range => _range;
        public Transform CurrentTarget => _currentTarget;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _building = GetComponent<BuildingBase>();
        }

        private void Start()
        {
            // Auto-find enemy set if not assigned
            if (_enemySet == null)
            {
                var sets = Resources.FindObjectsOfTypeAll<TransformRuntimeSet>();
                if (sets.Length > 0) _enemySet = sets[0];
            }
        }

        private void Update()
        {
            if (_enemySet == null || _enemySet.Count == 0)
            {
                _currentTarget = null;
                return;
            }

            FindTarget();

            if (_currentTarget == null) return;

            _fireTimer -= Time.deltaTime;
            if (_fireTimer <= 0f)
            {
                Fire();
                float rate = _fireRate * (_building != null ? _building.LevelMultiplier : 1f);
                _fireTimer = 1f / Mathf.Max(0.1f, rate);
            }
        }

        #endregion

        #region Public Methods

        public void SetStats(float damage, float fireRate, float range)
        {
            _damage = damage;
            _fireRate = fireRate;
            _range = range;
        }

        public void SetEnemySet(TransformRuntimeSet set)
        {
            _enemySet = set;
        }

        #endregion

        #region Private Methods

        private void FindTarget()
        {
            _currentTarget = null;
            float closestDist = float.MaxValue;

            for (int i = _enemySet.Items.Count - 1; i >= 0; i--)
            {
                var enemy = _enemySet.Items[i];
                if (enemy == null) continue;

                float dist = Vector2.Distance(transform.position, enemy.position);
                if (dist <= _range && dist < closestDist)
                {
                    closestDist = dist;
                    _currentTarget = enemy;
                }
            }
        }

        private void Fire()
        {
            if (_currentTarget == null) return;

            var direction = (_currentTarget.position - transform.position).normalized;

            // Create projectile
            var projGo = new GameObject("TowerProjectile");
            projGo.transform.position = transform.position + Vector3.up * 0.5f;

            var sr = projGo.AddComponent<SpriteRenderer>();
            sr.color = Color.yellow;
            sr.sortingOrder = 12;
            var tex = new Texture2D(8, 8);
            var pixels = new Color[64];
            for (int i = 0; i < 64; i++) pixels[i] = Color.yellow;
            tex.SetPixels(pixels);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8f);

            var col = projGo.AddComponent<CircleCollider2D>();
            col.radius = 0.15f;
            col.isTrigger = true;

            var proj = projGo.AddComponent<TowerProjectile>();
            proj.Initialize(direction, Damage);
        }

        #endregion
    }
}
