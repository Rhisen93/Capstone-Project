using System.IO;
using UnityEngine;

namespace ForestSlice.Core
{
    public static class SaveSystem
    {
        private static string GetSavePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        }

        /// <summary>
        /// Save data with generic type
        /// </summary>
        public static void SaveData<T>(string fileName, T data)
        {
            string path = GetSavePath(fileName);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"Data saved to: {path}");
        }

        /// <summary>
        /// Load data with generic type
        /// </summary>
        public static T LoadData<T>(string fileName) where T : class
        {
            string path = GetSavePath(fileName);
            
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Save file not found: {path}");
                return null;
            }

            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);
            Debug.Log($"Data loaded from: {path}");
            return data;
        }

        /// <summary>
        /// Check if save exists
        /// </summary>
        public static bool SaveExists(string fileName)
        {
            return File.Exists(GetSavePath(fileName));
        }

        /// <summary>
        /// Delete save file
        /// </summary>
        public static void DeleteSave(string fileName)
        {
            string path = GetSavePath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"Save file deleted: {path}");
            }
        }

        // Legacy methods for backward compatibility
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static void SaveGame(GameSaveData data)
        {
            SaveData("save", data);
        }

        public static GameSaveData LoadGame()
        {
            GameSaveData data = LoadData<GameSaveData>("save");
            return data ?? new GameSaveData();
        }

        public static bool SaveExists()
        {
            return SaveExists("save");
        }

        public static void DeleteSave()
        {
            DeleteSave("save");
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public float playerHealth = 100f;
        public Vector3 playerPosition = Vector3.zero;
        public string currentScene = "Forest_Slice";
        public int checkpointId = 0;
        public string[] defeatedBosses = new string[0];
        public string[] collectedItems = new string[0];
        public float playTime = 0f;
    }
}
