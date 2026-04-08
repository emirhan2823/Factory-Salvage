using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A collectible resource node on the map. Player interacts to gather resources.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        #region Fields

        [SerializeField] private ResourceDefinition _resourceType;
        [SerializeField] private int _amountPerGather = 1;
        [SerializeField] private int _maxGathers = 3;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private int _gathersRemaining;

        #endregion

        #region Properties

        public ResourceDefinition ResourceType => _resourceType;
        public int GathersRemaining => _gathersRemaining;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _gathersRemaining = _maxGathers;
            UpdateVisual();
        }

        #endregion

        #region Public Methods

        public void Initialize(ResourceDefinition resourceType, int amountPerGather, int maxGathers)
        {
            _resourceType = resourceType;
            _amountPerGather = amountPerGather;
            _maxGathers = maxGathers;
            _gathersRemaining = maxGathers;
            UpdateVisual();
        }

        public void Interact(PlayerController player)
        {
            if (_gathersRemaining <= 0) return;

            var inventory = player.GetComponent<Inventory>();
            if (inventory == null) return;

            inventory.AddResource(_resourceType, _amountPerGather);
            _gathersRemaining--;

            UpdateVisual();

            if (_gathersRemaining <= 0)
            {
                Deplete();
            }
        }

        public string GetInteractionPrompt()
        {
            if (_resourceType == null) return "Gather";
            return $"Gather {_resourceType.ResourceName} ({_gathersRemaining})";
        }

        #endregion

        #region Private Methods

        private void UpdateVisual()
        {
            if (_spriteRenderer == null) return;

            // Fade as gathers decrease
            float alpha = _maxGathers > 0 ? (float)_gathersRemaining / _maxGathers : 1f;
            alpha = Mathf.Clamp(alpha, 0.3f, 1f);

            var color = _resourceType != null ? _resourceType.Color : Color.white;
            color.a = alpha;
            _spriteRenderer.color = color;
        }

        private void Deplete()
        {
            // Return to pool if pooled, otherwise deactivate
            var pooled = GetComponent<Core.PooledObject>();
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
