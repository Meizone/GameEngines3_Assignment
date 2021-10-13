using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Trigger : IDescribable
{
    [System.Flags] public enum Events
    {
        BattleStarted = (1 << 0),
        SkillActivated = (1 << 1),
        TurnPreStart = (1 << 2),
        TurnStarted = (1 << 3),
        TurnEnded = (1 << 4),
        ResourceChanged = (1 << 5),
    }

    public enum Evaluation
    {
        All,
        Any,
        Each,
    }

    [System.Flags] public enum Targets
    {
        Target = (1 << 0),
        Self = (1 << 1),
        Allies = (1 << 2),
        Enemies = (1 << 3),
    }

    [SerializeField, TextArea] private string _description;
    [SerializeField] private Events _triggers;
    [SerializeField] private string _triggeredAnimation;
    [SerializeField] private Targets _possibleTargets;
    [SerializeField, Tooltip("If this is less than the number of possible targets, it will choose randomly from within that list.\n\nIf this is zero, it will choose all of them.")]
    private uint _maxTargets;
    [SerializeField, Tooltip("All: Conditions must evaluate true on all targets for the effect to Trigger.\n\nAny: Conditions must evaluation true on at least one target for the effect to Trigger.\n\nEach: Triggers the effect targeting each target on which the conditions evaluate true.")] 
    private Evaluation _evaluateTargets;
    [SerializeField, Tooltip("All: Returns true only if all conditions do.\n\nAny: Returns true if any condition does.\n\nEach: Returns true each time a condition does.")]
    private Evaluation _evaluateConditions;
    [SerializeField] private Condition[] _conditions;
    [SerializeField] private Effect[] _effects;

    public bool HasTrigger(Events trigger) { return _triggers.HasFlag(trigger); }

    public string GetDescription() { return _description; }

    

    public void Activate(AbilitySlot slot, Combatant caster, Combatant target)
    {
        if (slot.combatant != caster)
            return;
        /// There should be logic in here to assess, like, what happens if the ability being activated isn't this one?
        /// Like, what if there's a condition that goes 'if someone else uses an ability, do this first before they do!
        /// I think I need to be more awake to figure out how that would go.

        LinkedList<Combatant> selectedTargets = new LinkedList<Combatant>();
        if (_possibleTargets.HasFlag(Targets.Self | Targets.Target) && !_possibleTargets.HasFlag(Targets.Allies | Targets.Enemies))
        {
            /// Here, there's no need to get the whole list from the battle manager if we already have all our relevant targets given to us,
            /// but we might still need to randomize if there's less than 2 max targets.
            if (_possibleTargets.HasFlag(Targets.Self))
                if (_maxTargets > 2 || UnityEngine.Random.value < 0.5f)
                    selectedTargets.AddLast(caster);
            if (_possibleTargets.HasFlag(Targets.Target))
                if (_maxTargets > 2 || selectedTargets.Count < 1)
                    selectedTargets.AddLast(target);
        }
        else if (_possibleTargets.HasFlag(Targets.Allies | Targets.Enemies))
        {
            /// This ungainly chunk of code, written at 4:27am, is meant to take the list of all combatants in the fight,
            /// and then FIRST pair it down by excluding unwanted targets, SECOND reshuffle it, and then THIRD reduce it down to the max
            selectedTargets = new LinkedList<Combatant>(caster.battle.combatants.SkipWhile((o) =>
            {
                return ((o == caster && !_possibleTargets.HasFlag(Targets.Self)) ||
                (o.faction == caster.faction && !_possibleTargets.HasFlag(Targets.Allies)) ||
                (o.faction != caster.faction && !_possibleTargets.HasFlag(Targets.Enemies)));
            }).OrderBy((o) => { return UnityEngine.Random.value; }));
            if (_maxTargets > 0)
                while (selectedTargets.Count > _maxTargets)
                    selectedTargets.RemoveLast();
        }

        switch (_evaluateTargets)
        {
            case Evaluation.All:
                foreach (Combatant combatant in selectedTargets)
                    if (!EvaluateConditions(caster, combatant))
                        return;
                Execute(caster, target);
                return;
            case Evaluation.Any:
                foreach (Combatant combatant in selectedTargets)
                {
                    if (EvaluateConditions(caster, combatant))
                    {
                        Execute(caster, target);
                        return;
                    }
                }
                return;
            case Evaluation.Each:
                foreach (Combatant combatant in selectedTargets)
                    if (EvaluateConditions(caster, combatant))
                        Execute(caster, target);
                return;
            default:
                break;
        }
    }

    private bool EvaluateConditions(Combatant caster, Combatant target)
    {
        switch (_evaluateConditions)
        {
            case Evaluation.All:
                foreach (Condition condition in _conditions)
                    if (!condition.Evaluate(caster, target))
                        return false;
                return true;
            case Evaluation.Any:
                foreach (Condition condition in _conditions)
                    if (condition.Evaluate(caster, target))
                        return true;
                return false;
            case Evaluation.Each:
                foreach (Condition condition in _conditions)
                    if (condition.Evaluate(caster, target))
                        Execute(caster, target);
                return false;
            default:
                return true;
        }
    }

    private void Execute(Combatant combatant, Combatant target)
    {
        foreach (Effect effect in _effects)
            effect.Execute(combatant, target);
    }
}