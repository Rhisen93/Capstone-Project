using UnityEngine;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// State Machine specifically designed for enemy AI behavior.
    /// Manages state transitions and execution.
    /// </summary>
    public class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; }
        private EnemyController enemy;

        public EnemyStateMachine(EnemyController enemy)
        {
            this.enemy = enemy;
        }

        /// <summary>
        /// Initialize the state machine with a starting state
        /// </summary>
        public void Initialize(EnemyState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        /// <summary>
        /// Change to a new state
        /// </summary>
        public void ChangeState(EnemyState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = newState;
            
            if (CurrentState != null)
            {
                CurrentState.Enter();
            }
        }

        /// <summary>
        /// Execute the current state's logic every frame
        /// </summary>
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Execute();
            }
        }

        /// <summary>
        /// Execute the current state's physics logic
        /// </summary>
        public void FixedUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.FixedExecute();
            }
        }
    }
}
