using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Resource.Type;

public class BattleManager : MonoBehaviour
{
    #region "Delegates, events, related things"
    public delegate void CombatantChangedEvent(Combatant combatant);
    public delegate void TargettingEvent(AbilityData ability);
    public delegate void AbilityActivatedEvent(AbilityData activatedAbility, Combatant target);
    public delegate void ResourceChangedEvent(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final);
    public delegate void TurnChangedEvent(Combatant combatant, uint turn, uint turnInBattle);
    public event CombatantChangedEvent onCombatantAdded;
    public event CombatantChangedEvent onCombatantRemoved;
    public event TargettingEvent onTargettingStarted;
    public event TargettingEvent onTargettingCancelled;
    public event AbilityActivatedEvent onAbilityActivated;
    public event ResourceChangedEvent onResourceChanged;
    public event TurnChangedEvent onBattleStarted;
    public event TurnChangedEvent onBeforeTurnStarted;
    public event TurnChangedEvent onTurnStarted;
    public event TurnChangedEvent onTurnEnded;

    [System.Flags] public enum Events
    {
        BattleStarted = (1 << 0),
        AbilityActivated = (1 << 1),
        TurnPreStart = (1 << 2),
        TurnStarted = (1 << 3),
        TurnEnded = (1 << 4),
        ResourceChanged = (1 << 5),
    }
    #endregion
    public enum ExitState { Victory, Loss, Tie }

    private LinkedList<Combatant> _combatants = new LinkedList<Combatant>();
    public LinkedList<Combatant> combatants { get { return _combatants; } }

    private Combatant activeCombatant;
    private AbilityData activatingAbility;
    private float waitingSpeedScale = 1.0f;
    private float activeSpeedScale = 0.0f;
    private uint turn;
    public bool isTargetting { get; private set; } = false;

    private void Start()
    {
        StartBattle(FindObjectsOfType<Combatant>());
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
            onBattleStarted?.Invoke(combatant, 0, 0);
    }

    public void AddCombatant(Combatant combatant)
    {
        _combatants.AddLast(combatant);
        combatant.StartBattle(this);
        onCombatantAdded?.Invoke(combatant);
    }

    private void Update()
    {
        float scaledSpeed = activeCombatant == null ? waitingSpeedScale : activeSpeedScale;

        if (_combatants.Count <= 0)
            return;

        Combatant mostReady = _combatants.First.Value;
        foreach (Combatant combatant in _combatants)
        {
            combatant.UpdateReadiness(scaledSpeed);
            if (combatant.readiness > mostReady.readiness)
                mostReady = combatant;
        }

        if (activeCombatant == null && mostReady.readiness > 100)
        {
            StartTurn(mostReady);
        }
    }

    private void StartTurn(Combatant combatant)
    {
        if (combatant == null || combatant.gameObject == null || !combatant.gameObject.activeInHierarchy)
            return;

        ++turn;
        activeCombatant = combatant;
        onBeforeTurnStarted?.Invoke(activeCombatant, activeCombatant.turn, turn);
        activeCombatant.StartTurn();
        onTurnStarted?.Invoke(activeCombatant, activeCombatant.turn, turn);
        if (!activeCombatant.isPlayerControlled)
        {
            activeCombatant.ChooseRandomAbility();
        }
    }

    public ref readonly Combatant GetActiveCombatant()
    {
        return ref activeCombatant;
    }

    public ref readonly AbilityData GetActivatingAbility()
    {
        return ref activatingAbility;
    }

    public void ExitCombat(ExitState exitState)
    {
        turn = 0;
        Debug.LogFormat("<color=yellow>Exiting combat with exit state {0} has not yet been implemented.</color>", exitState);
        LevelLoader.LoadLevel(LevelLoader.previousLevel);
        _combatants.Clear();
    }

    public void EventResourceChanged(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        onResourceChanged?.Invoke(combatant, resource, change, final);
    }

    public void StartChoosingTarget(AbilityData ability)
    {
        isTargetting = true;
        activatingAbility = ability;
        onTargettingStarted?.Invoke(ability);

        if (!ability.isTargetted)
            EventTargetChosen(activeCombatant);

        bool clear = false;
        foreach (Combatant combatant in _combatants)
        {
            if (combatant == null)
            {
                clear = true;
                continue;
            }
            if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Self) && combatant == ability.combatant)
                combatant.SetTargettable(true);
            else if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Ally) && combatant.faction == ability.combatant.faction)
                combatant.SetTargettable(true);
            else if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Enemy) && combatant.faction != ability.combatant.faction)
                combatant.SetTargettable(true);
            else
                combatant.SetTargettable(false);

        }
        if (clear)
        {
            _combatants.Clear();
            foreach (Combatant combatant in FindObjectsOfType<Combatant>())
            {
                _combatants.AddLast(combatant);
            }            
        }
    }

    public void ChooseRandomTarget(AbilityData ability)
    {
        LinkedList<Combatant> possibleTargets = new LinkedList<Combatant>();
        activatingAbility = ability;
        foreach (Combatant combatant in _combatants)
        {
            if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Self) && combatant == ability.combatant)
                possibleTargets.AddLast(combatant);
            else if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Ally) && combatant.faction == ability.combatant.faction)
                possibleTargets.AddLast(combatant);
            else if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Enemy) && combatant.faction != ability.combatant.faction)
                possibleTargets.AddLast(combatant);
        }

        if (possibleTargets.Count == 0)
        {
            EventTargetChosen(ability.combatant);
        }
        else
        {
            int i = UnityEngine.Random.Range(0, possibleTargets.Count - 1);
            EventTargetChosen(possibleTargets.ElementAt(i));
        }
        isTargetting = false;
    }

    public void CancelChoosingTarget()
    {
        activatingAbility = null;
        isTargetting = false;
    }

    public void EventTargetChosen(Combatant target)
    {
        if (activatingAbility != null)
            if (activatingAbility.Pay(target))
                onAbilityActivated?.Invoke(activatingAbility, target);
        onTurnEnded?.Invoke(activeCombatant, activeCombatant.turn, turn);
        activeCombatant = null;
        isTargetting = false;
    }
}