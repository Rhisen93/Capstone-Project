using UnityEngine;
using UnityEditor;

namespace ForestSlice.Editor
{
    /// <summary>
    /// Editor utility to find and remove missing script references
    /// Menu: Tools/Clean Missing Scripts
    /// </summary>
    public class CleanMissingScripts : EditorWindow
    {
        [MenuItem("Tools/Clean Missing Scripts")]
        static void CleanUp()
        {
            // Find all GameObjects in scene
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            
            int missingCount = 0;
            int cleanedCount = 0;

            foreach (GameObject go in allObjects)
            {
                // Get all components
                Component[] components = go.GetComponents<Component>();
                
                // Check each component
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        missingCount++;
                        Debug.LogWarning($"Missing script found on: {GetGameObjectPath(go)}", go);
                    }
                }

                // Remove missing components
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                if (removed > 0)
                {
                    cleanedCount += removed;
                    Debug.Log($"Removed {removed} missing script(s) from: {GetGameObjectPath(go)}", go);
                }
            }

            if (missingCount > 0)
            {
                Debug.Log($"<color=green>Cleanup complete! Removed {cleanedCount} missing script(s) from {allObjects.Length} GameObjects.</color>");
                EditorUtility.DisplayDialog("Cleanup Complete", 
                    $"Removed {cleanedCount} missing script reference(s).", "OK");
            }
            else
            {
                Debug.Log("<color=green>No missing scripts found in scene!</color>");
                EditorUtility.DisplayDialog("Cleanup Complete", 
                    "No missing script references found.", "OK");
            }
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
    }
}
