using System;
using System.Collections.Generic;
using DH.Core.Helpers.Extensions;
using UnityEngine;
using NaughtyAttributes;

namespace DH.Core.StateMachine
{
    public abstract class StateManager<EState> : Managers.GameManagerBase where EState : Enum
    {
        public Dictionary<EState, IState<EState>> States = new();

        public IState<EState> CurrentState
        {
            get => _currentStateInstance;
            set
            {
                _currentStateInstance = value;
                _currentState = _currentStateInstance.StateKey;
            }
        }

        public IState<EState> PreviousState
        {
            get => _previousStateInstance;
            private set
            {
                _previousStateInstance = value;
                _previousState = _previousStateInstance.StateKey;
            }
        }

        protected IState<EState> _currentStateInstance;
        protected IState<EState> _previousStateInstance;

        [Header("Basics")]
        [SerializeField, ReadOnly] protected EState _currentState;
        [SerializeField, ReadOnly] protected EState _previousState; 
        [SerializeField] Animator _animator;
        [SerializeField, ReadOnly] protected string _currentAnimationState;

        protected bool isTransitioningState = false;

        public bool IsCurrentState(params EState[] states)
        {
            foreach (var state in states)
            {
                if (CurrentState.StateKey.Equals(state))
                    return true;
            }
            return false;
        }

        public void ChangeAnimationState(string newState) => _animator.ChangeAnimationState(newState, out _currentAnimationState);

        public void ResetAnimationState() => _animator.ResetAnimationState();

        public void UnpauseAnimationState() => _animator.UnpauseAnimationState();

        public void PauseAnimationState() => _animator.PauseAnimationState();

        public bool IsAnimationFinished(string animationName) => _animator.IsAnimationFinished(animationName);

        public void Destroy(GameObject gameObject) => UnityEngine.Object.Destroy(gameObject);

        void Start()
        {
            Debug.Log("CurrentState: " + CurrentState.ToString());
            CurrentState.EnterState();
        }

        public virtual void Update()
        {
            EState nextStateKey = CurrentState.GetNextState();

            if (!isTransitioningState)
            {
                if (nextStateKey.Equals(CurrentState.StateKey))
                {
                    CurrentState.UpdateState();
                }
                else
                {
                    TransitionToState(nextStateKey);
                }
            }
        }

        void FixedUpdate()
        {
            if (!isTransitioningState)
                CurrentState.FixedUpdateState();
        }

        private void TransitionToState(EState stateKey)
        {
            isTransitioningState = true;
            CurrentState.ExitState();
            PreviousState = CurrentState;
            CurrentState = States[stateKey];
            CurrentState.EnterState();
            isTransitioningState = false;
            Debug.Log(CurrentState.ToString());
        }

        public void SwitchStateTo(EState stateKey)
        {
            TransitionToState(stateKey);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            CurrentState.OnTriggerEnter2D(other);
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            CurrentState.OnTriggerStay2D(other);
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            CurrentState.OnTriggerExit2D(other);
        }

        public virtual void OnDrawGizmos()
        {
            foreach (var (item, value) in States)
            {
                value.OnDrawGizmosGlobal();
            }
            try
            {
                CurrentState.OnDrawGizmosState();
            }
            catch { }
        }

    }
}
