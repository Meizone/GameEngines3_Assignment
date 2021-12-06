using UnityEngine;

[System.Serializable]
public abstract class Condition : IDescribable
{
    [SerializeField, TextArea] protected string description;
    public abstract string Description { get; }

    public abstract bool Evaluate(Combatant caster, Combatant target);
}