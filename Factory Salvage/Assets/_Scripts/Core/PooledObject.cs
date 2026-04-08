using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Attach to pooled prefabs. Provides ReturnToPool and optional auto-return.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _autoReturnTime = -1f;

        private ObjectPool _pool;
        private float _timer;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (_autoReturnTime > 0f)
            {
                _timer = _autoReturnTime;
            }
        }

        private void Update()
        {
            if (_autoReturnTime <= 0f) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                ReturnToPool();
            }
        }

        #endregion

        #region Public Methods

        public void SetPool(ObjectPool pool)
        {
            _pool = pool;
        }

        public void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Return(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
