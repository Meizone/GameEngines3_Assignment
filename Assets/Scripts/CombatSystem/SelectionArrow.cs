using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    public enum State
    {
        Active,
        Inactive,
        Ally,
        Enemy,
    }

    [SerializeField] private Image arrow;
    [SerializeField] private State _state;
    [SerializeField] private Color _activeSelectionColour;
    [SerializeField] private Color _inactiveSelectionColour;
    [SerializeField] private Color _allySelectionColour;
    [SerializeField] private Color _enemySelectionColour;
    public State state { get { return _state; } set { _state = value; UpdateColour(); } }
    public Color activeSelectionColour { get { return _activeSelectionColour; } set { _activeSelectionColour = value; UpdateColour(); } }
    public Color inactiveSelectionColour { get { return _inactiveSelectionColour; } set { _inactiveSelectionColour = value; UpdateColour(); } }
    public Color allySelectionColour { get { return _allySelectionColour; } set { _allySelectionColour = value; UpdateColour(); } }
    public Color enemySelectionColour { get { return _enemySelectionColour; } set { _enemySelectionColour = value; UpdateColour(); } }

    private void UpdateColour()
    {
        switch (state)
        {
            case State.Active:
                arrow.color = _activeSelectionColour;
                break;
            case State.Inactive:
                arrow.color = _inactiveSelectionColour;
                break;
            case State.Ally:
                arrow.color = _allySelectionColour;
                break;
            case State.Enemy:
                arrow.color = _enemySelectionColour;
                break;
            default:
                break;
        }
    }
}