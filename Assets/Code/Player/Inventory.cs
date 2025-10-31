using UnityEngine;
using UnityEngine.Events;
using ForestSlice.Data;
using System.Collections.Generic;

namespace ForestSlice.Player
{
    /// <summary>
    /// Player inventory system with equipment slots and consumable
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [Header("Equipment Slots")]
        [SerializeField] private ItemData equippedWeapon;
        [SerializeField] private ItemData equippedArmor;
        [SerializeField] private ItemData equippedConsumable;

        [Header("Consumable Settings")]
        [SerializeField] private int consumableCount = 0;
        [SerializeField] private int maxConsumableStack = 5;

        // Events
        public UnityEvent<ItemData> OnItemEquipped = new UnityEvent<ItemData>();
        public UnityEvent<ItemData> OnItemUnequipped = new UnityEvent<ItemData>();
        public UnityEvent OnInventoryChanged = new UnityEvent();

        // Properties
        public ItemData EquippedWeapon => equippedWeapon;
        public ItemData EquippedArmor => equippedArmor;
        public ItemData EquippedConsumable => equippedConsumable;
        public int ConsumableCount => consumableCount;

        private PlayerController playerController;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        /// <summary>
        /// Add item to inventory (auto-equip or stack)
        /// </summary>
        public bool AddItem(ItemData item)
        {
            if (item == null) return false;

            switch (item.itemType)
            {
                case ItemType.Weapon:
                    EquipWeapon(item);
                    return true;

                case ItemType.Armor:
                    EquipArmor(item);
                    return true;

                case ItemType.Consumable:
                    return AddConsumable(item);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Equip weapon (replaces current)
        /// </summary>
        public void EquipWeapon(ItemData weapon)
        {
            if (weapon == null || weapon.itemType != ItemType.Weapon) return;

            if (equippedWeapon != null)
            {
                OnItemUnequipped?.Invoke(equippedWeapon);
            }

            equippedWeapon = weapon;
            OnItemEquipped?.Invoke(weapon);
            OnInventoryChanged?.Invoke();

            Debug.Log($"Equipped weapon: {weapon.itemName} (+{weapon.damageBonus} damage)");
        }

        /// <summary>
        /// Equip armor (replaces current)
        /// </summary>
        public void EquipArmor(ItemData armor)
        {
            if (armor == null || armor.itemType != ItemType.Armor) return;

            if (equippedArmor != null)
            {
                OnItemUnequipped?.Invoke(equippedArmor);
            }

            equippedArmor = armor;
            OnItemEquipped?.Invoke(armor);
            OnInventoryChanged?.Invoke();

            Debug.Log($"Equipped armor: {armor.itemName} (+{armor.defenseBonus} defense)");
        }

        /// <summary>
        /// Add consumable to stack
        /// </summary>
        public bool AddConsumable(ItemData consumable)
        {
            if (consumable == null || !consumable.isConsumable) return false;

            // First consumable or same type
            if (equippedConsumable == null || equippedConsumable == consumable)
            {
                if (consumableCount < maxConsumableStack)
                {
                    equippedConsumable = consumable;
                    consumableCount++;
                    OnInventoryChanged?.Invoke();
                    Debug.Log($"Added consumable: {consumable.itemName} ({consumableCount}/{maxConsumableStack})");
                    return true;
                }
                else
                {
                    Debug.Log($"Consumable stack full! ({maxConsumableStack})");
                    return false;
                }
            }
            else
            {
                Debug.Log($"Already have different consumable: {equippedConsumable.itemName}");
                return false;
            }
        }

        /// <summary>
        /// Use consumable (decreases stack)
        /// </summary>
        public void UseConsumable()
        {
            if (equippedConsumable == null || consumableCount <= 0) return;

            equippedConsumable.Use(gameObject);
            consumableCount--;

            if (consumableCount <= 0)
            {
                equippedConsumable = null;
            }

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Get total damage bonus from equipment
        /// </summary>
        public float GetTotalDamageBonus()
        {
            float bonus = 0f;
            if (equippedWeapon != null) bonus += equippedWeapon.damageBonus;
            return bonus;
        }

        /// <summary>
        /// Get total defense bonus from equipment
        /// </summary>
        public float GetTotalDefenseBonus()
        {
            float bonus = 0f;
            if (equippedArmor != null) bonus += equippedArmor.defenseBonus;
            return bonus;
        }

        /// <summary>
        /// Get total speed bonus from equipment
        /// </summary>
        public float GetTotalSpeedBonus()
        {
            float bonus = 0f;
            if (equippedWeapon != null) bonus += equippedWeapon.speedBonus;
            if (equippedArmor != null) bonus += equippedArmor.speedBonus;
            return bonus;
        }

        /// <summary>
        /// Clear all inventory
        /// </summary>
        public void ClearInventory()
        {
            equippedWeapon = null;
            equippedArmor = null;
            equippedConsumable = null;
            consumableCount = 0;
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Get inventory data for saving
        /// </summary>
        public InventorySaveData GetSaveData()
        {
            return new InventorySaveData
            {
                weaponName = equippedWeapon != null ? equippedWeapon.name : "",
                armorName = equippedArmor != null ? equippedArmor.name : "",
                consumableName = equippedConsumable != null ? equippedConsumable.name : "",
                consumableCount = consumableCount
            };
        }

        /// <summary>
        /// Load inventory from save data
        /// </summary>
        public void LoadFromSaveData(InventorySaveData data)
        {
            // Load items from Resources by name
            if (!string.IsNullOrEmpty(data.weaponName))
            {
                ItemData weapon = Resources.Load<ItemData>($"Items/{data.weaponName}");
                if (weapon != null) EquipWeapon(weapon);
            }

            if (!string.IsNullOrEmpty(data.armorName))
            {
                ItemData armor = Resources.Load<ItemData>($"Items/{data.armorName}");
                if (armor != null) EquipArmor(armor);
            }

            if (!string.IsNullOrEmpty(data.consumableName))
            {
                ItemData consumable = Resources.Load<ItemData>($"Items/{data.consumableName}");
                if (consumable != null)
                {
                    equippedConsumable = consumable;
                    consumableCount = data.consumableCount;
                }
            }

            OnInventoryChanged?.Invoke();
        }
    }

    /// <summary>
    /// Serializable inventory data for saving
    /// </summary>
    [System.Serializable]
    public class InventorySaveData
    {
        public string weaponName;
        public string armorName;
        public string consumableName;
        public int consumableCount;
    }
}
