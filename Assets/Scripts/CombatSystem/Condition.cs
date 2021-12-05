using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Condition : IDescribable
{
    [SerializeField, TextArea] private string _description;
    public string description => _description;

    public abstract bool Evaluate(Combatant caster, Combatant target);
}