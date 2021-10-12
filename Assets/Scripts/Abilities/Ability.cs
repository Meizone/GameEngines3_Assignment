using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[System.Serializable, CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject, IDescribable
{
    public enum Target {[Tooltip("Self/AoE")] Self, Ally, Enemy }
    public enum Animation { RaiseWeapon, SwingWeapon, Run }

    public string abilityName;
    public uint cooldown;
    public Sprite icon;
    public Animation animation;
    public Target target;
    [TextArea] public string description;
    public Cost[] costs;
    public Effect[] effects;

    public string GetDescription()
    {
        return string.Format(description, effects);
    }

    public void Activate(Combatant combatant)
    {
        Combatant abilityTarget = combatant;
        switch (target)
        {
            case Target.Enemy:
                Debug.LogFormat("<color=red>Targeting mechanic for enemies not yet implemented.</color>");
                break;
            case Target.Ally:
                Debug.LogFormat("<color=red>Targeting mechanic for allies not yet implemented.</color>");
                break;
            default:
                break;
        }

        foreach (Effect effect in effects)
            effect.Activate(combatant, abilityTarget);
    }
}
