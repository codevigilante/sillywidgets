using System;

namespace SillyWidgets.Gizmos
{
    public interface IStateMachineGizmo<S, I>
    {
        void Transition(S toState, I input);
        void Accept(I input);
    }

    public interface IStateGizmo<I, C>
    {
        void Enter(I input, C context);
        void Accept(I input, C context);
        //void End(C context);
    }
}