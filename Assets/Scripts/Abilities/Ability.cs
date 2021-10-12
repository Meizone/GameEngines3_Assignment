using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject
{
    public Sprite icon;
    public string abilityName;
    public string description;
    public uint cooldown;
    public AnimationType animation;
    public Effect[] effects;

    public string GetDescription()
    {
        return string.Format(description, effects);
    }

    public void Activate(Combatant combatant)
    {
        foreach (Effect effect in effects)
            effect.Activate(combatant);
    }
}
