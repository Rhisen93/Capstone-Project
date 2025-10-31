using UnityEngine;

namespace ForestSlice.Core
{
    public abstract class State
    {
        protected StateMachine stateMachine;

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }

    public class StateMachine
    {
        private State currentState;

        public State CurrentState => currentState;

        public void ChangeState(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }
    }
}
