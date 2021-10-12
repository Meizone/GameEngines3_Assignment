using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilitySlot
{
    [SerializeField]
    private uint cooldown;
    [SerializeField]
    private Ability ability;

    public void Activate()
    {
        cooldown = ability.cooldown;
    }

    public void Update()
    {
        if (cooldown > 0)
            --cooldown;
    }
}
