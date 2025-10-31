using UnityEngine;

namespace ForestSlice.Narrative
{
    [System.Serializable]
    public class NarrativeDatabase
    {
        public NarrativeEntryData[] entries;
    }

    public class NarrativeJsonLoader : MonoBehaviour
    {
        private static NarrativeDatabase database;

        public static void LoadFromJson()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("Narrative/narrative_db");
            
            if (jsonFile == null)
            {
                Debug.LogError("narrative_db.json not found in Resources/Narrative/");
                return;
            }

            database = JsonUtility.FromJson<NarrativeDatabase>(jsonFile.text);
            Debug.Log($"Loaded {database.entries.Length} narrative entries from JSON.");
        }

        public static NarrativeEntryData GetEntry(string id)
        {
            if (database == null)
            {
                LoadFromJson();
            }

            if (database == null || database.entries == null)
            {
                Debug.LogError("Narrative database not loaded or empty.");
                return null;
            }

            foreach (var entry in database.entries)
            {
                if (entry.id == id)
                    return entry;
            }

            Debug.LogWarning($"Narrative entry not found: {id}");
            return null;
        }

        public static NarrativeEntryData[] GetAllEntries()
        {
            if (database == null)
            {
                LoadFromJson();
            }

            return database?.entries;
        }
    }
}
