using UnityEngine;

namespace ForestSlice.Player.Abilities
{
    public enum AbilityType
    {
        Projectile,
        DashStrike,
        AoE,
        Utility
    }

    [CreateAssetMenu(fileName = "NewAbility", menuName = "ForestSlice/Abilities/Ability Base")]
    public class AbilityBase : ScriptableObject
    {
        [Header("Base Info")]
        public string abilityName = "New Ability";
        [TextArea(2, 4)]
        public string description = "Ability description";
        public AbilityType type;
        public Sprite icon;

        [Header("Stats")]
        public float cooldown = 1f;
        public float damage = 20f;
        public float range = 5f;
        public float energyCost = 10f;

        [Header("VFX")]
        public GameObject castVFX;
        public GameObject impactVFX;

        public virtual void Execute(PlayerController player)
        {
            Debug.Log($"Executing ability: {abilityName}");
        }

        public virtual bool CanExecute(PlayerController player)
        {
            return true;
        }
    }
}
