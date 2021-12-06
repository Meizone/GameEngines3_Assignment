using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "ResourceCondition", menuName = "Abilities/Conditions/ResourceCondition")]
public class ResourceCondition : Condition
{
    public Resource.Type resource;
    public Resource.Query query;
    public BinaryPredicate binaryPredicate;
    public float amount;

    public override string Description
    {
        get
        {
            string predStr = "";
            bool et = binaryPredicate.HasFlag(BinaryPredicate.EqualTo);
            bool gt = binaryPredicate.HasFlag(BinaryPredicate.GreaterThan);
            bool lt = binaryPredicate.HasFlag(BinaryPredicate.LessThan);
            if (binaryPredicate == 0)
                return string.Format("{0} {1}", resource, "does not exist as a concept.");
            else if (lt && !et && !gt)
                predStr = "is less than";
            else if (!lt && et && !gt)
                predStr = "is equal to";
            else if (!lt && !et && gt)
                predStr = "is greater than";
            else if (lt && et && !gt)
                predStr = "is less than or equal to";
            else if (!lt && et && gt)
                predStr = "is greater than or equal to";
            else if (lt && !et && gt)
                predStr = "is not equal to";
            else
                return string.Format("{0} {1}", resource, "exists as a concept.");

            return string.Format(description, resource, query.ToString().ToLower(), predStr, amount);
        }
    }
    public ResourceCondition()
    {
        description = "{0} {1} {2} {3}.";
    }
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