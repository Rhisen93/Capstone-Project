using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Attack state - Enemy performs attack actions
    /// </summary>
    public class AttackState : EnemyState
    {
        private float attackTimer;

        public AttackState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log($"{enemy.name} entered Attack state");
            enemy.StopMovement();
            attackTimer = 0f; // Ready to attack immediately
        }

        public override void Execute()
        {
            // Check if player is still in range and alive
            if (!enemy.CanSeePlayer())
            {
                stateMachine.ChangeState(enemy.IdleState);
                return;
            }

            if (!enemy.IsInAttackRange())
            {
                stateMachine.ChangeState(enemy.ChaseState);
                return;
            }

            // Face the player
            enemy.FaceTarget(enemy.PlayerTransform.position);

            // Attack cooldown
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                enemy.PerformAttack();
                attackTimer = enemy.AttackCooldown;
            }
        }

        public override void Exit()
        {
            // Clean up if needed
        }
    }
}
