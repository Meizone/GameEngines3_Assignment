/*****************************************************
File:       Trigger.cs
Author:     Kiera Bacon
Project:    GAME3023 Assignment 1 (By Kiera Bacon and Nathan Nguyen)
Created:    October 12th, 2021
Modified:   December 5th, 2021 - Changed Effects and Conditions to be serialized references
            October 15th, 2021
Description:
    Class for controlling the execution of effects.
A trigger is activated by the battle manager, then
evaluates its conditions before executing its effects.
*****************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable, ExecuteInEditMode]
public class Trigger : IDescribable
{
    #region "Encapsulated enumerations"
    public enum Evaluation
    {
        All,
        Any,
        Each,
        Only,
    }

    [System.Flags] public enum Targets
    {
        Target = (1 << 0),
        Self = (1 << 1),
        Allies = (1 << 2),
        Enemies = (1 << 3),
    }
    #endregion

    #region "Member variables, properties, and accessors"
    [SerializeField, TextArea]
    private string _description;
    [SerializeField, Tooltip("Each of the delegates that, when invoked, will activate the Trigger.")]
    private BattleManager.Events _triggeringEvents;
    [SerializeField, Tooltip("All targets that are evaluated to meet the conditions")]
    private Targets _possibleTargets;
    [SerializeField, Tooltip("If this is less than the number of possible targets, it will choose randomly from within that list.\n\nIf this is zero, it will choose all of them.")]
    private uint _maxTargets;
    [SerializeField, Tooltip("All: Conditions must evaluate true on all targets for the effect to Trigger.\n\nAny: Conditions must evaluation true on at least one target for the effect to Trigger.\n\nEach: Triggers the effect targeting each target on which the conditions evaluate true.")] 
    private Evaluation _evaluateTargets;
    [SerializeField, Tooltip("All: Returns true only if all conditions do.\n\nAny: Returns true if any condition does.\n\nEach: Returns true each time a condition does.")]
    private Evaluation _evaluateConditions;
    [SerializeReference, SelectableReferenceType(typeof(Condition))] private Condition[] _conditions;
    [SerializeReference, SelectableReferenceType(typeof(Effect))] private Effect[] _effects;
    public string Description {
        get
        {
            return _description;
            //return string.Format(_description, _conditions.Select(c => c.Description).ToArray());
        }
    }
    
    public bool IsTriggeredBy(BattleManager.Events trigger) { return _triggeringEvents.HasFlag(trigger); }
    #endregion

    #region "Public functions"
    public void Register(AbilityData callingAbility)
    {
        if (callingAbility == null)
        {
            Debug.LogError("CallingAbility was null.");
            return;
        }
        if (callingAbility.combatant == null)
        {
            Debug.LogError("Combatant was null.");
            return;
        }
        if (callingAbility.combatant.battle == null)
        {
            Debug.LogError("Battle was null.");
            return;
        }

        BattleManager battle = callingAbility.combatant.battle;
        battle.onAbilityActivated += (activatedAbility, target) => OnAbilityActivated(callingAbility, activatedAbility, target);
        battle.onResourceChanged += (combatant, resource, change, final) => OnResourceChanged(callingAbility, combatant, resource, change, final);
        battle.onBattleStarted += (combatant, turn, turnInBattle) => OnBattleStarted(callingAbility, combatant);
        battle.onBeforeTurnStarted += (combatant, turn, turnInBattle) => OnBeforeTurnStarted(callingAbility, combatant, turn, turnInBattle);
        battle.onTurnStarted += (combatant, turn, turnInBattle) => OnTurnStarted(callingAbility, combatant, turn, turnInBattle);
        battle.onTurnEnded += (combatant, turn, turnInBattle) => OnTurnEnded(callingAbility, combatant, turn, turnInBattle);
    }
    public void Deregister(AbilityData callingAbility)
    {
        if (callingAbility == null)
        {
            Debug.LogError("CallingAbility was null.");
            return;
        }
        if (callingAbility.combatant == null)
        {
            Debug.LogError("Combatant was null.");
            return;
        }
        if (callingAbility.combatant.battle == null)
        {
            Debug.LogError("Battle was null.");
            return;
        }

        BattleManager battle = callingAbility.combatant.battle;
        battle.onAbilityActivated -= (activatedAbility, target) => OnAbilityActivated(callingAbility, activatedAbility, target);
        battle.onResourceChanged -= (combatant, resource, change, final) => OnResourceChanged(callingAbility, combatant, resource, change, final);
        battle.onBattleStarted -= (combatant, turn, turnInBattle) => OnBattleStarted(callingAbility, combatant);
        battle.onBeforeTurnStarted -= (combatant, turn, turnInBattle) => OnBeforeTurnStarted(callingAbility, combatant, turn, turnInBattle);
        battle.onTurnStarted -= (combatant, turn, turnInBattle) => OnTurnStarted(callingAbility, combatant, turn, turnInBattle);
        battle.onTurnEnded -= (combatant, turn, turnInBattle) => OnTurnEnded(callingAbility, combatant, turn, turnInBattle);
    }
    #endregion

    #region "Callbacks"
    private bool OnAbilityActivated(AbilityData callingAbility, AbilityData activatedAbility, Combatant target)
    {
        if (!_triggeringEvents.HasFlag(BattleManager.Events.AbilityActivated))
            return false;

        Activate(callingAbility, activatedAbility, target);
        return true;
;    }

    private bool OnResourceChanged(AbilityData callingAbility, Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        if (!_triggeringEvents.HasFlag(BattleManager.Events.ResourceChanged))
            return false;
        
        return true;
    }

    private bool OnBattleStarted(AbilityData callingAbility, Combatant combatant)
    {
        if (!_triggeringEvents.HasFlag(BattleManager.Events.BattleStarted))
            return false;

        return true;
    }

    private bool OnBeforeTurnStarted(AbilityData callingAbility, Combatant combatant, uint turn, uint turnInBattle)
    {
        if (!_triggeringEvents.HasFlag(BattleManager.Events.TurnPreStart))
            return false;

        return true;
    }
    private bool OnTurnStarted(AbilityData callingAbility, Combatant combatant, uint turn, uint turnInBattle)
    {
        if (!_triggeringEvents.HasFlag(BattleManager.Events.TurnStarted))
            return false;

        return true;
    }
    private bool OnTurnEnded(AbilityData callingAbility, Combatant combatant, uint turn, uint turnInBattle)
    {
        if (!_triggeringEvents.HasFlag(BattleManager.Events.TurnEnded))
            return false;

        return true;
    }
    #endregion

    #region "Private functions"
    private void ExecuteEffects(Combatant combatant, Combatant target)
    {
        Debug.Log("ExecuteEffects for " + combatant.gameObject.name + " with target " + target.gameObject.name + ".");

        foreach (Effect effect in _effects)
        {
            if (effect == null)
                continue;

            effect.Execute(combatant, target);
        }

        combatant.PlayAttackAnim();
    }

    private void EvaluateTargets(LinkedList<Combatant> selectedTargets, AbilityData activatedAbility)
    {
        switch (_evaluateTargets)
        {
            case Evaluation.All:
                foreach (Combatant combatant in selectedTargets)
                {
                    if (!EvaluateConditions(activatedAbility.combatant, combatant))
                    {
                        return;
                    }
                }
                foreach (Combatant combatant in selectedTargets)
                {
                    ExecuteEffects(activatedAbility.combatant, combatant);
                }
                return;
            case Evaluation.Any:
                foreach (Combatant combatant in selectedTargets)
                {
                    if (EvaluateConditions(activatedAbility.combatant, combatant))
                    {
                        ExecuteEffects(activatedAbility.combatant, combatant);
                        return;
                    }
                }
                return;
            case Evaluation.Each:
                foreach (Combatant combatant in selectedTargets)
                {
                    if (EvaluateConditions(activatedAbility.combatant, combatant))
                    {
                        ExecuteEffects(activatedAbility.combatant, combatant);
                    }
                }
                return;
            default:
                break;
        }
    }

    private bool EvaluateConditions(Combatant caster, Combatant target)
    {
        if (caster == null || caster.gameObject == null || target == null || target.gameObject == null)
            return false;

        switch (_evaluateConditions)
        {
            case Evaluation.All:
                foreach (Condition condition in _conditions)
                {
                    if (condition == null)
                        continue;

                    if (!condition.Evaluate(caster, target))
                    {
                        return false;
                    }
                }
                return true;
            case Evaluation.Any:
                foreach (Condition condition in _conditions)
                {
                    if (condition == null)
                        continue;

                    if (condition.Evaluate(caster, target))
                    {
                        return true;
                    }
                }
                return false;
            case Evaluation.Each:
                foreach (Condition condition in _conditions)
                {
                    if (condition == null)
                        continue;

                    if (condition.Evaluate(caster, target))
                    {
                        ExecuteEffects(caster, target);
                    }
                }
                return false;
            default:
                return true;
        }
    }

    /// <summary>
    /// Activate a particular trigger.
    /// </summary>
    /// <param name="callingAbility">The ability that is immediately responding to the causing event. Cannot be null.</param>
    /// <param name="activatedAbility">The ability whose casting triggered the event to happen. Cannot be null.</param>
    /// <param name="target">The target that was selected by the player/ai when casting the activated ability. Cannot be null.</param>
    private void Activate(AbilityData callingAbility, AbilityData activatedAbility, Combatant target)
    {
        if (callingAbility == null || callingAbility.combatant == null || callingAbility.ability == null || 
            activatedAbility == null || activatedAbility.combatant == null || activatedAbility.ability == null ||
            target == null)
        {
            Debug.Log("Something was null that shouldn't have been.");
            return;
        }

        if (callingAbility != activatedAbility)
        {
            //Debug.Log("callingAbility != activatedAbility: " + callingAbility.ability.name + " " + activatedAbility.ability.name + ".");
            return; // FOR NOW
        }
        //Debug.Log("Activate for " + activatedAbility.ability.name + ".");

        if (callingAbility.combatant != activatedAbility.combatant)
            return;


        /// There should be logic in here to assess, like, what happens if the ability being activated isn't this one?
        /// Like, what if there's a condition that goes 'if someone else uses an ability, do this first before they do!
        /// I think I need to be more awake to figure out how that would go.

        LinkedList<Combatant> selectedTargets = new LinkedList<Combatant>();
        if ((_possibleTargets.HasFlag(Targets.Self) || _possibleTargets.HasFlag(Targets.Target))
            && !_possibleTargets.HasFlag(Targets.Allies) && !_possibleTargets.HasFlag(Targets.Enemies))
        {
            /// Here, there's no need to get the whole list from the battle manager if we already have all our relevant targets given to us,
            /// but we might still need to randomize if there's less than 2 max targets.
            if (_possibleTargets.HasFlag(Targets.Self))
                if ((_maxTargets > 2 || UnityEngine.Random.value < 0.5f) && activatedAbility.combatant != null)
                    selectedTargets.AddLast(activatedAbility.combatant);
            if (_possibleTargets.HasFlag(Targets.Target))
                if ((_maxTargets > 2 || selectedTargets.Count < 1) && target != null)
                    selectedTargets.AddLast(target);
        }
        else if (_possibleTargets.HasFlag(Targets.Allies) || _possibleTargets.HasFlag(Targets.Enemies))
        {
            /// This ungainly chunk of code, written at 4:27am, is meant to take the list of all combatants in the fight,
            /// and then FIRST pair it down by excluding unwanted targets, SECOND reshuffle it, and then THIRD reduce it down to the max
            selectedTargets = new LinkedList<Combatant>(activatedAbility.combatant.battle.combatants.SkipWhile((o) =>
            {
                return ((o == activatedAbility.combatant && !_possibleTargets.HasFlag(Targets.Self)) ||
                (o.faction == activatedAbility.combatant.faction && !_possibleTargets.HasFlag(Targets.Allies)) ||
                (o.faction != activatedAbility.combatant.faction && !_possibleTargets.HasFlag(Targets.Enemies)));
            }).OrderBy((o) => { return UnityEngine.Random.value; }));
            if (_maxTargets > 0)
                while (selectedTargets.Count > _maxTargets)
                    selectedTargets.RemoveLast();
        }

        EvaluateTargets(selectedTargets, activatedAbility);
        return;
    }
    #endregion
}