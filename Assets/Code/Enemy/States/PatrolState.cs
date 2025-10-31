using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Patrol state - Enemy moves between waypoints
    /// </summary>
    public class PatrolState : EnemyState
    {
        private float waypointReachedDistance = 0.5f;

        public PatrolState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log($"{enemy.name} entered Patrol state");
            enemy.SetSpeed(enemy.PatrolSpeed);
        }

        public override void Execute()
        {
            // Check if player is in detection range
            if (enemy.CanSeePlayer())
            {
                stateMachine.ChangeState(enemy.ChaseState);
                return;
            }

            // If no patrol points, return to idle
            if (!enemy.HasPatrolPoints())
            {
                stateMachine.ChangeState(enemy.IdleState);
                return;
            }

            // Move towards current waypoint
            Vector2 targetWaypoint = enemy.GetCurrentPatrolPoint();
            enemy.MoveTowards(targetWaypoint);

            // Check if reached waypoint
            float distanceToWaypoint = Vector2.Distance(enemy.transform.position, targetWaypoint);
            if (distanceToWaypoint < waypointReachedDistance)
            {
                enemy.NextPatrolPoint();
            }
        }

        public override void Exit()
        {
            // Clean up if needed
        }
    }
}
