using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resource.Type;

public class BattleManager : MonoBehaviour
{
    public delegate void AbilityActivatedEvent(Combatant combatant, Combatant target, Ability ability);
    public delegate void ResourceChangedEvent(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final);
    public delegate void TurnChangedEvent(Combatant combatant, uint turn, uint turnInBattle);
    public event AbilityActivatedEvent OnAbilityActivated;
    public event ResourceChangedEvent OnResourceChanged;
    public event TurnChangedEvent OnTurnPreStart;
    public event TurnChangedEvent OnTurnStarted;
    public event TurnChangedEvent OnTurnEnded;
    public event TurnChangedEvent OnBattleStarted;

    public enum ExitState { Victory, Loss, Tie }

    private LinkedList<Combatant> _combatants;
    public LinkedList<Combatant> combatants { get { return combatants; } }

    private Combatant activeCombatant;
    private AbilitySlot activatingAbility;
    private float waitingSpeedScale = 1.0f;
    private float activeSpeedScale = 0.0f;
    private uint turn;

    private void Awake()
    {
        _combatants = new LinkedList<Combatant>();
    }

    public void StartBattle(IEnumerable<Combatant> combatants)
    {
        if (turn != 0)
        {
            Debug.LogFormat("<color=yellow>Attempting to start battle while battle is already in progress.</color>");
            return;
        }

        if (_combatants == null)
            _combatants = new LinkedList<Combatant>();
        else
            _combatants.Clear();

        foreach (Combatant combatant in combatants)
            AddCombatant(combatant);

        foreach (Combatant combatant in _combatants)
            OnBattleStarted?.Invoke(combatant, 0, 0);
    }

    public void AddCombatant(Combatant combatant)
    {
        _combatants.AddLast(combatant);
        combatant.StartBattle(this);
    }

    private void Update()
    {
        float speedScale = activeCombatant == null ? waitingSpeedScale : activeSpeedScale;

        if (_combatants.Count <= 0)
            return;

        Combatant mostReady = _combatants.First.Value;
        foreach (Combatant combatant in _combatants)
        {
            combatant.UpdateReadiness(speedScale);
            if (combatant.readiness > mostReady.readiness)
                mostReady = combatant;
        }

        if (mostReady.readiness > 100)
        {
            StartTurn(mostReady);
        }
    }

    private void StartTurn(Combatant combatant)
    {
        ++turn;
        activeCombatant = combatant;
        OnTurnPreStart?.Invoke(activeCombatant, activeCombatant.turn, turn);
        activeCombatant.StartTurn();
        OnTurnStarted?.Invoke(activeCombatant, activeCombatant.turn, turn);
    }

    public void ExitCombat(ExitState exitState)
    {
        turn = 0;
        Debug.LogFormat("<color=yellow>Exiting combat with exit state {0} has not yet been implemented.</color>", exitState);
    }

    public void EventResourceChanged(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        OnResourceChanged?.Invoke(combatant, resource, change, final);
    }

    public void EventAbilityActivated(Combatant combatant, Combatant target, Ability ability)
    {
        OnAbilityActivated?.Invoke(combatant, target, ability);
    }

    public void StartChoosingTarget(AbilitySlot slot)
    {
        activatingAbility = slot;

        Debug.LogFormat("<color=red>Target choosing not yet implemented!");
        switch (activatingAbility.ability.targetType)
        {
            case Ability.TargetType.Ally:
                break;
            case Ability.TargetType.Enemy:
                break;
            case Ability.TargetType.Self:
            default:
                break;
        }

        EventTargetChosen(activatingAbility.combatant);
    }

    public void CancelChoosingTarget()
    {
        activatingAbility = null;
    }

    public void EventTargetChosen(Combatant target)
    {
        if (activatingAbility != null)
            activatingAbility.OnTargetChosen(target);
    }
}