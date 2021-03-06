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
    private LinkedList<Combatant> _deadCombatants = new LinkedList<Combatant>();
    public LinkedList<Combatant> combatants { get { return _combatants; } }

    private Combatant activeCombatant;
    private AbilityData activatingAbility;
    private float waitingSpeedScale = 1.0f;
    private float activeSpeedScale = 0.0f;
    private uint turn;
    private bool hasStarted = false;
    private bool hasEnded = false;
    public bool isTargetting { get; private set; } = false;
    [SerializeField] private string VictorySound;
    [SerializeField] private string LossSound;
    [SerializeField] private string TieSound;

    private void Clear()
    {
        onCombatantAdded = null;
        onCombatantRemoved = null;
        onTargettingStarted = null;
        onTargettingCancelled = null;
        onAbilityActivated = null;
        onResourceChanged = null;
        onBattleStarted = null;
        onBeforeTurnStarted = null;
        onTurnStarted = null;
        onTurnEnded = null;
    }

    private void OnDestroy()
    {
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
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

        hasStarted = true;
    }

    public void AddCombatant(Combatant combatant)
    {
        if (_combatants.Contains(combatant))
            return;

        _combatants.AddLast(combatant);
        combatant.StartBattle(this);
        onCombatantAdded?.Invoke(combatant);
    }

    public LinkedList<uint> GetFactions()
    {
        LinkedList<uint> factionsInScenario = new LinkedList<uint>();
        foreach (Combatant combatant in _combatants)
        {
            if (!factionsInScenario.Contains(combatant.faction))
                factionsInScenario.AddLast(combatant.faction);
        }
        return factionsInScenario;
    }

    private void Update()
    {
        if (!hasStarted)
        {
            StartBattle(FindObjectsOfType<Combatant>());
            return;
        }
        if (hasEnded)
        {
            return;
        }

        float scaledSpeed = activeCombatant == null ? waitingSpeedScale : activeSpeedScale;
        
        foreach (Combatant corpse in _deadCombatants)
        {
            _combatants.Remove(corpse);
            onCombatantRemoved?.Invoke(corpse);
            corpse.EndBattle(this);
        }
        _deadCombatants.Clear();

        if (_combatants.Count < 1 || _combatants.First == null || _combatants.First.Value == null)
        {
            Debug.LogError("There were no combatants in the combat scenario.");
            ExitCombat(ExitState.Tie);
            return;
        }
        else
        {
            LinkedList<uint> factionsInScenario = GetFactions();
            if (factionsInScenario.Count == 0)
            {
                ExitCombat(ExitState.Tie);
                return;
            }
            else if (factionsInScenario.Count == 1)
            {
                ExitCombat(factionsInScenario.First.Value == 1 ? ExitState.Victory : ExitState.Loss);
                return;
            }
        }

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
        
        foreach (Combatant combatant in _combatants)
            combatant.EndBattle(this);
        _combatants.Clear();

        switch (exitState)
        {
            case ExitState.Victory:
                AudioManager.Play(VictorySound);
                break;
            case ExitState.Loss:
                AudioManager.Play(LossSound);
                break;
            case ExitState.Tie:
                AudioManager.Play(TieSound);
                break;
            default:
                break;
        }

        hasEnded = true;
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

    internal void OnCombatantDied(Combatant caller, float aether)
    {
        _deadCombatants.AddLast(caller);

        LinkedList<Combatant> rewardees = new LinkedList<Combatant>();
        foreach (Combatant combatant in _combatants)
        {
            if (combatant.faction != caller.faction)
            {
                rewardees.AddLast(combatant);
            }
        }

        float divviedValue = aether / rewardees.Count;
        foreach (Combatant rewardee in rewardees)
        {
            rewardee.Pay(new Payment(Resource.Type.Aether, divviedValue));
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