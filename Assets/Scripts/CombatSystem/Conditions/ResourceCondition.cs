using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "ResourceCondition", menuName = "Abilities/Conditions/ResourceCondition")]
public class ResourceCondition : Condition
{
    public Resource.Type resource;
    public Resource.Query query;
    public BinaryPredicate binaryPredicate;
    public float amount;
    public override bool Evaluate(Combatant caster, Combatant target)
    {
        bool result = true;
        float value = target.GetResource(resource, query);

        if (result && binaryPredicate.HasFlag(BinaryPredicate.EqualTo))
            result = value == amount;
        if (result && binaryPredicate.HasFlag(BinaryPredicate.GreaterThan))
            result = value > amount;
        if (result && binaryPredicate.HasFlag(BinaryPredicate.LessThan))
            result = value < amount;

        return result;
    }
}