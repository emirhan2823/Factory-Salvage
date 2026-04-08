using System;
using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Shared health component for enemies, player base, and buildings.
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _maxHealth = 100f;

        private float _currentHealth;

        #endregion

        #region Properties

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public float HealthPercent => _maxHealth > 0f ? _currentHealth / _maxHealth : 0f;
        public bool IsDead => _currentHealth <= 0f;

        #endregion

        #region Events

        public event Action<float> OnDamaged;
        public event Action<Health> OnDied;
        public event Action<float> OnHealed;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _currentHealth = _maxHealth;
        }

        #endregion

        #region Public Methods

        public void Initialize(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(0f, _currentHealth);

            OnDamaged?.Invoke(amount);

            if (IsDead)
            {
                OnDied?.Invoke(this);
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            OnHealed?.Invoke(amount);
        }

        public void SetMaxHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        }

        #endregion
    }
}
