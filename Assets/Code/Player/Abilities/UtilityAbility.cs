using UnityEngine;
using ForestSlice.Core;
using System.Collections;

namespace ForestSlice.Player.Abilities
{
    public enum UtilityType
    {
        Heal,
        Shield,
        SpeedBoost
    }

    [CreateAssetMenu(fileName = "UtilityAbility", menuName = "ForestSlice/Abilities/Utility")]
    public class UtilityAbility : AbilityBase
    {
        [Header("Utility Settings")]
        public UtilityType utilityType = UtilityType.Heal;
        
        [Header("Heal Settings")]
        public float healAmount = 30f;

        [Header("Shield Settings")]
        public float shieldDuration = 3f;

        [Header("Speed Boost Settings")]
        public float speedMultiplier = 1.5f;
        public float boostDuration = 5f;

        public override void Execute(PlayerController player)
        {
            switch (utilityType)
            {
                case UtilityType.Heal:
                    ExecuteHeal(player);
                    break;
                case UtilityType.Shield:
                    ExecuteShield(player);
                    break;
                case UtilityType.SpeedBoost:
                    ExecuteSpeedBoost(player);
                    break;
            }

            if (castVFX != null)
            {
                Instantiate(castVFX, player.transform.position, Quaternion.identity);
            }
        }

        private void ExecuteHeal(PlayerController player)
        {
            Health health = player.GetComponent<Health>();
            if (health != null)
            {
                health.Heal(healAmount);
                Debug.Log($"Healed {healAmount} HP");
            }
        }

        private void ExecuteShield(PlayerController player)
        {
            Health health = player.GetComponent<Health>();
            if (health != null)
            {
                player.StartCoroutine(ShieldRoutine(health));
            }
        }

        private IEnumerator ShieldRoutine(Health health)
        {
            health.SetInvulnerable(true);
            Debug.Log($"Shield active for {shieldDuration}s");
            
            yield return new WaitForSeconds(shieldDuration);
            
            health.SetInvulnerable(false);
            Debug.Log("Shield expired");
        }

        private void ExecuteSpeedBoost(PlayerController player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                player.StartCoroutine(SpeedBoostRoutine(player));
            }
        }

        private IEnumerator SpeedBoostRoutine(PlayerController player)
        {
            Debug.Log($"Speed boost active for {boostDuration}s");
            
            yield return new WaitForSeconds(boostDuration);
            
            Debug.Log("Speed boost expired");
        }

        public override bool CanExecute(PlayerController player)
        {
            if (utilityType == UtilityType.Heal)
            {
                Health health = player.GetComponent<Health>();
                if (health != null && health.CurrentHealth >= health.MaxHealth)
                {
                    Debug.Log("Already at full health");
                    return false;
                }
            }
            return true;
        }
    }
}
