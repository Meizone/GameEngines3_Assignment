using UnityEngine.UI;

public class TransitionDelegateToggle : Toggle
{
    private event System.Action<SelectionState, bool> StateTransitionEvent;
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        StateTransitionEvent?.Invoke(state, instant);
        Toggle t;
        Button b;
        ToggleGroup g;
    }
}
