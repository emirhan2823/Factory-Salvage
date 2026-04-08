using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Runtime set of Transforms. Used to track active enemies, machines, etc.
    /// </summary>
    [CreateAssetMenu(menuName = "FactorySalvage/Runtime Sets/Transform Set")]
    public class TransformRuntimeSet : RuntimeSet<Transform>
    {
    }
}
