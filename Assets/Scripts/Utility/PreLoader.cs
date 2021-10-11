using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PreLoader : MonoBehaviour
{
    private static bool loaded = false;
    
    [SerializeField]
    GameObject[] PreLoadedPrefabs;

    private void Awake()
    {
        if (loaded == false)
        {
            loaded = true;
            foreach (GameObject go in PreLoadedPrefabs)
            {
                GameObject inst = Instantiate(go);
                inst.name = go.name;
            }
        }
        Destroy(gameObject);
    }
}
