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
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        Debug.Log("SceneManager_sceneLoaded");
        Combatant combatant = Instantiate(EnemyList[0], transform);
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.AddCombatant(combatant);
    }
}


enum EnemyType
{
    bat
}