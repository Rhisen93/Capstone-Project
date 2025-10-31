using UnityEngine;

namespace ForestSlice.Boss.States
{
    /// <summary>
    /// Intro state - plays dialogue and transitions to Phase 1
    /// </summary>
    public class IntroState : BossState
    {
        private float introDuration = 3f;
        private float timer = 0f;

        public IntroState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
        {
        }

        public override void Enter()
        {
            timer = 0f;
            boss.StopMovement();
            boss.FacePlayer();
            
            Debug.Log($"{boss.BossName} Intro State");
        }

        public override void Execute()
        {
            timer += Time.deltaTime;

            // After intro duration, start Phase 1
            if (timer >= introDuration)
            {
                boss.StartPhase1();
            }
        }

        public override void Exit()
        {
            // Ready for combat
        }
    }
}
