using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "PaymentEffect", menuName = "Abilities/Effects/PaymentEffect")]
public class PaymentEffect : Effect
{
    public Payment payment;

    public override void Execute(Combatant caster, Combatant target)
    {
        target.Pay(payment);
    }
}
