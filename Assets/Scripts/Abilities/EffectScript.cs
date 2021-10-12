using UnityEngine;
[System.Serializable]
public abstract class EffectScript : ScriptableObject, IActivated, IDescribable
{
    public string description;

    public abstract void Activate(Combatant combatant, Combatant target);

    public string GetDescription()
    {
        return description;
    }
}
