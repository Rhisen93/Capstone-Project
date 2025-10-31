using UnityEngine;
using ForestSlice.Data;
using System.Collections.Generic;

namespace ForestSlice.Items
{
    /// <summary>
    /// Defines loot drops with weighted random selection
    /// </summary>
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "ForestSlice/Items/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [System.Serializable]
        public class LootEntry
        {
            public ItemData item;
            [Range(0f, 100f)]
            public float dropChance = 50f; // Percentage
            public int minQuantity = 1;
            public int maxQuantity = 1;
        }

        [Header("Loot Configuration")]
        public LootEntry[] possibleDrops;
        
        [Header("Drop Settings")]
        [Range(0f, 100f)]
        public float globalDropChance = 100f; // Chance that ANY loot drops
        public int minDrops = 1;
        public int maxDrops = 1;

        /// <summary>
        /// Roll for loot drops
        /// </summary>
        public List<ItemDrop> RollLoot()
        {
            List<ItemDrop> drops = new List<ItemDrop>();

            // Check if any loot drops at all
            if (Random.Range(0f, 100f) > globalDropChance)
            {
                return drops; // No loot
            }

            // Determine how many items to drop
            int dropCount = Random.Range(minDrops, maxDrops + 1);

            for (int i = 0; i < dropCount; i++)
            {
                ItemDrop drop = RollSingleItem();
                if (drop != null)
                {
                    drops.Add(drop);
                }
            }

            return drops;
        }

        /// <summary>
        /// Roll for a single item
        /// </summary>
        private ItemDrop RollSingleItem()
        {
            if (possibleDrops == null || possibleDrops.Length == 0) return null;

            // Try each entry
            foreach (LootEntry entry in possibleDrops)
            {
                if (entry.item == null) continue;

                float roll = Random.Range(0f, 100f);
                if (roll <= entry.dropChance)
                {
                    int quantity = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                    return new ItemDrop
                    {
                        itemData = entry.item,
                        quantity = quantity
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Spawn loot at a position
        /// </summary>
        public void SpawnLoot(Vector3 position, GameObject pickupPrefab)
        {
            List<ItemDrop> drops = RollLoot();

            foreach (ItemDrop drop in drops)
            {
                SpawnPickup(drop, position, pickupPrefab);
            }
        }

        /// <summary>
        /// Spawn a single pickup
        /// </summary>
        private void SpawnPickup(ItemDrop drop, Vector3 position, GameObject pickupPrefab)
        {
            if (drop == null || drop.itemData == null) return;

            // Use item's world prefab if available, otherwise use default
            GameObject prefabToUse = drop.itemData.worldPrefab != null 
                ? drop.itemData.worldPrefab 
                : pickupPrefab;

            if (prefabToUse == null)
            {
                Debug.LogWarning("No pickup prefab available!");
                return;
            }

            // Random offset for visual spread
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPos = position + new Vector3(offset.x, offset.y, 0f);

            GameObject pickupObj = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
            PickupItem pickup = pickupObj.GetComponent<PickupItem>();

            if (pickup != null)
            {
                pickup.Initialize(drop.itemData, drop.quantity);
            }

            Debug.Log($"Spawned loot: {drop.itemData.itemName} x{drop.quantity}");
        }
    }

    /// <summary>
    /// Represents a dropped item with quantity
    /// </summary>
    public class ItemDrop
    {
        public ItemData itemData;
        public int quantity;
    }
}
