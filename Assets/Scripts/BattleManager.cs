using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum ExitState { Victory, Loss, Tie }
    private static BattleManager instance;

    Combatant[] combatants;
    Combatant activeCombatant;
    float waitingSpeedScale = 1.0f;
    float activeSpeedScale = 0.0f;

    private void Start()
    {
        combatants = FindObjectsOfType<Combatant>();
        foreach (Combatant combatant in combatants)
        {
            combatant.StartBattle();
        }
    }

    private void Update()
    {
        float speedScale = activeCombatant == null ? waitingSpeedScale : activeSpeedScale;

        Combatant mostReady = combatants[0];
        foreach (Combatant combatant in combatants)
        {
            combatant.UpdateReadiness(speedScale);
            if (combatant.readiness > mostReady.readiness)
                mostReady = combatant;
        }

        if (mostReady.readiness > 100)
        {
            activeCombatant = mostReady;
            activeCombatant.Activate();
        }
    }

    public static void ExitCombat(ExitState exitState)
    {
        Debug.LogFormat("<color=yellow>Exiting combat with exit state {0} has not yet been implemented.</color>", exitState);
    }
}
