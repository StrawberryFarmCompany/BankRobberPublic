using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IStateMachine
{
    interface IStateMachineBase<T> where T : IStateBase, new()
    {
        void ChangeState(T next);
        void ForceSet(T next);
    }
    interface IStateBase
    {
        void Enter();
        void Execute();
        void Exit();
    }

}

