using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/Ability")]
public class Ability : ScriptableObject, IDescribable
{
    #region "Encapsulated enumerations"
    [System.Flags] public enum SelectableTargets
    {
        Self = (1 << 0),
        Ally = (1 << 1),
        Enemy = (1 << 2),
    }
    #endregion

    #region "Member variables and properties"
    [SerializeField, ReadOnly] private string description;
    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private uint _cooldown;
    [SerializeField] private SelectableTargets _selectableTargets;
    [SerializeField, TextArea] private string _description;
    [SerializeField] private Payment[] _costs;
    [SerializeField] protected Trigger[] _triggers;
    public string displayName { get { return _displayName; } }
    public Sprite icon { get { return _icon; } }
    public uint cooldown { get { return _cooldown; } }
    public SelectableTargets targetType { get { return _selectableTargets; } }
    public bool isTargetted {
        get {
            if (_selectableTargets.HasFlag(SelectableTargets.Ally | SelectableTargets.Enemy))
            {
                foreach (Trigger effect in _triggers)
                    if (effect.IsTriggeredBy(BattleManager.Events.AbilityActivated))
                        return true;
            }
            return false; } }
    public string Description => _description;
    public Payment[] costs { get { return _costs; } }
    public Trigger[] triggers { get { return _triggers; } }

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        string n = "";
        foreach (Trigger trigger in triggers)
        {
            n = n + trigger.Description;
        }

        description = n;
    }
#endif
}