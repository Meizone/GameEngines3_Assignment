using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawnerScript : MonoBehaviour
{

    [SerializeField]
    List<Combatant> EnemyList;
    private BattleManager battleManager;
    private void Awake()
    {
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        Debug.Log("SceneManager_sceneUnloaded");
        Combatant combatant = Instantiate(EnemyList[0], transform);
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.AddCombatant(combatant);
    }

    void Start()
    {
        Debug.Log("Start");
        Combatant combatant = Instantiate(EnemyList[0], transform);
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.AddCombatant(combatant);
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("OnLevelWasLoaded");
        Combatant combatant = Instantiate(EnemyList[0], transform);
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.AddCombatant(combatant);
    }
}


enum EnemyType
{
    bat
}