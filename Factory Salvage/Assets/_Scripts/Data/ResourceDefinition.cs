using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines a resource type (ScrapMetal, Wire, Circuit, Gear).
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Resource Definition")]
    public class ResourceDefinition : ScriptableObject
    {
        #region Fields

        [SerializeField] private string _resourceName;
        [SerializeField] private string _resourceId;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _maxStack = 999;
        [SerializeField] private Color _color = Color.white;

        #endregion

        #region Properties

        public string ResourceName => _resourceName;
        public string ResourceId => _resourceId;
        public Sprite Icon => _icon;
        public int MaxStack => _maxStack;
        public Color Color => _color;

        #endregion
    }
}
