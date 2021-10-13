using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System;

[System.Serializable, CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject, IDescribable
{
    [System.Flags] public enum TargetType
    {
        Self = (1 << 0),
        Ally = (1 << 1),
        Enemy = (1 << 2),
    }
    public enum Animation { RaiseWeapon, SwingWeapon, Run }

    [SerializeField] private string _abilityName;
    [SerializeField] private uint _cooldown;
    [SerializeField] private Sprite _icon;
    [SerializeField] private TargetType _targetType;
    [SerializeField, TextArea] private string _description;
    [SerializeField] private Payment[] _costs;
    [SerializeField] private Trigger[] _triggers;

    public string abilityName { get { return _abilityName; } }
    public uint cooldown { get { return _cooldown; } }
    public Sprite icon { get { return _icon; } }
    public TargetType targetType { get { return _targetType; } }
    public Payment[] costs { get { return _costs; } }

    public bool isTargetted { 
        get {
            if (_targetType.HasFlag(TargetType.Ally | TargetType.Enemy))
            {
                foreach (Trigger effect in _triggers)
                    if (effect.HasTrigger(Trigger.Events.SkillActivated))
                        return true;
            }
            return false; } }

    public string GetDescription()
    {
        return string.Format(_description, _triggers);
    }

    #region Callbacks
    public void Battle_OnBattleStarted(AbilitySlot slot, Combatant combatant, uint turn, uint turnInBattle)
    {
        throw new NotImplementedException();
    }

    public void Battle_OnAbilityActivated(AbilitySlot slot, Combatant combatant, Combatant target, Ability ability)
    {
        foreach (Trigger effect in _triggers)
        {
            if (effect.HasTrigger(Trigger.Events.BattleStarted))
            {
                effect.Activate(slot, combatant, target);
            }
        }
    }

    public void Battle_OnResourceChanged(AbilitySlot slot, Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        throw new NotImplementedException();
    }

    public void Battle_OnTurnPreStart(AbilitySlot slot, Combatant combatant, uint turn, uint turnInBattle)
    {
        throw new NotImplementedException();
    }

    public void Battle_OnTurnStarted(AbilitySlot slot, Combatant combatant, uint turn, uint turnInBattle)
    {
        throw new NotImplementedException();
    }

    public void Battle_OnTurnEnded(AbilitySlot slot, Combatant combatant, uint turn, uint turnInBattle)
    {
        throw new NotImplementedException();
    }
    #endregion
}