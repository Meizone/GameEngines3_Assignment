using UnityEngine;
[System.Serializable]
public class Effect : IActivated, IDescribable
{
    public enum Trigger { Activated, Passive, Reactive }
    public enum Target { Target, Enemies_All, Enemies_Each, Allies_All, Allies_Each, All, Each, Self }

    public string description;
    public Target target;
    public Resource resource;
    public float amountValue;
    public float amountPercent;
    public float chance;
    public Effect[] secondaryEffects;
    public EffectScript[] scriptedEffects;

    public string GetDescription() { return description; }

    public void Activate(Combatant combatant, Combatant target)
    {
        switch (this.target)
        {
            case Target.Target:
                break;
            case Target.Enemies_All:
                break;
            case Target.Allies_All:
                break;
            default:
                break;
        }
    }
}