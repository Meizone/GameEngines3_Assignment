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
    public enum Animation { RaiseWeapon, SwingWeapon, Run }
    #endregion

    #region "Member variables and properties"

    [SerializeField] private string _abilityName;
    [SerializeField] private uint _cooldown;
    [SerializeField] private Sprite _icon;
    [SerializeField] private SelectableTargets _selectableTargets;
    [SerializeField, TextArea] private string _description;
    [SerializeField] private Payment[] _costs;
    [SerializeField] private Trigger[] _triggers;
    public Trigger[] triggers { get { return _triggers; } }

    public string abilityName { get { return _abilityName; } }
    public uint cooldown { get { return _cooldown; } }
    public Sprite icon { get { return _icon; } }
    public SelectableTargets targetType { get { return _selectableTargets; } }
    public Payment[] costs { get { return _costs; } }

    public bool isTargetted {
        get {
            if (_selectableTargets.HasFlag(SelectableTargets.Ally | SelectableTargets.Enemy))
            {
                foreach (Trigger effect in _triggers)
                    if (effect.IsTriggeredBy(BattleManager.Events.AbilityActivated))
                        return true;
            }
            return false; } }

    public string description => _description;
    #endregion
}