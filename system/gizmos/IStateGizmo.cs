using System;

namespace SillyWidgets.Gizmos
{
    public interface IStateMachineGizmo<A, S, I>
    {
        void Transition(S toState, I input);
        void Deposit(A artifact);
        void Accept(I input);
        void End();
    }

    public interface IStateGizmo<I, C>
    {
        void Enter(I input);
        void Accept(I input, C context);
        void End(C context);
    }
}