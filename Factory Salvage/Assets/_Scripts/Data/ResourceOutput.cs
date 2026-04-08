namespace FactorySalvage.Data
{
    /// <summary>
    /// A resource + amount pair for recipe outputs.
    /// </summary>
    [System.Serializable]
    public struct ResourceOutput
    {
        public ResourceDefinition Resource;
        public int Amount;
    }
}
