using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{

    [SerializeField]
    List<GameObject> EnemyList;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(EnemyList[0]);
    }

}


enum EnemyType
{
    bat
}