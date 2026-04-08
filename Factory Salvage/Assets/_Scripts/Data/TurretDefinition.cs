using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines a turret type: damage, fire rate, range, projectile.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Turret Definition")]
    public class TurretDefinition : ScriptableObject
    {
        #region Fields

        [SerializeField] private string _turretName;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _fireRate = 1f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private ResourceCost[] _buildCost;

        #endregion

        #region Properties

        public string TurretName => _turretName;
        public float Damage => _damage;
        public float FireRate => _fireRate;
        public float Range => _range;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public ResourceCost[] BuildCost => _buildCost;

        #endregion
    }
}
