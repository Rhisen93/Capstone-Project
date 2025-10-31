using UnityEngine;

namespace ForestSlice.Data
{
    /// <summary>
    /// Item types available in the game
    /// </summary>
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable
    }

    /// <summary>
    /// Item rarity for visual feedback and loot tables
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    /// <summary>
    /// Base ScriptableObject for all items in the game
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "ForestSlice/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName = "New Item";
        [TextArea(2, 4)]
        public string description = "Item description";
        public Sprite icon;
        public ItemType itemType;
        public ItemRarity rarity = ItemRarity.Common;

        [Header("Stats")]
        public float damageBonus = 0f;
        public float defenseBonus = 0f;
        public float speedBonus = 0f;
        public float healAmount = 0f;

        [Header("Consumable Settings")]
        public bool isConsumable = false;
        public int maxStackSize = 1;

        [Header("Visual")]
        public GameObject worldPrefab; // Prefab for pickup in world
        public Color rarityColor = Color.white;

        /// <summary>
        /// Use this item (for consumables)
        /// </summary>
        public virtual void Use(GameObject user)
        {
            if (!isConsumable) return;

            if (healAmount > 0)
            {
                var health = user.GetComponent<ForestSlice.Core.Health>();
                if (health != null)
                {
                    health.Heal(healAmount);
                    Debug.Log($"Used {itemName}: Healed {healAmount} HP");
                }
            }

            // Add more consumable effects here (buffs, etc.)
        }

        /// <summary>
        /// Get color based on rarity
        /// </summary>
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common: return Color.white;
                case ItemRarity.Uncommon: return Color.green;
                case ItemRarity.Rare: return Color.blue;
                case ItemRarity.Epic: return new Color(0.6f, 0f, 1f); // Purple
                case ItemRarity.Legendary: return new Color(1f, 0.6f, 0f); // Orange
                default: return Color.white;
            }
        }
    }
}
