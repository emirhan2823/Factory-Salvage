using System;
using System.Collections.Generic;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Serializable game state for save/load.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public List<ResourceEntry> Resources = new();
        public List<MachineSaveData> PlacedMachines = new();
        public List<UpgradeEntry> Upgrades = new();
        public int CurrentWave;
        public float BaseHealth;
    }

    [Serializable]
    public struct ResourceEntry
    {
        public string ResourceId;
        public int Amount;
    }

    [Serializable]
    public struct MachineSaveData
    {
        public string MachineId;
        public int GridX;
        public int GridY;
    }

    [Serializable]
    public struct UpgradeEntry
    {
        public string UpgradeId;
        public int Level;
    }
}
