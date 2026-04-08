using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Queue-based object pool. Attach to a GameObject, configure prefab and initial size.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize = 10;
        [SerializeField] private Transform _poolParent;

        private readonly Queue<GameObject> _available = new();

        #endregion

        #region Properties

        public int CountAvailable => _available.Count;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (_poolParent == null)
            {
                _poolParent = transform;
            }
            PreWarm();
        }

        #endregion

        #region Public Methods

        public void PreWarm()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                var obj = CreateNewInstance();
                obj.SetActive(false);
                _available.Enqueue(obj);
            }
        }

        public GameObject Get()
        {
            GameObject obj;

            if (_available.Count > 0)
            {
                obj = _available.Dequeue();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] Pool for {_prefab.name} empty — instantiating new. Consider increasing initial size.");
                obj = CreateNewInstance();
            }

            obj.SetActive(true);
            return obj;
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            var obj = Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(_poolParent);
            _available.Enqueue(obj);
        }

        #endregion

        #region Private Methods

        private GameObject CreateNewInstance()
        {
            var obj = Instantiate(_prefab, _poolParent);
            var pooled = obj.GetComponent<PooledObject>();
            if (pooled != null)
            {
                pooled.SetPool(this);
            }
            return obj;
        }

        #endregion
    }
}
