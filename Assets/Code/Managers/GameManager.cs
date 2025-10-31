using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using ForestSlice.Core;
using ForestSlice.Systems;

namespace ForestSlice.Managers
{
    /// <summary>
    /// Game Manager - Handles save/load, scene management, and game state
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject playerPrefab;

        private GameObject currentPlayer;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Start new game
        /// </summary>
        public void NewGame()
        {
            // Delete existing save
            SaveSystem.DeleteSave("player_save");

            // Load first scene
            SceneManager.LoadScene("Forest_Slice");
            
            Debug.Log("New game started!");
        }

        /// <summary>
        /// Load saved game
        /// </summary>
        public bool LoadGame()
        {
            PlayerSaveData saveData = SaveSystem.LoadData<PlayerSaveData>("player_save");

            if (saveData == null)
            {
                Debug.LogWarning("No save data found!");
                return false;
            }

            // Restore player immediately (if in same scene)
            StartCoroutine(RestorePlayerAfterLoad(saveData));

            Debug.Log($"Game loaded from checkpoint: {saveData.checkpointId}");
            return true;
        }

        private System.Collections.IEnumerator RestorePlayerAfterLoad(PlayerSaveData saveData)
        {
            // Wait for scene to load
            yield return new WaitForSeconds(0.5f);

            // Find or spawn player
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning("Player not found in scene!");
                yield break;
            }

            // Restore position
            player.transform.position = saveData.position;

            // Restore health
            var health = player.GetComponent<Health>();
            if (health != null)
            {
                health.SetHealth(saveData.currentHealth);
            }

            // Restore inventory
            var inventory = player.GetComponent<ForestSlice.Player.Inventory>();
            if (inventory != null)
            {
                inventory.LoadFromSaveData(saveData.inventoryData);
            }

            Debug.Log($"Player restored at {saveData.position}");
        }

        /// <summary>
        /// Quick save (F5 key or manual)
        /// </summary>
        public void QuickSave()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("No player to save!");
                return;
            }

            var health = player.GetComponent<Health>();
            var inventory = player.GetComponent<ForestSlice.Player.Inventory>();

            if (health == null || inventory == null) return;

            PlayerSaveData saveData = new PlayerSaveData
            {
                checkpointId = "quicksave",
                position = player.transform.position,
                currentHealth = health.CurrentHealth,
                maxHealth = health.MaxHealth,
                inventoryData = inventory.GetSaveData()
            };

            SaveSystem.SaveData("player_save", saveData);
            Debug.Log("Quick save completed!");
        }

        /// <summary>
        /// Check if save exists
        /// </summary>
        public bool HasSaveData()
        {
            return SaveSystem.SaveExists("player_save");
        }

        private void Update()
        {
            // Quick save with F5
            if (Keyboard.current != null && Keyboard.current.f5Key.wasPressedThisFrame)
            {
                QuickSave();
            }

            // Quick load with F9
            if (Keyboard.current != null && Keyboard.current.f9Key.wasPressedThisFrame)
            {
                LoadGame();
            }
        }
    }
}
