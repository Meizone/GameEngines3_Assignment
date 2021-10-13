using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AbilitySlot : MonoBehaviour
{
    public delegate bool AvailabiltyChangeEvent(float cooldownPercent, bool isPayable);
    public AvailabiltyChangeEvent OnAvailabilityChanged;

    [SerializeField]
    private uint cooldown;
    [SerializeField]
    private Ability _ability;
    public Ability ability { get { return _ability; } }
    [SerializeField]
    private Combatant _combatant;
    public Combatant combatant { get { return _combatant; } }
    private bool isPayable = false;

    public void ChooseTarget()
    {
        if (CheckPayable())
        {
            _combatant.battle.StartChoosingTarget(this);
        }
    }

    public void OnTargetChosen(Combatant target)
    {
        _combatant.Pay(ability.costs);
        cooldown = ability.cooldown;

        combatant.battle.EventAbilityActivated(_combatant, target, _ability);
    }

    public void Update()
    {
        CheckPayable();
    }

    private bool CheckPayable()
    {
        if (cooldown == 0 && _combatant.CanPay(ability.costs))
        {
            if (!isPayable)
                OnAvailabilityChanged?.Invoke(cooldown / ability.cooldown, isPayable);
            return true;
        }
        else
        {
            if (isPayable)
                OnAvailabilityChanged?.Invoke(cooldown / ability.cooldown, isPayable);
            return false;
        }
    }

    public void Cooldown()
    {
        if (cooldown > 0)
        {
            --cooldown;
            OnAvailabilityChanged?.Invoke(cooldown / ability.cooldown, isPayable);
        }
    }

    internal void Register()
    {
        _combatant.battle.OnBattleStarted += Battle_OnBattleStarted;
        _combatant.battle.OnAbilityActivated += Battle_OnAbilityActivated;
        _combatant.battle.OnResourceChanged += Battle_OnResourceChanged;
        _combatant.battle.OnTurnPreStart += Battle_OnTurnPreStart;
        _combatant.battle.OnTurnStarted += Battle_OnTurnStarted;
        _combatant.battle.OnTurnEnded += Battle_OnTurnEnded;
    }

    internal void Deregister()
    {
        _combatant.battle.OnBattleStarted -= Battle_OnBattleStarted;
        _combatant.battle.OnAbilityActivated -= Battle_OnAbilityActivated;
        _combatant.battle.OnResourceChanged -= Battle_OnResourceChanged;
        _combatant.battle.OnTurnPreStart -= Battle_OnTurnPreStart;
        _combatant.battle.OnTurnStarted -= Battle_OnTurnStarted;
        _combatant.battle.OnTurnEnded -= Battle_OnTurnEnded;
    }

    private void Battle_OnBattleStarted(Combatant combatant, uint turn, uint turnInBattle)
    {
        ability.Battle_OnBattleStarted(this, combatant, turn, turnInBattle);
    }

    private void Battle_OnAbilityActivated(Combatant combatant, Combatant target, Ability ability)
    {
        ability.Battle_OnAbilityActivated(this, combatant, target, ability);
    }

    private void Battle_OnResourceChanged(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        ability.Battle_OnResourceChanged(this, combatant, resource, change, final);
    }

    private void Battle_OnTurnPreStart(Combatant combatant, uint turn, uint turnInBattle)
    {
        ability.Battle_OnTurnPreStart(this, combatant, turn, turnInBattle);
    }

    private void Battle_OnTurnStarted(Combatant combatant, uint turn, uint turnInBattle)
    {
        ability.Battle_OnTurnStarted(this, combatant, turn, turnInBattle);
    }

    private void Battle_OnTurnEnded(Combatant combatant, uint turn, uint turnInBattle)
    {
        ability.Battle_OnTurnEnded(this, combatant, turn, turnInBattle);
    }
}