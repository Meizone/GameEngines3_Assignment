using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static EventSystem eventSystem;
    private static Camera mainCamera;
    private static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //SceneManager.sceneLoaded += HuntAndDestroy;
        HuntAndDestroy(gameObject.scene, LoadSceneMode.Additive);
    }

    private void HuntAndDestroy(Scene arg0, LoadSceneMode arg1)
    {
        GameObject[] gos = arg0.GetRootGameObjects();
        foreach(GameObject go in gos)
        {
            if (go.name == "Main Camera")
            {
                if (mainCamera == null)
                {
                    mainCamera = go.GetComponent<Camera>();
                    mainCamera.gameObject.SetActive(false);
                    mainCamera.gameObject.SetActive(true);
                    DontDestroyOnLoad(go);
                }
                else
                {
                    mainCamera.gameObject.SetActive(false);
                    mainCamera.gameObject.SetActive(true);
                    Destroy (go);
                }
            }

            else if (go.name == "EventSystem")
            {
                if (eventSystem == null)
                {
                    eventSystem = go.GetComponent<EventSystem>();
                    DontDestroyOnLoad(go);
                }
                else
                    Destroy(go);
            }
        }
    }
}
