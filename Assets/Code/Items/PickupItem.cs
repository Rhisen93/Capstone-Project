using UnityEngine;
using ForestSlice.Data;

namespace ForestSlice.Items
{
    /// <summary>
    /// Pickup item in the world that player can collect
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PickupItem : MonoBehaviour
    {
        [Header("Item Configuration")]
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity = 1; // For consumables

        [Header("Pickup Settings")]
        [SerializeField] private bool autoPickup = true;
        [SerializeField] private float pickupRadius = 1.5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool floatAnimation = true;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatHeight = 0.3f;

        private Vector3 startPosition;
        private Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            startPosition = transform.position;
        }

        private void Start()
        {
            // Set sprite from item data
            if (itemData != null && spriteRenderer != null && itemData.icon != null)
            {
                // Create sprite from icon (if not using world prefab)
                spriteRenderer.sprite = itemData.icon;
                spriteRenderer.color = itemData.GetRarityColor();
            }
        }

        private void Update()
        {
            // Float animation
            if (floatAnimation)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }

            // Auto pickup check
            if (autoPickup)
            {
                CheckForPlayer();
            }
        }

        private void CheckForPlayer()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius, playerLayer);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    TryPickup(hit.gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!autoPickup && collision.CompareTag("Player"))
            {
                TryPickup(collision.gameObject);
            }
        }

        /// <summary>
        /// Attempt to pick up this item
        /// </summary>
        public void TryPickup(GameObject player)
        {
            if (itemData == null)
            {
                Debug.LogError($"{gameObject.name}: ItemData not assigned!");
                return;
            }

            var inventory = player.GetComponent<ForestSlice.Player.Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Player has no Inventory component!");
                return;
            }

            bool success = false;

            // Add consumables with quantity
            if (itemData.isConsumable && quantity > 1)
            {
                for (int i = 0; i < quantity; i++)
                {
                    if (!inventory.AddItem(itemData))
                    {
                        break; // Stack full
                    }
                }
                success = true;
            }
            else
            {
                success = inventory.AddItem(itemData);
            }

            if (success)
            {
                Debug.Log($"Picked up: {itemData.itemName} x{quantity}");
                OnPickedUp();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Cannot pickup {itemData.itemName} (inventory full or incompatible)");
            }
        }

        /// <summary>
        /// Called when item is successfully picked up
        /// </summary>
        protected virtual void OnPickedUp()
        {
            // Play pickup sound/VFX here
            // TODO: Instantiate pickup VFX
        }

        /// <summary>
        /// Initialize pickup with specific item data
        /// </summary>
        public void Initialize(ItemData data, int qty = 1)
        {
            itemData = data;
            quantity = qty;

            // Find sprite renderer if not set
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (spriteRenderer != null && data != null && data.icon != null)
            {
                spriteRenderer.sprite = data.icon;
                spriteRenderer.color = data.GetRarityColor();
            }

            startPosition = transform.position;
            
            Debug.Log($"PickupItem initialized: {data?.itemName ?? "NULL"} x{qty}");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }
}
