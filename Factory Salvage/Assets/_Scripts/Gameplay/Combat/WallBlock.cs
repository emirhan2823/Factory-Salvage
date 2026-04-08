using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A defensive wall. Blocks enemy paths. Has health — can be destroyed.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class WallBlock : MachineBase
    {
        #region Fields

        [SerializeField] private Health _health;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (_health == null) _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (_health != null) _health.OnDied += OnWallDestroyed;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnDied -= OnWallDestroyed;
        }

        #endregion

        #region Protected Methods

        protected override void OnPlaced()
        {
            base.OnPlaced();
            // Wall cells block pathfinding — already handled by OccupyCell
        }

        #endregion

        #region Private Methods

        private void OnWallDestroyed(Health health)
        {
            Remove(); // Free the grid cell
            Destroy(gameObject);
        }

        #endregion
    }
}
