using UnityEngine;
using ForestSlice.Boss.States;

namespace ForestSlice.Boss.Tharion
{
    /// <summary>
    /// Tharion - First boss encounter
    /// Phase 1: Melee attacks and charges
    /// Phase 2: Enhanced attacks with ground slam AoE
    /// </summary>
    public class TharionBoss : BossController
    {
        [Header("Tharion Settings")]
        [SerializeField] private float phase1MoveSpeed = 4f;
        [SerializeField] private float phase2MoveSpeed = 5f;

        protected override void Awake()
        {
            // Set boss info
            bossName = "Tharion the Cursed";
            bossId = "tharion";

            // Set narrative IDs
            introDialogueId = "boss_intro_tharion";
            phase2DialogueId = "boss_phase2_tharion";
            defeatDialogueId = "boss_defeat_tharion";

            base.Awake();
        }

        protected override void InitializeStates()
        {
            // Create all states
            IntroState = new IntroState(this, stateMachine);
            Phase1State = new TharionPhase1State(this, stateMachine);
            Phase2State = new TharionPhase2State(this, stateMachine);
            DeathState = new DeathState(this, stateMachine);
        }

        public override void StartPhase1()
        {
            base.StartPhase1();
            moveSpeed = phase1MoveSpeed;
        }

        public override void StartPhase2()
        {
            base.StartPhase2();
            moveSpeed = phase2MoveSpeed; // Faster in Phase 2
        }

        protected override void OnEnrage()
        {
            base.OnEnrage();
            // Increase attack speed or damage
            attackDamage *= 1.2f;
            Debug.Log($"{bossName} is enraged! +20% damage!");
        }
    }
}
