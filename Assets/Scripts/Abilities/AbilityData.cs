using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AbilityData
{
    #region "Delegates and events"
    public delegate void AvailabiltyChangeEventHandler(uint cooldownValue, float cooldownPercent, bool isPayable);
    public AvailabiltyChangeEventHandler AvailabilityChangedEvent;
    #endregion

    #region "Member variables and properties"
    [SerializeField] private uint _cooldown;
    [SerializeField] private Ability _ability;
    private Combatant _combatant;

    public Ability ability { get { return _ability; } }
    public Combatant combatant { get { return _combatant; } }
    public bool isPayable { get { return _cooldown == 0 && _combatant.CanPay(ability.costs); } }
    public bool isTargetted { get { return _ability.isTargetted; } }
    public float cooldownPercent { get { return _cooldown / ability.cooldown; } }
    #endregion

    #region "Public functions"
    public bool Pay(Combatant target)
    {
        _combatant.Pay(ability.costs);
        _cooldown = ability.cooldown;
        return true;
    }
    #endregion

    #region "Private functions"
    private void Cooldown()
    {
        if (_cooldown > 0)
        {
            --_cooldown;
            AvailabilityChangedEvent?.Invoke(_cooldown, cooldownPercent, isPayable);
        }
    }
    #endregion
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
}