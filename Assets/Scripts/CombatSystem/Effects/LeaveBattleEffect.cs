using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable, CreateAssetMenu(fileName = "LeaveBattleEffect", menuName = "Abilities/Effects/LeaveBattleEffect")]
public class LeaveBattleEffect : Effect
{
    public BattleManager.ExitState exitState;

    public override void Execute(Combatant caster, Combatant target)
    {
        caster.battle.ExitCombat(exitState);
    }
}
