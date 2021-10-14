using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void Execute(Combatant caster, Combatant target);
}
