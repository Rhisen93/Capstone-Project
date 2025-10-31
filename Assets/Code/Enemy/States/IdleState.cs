using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Idle state - Enemy is stationary and scanning for player
    /// </summary>
    public class IdleState : EnemyState
    {
        private float idleTimer;

        public IdleState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void Enter()
        {
            idleTimer = Random.Range(1f, 3f);
            enemy.StopMovement();
            Debug.Log($"{enemy.name} entered Idle state");
        }

        public override void Execute()
        {
            // Check if player is in detection range
            if (enemy.CanSeePlayer())
            {
                stateMachine.ChangeState(enemy.ChaseState);
                return;
            }

            // After idle timeout, start patrolling (if patrol enabled)
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f && enemy.HasPatrolPoints())
            {
                stateMachine.ChangeState(enemy.PatrolState);
            }
        }

        public override void Exit()
        {
            // Clean up if needed
        }
    }
}
