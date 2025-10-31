using UnityEngine;

namespace ForestSlice.Boss
{
    /// <summary>
    /// Boss phase identifier
    /// </summary>
    public enum BossPhase
    {
        Inactive,
        Intro,
        Phase1,
        Phase2,
        Death
    }

    /// <summary>
    /// Base class for all boss states
    /// </summary>
    public abstract class BossState
    {
        protected BossController boss;
        protected BossStateMachine stateMachine;

        public BossState(BossController boss, BossStateMachine stateMachine)
        {
            this.boss = boss;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter()
        {
            // Override in derived classes
        }

        public virtual void Execute()
        {
            // Override in derived classes
        }

        public virtual void Exit()
        {
            // Override in derived classes
        }

        public virtual void FixedExecute()
        {
            // Override for physics
        }
    }
}
