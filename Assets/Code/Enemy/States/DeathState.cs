using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Death state - Enemy has died
    /// </summary>
    public class DeathState : EnemyState
    {
        private float deathDelay = 2f;
        private float deathTimer;

        public DeathState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log($"{enemy.name} entered Death state");
            
            // Stop all movement
            enemy.StopMovement();
            enemy.DisableCollision();
            
            // Start death animation/VFX if available
            // TODO: Play death animation
            // TODO: Spawn death VFX
            
            deathTimer = deathDelay;
        }

        public override void Execute()
        {
            // Wait for death animation/effects to finish
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0f)
            {
                // Destroy enemy GameObject
                Object.Destroy(enemy.gameObject);
            }
        }

        public override void Exit()
        {
            // This state is terminal, no exit
        }
    }
}
