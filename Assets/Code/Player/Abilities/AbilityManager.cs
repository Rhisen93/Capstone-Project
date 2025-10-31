using UnityEngine;

namespace ForestSlice.Player.Abilities
{
    [System.Serializable]
    public class AbilitySlot
    {
        public AbilityBase ability;
        public float cooldownTimer;
        public bool IsReady => cooldownTimer <= 0f;

        public void UpdateCooldown(float deltaTime)
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= deltaTime;
            }
        }

        public void StartCooldown()
        {
            if (ability != null)
            {
                cooldownTimer = ability.cooldown;
            }
        }

        public float GetCooldownPercent()
        {
            if (ability == null || ability.cooldown <= 0f) return 0f;
            return Mathf.Clamp01(cooldownTimer / ability.cooldown);
        }
    }

    public class AbilityManager : MonoBehaviour
    {
        [Header("Ability Slots")]
        [SerializeField] private AbilitySlot[] abilitySlots = new AbilitySlot[4];

        [Header("Energy System")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float currentEnergy = 100f;
        [SerializeField] private float energyRegenRate = 5f;

        private PlayerController playerController;

        public float CurrentEnergy => currentEnergy;
        public float MaxEnergy => maxEnergy;
        public float EnergyPercent => currentEnergy / maxEnergy;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            
            if (abilitySlots == null || abilitySlots.Length == 0)
            {
                abilitySlots = new AbilitySlot[4];
                for (int i = 0; i < abilitySlots.Length; i++)
                {
                    abilitySlots[i] = new AbilitySlot();
                }
            }
        }

        private void Update()
        {
            UpdateCooldowns();
            RegenerateEnergy();
        }

        private void UpdateCooldowns()
        {
            foreach (var slot in abilitySlots)
            {
                slot.UpdateCooldown(Time.deltaTime);
            }
        }

        private void RegenerateEnergy()
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += energyRegenRate * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
            }
        }

        public bool TryExecuteAbility(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= abilitySlots.Length)
            {
                Debug.LogWarning($"Invalid ability slot index: {slotIndex}");
                return false;
            }

            AbilitySlot slot = abilitySlots[slotIndex];

            if (slot.ability == null)
            {
                Debug.LogWarning($"No ability assigned to slot {slotIndex}");
                return false;
            }

            if (!slot.IsReady)
            {
                Debug.Log($"Ability {slot.ability.abilityName} is on cooldown");
                return false;
            }

            if (currentEnergy < slot.ability.energyCost)
            {
                Debug.Log($"Not enough energy for {slot.ability.abilityName}");
                return false;
            }

            if (!slot.ability.CanExecute(playerController))
            {
                return false;
            }

            slot.ability.Execute(playerController);
            slot.StartCooldown();
            ConsumeEnergy(slot.ability.energyCost);

            return true;
        }

        private void ConsumeEnergy(float amount)
        {
            currentEnergy -= amount;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        }

        public void SetAbility(int slotIndex, AbilityBase ability)
        {
            if (slotIndex >= 0 && slotIndex < abilitySlots.Length)
            {
                if (abilitySlots[slotIndex] == null)
                {
                    abilitySlots[slotIndex] = new AbilitySlot();
                }
                abilitySlots[slotIndex].ability = ability;
            }
        }

        public AbilitySlot GetAbilitySlot(int index)
        {
            if (index >= 0 && index < abilitySlots.Length)
            {
                return abilitySlots[index];
            }
            return null;
        }
    }
}
