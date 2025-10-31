using UnityEngine;

namespace ForestSlice.Boss
{
    /// <summary>
    /// State machine for boss behavior
    /// </summary>
    public class BossStateMachine
    {
        public BossState CurrentState { get; private set; }
        private BossController boss;

        public BossStateMachine(BossController boss)
        {
            this.boss = boss;
        }

        public void Initialize(BossState startingState)
        {
            CurrentState = startingState;
            CurrentState?.Enter();
        }

        public void ChangeState(BossState newState)
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

        public void Update()
        {
            CurrentState?.Execute();
        }

        public void FixedUpdate()
        {
            CurrentState?.FixedExecute();
        }
    }
}
