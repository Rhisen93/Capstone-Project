using UnityEngine;

namespace ForestSlice.Narrative
{
    [System.Serializable]
    public class NarrativeEntryData
    {
        public string id;
        public NarrativeType type;
        public string title;
        public string text;
        public Biome biome;
        public string bossId;
        public float displayDuration = 5f;
        public bool canSkip = true;

        public NarrativeEntryData() { }

        public NarrativeEntryData(string id, NarrativeType type, string title, string text)
        {
            this.id = id;
            this.type = type;
            this.title = title;
            this.text = text;
        }
    }

    [CreateAssetMenu(fileName = "NarrativeEntry", menuName = "ForestSlice/Narrative/Entry")]
    public class NarrativeEntrySO : ScriptableObject
    {
        public NarrativeEntryData data;
    }

    [CreateAssetMenu(fileName = "NarrativeDatabase", menuName = "ForestSlice/Narrative/Database")]
    public class NarrativeDatabaseSO : ScriptableObject
    {
        public NarrativeEntryData[] entries;

        public NarrativeEntryData GetEntry(string id)
        {
            foreach (var entry in entries)
            {
                if (entry.id == id)
                    return entry;
            }
            Debug.LogWarning($"Narrative entry not found: {id}");
            return null;
        }
    }
}
