using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Chase state - Enemy pursues the player
    /// </summary>
    public class ChaseState : EnemyState
    {
        public ChaseState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log($"{enemy.name} entered Chase state");
            enemy.SetSpeed(enemy.ChaseSpeed);
        }

        public override void Execute()
        {
            // Check if player is still alive and in range
            if (!enemy.CanSeePlayer())
            {
                // Lost sight of player, return to idle/patrol
                stateMachine.ChangeState(enemy.IdleState);
                return;
            }

            // Check if in attack range
            if (enemy.IsInAttackRange())
            {
                stateMachine.ChangeState(enemy.AttackState);
                return;
            }

            // Move towards player
            enemy.MoveTowardsPlayer();
        }

        public override void Exit()
        {
            // Clean up if needed
        }
    }
}
