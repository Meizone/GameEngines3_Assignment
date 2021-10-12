using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    Combatant[] combatants;
    Combatant active;
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
        float speedScale = active == null ? waitingSpeedScale : activeSpeedScale;
        Combatant mostReady = combatants[0];
        foreach (Combatant combatant in combatants)
        {
            combatant.UpdateReadiness(speedScale);
            if (combatant.readiness > mostReady.readiness)
                mostReady = combatant;
        }

        if (mostReady.readiness > 100)
            active = mostReady;
    }
}
