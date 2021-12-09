using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public event BattleManager.TargettingEvent onTargettingStarted;
    public event BattleManager.TargettingEvent onTargettingCancelled;
    public event BattleManager.AbilityActivatedEvent onAbilityActivated;
    public event BattleManager.ResourceChangedEvent onResourceChanged;
    public event BattleManager.TurnChangedEvent onTurnStarted;
    public event BattleManager.TurnChangedEvent onTurnEnded;
    private BattleManager battle;
    [SerializeField]
    private Settings settings;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private CombatantUI combatantUIPrefab;
    [SerializeField]
    private Queue<CombatantUI> uiPool = new Queue<CombatantUI>();
    private Dictionary<Combatant, CombatantUI> combatantUIs = new Dictionary<Combatant, CombatantUI>();
    [SerializeField]
    private AbilityBar abilityBar;

    private void Start()
    {
        SetBattle(FindObjectOfType<BattleManager>());
        canvas.worldCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        foreach (KeyValuePair<Combatant, CombatantUI> kvp in combatantUIs)
        {
            if (kvp.Key == null)
            {
                ReturnCombatantUI(kvp.Value);
                continue;
            }
            if (kvp.Value == null)
            {
                OnCombatantAdded(kvp.Key);
            }

            kvp.Value.transform.position = kvp.Key.uiOffset.position;
        }
    }

    public void SetBattle(BattleManager battle)
    {
        if (this.battle != null)
        {
            Deregister();
        }
        this.battle = battle;
        Register();
        abilityBar.SetBattle(battle);
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        Deregister();
    }

    private void Register()
    {
        if (battle == null)
            return;

        battle.onCombatantAdded += OnCombatantAdded;
        battle.onCombatantRemoved += OnCombatantRemoved;
        battle.onTurnStarted += OnTurnStarted;
        battle.onTurnEnded += OnTurnEnded;
        battle.onTargettingStarted += OnTargettingStarted;
        battle.onTargettingCancelled += OnTargettingCancelled;
        battle.onAbilityActivated += OnAbilityActivated;
        battle.onResourceChanged += OnResourceChanged;
    }

    private void Deregister()
    {
        if (battle == null)
            return;

        battle.onCombatantAdded -= OnCombatantAdded;
        battle.onCombatantRemoved -= OnCombatantRemoved;
        battle.onTurnStarted -= OnTurnStarted;
        battle.onTurnEnded -= OnTurnEnded;
        battle.onTargettingStarted -= OnTargettingStarted;
        battle.onTargettingCancelled -= OnTargettingCancelled;
        battle.onAbilityActivated -= OnAbilityActivated;
        battle.onResourceChanged -= OnResourceChanged;
    }

    private void OnCombatantAdded(Combatant combatant)
    {
        Debug.Log("BattleUI.OnCombatantAdded");
        CombatantUI ui = GetCombatantUI();
        combatantUIs[combatant] = ui;
        ui.Init(this, settings, combatant);
    }

    private void OnCombatantRemoved(Combatant combatant)
    {
        ReturnCombatantUI(combatantUIs[combatant]);
        combatantUIs[combatant] = null;
    }

    private void OnTurnStarted(Combatant combatant, uint turn, uint turnInBattle)
    {
        onTurnStarted?.Invoke(combatant, turn, turnInBattle);
    }

    private void OnTurnEnded(Combatant combatant, uint turn, uint turnInBattle)
    {
        onTurnEnded?.Invoke(combatant, turn, turnInBattle);
    }

    private void OnTargettingStarted(AbilityData ability)
    {
        onTargettingStarted?.Invoke(ability);
    }

    private void OnTargettingCancelled(AbilityData ability)
    {
        onTargettingCancelled?.Invoke(ability);
    }

    private void OnAbilityActivated(AbilityData activatedAbility, Combatant target)
    {
        onAbilityActivated?.Invoke(activatedAbility, target);
    }

    private void OnResourceChanged(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        onResourceChanged?.Invoke(combatant, resource, change, final);
    }

    private CombatantUI GetCombatantUI()
    {
        if (uiPool.Count < 1)
            uiPool.Enqueue(Instantiate(combatantUIPrefab, canvas.transform));

        CombatantUI ui = uiPool.Dequeue();
        ui.gameObject.SetActive(true);
        return ui;
    }

    private void ReturnCombatantUI(CombatantUI ui)
    {
        ui.gameObject.SetActive(false);
        if (!uiPool.Contains(ui))
            uiPool.Enqueue(ui);
    }

    public ref readonly Combatant GetActiveCombatant()
    {
        return ref battle.GetActiveCombatant();
    }

    public ref readonly AbilityData GetActivatingAbility()
    {
        return ref battle.GetActivatingAbility();
    }

    public SelectionArrow.State DetermineSelectionState(Combatant combatant)
    {
        AbilityData ability = GetActivatingAbility();
        if (ability == null)
        {
            Combatant active = GetActiveCombatant();
            if (active == combatant)
                return SelectionArrow.State.Active;
            return SelectionArrow.State.Inactive;
        }
        else if (ability.combatant == combatant)
        {
            if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Self))
                return SelectionArrow.State.Ally;
            else
                return SelectionArrow.State.Active;
        }
        else if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Ally) && ability.combatant.faction == combatant.faction)
        {
            return SelectionArrow.State.Ally;
        }
        else if (ability.ability.targetType.HasFlag(Ability.SelectableTargets.Enemy) && ability.combatant.faction != combatant.faction)
        {
            return SelectionArrow.State.Enemy;
        }
        else
        {
            return SelectionArrow.State.Inactive;
        }
    }
}
