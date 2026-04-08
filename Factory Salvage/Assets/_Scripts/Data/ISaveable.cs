namespace FactorySalvage.Data
{
    /// <summary>
    /// Interface for components that can save/load state.
    /// </summary>
    public interface ISaveable
    {
        void SaveState(SaveData data);
        void LoadState(SaveData data);
    }
}
