using UnityEngine;
using System;

namespace DH.Core.StateMachine
{
    public abstract class BaseStateSingleton<EState, T> : BaseState<EState, T> where T : StateManager<EState> where EState : Enum
    {
        public static BaseState<EState, T> Instance { get; private set; }
        public BaseStateSingleton( T stateManager) : base( stateManager)
        {
            Instance = this;
        }
    }
}
