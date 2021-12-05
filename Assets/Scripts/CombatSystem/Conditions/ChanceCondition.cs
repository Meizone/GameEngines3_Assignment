using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "ChanceCondition", menuName = "Abilities/Conditions/ChanceCondition")]
public class ChanceCondition : Condition
{
    [Range(0, 100)] public float chance;

    public override bool Evaluate(Combatant caster, Combatant target)
    {
        return Random.Range(0, 99.999f) < chance;
    }
}