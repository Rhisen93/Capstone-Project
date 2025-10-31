using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Base class for all enemy states. Provides the structure for state behavior.
    /// </summary>
    public abstract class EnemyState
    {
        protected EnemyController enemy;
        protected EnemyStateMachine stateMachine;

        public EnemyState(EnemyController enemy, EnemyStateMachine stateMachine)
        {
            this.enemy = enemy;
            this.stateMachine = stateMachine;
        }

        /// <summary>
        /// Called once when entering this state
        /// </summary>
        public virtual void Enter()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        public virtual void Execute()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called once when exiting this state
        /// </summary>
        public virtual void Exit()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called during FixedUpdate for physics-based movement
        /// </summary>
        public virtual void FixedExecute()
        {
            // Override in derived classes if physics movement needed
        }
    }
}
