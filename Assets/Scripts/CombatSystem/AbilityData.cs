using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AbilityData
{
    #region "Delegates and events"
    public delegate void AvailabiltyChangeEvent(uint cooldownValue, float cooldownPercent, bool isPayable);
    public AvailabiltyChangeEvent onAvailabilityChanged;
    #endregion

    #region "Member variables and properties"
    [SerializeField] private uint _cooldown;
    [SerializeField] private Ability _ability;
    private Combatant _combatant;

    public Ability ability { get { return _ability; }
        set
        {
            _ability = value;
            onAvailabilityChanged?.Invoke(_cooldown, cooldownPercent, isPayable);
        }
    }
    public Combatant combatant { get { return _combatant; } }
    public bool isPayable { get { return _cooldown == 0 && _combatant.CanPay(ability.costs); } }
    public bool isTargetted { get { return _ability.isTargetted; } }
    public uint cooldownValue { get { return _cooldown; } }
    public float cooldownPercent { get { return Mathf.Clamp(_cooldown / ability.cooldown, 0, 1); } }
    #endregion

    #region "Public functions"
    public AbilityData(Combatant combatant, Ability ability)
    {
        _combatant = combatant;
        _ability = ability;
        _cooldown = 0;
    }

    public bool Pay(Combatant target)
    {
        _combatant.Pay(ability.costs);
        _cooldown = ability.cooldown;
        return true;
    }

    internal void Register(Combatant.TurnStartedEvent onTurnStarted)
    {
        onTurnStarted += Cooldown;
        foreach (Trigger trigger in _ability.triggers)
            trigger.Register(this);
    }

    internal void Deregister(Combatant.TurnStartedEvent onTurnStarted)
    {
        onTurnStarted -= Cooldown;
        foreach (Trigger trigger in _ability.triggers)
            trigger.Deregister(this);
    }
    #endregion

    #region "Private functions"
    private void Cooldown()
    {
        if (_cooldown > 0)
        {
            --_cooldown;
            onAvailabilityChanged?.Invoke(_cooldown, cooldownPercent, isPayable);
        }
    }
    #endregion
}