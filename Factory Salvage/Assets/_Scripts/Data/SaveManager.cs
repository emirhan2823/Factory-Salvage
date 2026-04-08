using System.IO;
using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Data
{
    /// <summary>
    /// Saves/loads game state to JSON file in persistent data path.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        #region Fields

        private const string SaveFileName = "save.json";

        #endregion

        #region Properties

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SaveManager>();
        }

        #endregion

        #region Public Methods

        public void Save(SaveData data)
        {
            if (data == null) return;

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveManager] Game saved to {SavePath}");
        }

        public SaveData Load()
        {
            if (!HasSave())
            {
                Debug.LogWarning("[SaveManager] No save file found");
                return null;
            }

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[SaveManager] Game loaded");
            return data;
        }

        public bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public void DeleteSave()
        {
            if (HasSave())
            {
                File.Delete(SavePath);
                Debug.Log("[SaveManager] Save file deleted");
            }
        }

        #endregion
    }
}
