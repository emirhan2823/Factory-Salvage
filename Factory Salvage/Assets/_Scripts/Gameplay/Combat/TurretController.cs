using UnityEngine;
using FactorySalvage.Core;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Auto-targeting turret. Finds nearest enemy via RuntimeSet, fires projectiles.
    /// </summary>
    public class TurretController : MachineBase
    {
        #region Fields

        [Header("Turret")]
        [SerializeField] private TurretDefinition _turretDef;
        [SerializeField] private ObjectPool _projectilePool;
        [SerializeField] private TransformRuntimeSet _enemySet;
        [SerializeField] private Transform _firePoint;

        private float _fireTimer;
        private Transform _currentTarget;

        #endregion

        #region Properties

        public TurretDefinition TurretDef => _turretDef;
        public Transform CurrentTarget => _currentTarget;

        #endregion

        #region Unity Callbacks

        private void Update()
        {
            if (!IsPlaced || _turretDef == null) return;

            FindTarget();

            if (_currentTarget == null) return;

            _fireTimer -= Time.deltaTime;
            if (_fireTimer <= 0f)
            {
                Fire();
                _fireTimer = 1f / Mathf.Max(0.1f, _turretDef.FireRate);
            }
        }

        #endregion

        #region Public Methods

        public void SetTurretDefinition(TurretDefinition def)
        {
            _turretDef = def;
        }

        #endregion

        #region Private Methods

        private void FindTarget()
        {
            _currentTarget = null;

            if (_enemySet == null || _enemySet.Count == 0) return;

            float closestDist = float.MaxValue;

            for (int i = 0; i < _enemySet.Items.Count; i++)
            {
                var enemy = _enemySet.Items[i];
                if (enemy == null) continue;

                float dist = Vector2.Distance(transform.position, enemy.position);
                if (dist <= _turretDef.Range && dist < closestDist)
                {
                    closestDist = dist;
                    _currentTarget = enemy;
                }
            }
        }

        private void Fire()
        {
            if (_currentTarget == null) return;

            var spawnPos = _firePoint != null ? _firePoint.position : transform.position;
            var direction = (_currentTarget.position - spawnPos).normalized;

            GameObject projectileObj;
            if (_projectilePool != null)
            {
                projectileObj = _projectilePool.Get(spawnPos, Quaternion.identity);
            }
            else
            {
                return;
            }

            var projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(direction, _turretDef.Damage);
            }
        }

        #endregion
    }
}
