using UnityEngine;
using ForestSlice.Core;

namespace ForestSlice.Systems
{
    /// <summary>
    /// Checkpoint for saving player progress and respawn position
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint Settings")]
        [SerializeField] private string checkpointId = "checkpoint_01";
        [SerializeField] private bool autoActivate = true;
        [SerializeField] private float activationRadius = 2f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color inactiveColor = Color.gray;
        [SerializeField] private Color activeColor = Color.cyan;
        [SerializeField] private GameObject activationVFX;

        private bool isActivated = false;

        public string CheckpointId => checkpointId;
        public bool IsActivated => isActivated;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            UpdateVisual();
        }

        private void Update()
        {
            if (autoActivate && !isActivated)
            {
                CheckForPlayer();
            }
        }

        private void CheckForPlayer()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, activationRadius, playerLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Activate(hit.gameObject);
                    break;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!autoActivate && !isActivated && collision.CompareTag("Player"))
            {
                Activate(collision.gameObject);
            }
        }

        /// <summary>
        /// Activate this checkpoint
        /// </summary>
        public void Activate(GameObject player)
        {
            if (isActivated) return;

            isActivated = true;
            UpdateVisual();

            // Save game at this checkpoint
            SaveAtCheckpoint(player);

            // Spawn VFX
            if (activationVFX != null)
            {
                Instantiate(activationVFX, transform.position, Quaternion.identity);
            }

            Debug.Log($"Checkpoint activated: {checkpointId} at {transform.position}");
        }

        /// <summary>
        /// Save game progress at this checkpoint
        /// </summary>
        private void SaveAtCheckpoint(GameObject player)
        {
            var health = player.GetComponent<Health>();
            var inventory = player.GetComponent<ForestSlice.Player.Inventory>();

            if (health == null || inventory == null)
            {
                Debug.LogWarning("Player missing Health or Inventory component!");
                return;
            }

            // Create save data
            PlayerSaveData saveData = new PlayerSaveData
            {
                checkpointId = checkpointId,
                position = transform.position,
                currentHealth = health.CurrentHealth,
                maxHealth = health.MaxHealth,
                inventoryData = inventory.GetSaveData()
            };

            // Save to file
            SaveSystem.SaveData("player_save", saveData);
            Debug.Log($"Game saved at checkpoint: {checkpointId}");
        }

        /// <summary>
        /// Update visual state
        /// </summary>
        private void UpdateVisual()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isActivated ? activeColor : inactiveColor;
            }
        }

        /// <summary>
        /// Reset checkpoint (for testing)
        /// </summary>
        public void Reset()
        {
            isActivated = false;
            UpdateVisual();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isActivated ? Color.cyan : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, activationRadius);
        }
    }

    /// <summary>
    /// Player save data structure
    /// </summary>
    [System.Serializable]
    public class PlayerSaveData
    {
        public string checkpointId;
        public Vector3 position;
        public float currentHealth;
        public float maxHealth;
        public ForestSlice.Player.InventorySaveData inventoryData;
        public System.DateTime saveTime;

        public PlayerSaveData()
        {
            saveTime = System.DateTime.Now;
        }
    }
}
