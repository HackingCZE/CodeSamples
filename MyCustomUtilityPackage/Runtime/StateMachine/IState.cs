using System;
using UnityEngine;

namespace DH.Core.StateMachine
{
    public interface IState<EState> where EState : Enum
    {

        public EState StateKey { get; }
        public void EnterState() { }
        public void ExitState() { }
        public void UpdateState() { }
        public void FixedUpdateState() { }
        public void OnDrawGizmosState() { }
        public void OnDrawGizmosGlobal() { }
        public EState GetNextState() { return StateKey; }
        public void OnTriggerEnter2D(Collider2D other) { }
        public void OnTriggerStay2D(Collider2D other) { }
        public void OnTriggerExit2D(Collider2D other) { }

    }
}