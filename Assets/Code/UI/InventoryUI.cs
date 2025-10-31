using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ForestSlice.Player;
using ForestSlice.Data;

namespace ForestSlice.UI
{
    /// <summary>
    /// Simple inventory UI displaying equipped items
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory playerInventory;

        [Header("Item Slots")]
        [SerializeField] private Image weaponIcon;
        [SerializeField] private Image armorIcon;
        [SerializeField] private Image consumableIcon;
        [SerializeField] private TextMeshProUGUI consumableCountText;

        [Header("Item Info")]
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI armorNameText;
        [SerializeField] private TextMeshProUGUI consumableNameText;

        [Header("Empty Slot")]
        [SerializeField] private Sprite emptySlotSprite;
        [SerializeField] private Color emptySlotColor = new Color(1f, 1f, 1f, 0.3f);

        private void Start()
        {
            if (playerInventory == null)
            {
                playerInventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
            }

            if (playerInventory != null)
            {
                // Subscribe to inventory changes
                playerInventory.OnInventoryChanged.AddListener(UpdateUI);
                UpdateUI();
            }
            else
            {
                Debug.LogWarning("InventoryUI: Player inventory not found!");
            }
        }

        /// <summary>
        /// Update all UI elements
        /// </summary>
        public void UpdateUI()
        {
            UpdateWeaponSlot();
            UpdateArmorSlot();
            UpdateConsumableSlot();
        }

        private void UpdateWeaponSlot()
        {
            ItemData weapon = playerInventory.EquippedWeapon;

            if (weapon != null)
            {
                if (weaponIcon != null)
                {
                    weaponIcon.sprite = weapon.icon;
                    weaponIcon.color = Color.white;
                }

                if (weaponNameText != null)
                {
                    weaponNameText.text = $"{weapon.itemName}\n+{weapon.damageBonus} DMG";
                    weaponNameText.color = weapon.GetRarityColor();
                }
            }
            else
            {
                // Empty slot
                if (weaponIcon != null)
                {
                    weaponIcon.sprite = emptySlotSprite;
                    weaponIcon.color = emptySlotColor;
                }

                if (weaponNameText != null)
                {
                    weaponNameText.text = "No Weapon";
                    weaponNameText.color = Color.gray;
                }
            }
        }

        private void UpdateArmorSlot()
        {
            ItemData armor = playerInventory.EquippedArmor;

            if (armor != null)
            {
                if (armorIcon != null)
                {
                    armorIcon.sprite = armor.icon;
                    armorIcon.color = Color.white;
                }

                if (armorNameText != null)
                {
                    armorNameText.text = $"{armor.itemName}\n+{armor.defenseBonus} DEF";
                    armorNameText.color = armor.GetRarityColor();
                }
            }
            else
            {
                if (armorIcon != null)
                {
                    armorIcon.sprite = emptySlotSprite;
                    armorIcon.color = emptySlotColor;
                }

                if (armorNameText != null)
                {
                    armorNameText.text = "No Armor";
                    armorNameText.color = Color.gray;
                }
            }
        }

        private void UpdateConsumableSlot()
        {
            ItemData consumable = playerInventory.EquippedConsumable;
            int count = playerInventory.ConsumableCount;

            if (consumable != null && count > 0)
            {
                if (consumableIcon != null)
                {
                    consumableIcon.sprite = consumable.icon;
                    consumableIcon.color = Color.white;
                }

                if (consumableNameText != null)
                {
                    consumableNameText.text = consumable.itemName;
                    consumableNameText.color = consumable.GetRarityColor();
                }

                if (consumableCountText != null)
                {
                    consumableCountText.text = $"x{count}";
                }
            }
            else
            {
                if (consumableIcon != null)
                {
                    consumableIcon.sprite = emptySlotSprite;
                    consumableIcon.color = emptySlotColor;
                }

                if (consumableNameText != null)
                {
                    consumableNameText.text = "No Consumable";
                    consumableNameText.color = Color.gray;
                }

                if (consumableCountText != null)
                {
                    consumableCountText.text = "";
                }
            }
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged.RemoveListener(UpdateUI);
            }
        }
    }
}
