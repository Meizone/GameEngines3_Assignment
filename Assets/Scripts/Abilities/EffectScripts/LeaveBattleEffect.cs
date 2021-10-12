using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/LeaveBattleEffect")]
public class LeaveBattleEffect : EffectScript
{
    public BattleManager.ExitState exitState;
    public override void Activate(Combatant combatant, Combatant target)
    {
        BattleManager.ExitCombat(exitState);
    }
}
