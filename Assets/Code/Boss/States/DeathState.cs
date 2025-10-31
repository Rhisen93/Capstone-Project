using UnityEngine;

namespace ForestSlice.Boss.States
{
    /// <summary>
    /// Death state - plays death animation and drops loot
    /// </summary>
    public class DeathState : BossState
    {
        private float deathDuration = 2f;
        private float timer = 0f;
        private bool lootDropped = false;

        public DeathState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
        {
        }

        public override void Enter()
        {
            timer = 0f;
            lootDropped = false;
            boss.StopMovement();
            
            Debug.Log($"{boss.BossName} Death State");
        }

        public override void Execute()
        {
            timer += Time.deltaTime;

            // Drop loot after short delay
            if (!lootDropped && timer >= 0.5f)
            {
                DropLoot();
                lootDropped = true;
            }

            // Destroy boss after death animation
            if (timer >= deathDuration)
            {
                Object.Destroy(boss.gameObject);
            }
        }

        private void DropLoot()
        {
            // TODO: Integrate with loot system
            Debug.Log($"{boss.BossName} dropped boss loot!");
        }

        public override void Exit()
        {
            // Boss is dead
        }
    }
}
