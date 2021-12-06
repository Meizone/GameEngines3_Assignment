using UnityEngine;

public class StatusEffect : ScriptableObject, IDescribable
{
    #region "Encapsulated enumerations"
    public enum Type
    {
        Buff,
        Debuff,
    }

    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _icon;
    private uint _duration;
    [SerializeField, TextArea] private string _description;
    [SerializeField] protected Trigger[] _triggers;
    public string displayName { get { return _displayName; } }
    public Sprite icon { get { return _icon; } }
    public uint duration { get { return _duration; } }
    public string Description => _description;
    public Trigger[] triggers { get { return _triggers; } }
    #endregion
}
