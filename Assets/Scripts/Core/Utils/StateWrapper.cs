using UnityEngine;
using System;

namespace CGDK.Core
{
    public struct StateChangeEvent<TState, TTarget> where TState : struct, IComparable, IConvertible, IFormattable
    {
        public TTarget Target;
        public TState NewState;
        public TState OldState;

        public StateChangeEvent(StateWrapper<TState, TTarget> stateWrapper)
        {
            Target = stateWrapper.Target;
            NewState = stateWrapper.CurState;
            OldState = stateWrapper.OldState;
        }
    }

    /// <summary>
    /// 简易状态机，使用 enum 类做作为泛型参数
    /// </summary>
    public class StateWrapper<TState, TTarget> where TState : struct, IComparable, IConvertible, IFormattable
    {
        public bool IsTriggerEvent;
        public TTarget Target;
        public TState CurState { get; protected set; }
        public TState OldState { get; protected set; }

        public StateWrapper(TState initState, TTarget target, bool isTriggerEvent = true)
        {
            OldState = initState;
            CurState = initState;
            Target = target;
            IsTriggerEvent = isTriggerEvent;
        }

        public virtual void ChangeState(TState newState)
        {
            if (newState.Equals(CurState)) return;

            OldState = CurState;
            CurState = newState;

            if (IsTriggerEvent)
            {
                EventBus.Trigger(new StateChangeEvent<TState, TTarget>(this));
            }
        }

        public virtual void RestoreState()
        {
            if (CurState.Equals(OldState)) return;
            CurState = OldState;

            if (IsTriggerEvent)
            {
                EventBus.Trigger(new StateChangeEvent<TState, TTarget>(this));
            }
        }
    }
}