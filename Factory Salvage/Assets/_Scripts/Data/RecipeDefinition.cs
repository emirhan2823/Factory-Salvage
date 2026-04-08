using UnityEngine;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Defines a production recipe: inputs → outputs in a specific machine.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Data/Recipe")]
    public class RecipeDefinition : ScriptableObject
    {
        #region Fields

        [SerializeField] private string _recipeName;
        [SerializeField] private ResourceCost[] _inputs;
        [SerializeField] private ResourceOutput[] _outputs;
        [SerializeField] private float _processTime = 3f;

        #endregion

        #region Properties

        public string RecipeName => _recipeName;
        public ResourceCost[] Inputs => _inputs;
        public ResourceOutput[] Outputs => _outputs;
        public float ProcessTime => _processTime;

        #endregion
    }
}
