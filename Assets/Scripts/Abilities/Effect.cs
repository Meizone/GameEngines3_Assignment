using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
//public class Effect
//{
//    [SerializeField]
//    private string description;
//    public virtual void Activate(Combatant combatant) { }
//    public virtual string GetDescription() { return description; }
//
//    public override string ToString()
//    {
//        return GetDescription();
//    }
//
//}
[System.Serializable]
public class Effect
{
    [TextArea] public string description;
    public enum Type { Passive, OnActivation, Chance, AffectResource };
    public Type type;
    public float chance;
    public ResourceType resourceType;
    public float amount;
    public Effect[] effects;

    public virtual void Activate(Combatant combatant) { }
    public virtual string GetDescription() { return description; }

}