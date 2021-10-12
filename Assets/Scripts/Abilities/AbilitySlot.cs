using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AbilitySlot : MonoBehaviour
{
    public UnityEvent<float, bool> OnStatusChanged;

    [SerializeField]
    private uint cooldown;
    [SerializeField]
    private Ability ability;
    public Ability GetAbility { get { return ability; } }
    [SerializeField]
    private Combatant combatant;
    private bool isPayable = false;

    public void Activate()
    {
        if (CheckPayable())
        {
            cooldown = ability.cooldown;
            combatant.Pay(ability.costs);
            ability.Activate(combatant);
        }
    }

    public void Update()
    {
        CheckPayable();
    }

    private bool CheckPayable()
    {
        if (cooldown == 0 && combatant.CanPay(ability.costs))
        {
            if (!isPayable)
                OnStatusChanged?.Invoke(cooldown / ability.cooldown, isPayable);

            // highlight the skill as usable

            return true;
        }
        else
        {
            if (isPayable)
                OnStatusChanged?.Invoke(cooldown / ability.cooldown, isPayable);

            // highlight the skill as unusable

            return false;
        }
    }

    public void Cooldown()
    {
        if (cooldown > 0)
        {
            --cooldown;
            OnStatusChanged?.Invoke(cooldown / ability.cooldown, isPayable);
        }
    }
}
