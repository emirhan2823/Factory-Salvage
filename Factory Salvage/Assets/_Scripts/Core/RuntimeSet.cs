using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Abstract SO that tracks a runtime set of items.
    /// Use to query active entities without FindObjectOfType.
    /// </summary>
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        #region Fields

        private readonly List<T> _items = new();

        #endregion

        #region Properties

        public IReadOnlyList<T> Items => _items;
        public int Count => _items.Count;

        #endregion

        #region Public Methods

        public void Add(T item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
            }
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        #endregion
    }
}
