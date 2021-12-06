using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "ChanceCondition", menuName = "Abilities/Conditions/ChanceCondition")]
public class ChanceCondition : Condition
{
    [Range(0, 100)] public float chance;

    public override string Description { get { return string.Format(description, chance); } }

    public ChanceCondition()
    {
        description = "{0}% chance to trigger effects.";
    }

    public override bool Evaluate(Combatant caster, Combatant target)
    {
        return Random.Range(0, 99.999f) < chance;
    }
}