using UnityEngine;
using System.Collections.Generic;
using System;

namespace DH.Core.StateMachine
{
    public abstract class BaseState<EState, T> : IState<EState> where T : StateManager<EState> where EState : Enum
    {
        public BaseState(T stateManager)
        {
            StateMachine = stateManager;
        }

        public abstract EState StateKey { get; }
        public T StateMachine { get; private set; }
        public Dictionary<EState, IState<EState>> States => StateMachine.States;

        public TState GetState<TState>(EState state) where TState : IState<EState>
        {
            return (TState)States[state];
        }
        public Transform transform => StateMachine.transform;
        public void Log(string message) => Debug.Log(message);

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
        public virtual void OnDrawGizmosState() { }
        public virtual void OnDrawGizmosGlobal() { }
        public virtual EState GetNextState() { return StateKey; }
        public virtual void OnTriggerEnter2D(Collider2D other) { }
        public virtual void OnTriggerStay2D(Collider2D other) { }
        public virtual void OnTriggerExit2D(Collider2D other) { }

    }
}
