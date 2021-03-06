using UnityEngine;

[System.Serializable]
public abstract class Effect : IDescribable
{
    [SerializeField, TextArea] private string _description;
    public string Description => _description;
    public abstract void Execute(Combatant caster, Combatant target);
}
