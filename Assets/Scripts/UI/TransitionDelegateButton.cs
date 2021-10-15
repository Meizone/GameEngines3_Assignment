using UnityEngine.UI;

public class TransitionDelegateButton : Button
{
    private event System.Action<SelectionState, bool> StateTransitionEvent;
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        StateTransitionEvent?.Invoke(state, instant);
    }
}
