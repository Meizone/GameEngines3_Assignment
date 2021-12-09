using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{

    [SerializeField]
    List<GameObject> EnemyList;
    private BattleManager battleManager;

    // Start is called before the first frame update
    void Start()
    {
        Combatant combatant = Instantiate(EnemyList[0]).GetComponent<Combatant>();
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.AddCombatant(combatant);
    }

}


enum EnemyType
{
    bat
}